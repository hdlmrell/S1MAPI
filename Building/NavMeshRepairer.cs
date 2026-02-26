using System.Collections.Generic;
using S1MAPI.Building.Structural;
using S1MAPI.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace S1MAPI.Building
{
    /// <summary>
    /// Records the position and direction of an exterior doorway for NavMeshLink generation.
    /// </summary>
    public sealed class ExteriorDoorwayInfo
    {
        /// <summary>Center of the doorway in local building coordinates (Y=0, floor level).</summary>
        public Vector3 Center { get; }

        /// <summary>Width of the doorway opening in meters.</summary>
        public float Width { get; }

        /// <summary>Height of the doorway opening in meters.</summary>
        public float Height { get; }

        /// <summary>Unit vector pointing from exterior toward interior (into the building).</summary>
        public Vector3 InwardNormal { get; }

        /// <summary>Thickness of the wall containing this doorway.</summary>
        public float WallThickness { get; }

        /// <summary>
        /// Position at the base of the stairs in local building coordinates (ground level).
        /// Null when no foundation/stairs — link is placed at the door instead.
        /// </summary>
        public Vector3? StairBasePosition { get; }

        /// <summary>
        /// Create an exterior doorway info record.
        /// </summary>
        /// <param name="center">Door center in local building coordinates (Y=0)</param>
        /// <param name="width">Doorway width in meters</param>
        /// <param name="height">Doorway height in meters</param>
        /// <param name="inwardNormal">Unit vector pointing into the building</param>
        /// <param name="wallThickness">Wall thickness in meters</param>
        /// <param name="stairBasePosition">Position at stair base (ground level), or null if no stairs</param>
        public ExteriorDoorwayInfo(
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
    /// Collects building geometry (floor, walls, stairs) via physics colliders,
    /// builds a walkable NavMesh surface, and creates NavMeshLinks at doorways
    /// to connect interior rooms and bridge to the exterior NavMesh.
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
        private readonly IReadOnlyList<DoorwayInfo> _interiorDoorways;
        private readonly IReadOnlyList<ExteriorDoorwayInfo> _exteriorDoorways;
        private readonly int _agentTypeID;
        private readonly float _foundationHeight;

        private NavMeshDataInstance _navInstance;
        private readonly List<NavMeshLinkInstance> _links = new List<NavMeshLinkInstance>();
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
        /// <param name="interiorDoorways">Doorway positions from interior walls</param>
        /// <param name="exteriorDoorways">Doorway positions from exterior walls</param>
        /// <param name="agentTypeID">NavMesh agent type to build for (0 = default agent)</param>
        /// <param name="foundationHeight">Foundation height in meters (0 = no foundation). Used to carve ground NavMesh.</param>
        public NavMeshRepairer(
            Transform buildingRoot,
            Vector3 roomSize,
            IReadOnlyList<DoorwayInfo> interiorDoorways,
            IReadOnlyList<ExteriorDoorwayInfo> exteriorDoorways,
            int agentTypeID = 0,
            float foundationHeight = 0f)
        {
            _buildingRoot = buildingRoot;
            _roomSize = roomSize;
            _interiorDoorways = interiorDoorways;
            _exteriorDoorways = exteriorDoorways;
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
        /// Build interior NavMesh and create doorway links.
        /// Must be called after the building is positioned in the scene.
        /// </summary>
        public void Build()
        {
            if (_isBuilt)
            {
                DebugLog.Warning("[NavMeshRepairer] NavMesh already built. Call Rebuild() to refresh.");
                return;
            }

            // 1. Create invisible ramp colliders so the NavMesh has continuous walkable
            //    surface from floor level to ground level at each staircase.
            //    Individual stair steps are too narrow for the agent radius, so without
            //    a ramp the only path is a single-point NavMeshLink (bottleneck).
            CreateStairRamps();

            // 2. Collect physics colliders from the building hierarchy as NavMesh sources.
#if MONO
            //    On Mono we can use Unity's built-in CollectSources which handles
            //    all coordinate math, collider types, and source construction correctly.
            //    Exclude stair step colliders — their individual treads are too narrow
            //    for the agent radius and block walkable surface on the ramp beneath them.
            var sources = new List<NavMeshBuildSource>();
            var markups = new List<NavMeshBuildMarkup>();
            foreach (Transform child in _buildingRoot)
            {
                if (child.name == Constants.Spatial.StairsFolderName)
                {
                    markups.Add(new NavMeshBuildMarkup { root = child, ignoreFromBuild = true });
                }
            }
            NavMeshBuilder.CollectSources(
                _buildingRoot, ~0, NavMeshCollectGeometry.PhysicsColliders,
                0, markups, sources);
#elif IL2CPP
            //    On IL2CPP, CollectSources(Transform) has type compatibility issues
            //    with Il2CppSystem.Collections.Generic.List, so we collect manually.
            List<NavMeshBuildSource> sources = CollectBuildingSources();
#endif

            if (sources.Count == 0)
            {
                DebugLog.Warning("[NavMeshRepairer] No colliders found in building hierarchy.");
                return;
            }

            // 3. Compute bounds from collected sources
            Bounds localBounds = ComputeBoundsFromSources(sources);

            // 4. Build NavMesh data
            NavMeshBuildSettings settings = NavMesh.GetSettingsByID(_agentTypeID);

            // Pad bounds to avoid clipping walkable surfaces at edges
            localBounds.Expand(Constants.NavMesh.BoundsExpansion);

#if IL2CPP
            Il2CppSystem.Collections.Generic.List<NavMeshBuildSource> il2CppSources = sources.ToIl2CppList();
            NavMeshData navData = NavMeshBuilder.BuildNavMeshData(
                settings,
                il2CppSources,
                localBounds,
                _buildingRoot.position,
                _buildingRoot.rotation);
#elif MONO
            NavMeshData navData = NavMeshBuilder.BuildNavMeshData(
                settings,
                sources,
                localBounds,
                _buildingRoot.position,
                _buildingRoot.rotation);
#endif

            if (navData == null)
            {
                DebugLog.Error("[NavMeshRepairer] NavMeshBuilder.BuildNavMeshData returned null.");
                return;
            }

            // 5. Register with NavMesh system and verify triangles were added
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

            // 6. Verify the runtime NavMesh is queryable at room center
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

            // 7. Carve the game's ground-level NavMesh under the building so NPCs
            //    cannot path through the foundation and must use stair links instead.
            if (_foundationHeight > 0f)
            {
                CreateGroundObstacle();
            }

            // 8. Create doorway links
            CreateInteriorDoorwayLinks();
            CreateExteriorDoorwayLinks();

            _isBuilt = true;

            DebugLog.Info($"[NavMeshRepairer] Built NavMesh: {sources.Count} sources, " +
                          $"{_interiorDoorways.Count} interior links, {_exteriorDoorways.Count} exterior links");
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
        /// Remove all NavMesh data and links.
        /// Call when the building is destroyed.
        /// </summary>
        public void Remove()
        {
            if (!_isBuilt) return;

            _navInstance.Remove();

            foreach (NavMeshLinkInstance link in _links)
            {
                link.Remove();
            }
            _links.Clear();

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

            DebugLog.Info("[NavMeshRepairer] Removed NavMesh data and links.");
        }

        #endregion

        #region Private Methods — Ground Carving

        /// <summary>
        /// Create a NavMeshObstacle that carves the game's baked ground-level NavMesh
        /// under the building footprint. Without this, NPCs path on the ground mesh
        /// straight through the foundation instead of using the stair links.
        /// The obstacle is positioned at ground level and sized to the room footprint.
        /// Stairs extend outward beyond the footprint and are not affected.
        /// </summary>
        private void CreateGroundObstacle()
        {
            _obstacleGO = new GameObject("NavMeshObstacle");
            _obstacleGO.transform.SetParent(_buildingRoot);
            _obstacleGO.transform.localPosition = new Vector3(
                _roomSize.x / 2f, -_foundationHeight, _roomSize.z / 2f);

            var obstacle = _obstacleGO.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.center = Vector3.zero;
            obstacle.size = new Vector3(
                _roomSize.x + Constants.NavMesh.ObstacleExpand,
                Constants.NavMesh.ObstacleHeight,
                _roomSize.z + Constants.NavMesh.ObstacleExpand);
            obstacle.carving = true;
        }

        #endregion

        #region Private Methods — Stair Ramps

        /// <summary>
        /// Create invisible ramp colliders at each exterior doorway with stairs.
        /// The ramp provides continuous walkable NavMesh from floor level down to
        /// ground level, so multiple NPCs can walk the slope simultaneously instead
        /// of queuing at a single NavMeshLink portal.
        /// </summary>
        private void CreateStairRamps()
        {
            foreach (ExteriorDoorwayInfo doorway in _exteriorDoorways)
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

                // Width must exceed the NavMeshLink width (3m) plus agent-radius
                // erosion on both edges, otherwise the walkable strip is so narrow
                // that NPCs path to the corner instead of walking up the center.
                float rampWidth = Mathf.Max(doorway.Width, Constants.NavMesh.MinLinkWidth) + Constants.NavMesh.RampErosionBuffer;

                BoxCollider col = rampGO.AddComponent<BoxCollider>();
                col.center = Vector3.zero;
                col.size = new Vector3(rampWidth, Constants.NavMesh.RampColliderThickness, rampLength);

                _rampObjects.Add(rampGO);
            }
        }

        #endregion

        #region Private Methods — Source Collection

        /// <summary>
        /// Scan the building hierarchy for BoxColliders and create NavMeshBuildSources.
        /// Sources are in local building coordinates so BuildNavMeshData can transform
        /// them using the building's world position and rotation.
        /// </summary>
        private List<NavMeshBuildSource> CollectBuildingSources()
        {
            var sources = new List<NavMeshBuildSource>();
            BoxCollider[] colliders = _buildingRoot.GetComponentsInChildren<BoxCollider>();

            foreach (BoxCollider collider in colliders)
            {
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
        /// Compute an encapsulating bounds from collected NavMesh build sources.
        /// </summary>
        private Bounds ComputeBoundsFromSources(List<NavMeshBuildSource> sources)
        {
            // Start with a reasonable default based on room size
            var bounds = new Bounds(
                new Vector3(_roomSize.x / 2f, _roomSize.y / 2f, _roomSize.z / 2f),
                _roomSize);

            foreach (NavMeshBuildSource source in sources)
            {
                // Extract position from the source transform matrix
                Vector3 sourcePos = new Vector3(
                    source.transform.m03,
                    source.transform.m13,
                    source.transform.m23);

                // Expand bounds to include source position + half size
                var sourceBounds = new Bounds(sourcePos, source.size);
                bounds.Encapsulate(sourceBounds);
            }

            return bounds;
        }

        #endregion

        #region Private Methods — Interior Links

        /// <summary>
        /// Create NavMeshLinks at each interior doorway to connect rooms through walls.
        /// </summary>
        private void CreateInteriorDoorwayLinks()
        {
            foreach (DoorwayInfo doorway in _interiorDoorways)
            {
                Vector3 facingDir = doorway.FacesAlongZ ? Vector3.forward : Vector3.right;
                float halfWall = doorway.WallThickness / 2f + Constants.NavMesh.LinkOffset;

                var linkData = new NavMeshLinkData();
                linkData.startPosition = doorway.Center - facingDir * halfWall;
                linkData.endPosition = doorway.Center + facingDir * halfWall;
                linkData.width = doorway.Width;
                linkData.bidirectional = true;
                linkData.area = 0;
                linkData.agentTypeID = _agentTypeID;
                linkData.costModifier = Constants.NavMesh.DefaultLinkCostModifier;

                NavMeshLinkInstance link = NavMesh.AddLink(
                    linkData,
                    _buildingRoot.position,
                    _buildingRoot.rotation);

                _links.Add(link);
            }
        }

        #endregion

        #region Private Methods — Exterior Links

        /// <summary>
        /// Create NavMeshLinks at each exterior doorway to connect interior to exterior NavMesh.
        /// When stairs exist, the link is placed at the stair base (ground level).
        /// Without stairs, the link bridges directly through the wall.
        /// </summary>
        private void CreateExteriorDoorwayLinks()
        {
            foreach (ExteriorDoorwayInfo doorway in _exteriorDoorways)
            {
                float halfWall = doorway.WallThickness / 2f + Constants.NavMesh.LinkOffset;

                Vector3 interiorEnd;
                Vector3 exteriorEnd;

                if (doorway.StairBasePosition.HasValue)
                {
                    // Interior endpoint at floor level (inside the door) — on our runtime NavMesh.
                    // Exterior endpoint at ground level (outside the stair base) — on the game's NavMesh.
                    // The invisible ramp provides walkable surface between floor and ground for
                    // NPCs already on our NavMesh; the link handles the cross-mesh bridge.
                    interiorEnd = doorway.Center + doorway.InwardNormal * halfWall;
                    exteriorEnd = doorway.StairBasePosition.Value
                                  - doorway.InwardNormal * Constants.NavMesh.LinkOffset;
                }
                else
                {
                    // No foundation: link directly through the wall at floor level
                    interiorEnd = doorway.Center + doorway.InwardNormal * halfWall;
                    exteriorEnd = doorway.Center - doorway.InwardNormal * halfWall;
                }

                var linkData = new NavMeshLinkData();
                linkData.startPosition = interiorEnd;
                linkData.endPosition = exteriorEnd;
                linkData.width = Mathf.Max(doorway.Width, Constants.NavMesh.MinLinkWidth);
                linkData.bidirectional = true;
                linkData.area = 0;
                linkData.agentTypeID = _agentTypeID;
                linkData.costModifier = Constants.NavMesh.DefaultLinkCostModifier;

                NavMeshLinkInstance link = NavMesh.AddLink(
                    linkData,
                    _buildingRoot.position,
                    _buildingRoot.rotation);

                _links.Add(link);
            }
        }

        #endregion
    }
}