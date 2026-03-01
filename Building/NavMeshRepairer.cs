using System.Collections.Generic;
using S1MAPI.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace S1MAPI.Building
{
    /// <summary>
    /// Records the position and dimensions of a doorway for NavMesh source filtering
    /// and ramp/ground plane generation. Used for both exterior and interior doorways.
    /// </summary>
    public sealed class NavMeshDoorwayInfo
    {
        /// <summary>Center of the doorway in local building coordinates (Y=0, floor level).</summary>
        public Vector3 Center { get; }

        /// <summary>Width of the doorway opening in meters.</summary>
        public float Width { get; }

        /// <summary>Height of the doorway opening in meters.</summary>
        public float Height { get; }

        /// <summary>Unit vector perpendicular to the wall face in the XZ plane.</summary>
        public Vector3 InwardNormal { get; }

        /// <summary>Thickness of the wall containing this doorway.</summary>
        public float WallThickness { get; }

        /// <summary>
        /// Position at the base of the stairs in local building coordinates (ground level).
        /// Null for interior doorways or exterior doorways without stairs.
        /// </summary>
        public Vector3? StairBasePosition { get; }

        /// <summary>
        /// Create a NavMesh doorway info record.
        /// </summary>
        /// <param name="center">Door center in local building coordinates (Y=0)</param>
        /// <param name="width">Doorway width in meters</param>
        /// <param name="height">Doorway height in meters</param>
        /// <param name="inwardNormal">Unit vector perpendicular to the wall face</param>
        /// <param name="wallThickness">Wall thickness in meters</param>
        /// <param name="stairBasePosition">Position at stair base (ground level), or null if no stairs</param>
        public NavMeshDoorwayInfo(
            Vector3 center, float width, float height,
            Vector3 inwardNormal, float wallThickness,
            Vector3? stairBasePosition = null)
        {
            Center = center;
            Width = width;
            Height = height;
            InwardNormal = inwardNormal;
            WallThickness = wallThickness;
            StairBasePosition = stairBasePosition;
        }
    }

    /// <summary>
    /// Builds and manages runtime NavMesh for building interiors.
    /// Collects building geometry (floor, walls, ramps) from the building hierarchy,
    /// adds manual ground planes at stair bases, and builds a single connected NavMesh
    /// from the ground patch through the ramp into the interior.
    /// A carving obstacle removes the game's baked mesh in the covered area so NPCs
    /// pathfind exclusively on the runtime surface within the building zone.
    /// </summary>
    /// <remarks>
    /// Call <see cref="Build"/> after the building is positioned in the scene.
    /// Call <see cref="Rebuild"/> when interior objects change (e.g. furniture moved).
    /// Call <see cref="Remove"/> when the building is destroyed.
    /// </remarks>
    public sealed class NavMeshRepairer
    {
        #region Fields

        private readonly Transform _buildingRoot;
        private readonly Vector3 _roomSize;
        private readonly IReadOnlyList<NavMeshDoorwayInfo> _doorways;
        private readonly int _agentTypeID;
        private readonly float _foundationHeight;

        private NavMeshDataInstance _navInstance;
        private readonly List<GameObject> _rampObjects = new List<GameObject>();
        private GameObject? _obstacleGO;
        private bool _isBuilt;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new NavMesh repairer for a building.
        /// </summary>
        /// <param name="buildingRoot">Root transform of the building (must be positioned before calling Build)</param>
        /// <param name="roomSize">Interior room dimensions (width, height, depth)</param>
        /// <param name="doorways">Doorway positions for source filtering and ramp generation</param>
        /// <param name="agentTypeID">NavMesh agent type to build for (0 = default agent)</param>
        /// <param name="foundationHeight">Foundation height in meters (0 = no foundation). Used to carve ground NavMesh.</param>
        public NavMeshRepairer(
            Transform buildingRoot,
            Vector3 roomSize,
            IReadOnlyList<NavMeshDoorwayInfo> doorways,
            int agentTypeID = 0,
            float foundationHeight = 0f)
        {
            _buildingRoot = buildingRoot;
            _roomSize = roomSize;
            _doorways = doorways;
            _agentTypeID = agentTypeID;
            _foundationHeight = foundationHeight;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Whether the NavMesh is currently active.
        /// </summary>
        public bool IsBuilt =>
            _isBuilt;

        #endregion

        #region Public API

        /// <summary>
        /// Build a NavMesh covering the building interior, stair ramps, and ground patches
        /// at each stair base. A carving obstacle removes the game's baked mesh in the
        /// covered area. Must be called after the building is positioned in the scene.
        /// </summary>
        public void Build()
        {
            if (_isBuilt)
            {
                DebugLog.Warning("[NavMeshRepairer] NavMesh already built. Call Rebuild() to refresh.");
                return;
            }

            // 1. Create carving obstacle covering ONLY the building footprint.
            //    Does NOT extend to stair approach area — the baked mesh must persist
            //    there so it overlaps with our runtime ground planes, giving NPCs a
            //    cross-instance transition point.
            //    Created early to give Unity maximum time for async carving.
            if (_foundationHeight > 0f)
            {
                CreateGroundObstacle();
            }

            Bounds contentBounds = ComputeLocalBounds();

            // 2. Create invisible ramp colliders so the NavMesh has continuous walkable
            //    surface from floor level to ground level at each staircase.
            CreateStairRamps();

            // 3. Collect physics colliders from the building hierarchy as NavMesh sources.
            //    Sources are in local building coordinates.
#if MONO
            var sources = new List<NavMeshBuildSource>();
            var markups = new List<NavMeshBuildMarkup>();
            foreach (Transform child in _buildingRoot)
            {
                if (child.name == Constants.Spatial.StairsFolderName ||
                    child.name == Constants.Spatial.FoundationFolderName)
                {
                    markups.Add(new NavMeshBuildMarkup { root = child, ignoreFromBuild = true });
                }
            }
            NavMeshBuilder.CollectSources(
                _buildingRoot, ~0, NavMeshCollectGeometry.PhysicsColliders,
                0, markups, sources);
#elif IL2CPP
            List<NavMeshBuildSource> sources = CollectBuildingSources();
#endif

            // 4. Remove colliders that sit inside door openings (e.g. door panels/prefabs).
            //    Must run before adding manual sources so those aren't filtered out.
            FilterDoorwaySources(sources);

            // 5. Add flat ground plane sources at each stair base so the runtime mesh
            //    has walkable surface at ground level connecting the ramp to the baked mesh.
            AddGroundPlanes(sources);

            // 6. Add threshold sources at each doorway to bridge the wall-thickness gap
            //    between the ramp top (outer wall face) and the floor (inner wall face).
            AddDoorwayThresholds(sources);

            // 7. Add a "Not Walkable" blocker at ground level covering the building interior.
            //    Area 1 (Not Walkable) takes absolute precedence during voxelization —
            //    prevents the runtime mesh from creating a walkable phantom surface at
            //    ground level inside the building. The floor at Y=0 is unaffected.
            if (_foundationHeight > 0f)
            {
                sources.Add(new NavMeshBuildSource
                {
                    shape = NavMeshBuildSourceShape.Box,
                    size = new Vector3(_roomSize.x, 0.5f, _roomSize.z),
                    transform = Matrix4x4.TRS(
                        new Vector3(_roomSize.x / 2f, -_foundationHeight, _roomSize.z / 2f),
                        Quaternion.identity,
                        Vector3.one),
                    area = 1 // Not Walkable
                });
            }

            if (sources.Count == 0)
            {
                DebugLog.Warning("[NavMeshRepairer] No colliders found in building hierarchy.");
                return;
            }

            // 6. Compute bake bounds = content + expansion.
            //    The expansion zone extends past the obstacle, creating an overlap
            //    where both runtime and baked meshes coexist for smooth NPC transition.
            Bounds bakeBounds = contentBounds;
            bakeBounds.Expand(Constants.NavMesh.BoundsExpansion);

            // 7. Build NavMesh data — local sources positioned by building transform
            NavMeshBuildSettings settings = NavMesh.GetSettingsByID(_agentTypeID);

            // Override voxel size to satisfy the 4-voxel rule for vertical separation.
            // foundationHeight / voxelSize must be >= 4 to prevent surface merging.
            // Use 6 voxels for safety margin; only override if needed.
            if (_foundationHeight > 0f)
            {
                float maxVoxel = _foundationHeight / 6f;
                if (settings.voxelSize > maxVoxel)
                {
                    settings.overrideVoxelSize = true;
                    settings.voxelSize = maxVoxel;
                    DebugLog.Info($"[NavMeshRepairer] Overrode voxelSize to {maxVoxel:F3} " +
                                 $"(foundation={_foundationHeight:F2}, voxels={_foundationHeight / maxVoxel:F1})");
                }
            }

#if IL2CPP
            Il2CppSystem.Collections.Generic.List<NavMeshBuildSource> il2CppSources = sources.ToIl2CppList();
            NavMeshData navData = NavMeshBuilder.BuildNavMeshData(
                settings,
                il2CppSources,
                bakeBounds,
                _buildingRoot.position,
                _buildingRoot.rotation);
#elif MONO
            NavMeshData navData = NavMeshBuilder.BuildNavMeshData(
                settings,
                sources,
                bakeBounds,
                _buildingRoot.position,
                _buildingRoot.rotation);
#endif

            if (navData == null)
            {
                DebugLog.Error("[NavMeshRepairer] NavMeshBuilder.BuildNavMeshData returned null.");
                return;
            }

            // 8. Register with NavMesh system and verify triangles were added
#if MONO
            int trisBefore = NavMesh.CalculateTriangulation().indices.Length / 3;
#endif
            _navInstance = NavMesh.AddNavMeshData(navData);
#if MONO
            int trisAfter = NavMesh.CalculateTriangulation().indices.Length / 3;
            int trisDelta = trisAfter - trisBefore;
            if (trisDelta > 0)
                DebugLog.Info($"[NavMeshRepairer] NavMesh added {trisDelta} triangles (total: {trisAfter}).");
            else
                DebugLog.Warning($"[NavMeshRepairer] NavMesh added ZERO triangles! " +
                                 $"Runtime mesh is empty — interior pathing impossible. " +
                                 $"(before={trisBefore}, after={trisAfter})");
#endif

            // 9. Verify the runtime NavMesh is queryable at room center
            Vector3 testWorld = _buildingRoot.TransformPoint(
                new Vector3(_roomSize.x / 2f, Constants.NavMesh.VerifyTestYOffset, _roomSize.z / 2f));
            NavMeshHit hit;
            bool found = NavMesh.SamplePosition(testWorld, out hit, Constants.NavMesh.VerifySampleRadius, NavMesh.AllAreas);

            if (!found)
            {
                DebugLog.Warning("[NavMeshRepairer] No NavMesh surface found at room center. " +
                                 "Interior pathing will not work.");
            }
            else if (Mathf.Abs(hit.position.y - testWorld.y) >= Constants.NavMesh.VerifyMaxYDiff)
            {
                DebugLog.Warning($"[NavMeshRepairer] NavMesh surface at {hit.position} may be the game's " +
                                 $"ground mesh, not our interior surface (expected Y≈{testWorld.y:F2}).");
            }

            _isBuilt = true;

            DebugLog.Info($"[NavMeshRepairer] Built NavMesh: {sources.Count} sources, " +
                          $"bake bounds size={bakeBounds.size}");
        }

        /// <summary>
        /// Tear down and rebuild the NavMesh.
        /// Use when interior objects change (e.g. furniture placed or moved).
        /// </summary>
        public void Rebuild()
        {
            Remove();
            Build();
        }

        /// <summary>
        /// Remove all NavMesh data and cleanup.
        /// Call when the building is destroyed.
        /// </summary>
        public void Remove()
        {
            if (!_isBuilt) return;

            _navInstance.Remove();

            foreach (GameObject ramp in _rampObjects)
            {
                UnityEngine.Object.Destroy(ramp);
            }
            _rampObjects.Clear();

            if (_obstacleGO != null)
            {
                UnityEngine.Object.Destroy(_obstacleGO);
                _obstacleGO = null;
            }

            _isBuilt = false;

            DebugLog.Info("[NavMeshRepairer] Removed NavMesh data.");
        }

        #endregion

        #region Private Methods — Bounds

        /// <summary>
        /// Compute local-space bounds covering the building footprint, foundation depth,
        /// and ground patches at each stair base.
        /// </summary>
        private Bounds ComputeLocalBounds()
        {
            // Start with building footprint
            var bounds = new Bounds(
                new Vector3(_roomSize.x / 2f, _roomSize.y / 2f, _roomSize.z / 2f),
                _roomSize);

            // Extend down to ground level
            if (_foundationHeight > 0f)
            {
                bounds.Encapsulate(new Vector3(_roomSize.x / 2f, -_foundationHeight, _roomSize.z / 2f));
            }

            // Extend to cover ground patches past each stair base
            float extension = Constants.NavMesh.GroundPatchExtension;
            foreach (NavMeshDoorwayInfo doorway in _doorways)
            {
                if (!doorway.StairBasePosition.HasValue) continue;

                Vector3 stairBase = doorway.StairBasePosition.Value;
                Vector3 outward = -doorway.InwardNormal;
                Vector3 patchEnd = stairBase + outward * extension;

                bounds.Encapsulate(stairBase);
                bounds.Encapsulate(patchEnd);
            }

            return bounds;
        }

        #endregion

        #region Private Methods — Ground Carving

        /// <summary>
        /// Create a NavMeshObstacle that carves the game's baked NavMesh under the building
        /// footprint ONLY. Does not extend to the stair approach area — the baked mesh must
        /// persist there to overlap with the runtime ground planes, providing the cross-instance
        /// transition point where NPCs step from the baked terrain onto the runtime surface.
        /// Created early in <see cref="Build"/> to give Unity maximum time for async carving.
        /// </summary>
        private void CreateGroundObstacle()
        {
            _obstacleGO = new GameObject("NavMeshObstacle");
            _obstacleGO.transform.SetParent(_buildingRoot);
            _obstacleGO.transform.localPosition = new Vector3(
                _roomSize.x / 2f, -_foundationHeight, _roomSize.z / 2f);

            Vector3 obstacleSize = new Vector3(
                _roomSize.x,
                Constants.NavMesh.ObstacleHeight,
                _roomSize.z);

            var obstacle = _obstacleGO.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.center = Vector3.zero;
            obstacle.size = obstacleSize;
            obstacle.carving = true;
            obstacle.carvingTimeToStationary = 0f;

        }

        #endregion

        #region Private Methods — Stair Ramps

        /// <summary>
        /// Create invisible ramp colliders at each exterior doorway with stairs.
        /// The ramp provides continuous walkable NavMesh from floor level down to
        /// ground level, so multiple NPCs can walk the slope simultaneously.
        /// </summary>
        private void CreateStairRamps()
        {
            foreach (NavMeshDoorwayInfo doorway in _doorways)
            {
                if (!doorway.StairBasePosition.HasValue) continue;

                Vector3 top = doorway.Center;                          // (x, 0, z) at floor level
                Vector3 bottom = doorway.StairBasePosition.Value;      // (x, -fh, z) at ground level

                // Ramp geometry
                Vector3 flatDelta = new Vector3(bottom.x - top.x, 0f, bottom.z - top.z);
                float horizontalDist = flatDelta.magnitude;
                float verticalDist = Mathf.Abs(bottom.y);             // = foundationHeight
                float rampLength = Mathf.Sqrt(horizontalDist * horizontalDist + verticalDist * verticalDist);
                float slopeAngle = Mathf.Atan2(verticalDist, horizontalDist) * Mathf.Rad2Deg;

                Vector3 mid = (top + bottom) / 2f;
                Vector3 outward = flatDelta.normalized;                // flat direction from door to stair base

                // Create invisible ramp collider parented to building root.
                // CollectSources will pick it up automatically.
                GameObject rampGO = new GameObject("NavMeshRamp");
                rampGO.transform.SetParent(_buildingRoot);
                rampGO.transform.localPosition = mid;

                // Face outward, then tilt down by slope angle around local X (right axis)
                Quaternion facing = Quaternion.LookRotation(outward, Vector3.up);
                rampGO.transform.localRotation = facing * Quaternion.AngleAxis(slopeAngle, Vector3.right);

                // Width must exceed the minimum ramp width plus agent-radius
                // erosion on both edges, otherwise the walkable strip is so narrow
                // that NPCs path to the corner instead of walking up the center.
                float rampWidth = Mathf.Max(doorway.Width, Constants.NavMesh.MinRampWidth) + Constants.NavMesh.RampErosionBuffer;

                BoxCollider col = rampGO.AddComponent<BoxCollider>();
                col.center = Vector3.zero;
                col.size = new Vector3(rampWidth, Constants.NavMesh.RampColliderThickness, rampLength);

                _rampObjects.Add(rampGO);
            }
        }

        #endregion

        #region Private Methods — Ground Planes

        /// <summary>
        /// Add flat NavMeshBuildSource boxes at ground level past each stair base.
        /// These provide walkable surface at ground level that connects the ramp bottom
        /// to the edge of the baked NavMesh, bridging the gap created by obstacle carving.
        /// Sources are in local building coordinates.
        /// </summary>
        private void AddGroundPlanes(List<NavMeshBuildSource> sources)
        {
            float extension = Constants.NavMesh.GroundPatchExtension;

            foreach (NavMeshDoorwayInfo doorway in _doorways)
            {
                if (!doorway.StairBasePosition.HasValue) continue;

                Vector3 stairBase = doorway.StairBasePosition.Value;
                Vector3 outward = -doorway.InwardNormal;

                // Ground plane centered between stair base and the far edge
                Vector3 patchCenter = stairBase + outward * (extension / 2f);

                float rampWidth = Mathf.Max(doorway.Width, Constants.NavMesh.MinRampWidth)
                                  + Constants.NavMesh.RampErosionBuffer;

                // Align the plane with the outward direction
                Quaternion patchRot = Quaternion.LookRotation(outward, Vector3.up);

                sources.Add(new NavMeshBuildSource
                {
                    shape = NavMeshBuildSourceShape.Box,
                    size = new Vector3(rampWidth, Constants.NavMesh.RampColliderThickness, extension),
                    transform = Matrix4x4.TRS(patchCenter, patchRot, Vector3.one),
                    area = 0
                });
            }
        }

        #endregion

        #region Private Methods — Doorway Source Filtering

        /// <summary>
        /// Remove any collected source whose center falls inside a door opening volume.
        /// Door prefabs (e.g. wooden doors, sliding doors) have colliders that
        /// source collection picks up. These create low-overhead obstructions that
        /// block NavMesh in the doorway.
        /// Must be called after collecting sources but before adding manual sources
        /// (ground planes, thresholds, blocker).
        /// </summary>
        private void FilterDoorwaySources(List<NavMeshBuildSource> sources)
        {
            for (int i = sources.Count - 1; i >= 0; i--)
            {
                NavMeshBuildSource s = sources[i];
                // Source positions from CollectSources are in WORLD space,
                // but doorway coordinates are in LOCAL building space.
                // Transform doorway geometry to world space for comparison.
                Vector3 pos = new Vector3(s.transform.m03, s.transform.m13, s.transform.m23);

                foreach (NavMeshDoorwayInfo doorway in _doorways)
                {
                    Vector3 worldCenter = _buildingRoot.TransformPoint(doorway.Center);
                    Vector3 worldNormal = _buildingRoot.TransformDirection(doorway.InwardNormal);
                    Vector3 worldTangent = new Vector3(-worldNormal.z, 0f, worldNormal.x);

                    Vector3 delta = pos - worldCenter;
                    float normalDist = Mathf.Abs(Vector3.Dot(delta, worldNormal));
                    float tangentDist = Mathf.Abs(Vector3.Dot(delta, worldTangent));
                    float normalThreshold = doorway.WallThickness / 2f + Constants.NavMesh.DoorFilterNormalPadding;

                    // Y check in world space: source must be between floor level and door top
                    float floorY = worldCenter.y;
                    if (normalDist <= normalThreshold &&
                        tangentDist <= doorway.Width / 2f &&
                        pos.y > floorY && pos.y < floorY + doorway.Height)
                    {
                        sources.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        #endregion

        #region Private Methods — Doorway Thresholds

        /// <summary>
        /// Add flat walkable sources at each doorway to bridge the wall-thickness gap
        /// between the ramp top (outer wall face) and the floor (inner wall face).
        /// Without these, the NavMesh has a disconnected gap at every doorway.
        /// Sources are in local building coordinates.
        /// </summary>
        private void AddDoorwayThresholds(List<NavMeshBuildSource> sources)
        {
            foreach (NavMeshDoorwayInfo doorway in _doorways)
            {
                if (!doorway.StairBasePosition.HasValue) continue;

                // Threshold centered in the wall at floor height (Y=0).
                // Extends 0.2m past each wall face for robust overlap with ramp and floor.
                Vector3 thresholdCenter = doorway.Center
                    + doorway.InwardNormal * (doorway.WallThickness / 2f);

                float thresholdDepth = doorway.WallThickness + 0.4f;
                float rampWidth = Mathf.Max(doorway.Width, Constants.NavMesh.MinRampWidth)
                                  + Constants.NavMesh.RampErosionBuffer;

                Quaternion rot = Quaternion.LookRotation(doorway.InwardNormal, Vector3.up);

                sources.Add(new NavMeshBuildSource
                {
                    shape = NavMeshBuildSourceShape.Box,
                    size = new Vector3(rampWidth, Constants.NavMesh.RampColliderThickness, thresholdDepth),
                    transform = Matrix4x4.TRS(thresholdCenter, rot, Vector3.one),
                    area = 0
                });
            }
        }

        #endregion

        #region Private Methods — Source Collection

        /// <summary>
        /// Scan the building hierarchy for BoxColliders and create NavMeshBuildSources.
        /// Excludes colliders under the "Stairs" folder (steps block the ramp surface)
        /// and the "Foundation" folder (bottom face creates a phantom walkable surface).
        /// Sources are in local building coordinates so BuildNavMeshData can transform
        /// them using the building's world position and rotation.
        /// </summary>
        private List<NavMeshBuildSource> CollectBuildingSources()
        {
            // Find stair transforms to exclude (same logic as the Mono markup path)
            var excludedRoots = new HashSet<Transform>();
            foreach (Transform child in _buildingRoot)
            {
                if (child.name == Constants.Spatial.StairsFolderName ||
                    child.name == Constants.Spatial.FoundationFolderName)
                {
                    excludedRoots.Add(child);
                }
            }

            var sources = new List<NavMeshBuildSource>();
            BoxCollider[] colliders = _buildingRoot.GetComponentsInChildren<BoxCollider>();

            foreach (BoxCollider collider in colliders)
            {
                // Skip colliders under excluded stair roots
                if (IsChildOfAny(collider.transform, excludedRoots)) continue;

                // Compute world-space center, then convert to local building space
                Vector3 worldCenter = collider.transform.TransformPoint(collider.center);
                Vector3 localCenter = _buildingRoot.InverseTransformPoint(worldCenter);

                // Compute local rotation relative to building root
                Quaternion localRot = Quaternion.Inverse(_buildingRoot.rotation) * collider.transform.rotation;

                // Actual box dimensions = collider size * transform scale
                Vector3 worldSize = Vector3.Scale(collider.size, collider.transform.lossyScale);

                sources.Add(new NavMeshBuildSource
                {
                    shape = NavMeshBuildSourceShape.Box,
                    size = worldSize,
                    transform = Matrix4x4.TRS(localCenter, localRot, Vector3.one),
                    area = 0
                });
            }

            return sources;
        }

        /// <summary>
        /// Check whether <paramref name="t"/> is a descendant of any transform in <paramref name="roots"/>.
        /// </summary>
        private static bool IsChildOfAny(Transform t, HashSet<Transform> roots)
        {
            Transform current = t.parent;
            while (current != null)
            {
                if (roots.Contains(current)) return true;
                current = current.parent;
            }
            return false;
        }

        #endregion
    }
}
