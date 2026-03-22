using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using S1MAPI.Utils;
using UnityEngine;
using UnityEngine.AI;

#if IL2CPP
using Il2CppInterop.Runtime;
#endif

namespace S1MAPI.Building
{
    /// <summary>
    /// Core logic for NPC interior navigation using custom A* pathfinding.
    /// <para>
    /// This is a plain C# class (not a MonoBehaviour) to avoid IL2CPP ClassInjector
    /// warnings for methods with custom parameter types. The thin
    /// <see cref="InteriorNavigator"/> MonoBehaviour shell forwards Unity lifecycle
    /// calls (<c>Update</c>, <c>OnDestroy</c>) to this class.
    /// </para>
    /// </summary>
    internal sealed class InteriorNavigatorCore
    {
        #region Nested Types

        private enum NPCNavState
        {
            Approaching,     // NavMeshAgent walking to doorway exterior
            Entering,        // Agent disabled, lerping through doorway inward
            Inside,          // Following A* path via Transform movement
            Exiting,         // A* path to doorway interior point
            LeavingDoorway   // Lerping through doorway outward
        }

        private sealed class TrackedNPC
        {
            public Component NpcComponent;
            public NPCNavState State;
            public NavDoorwayInfo TargetDoorway;
            public Vector3 DoorwayExteriorWorld;
            public Vector3 DoorwayInteriorWorld;
            public List<Vector3>? Path;
            public int PathIndex;
            public Vector3 TargetLocal;
            public Transform? ChaseTarget;
            public float ChaseRepathTimer;
            public float Speed;
            public Action? OnArrival;
            public float LerpProgress;
            public float LerpDuration;
            public Vector3 LerpStart;
            public Vector3 LerpEnd;
            public Vector3? PendingExteriorDestination; // set when NPC exits to resume navigation
            public float RepathTimer;     // fallback re-pathfind timer for all NPCs
            public float StuckTimer;      // time NPC hasn't moved
            public float ApproachStartTime; // Time.time when Approaching state began

            // Cached reflection results
            public object? MovementRef;    // NPCMovement instance
            public NavMeshAgent? Agent;
            public Collider? NpcCollider;  // disabled while inside to prevent pushing player
        }

        /// <summary>
        /// Cross-platform member accessor for game types.
        /// On Mono, game members are fields; on IL2CPP (Il2CppInterop), they become properties.
        /// Tries property first, then falls back to field — works on both runtimes.
        /// </summary>
        private readonly struct MemberAccessor
        {
            private readonly FieldInfo? _field;
            private readonly PropertyInfo? _prop;

            public bool IsValid =>
                _field != null || _prop != null;

            public MemberAccessor(Type type, string name, BindingFlags flags)
            {
                _prop = type.GetProperty(name, flags);
                _field = _prop == null ? type.GetField(name, flags) : null;
            }

            public object? GetValue(object target) =>
                _field != null ? _field.GetValue(target) : _prop?.GetValue(target);

            public void SetValue(object target, object? value)
            {
                if (_field != null) _field.SetValue(target, value);
                else _prop?.SetValue(target, value);
            }
        }

        #endregion

        #region Fields

        private readonly InteriorPathGrid _grid;
        private readonly IReadOnlyList<NavDoorwayInfo> _doorways;
        private readonly Transform _buildingRoot;
        private readonly Vector3 _roomSize;
        private readonly float _foundationHeight;

        private readonly Dictionary<Component, TrackedNPC> _tracked =
            new Dictionary<Component, TrackedNPC>();
        private readonly List<Component> _removeQueue = new List<Component>();
        private readonly Dictionary<Component, Vector3> _lastPositions = new Dictionary<Component, Vector3>();
        private float _doorwayScanTimer;
        private float _approachLogTimer;
        private static readonly Collider[] _scanBuffer = new Collider[32];

        // Prevent NPC from being managed by two buildings simultaneously
        private static readonly HashSet<Component> _globallyManaged = new HashSet<Component>();

        // Static registry of all active buildings (for auto-detection patch)
        private static readonly List<InteriorNavigatorCore> _activeBuildings = new List<InteriorNavigatorCore>();
        private static bool _patchApplied;

        // Reflection cache (resolved once)
        private static bool _reflectionResolved;
        private static Type? _npcMovementType;
        private static MemberAccessor _agentAccessor;
        private static MethodInfo? _setAgentEnabled;
        private static MemberAccessor _walkSpeedAccessor;
        private static MemberAccessor _runSpeedAccessor;
        private static MemberAccessor _speedScaleAccessor;
        private static MemberAccessor _moveSpeedMultAccessor;
        private static MemberAccessor _hasDestinationAccessor;
        private static MethodInfo? _originalSetDestination;
        private static Harmony? _harmony;

        #endregion

        #region Constructor

        /// <summary>
        /// Create and initialize the interior navigation core.
        /// </summary>
        public InteriorNavigatorCore(
            InteriorPathGrid grid,
            IReadOnlyList<NavDoorwayInfo> doorways,
            Transform buildingRoot,
            Vector3 roomSize,
            float foundationHeight)
        {
            _grid = grid;
            _doorways = doorways;
            _buildingRoot = buildingRoot;
            _roomSize = roomSize;
            _foundationHeight = foundationHeight;

            ResolveReflection();

            _activeBuildings.Add(this);
            ApplyPatchIfNeeded();
        }

        #endregion

        #region Reflection & Patching

        private static void ResolveReflection()
        {
            if (_reflectionResolved) return;
            _reflectionResolved = true;

            // On IL2CPP, Il2CppInterop prefixes game namespaces with "Il2Cpp"
#if IL2CPP
            const string typeName = "Il2CppScheduleOne.NPCs.NPCMovement";
#else
            const string typeName = "ScheduleOne.NPCs.NPCMovement";
#endif
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                _npcMovementType = asm.GetType(typeName);
                if (_npcMovementType != null) break;
            }
            if (_npcMovementType == null)
            {
                DebugLog.Warning("[InteriorNavigator] NPCMovement type not found.");
                return;
            }

            // MemberAccessor tries GetProperty first (IL2CPP), falls back to GetField (Mono)
            const BindingFlags pub = BindingFlags.Public | BindingFlags.Instance;
            _agentAccessor = new MemberAccessor(_npcMovementType, "Agent", pub);
            _setAgentEnabled = _npcMovementType.GetMethod("SetAgentEnabled", pub);
            _walkSpeedAccessor = new MemberAccessor(_npcMovementType, "WalkSpeed", pub);
            _runSpeedAccessor = new MemberAccessor(_npcMovementType, "RunSpeed", pub);
            _speedScaleAccessor = new MemberAccessor(_npcMovementType, "MovementSpeedScale", pub);
            _moveSpeedMultAccessor = new MemberAccessor(_npcMovementType, "MoveSpeedMultiplier", pub);
            _hasDestinationAccessor = new MemberAccessor(_npcMovementType, "HasDestination", pub);
        }

        private static void ApplyPatchIfNeeded()
        {
            if (_patchApplied || _npcMovementType == null) return;
            _patchApplied = true;

            // Find the 4-param public SetDestination(Vector3, Action<WalkResult>, float, float)
            MethodInfo? target = null;
            foreach (MethodInfo m in _npcMovementType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (m.Name != "SetDestination") continue;
                ParameterInfo[] pars = m.GetParameters();
                if (pars.Length == 4 && pars[0].ParameterType == typeof(Vector3))
                {
                    target = m;
                    break;
                }
            }

            if (target == null)
            {
                DebugLog.Warning("[InteriorNavigator] Could not find SetDestination to patch.");
                return;
            }

            _originalSetDestination = target;

            _harmony = new Harmony("com.s1mapi.interiornavigator");

            // Patch SetDestination — intercept all NPC destination calls
            MethodInfo setDestPrefix = typeof(InteriorNavigatorCore).GetMethod(
                nameof(SetDestinationPrefix),
                BindingFlags.NonPublic | BindingFlags.Static)!;
            _harmony.Patch(target, prefix: new HarmonyMethod(setDestPrefix));

            // Patch UpdateDestination — prevent private 5-param SetDestination from
            // overwriting our agent destination for managed NPCs.
            // UpdateDestination is called from FixedUpdate and bypasses our SetDestination patch.
            MethodInfo? updateDest = _npcMovementType.GetMethod("UpdateDestination",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (updateDest != null)
            {
                MethodInfo updateDestPrefix = typeof(InteriorNavigatorCore).GetMethod(
                    nameof(UpdateDestinationPrefix),
                    BindingFlags.NonPublic | BindingFlags.Static)!;
                _harmony.Patch(updateDest, prefix: new HarmonyMethod(updateDestPrefix));
                DebugLog.Info("[InteriorNavigator] Patched UpdateDestination.");
            }
            else
            {
                DebugLog.Warning("[InteriorNavigator] Could not find UpdateDestination to patch.");
            }

            DebugLog.Info("[InteriorNavigator] Patched NPCMovement.SetDestination for auto-interception.");
        }

        #endregion

        #region Harmony Patch

        /// <summary>
        /// Harmony prefix on <c>NPCMovement.SetDestination</c>. Automatically intercepts
        /// NPCs whose destination falls inside a registered building and redirects them
        /// through the custom A* interior pathfinding system.
        /// <para>
        /// Always blocks the original for managed NPCs — we send the agent to the
        /// doorway ourselves. The UpdateDestination patch prevents the game's FixedUpdate
        /// from overwriting our agent destination.
        /// </para>
        /// </summary>
        private static bool SetDestinationPrefix(object __instance, Vector3 pos)
        {
            Component? movement = __instance as Component;
            if (movement == null) return true;

            // Check if destination is inside any registered building (0.5m margin prevents
            // oscillation when player is near building wall — slight position offsets won't
            // cause immediate exit/re-entry cycles)
            for (int i = 0; i < _activeBuildings.Count; i++)
            {
                InteriorNavigatorCore nav = _activeBuildings[i];
                if (nav == null || nav._buildingRoot == null) continue;

                Vector3 localPos = nav._buildingRoot.InverseTransformPoint(pos);
                if (!nav.IsInsideBuilding(localPos, margin: 0.5f))
                {
                    // Extended: combat AI resolves targets inside the building to NavMesh
                    // points at the carving boundary (~0.8m outside). Catch when player is inside.
                    if (!nav.IsInsideBuilding(localPos, margin: 3f) || !nav.IsPlayerInside())
                        continue;
                }

                // Clamp near-boundary positions to interior
                localPos.x = Mathf.Clamp(localPos.x, 0f, nav._roomSize.x);
                localPos.z = Mathf.Clamp(localPos.z, 0f, nav._roomSize.z);

                // Detect chase: if destination is near the player, set up continuous tracking
                Transform? chaseTarget = DetectChaseTarget(pos);

                // Already tracked by this building — update target, always block original
                if (nav._tracked.TryGetValue(movement, out TrackedNPC? existing))
                {
                    existing.TargetLocal = localPos;
                    existing.OnArrival = null;
                    if (chaseTarget != null)
                        existing.ChaseTarget = chaseTarget;

                    if (existing.State == NPCNavState.Inside)
                        nav.ComputePathToTarget(existing);

                    // Re-send agent to doorway if still approaching — the agent's path
                    // may have been consumed or cleared since the initial send.
                    if (existing.State == NPCNavState.Approaching &&
                        existing.Agent != null &&
                        existing.Agent.enabled &&
                        existing.Agent.isOnNavMesh)
                    {
                        existing.Agent.SetDestination(existing.DoorwayExteriorWorld);
                    }

                    return false;
                }

                // New NPC — register and send agent to doorway ourselves
                DebugLog.Info($"[InteriorNavigator] Tracking new NPC {movement.name} " +
                             $"targeting local ({localPos.x:F1}, {localPos.z:F1})");

                TrackedNPC tracked = nav.CreateTrackedNPC(movement);
                tracked.TargetLocal = localPos;
                tracked.ChaseTarget = chaseTarget;
                tracked.ChaseRepathTimer = 0f;
                tracked.OnArrival = null;

                NavDoorwayInfo door = nav.FindNearestExteriorDoorway(
                    nav._buildingRoot.InverseTransformPoint(movement.transform.position));
                tracked.TargetDoorway = door;
                nav.ComputeDoorwayPoints(tracked, door);
                tracked.State = NPCNavState.Approaching;
                tracked.ApproachStartTime = Time.time;

                // Send agent to doorway exterior directly — don't let the game send it
                // to the interior position (which the agent can't reach due to carving)
                if (tracked.Agent != null && tracked.Agent.enabled && tracked.Agent.isOnNavMesh)
                    tracked.Agent.SetDestination(tracked.DoorwayExteriorWorld);

                nav._tracked[movement] = tracked;
                _globallyManaged.Add(movement);

                return false; // block original — we've set the agent destination ourselves
            }

            // Destination not inside any building — check if this NPC is tracked and should exit
            if (_globallyManaged.Contains(movement))
            {
                for (int i = 0; i < _activeBuildings.Count; i++)
                {
                    InteriorNavigatorCore nav = _activeBuildings[i];
                    if (nav == null) continue;

                    if (nav._tracked.TryGetValue(movement, out TrackedNPC? data))
                    {
                        // During Approaching: only release if destination is clearly
                        // far from the building. Near-building destinations (carving
                        // boundary points from GetRandomReachablePointNear) should
                        // NOT release — the NPC is still pursuing a target inside.
                        if (data.State == NPCNavState.Approaching)
                        {
                            Vector3 destLocal = nav._buildingRoot.InverseTransformPoint(pos);
                            if (!nav.IsInsideBuilding(destLocal, margin: 5f))
                            {
                                _globallyManaged.Remove(movement);
                                nav._tracked.Remove(movement);
                                return true; // genuinely far — let original run
                            }
                            return false; // near building — keep tracking, block game
                        }

                        // Destination is outside building — recall NPC to exit
                        data.PendingExteriorDestination = pos;
                        nav.RecallNPC(movement);
                        return false; // suppress original while NPC exits the building
                    }
                }
            }

            return true; // not our concern, run original
        }

        /// <summary>
        /// Harmony prefix on <c>NPCMovement.UpdateDestination</c>. Skips the method
        /// entirely for managed NPCs — prevents the private 5-param SetDestination
        /// (called from FixedUpdate) from overwriting our agent destination.
        /// </summary>
        private static bool UpdateDestinationPrefix(object __instance)
        {
            Component? movement = __instance as Component;
            if (movement != null && _globallyManaged.Contains(movement))
                return false; // skip — we're managing this NPC's destination
            return true;
        }

        /// <summary>
        /// Check if the destination is near any player (indicating a chase scenario).
        /// Returns the nearest player's Transform if so, null otherwise.
        /// Uses <see cref="Camera.allCameras"/> to support multiplayer.
        /// </summary>
        private static Transform? DetectChaseTarget(Vector3 destination)
        {
            foreach (Camera cam in Camera.allCameras)
            {
                Vector3 camPos = cam.transform.position;
                float dx = destination.x - camPos.x;
                float dz = destination.z - camPos.z;
                float distSq = dx * dx + dz * dz;

                // If destination is within 3m of a player, this is a chase
                if (distSq < 9f)
                    return cam.transform;
            }

            return null;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Send an NPC to a position inside the building. The NPC walks to the nearest
        /// doorway on exterior NavMesh, enters via lerp, then follows A* path to target.
        /// </summary>
        public void SendNPCToPosition(Component npc, Vector3 localTarget, Action? onArrival)
        {
            // Already tracked by this building — update target in place
            if (_tracked.TryGetValue(npc, out TrackedNPC? existing))
            {
                existing.TargetLocal = localTarget;
                existing.OnArrival = onArrival;
                existing.ChaseTarget = null;
                if (existing.State == NPCNavState.Inside)
                    ComputePathToTarget(existing);
                return;
            }

            if (_globallyManaged.Contains(npc))
            {
                DebugLog.Warning("[InteriorNavigator] NPC is already managed by another building.");
                return;
            }

            TrackedNPC tracked = CreateTrackedNPC(npc);
            tracked.TargetLocal = localTarget;
            tracked.OnArrival = onArrival;
            tracked.ChaseTarget = null;

            BeginApproach(tracked);
            SendAgentToDoorway(tracked);
            _tracked[npc] = tracked;
            _globallyManaged.Add(npc);

            DebugLog.Info($"[InteriorNavigator] Sending NPC to local ({localTarget.x:F1}, {localTarget.z:F1})");
        }

        /// <summary>
        /// Send an NPC to chase a moving target inside the building.
        /// Re-pathfinds at <see cref="Constants.InteriorNav.ChaseRepathInterval"/>.
        /// </summary>
        public void SendNPCToChase(Component npc, Transform target)
        {
            if (_globallyManaged.Contains(npc))
            {
                DebugLog.Warning("[InteriorNavigator] NPC is already managed by a building.");
                return;
            }

            SendNPCToChaseInternal(npc, target);
        }

        private void SendNPCToChaseInternal(Component npc, Transform target)
        {
            Vector3 targetLocal = _buildingRoot.InverseTransformPoint(target.position);

            TrackedNPC tracked = CreateTrackedNPC(npc);
            tracked.TargetLocal = targetLocal;
            tracked.ChaseTarget = target;
            tracked.ChaseRepathTimer = 0f;
            tracked.OnArrival = null;

            BeginApproach(tracked);
            SendAgentToDoorway(tracked);
            _tracked[npc] = tracked;
            _globallyManaged.Add(npc);

            DebugLog.Info("[InteriorNavigator] NPC chasing player — continuous tracking enabled.");
        }

        /// <summary>
        /// Directly set the NavMeshAgent destination to the doorway exterior.
        /// Used by the consumer API (SendNPCToPosition/SendNPCToChase) where the
        /// game's behavior system isn't involved. The Harmony prefix path doesn't
        /// need this — the game's own SetDestination handles agent routing.
        /// </summary>
        private static void SendAgentToDoorway(TrackedNPC data)
        {
            if (data.Agent != null && data.Agent.enabled && data.Agent.isOnNavMesh)
                data.Agent.SetDestination(data.DoorwayExteriorWorld);
        }

        /// <summary>
        /// Recall an NPC from the building. If inside, begins exit via A* path to doorway.
        /// If still approaching, releases immediately.
        /// </summary>
        public void RecallNPC(Component npc)
        {
            if (!_tracked.TryGetValue(npc, out TrackedNPC? data)) return;

            if (data.State == NPCNavState.Approaching)
            {
                // Still on exterior NavMesh — just release
                ReleaseNPC(npc, data, warpToExterior: false);
            }
            else if (data.State != NPCNavState.Exiting &&
                     data.State != NPCNavState.LeavingDoorway)
            {
                BeginExit(npc, data);
            }
        }

        /// <summary>Whether this navigator is currently managing the given NPC.</summary>
        public bool IsTracking(Component npc) =>
            _tracked.ContainsKey(npc);

        /// <summary>
        /// Show/hide the walkability grid visualization (green = walkable, red = blocked).
        /// </summary>
        public void VisualizeGrid(bool show)
        {
            if (show)
                _grid.Visualize();
            else
                _grid.DestroyVisualization();
        }

        /// <summary>
        /// Diagnose why a specific grid cell is blocked. Logs details to console.
        /// Cell coordinates are shown in the visualization quad names (Cell_X_Z).
        /// </summary>
        public void DiagnoseCell(int gx, int gz) =>
            _grid.DiagnoseCell(gx, gz);

        /// <summary>
        /// Release all tracked NPCs. Warps those inside to the exterior and re-enables agents.
        /// Called automatically on building destruction.
        /// </summary>
        public void ReleaseAllNPCs()
        {
            foreach (KeyValuePair<Component, TrackedNPC> kvp in _tracked)
            {
                if (kvp.Key == null) continue;
                TrackedNPC data = kvp.Value;

                if (data.State != NPCNavState.Approaching)
                {
                    // NPC is inside — warp to exterior
                    if (data.DoorwayExteriorWorld != Vector3.zero)
                        kvp.Key.transform.position = data.DoorwayExteriorWorld;
                    if (data.NpcCollider != null)
                        data.NpcCollider.enabled = true;
                    EnableAgent(data);
                }

                _globallyManaged.Remove(kvp.Key);
            }
            _tracked.Clear();
        }

        #endregion

        #region Update Loop

        public void Update()
        {
            // Periodically scan for untracked NPCs near doorways.
            // Catches NPCs whose SetDestination resolved to a NavMesh point outside
            // the building (carving boundary) — the prefix can't intercept those.
            _doorwayScanTimer -= Time.deltaTime;
            if (_doorwayScanTimer <= 0f)
            {
                _doorwayScanTimer = 0.5f;
                ScanForNearbyNPCs();
            }

            if (_tracked.Count == 0) return;

            _removeQueue.Clear();

            foreach (KeyValuePair<Component, TrackedNPC> kvp in _tracked)
            {
                Component npc = kvp.Key;
                TrackedNPC data = kvp.Value;

                if (npc == null)
                {
                    _removeQueue.Add(npc!);
                    continue;
                }

                switch (data.State)
                {
                    case NPCNavState.Approaching:
                        UpdateApproaching(npc, data);
                        break;
                    case NPCNavState.Entering:
                        UpdateLerp(npc, data, onComplete: () =>
                        {
                            // Check if we still need phase 2 (through doorway)
                            float distToInterior = Vector3.Distance(
                                npc.transform.position, data.DoorwayInteriorWorld);
                            if (distToInterior > 0.5f)
                            {
                                // Phase 1 complete — now lerp through the doorway
                                data.Speed = GetNPCSpeed(data);
                                data.LerpStart = npc.transform.position;
                                data.LerpEnd = data.DoorwayInteriorWorld;
                                float dist = Vector3.Distance(data.LerpStart, data.LerpEnd);
                                data.LerpDuration = Mathf.Max(
                                    dist / Mathf.Max(data.Speed, 1f), 0.1f);
                                data.LerpProgress = 0f;
                            }
                            else
                            {
                                data.State = NPCNavState.Inside;
                                DebugLog.Info("[InteriorNavigator] NPC entered building, computing A* path...");
                                ComputePathToTarget(data);
                            }
                        });
                        break;
                    case NPCNavState.Inside:
                        UpdateInside(npc, data);
                        break;
                    case NPCNavState.Exiting:
                        UpdatePathFollow(npc, data, onComplete: () =>
                            BeginDoorwayLeave(npc, data));
                        break;
                    case NPCNavState.LeavingDoorway:
                        UpdateLerp(npc, data, onComplete: () =>
                            ReleaseNPC(npc, data, warpToExterior: false));
                        break;
                }
            }

            foreach (Component npc in _removeQueue)
            {
                _globallyManaged.Remove(npc);
                _tracked.Remove(npc);
                _lastPositions.Remove(npc);
            }
        }

        #endregion

        #region Doorway Proximity Scan

        /// <summary>
        /// Detect untracked NPCs standing near doorways while the player is inside.
        /// The game's combat AI resolves destinations to NavMesh carving boundary points
        /// (outside the building), so the SetDestination prefix may not intercept them.
        /// This scan catches those NPCs by proximity instead.
        /// </summary>
        private void ScanForNearbyNPCs()
        {
            if (_npcMovementType == null) return;

            // Only scan if any player is inside this building (multiplayer-safe)
            Transform? insidePlayer = FindPlayerInside();
            if (insidePlayer == null) return;

            Vector3 playerLocal = _buildingRoot.InverseTransformPoint(insidePlayer.position);
            float threshold = Constants.InteriorNav.DoorwayApproachThreshold;

            for (int d = 0; d < _doorways.Count; d++)
            {
                NavDoorwayInfo door = _doorways[d];
                if (door.IsInterior) continue;

                // Scan at ground level — cop stands at terrain height, not elevated floor
                Vector3 scanCenter = door.Center;
                scanCenter.y = door.StairBasePosition.HasValue
                    ? door.StairBasePosition.Value.y
                    : -_foundationHeight;
                Vector3 doorWorld = _buildingRoot.TransformPoint(scanCenter);
                int count = Physics.OverlapSphereNonAlloc(
                    doorWorld, threshold, _scanBuffer, Physics.AllLayers,
                    QueryTriggerInteraction.Collide);

                for (int c = 0; c < count; c++)
                {
                    if (_scanBuffer[c] == null) continue;

                    Component? movement = FindComponentInParent(_scanBuffer[c], _npcMovementType);
                    if (movement == null) continue;
                    if (_globallyManaged.Contains(movement)) continue;
                    if (_tracked.ContainsKey(movement)) continue;

                    DebugLog.Info($"[InteriorNavigator] Auto-detected NPC {movement.name} near doorway, registering...");

                    TrackedNPC tracked = CreateTrackedNPC(movement);
                    tracked.TargetLocal = playerLocal;
                    tracked.ChaseTarget = insidePlayer;
                    tracked.ChaseRepathTimer = 0f;
                    tracked.OnArrival = null;
                    tracked.TargetDoorway = door;
                    ComputeDoorwayPoints(tracked, door);

                    _tracked[movement] = tracked;
                    _globallyManaged.Add(movement);

                    // NPC is already at the doorway — skip Approaching, enter immediately
                    BeginDoorwayEntry(movement, tracked);
                }
            }
        }

        #endregion

        #region State: Approaching

        private void BeginApproach(TrackedNPC data)
        {
            // Find nearest exterior doorway to the NPC's current position
            Vector3 npcLocal = _buildingRoot.InverseTransformPoint(data.NpcComponent.transform.position);
            NavDoorwayInfo door = FindNearestExteriorDoorway(npcLocal);
            data.TargetDoorway = door;
            ComputeDoorwayPoints(data, door);

            data.State = NPCNavState.Approaching;
            data.ApproachStartTime = Time.time;

            DebugLog.Info($"[InteriorNavigator] NPC approaching doorway at {data.DoorwayExteriorWorld}");
        }

        private void UpdateApproaching(Component npc, TrackedNPC data)
        {
            Vector3 npcPos = npc.transform.position;
            float threshold = Constants.InteriorNav.DoorwayApproachThreshold;

            // Check distance to target doorway
            float distXZ = HorizontalDistance(npcPos, data.DoorwayExteriorWorld);

            if (distXZ < threshold)
            {
                DebugLog.Info($"[InteriorNavigator] NPC reached doorway (distXZ={distXZ:F2}), entering...");
                BeginDoorwayEntry(npc, data);
                return;
            }

            // Check all exterior doorways — NPC may be closer to a different one
            // (e.g., routed around the building by NavMesh)
            for (int i = 0; i < _doorways.Count; i++)
            {
                NavDoorwayInfo door = _doorways[i];
                if (door.IsInterior || door == data.TargetDoorway) continue;

                Vector3 doorWorld = _buildingRoot.TransformPoint(door.Center);
                float altDist = HorizontalDistance(npcPos, doorWorld);
                if (altDist < threshold)
                {
                    DebugLog.Info($"[InteriorNavigator] NPC near alternate doorway (dist={altDist:F2}), switching...");
                    data.TargetDoorway = door;
                    ComputeDoorwayPoints(data, door);
                    BeginDoorwayEntry(npc, data);
                    return;
                }
            }

            // Approach timeout — if the agent has been trying for too long, force entry
            // at the nearest doorway. Uses 2-phase lerp so NPC walks smoothly to the
            // doorway exterior before entering (not a warp).
            float elapsed = Time.time - data.ApproachStartTime;
            if (elapsed > 12f)
            {
                Vector3 npcLocal = _buildingRoot.InverseTransformPoint(npcPos);
                NavDoorwayInfo nearest = FindNearestExteriorDoorway(npcLocal);
                data.TargetDoorway = nearest;
                ComputeDoorwayPoints(data, nearest);

                float nearestDist = HorizontalDistance(npcPos, data.DoorwayExteriorWorld);
                if (nearestDist < 12f) // Only force if within reasonable distance
                {
                    DebugLog.Warning($"[InteriorNavigator] Approach timeout ({elapsed:F0}s), " +
                                     $"forcing entry (dist={nearestDist:F1}m)...");
                    BeginDoorwayEntry(npc, data);
                    return;
                }
            }

            if (data.Agent == null) return;

            bool pathPending = data.Agent.pathPending;
            bool hasPath = data.Agent.hasPath;
            float remaining = data.Agent.remainingDistance;
            bool remainingValid = !float.IsInfinity(remaining) && !float.IsNaN(remaining);

            // Agent finished its path near-ish to doorway — enter
            if (!pathPending && remainingValid && remaining < 0.5f && distXZ < 8f)
            {
                DebugLog.Info($"[InteriorNavigator] Agent path done near doorway (distXZ={distXZ:F2}), entering...");
                BeginDoorwayEntry(npc, data);
                return;
            }

            // Re-send agent to doorway when it has no active path
            if (!pathPending && !hasPath && data.Agent.enabled && data.Agent.isOnNavMesh)
            {
                data.Agent.SetDestination(data.DoorwayExteriorWorld);
            }

            // Periodic diagnostic logging
            _approachLogTimer -= Time.deltaTime;
            if (_approachLogTimer <= 0f)
            {
                _approachLogTimer = 3f;
                DebugLog.Info($"[InteriorNavigator] Approach: distXZ={distXZ:F2}, " +
                             $"elapsed={elapsed:F0}s, hasPath={hasPath}, remaining={remaining:F2}");
            }
        }

        private static float HorizontalDistance(Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dz = a.z - b.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        #endregion

        #region State: Entering / Leaving (Doorway Lerp)

        private void BeginDoorwayEntry(Component npc, TrackedNPC data)
        {
            // Stop the agent directly (avoid NPCMovement.Stop() which fires stale callbacks)
            if (data.Agent != null)
            {
                data.Agent.isStopped = true;
                data.Agent.ResetPath();
            }
            DisableAgent(data);

            // Disable NPC collider to prevent pushing the player while inside
            if (data.NpcCollider != null)
                data.NpcCollider.enabled = false;

            // Clear HasDestination so game's UpdateDestination() doesn't run
            // and warp the NPC back to the NavMesh surface
            ClearHasDestination(data);

            data.Speed = GetNPCSpeed(data);
            data.LerpStart = npc.transform.position;

            // 2-phase entry: if NPC is far from the doorway exterior, first walk to the
            // exterior point (phase 1), then through the doorway (phase 2). This prevents
            // wall clipping when entry triggers from an angle.
            float distToExterior = Vector3.Distance(npc.transform.position, data.DoorwayExteriorWorld);
            data.LerpEnd = distToExterior > 1.0f
                ? data.DoorwayExteriorWorld   // Phase 1: walk to exterior
                : data.DoorwayInteriorWorld;  // Already near exterior, go straight through

            float distance = Vector3.Distance(data.LerpStart, data.LerpEnd);
            data.LerpDuration = Mathf.Max(distance / Mathf.Max(data.Speed, 1f), 0.1f);
            data.LerpProgress = 0f;
            data.State = NPCNavState.Entering;
        }

        private void BeginDoorwayLeave(Component npc, TrackedNPC data)
        {
            data.Speed = GetNPCSpeed(data);
            data.LerpStart = npc.transform.position;
            data.LerpEnd = data.DoorwayExteriorWorld;
            float distance = Vector3.Distance(data.LerpStart, data.LerpEnd);
            data.LerpDuration = Mathf.Max(distance / Mathf.Max(data.Speed, 1f), 0.1f);
            data.LerpProgress = 0f;
            data.State = NPCNavState.LeavingDoorway;
        }

        private void UpdateLerp(Component npc, TrackedNPC data, Action onComplete)
        {
            data.LerpProgress += Time.deltaTime / data.LerpDuration;
            float t = Mathf.Clamp01(data.LerpProgress);
            float smooth = t * t * (3f - 2f * t); // smoothstep
            npc.transform.position = Vector3.Lerp(data.LerpStart, data.LerpEnd, smooth);

            // Face movement direction
            RotateToward(npc.transform, data.LerpEnd - data.LerpStart);

            if (t >= 1f)
            {
                npc.transform.position = data.LerpEnd;
                onComplete();
            }
        }

        #endregion

        #region State: Inside

        private void UpdateInside(Component npc, TrackedNPC data)
        {
            Vector3 pos = npc.transform.position;

            // Chase mode: periodically re-pathfind toward moving target.
            // Two-stage null check: first tests C# reference (was a target assigned?),
            // second tests Unity's operator== (was the GameObject destroyed at runtime?).
            if (data.ChaseTarget != null)
            {
                if (data.ChaseTarget == null)
                {
                    BeginExit(npc, data);
                    return;
                }

                Vector3 targetLocal = _buildingRoot.InverseTransformPoint(data.ChaseTarget.position);
                if (!IsInsideBuilding(targetLocal))
                {
                    BeginExit(npc, data);
                    return;
                }

                // Stopping distance: don't move when already close enough to target.
                // Prevents oscillation from overshooting + re-pathing.
                float distToTarget = Vector3.Distance(pos, data.ChaseTarget.position);
                if (distToTarget < Constants.InteriorNav.DestinationArrivalThreshold)
                {
                    // Close enough — just face the target and idle
                    RotateToward(npc.transform, data.ChaseTarget.position - pos);
                    data.Path = null;
                    data.ChaseRepathTimer = Constants.InteriorNav.ChaseRepathInterval;
                    return;
                }

                data.ChaseRepathTimer -= Time.deltaTime;
                if (data.ChaseRepathTimer <= 0f)
                {
                    data.ChaseRepathTimer = Constants.InteriorNav.ChaseRepathInterval;
                    data.TargetLocal = targetLocal;
                    ComputePathToTarget(data);
                }
            }

            // Fallback: re-pathfind periodically even without chase target.
            // This handles the case where the game stops calling SetDestination
            // after we cleared HasDestination (NPC would otherwise stand forever).
            data.RepathTimer -= Time.deltaTime;
            if (data.RepathTimer <= 0f)
            {
                data.RepathTimer = 0.5f;
                if (data.Path == null || data.PathIndex >= data.Path.Count)
                {
                    ComputePathToTarget(data);
                    if (data.Path != null)
                        DebugLog.Info($"[InteriorNavigator] Fallback re-path found {data.Path.Count} waypoints, speed={data.Speed:F1}");
                }
            }

            // Stuck detection — if NPC hasn't moved for 2s, recompute path
            if (data.Path != null && data.PathIndex < data.Path.Count)
            {
                float movedSq = (pos - _lastPositions.GetValueOrDefault(npc, pos)).sqrMagnitude;
                if (movedSq < 0.01f) // less than 0.1m moved
                {
                    data.StuckTimer += Time.deltaTime;
                    if (data.StuckTimer > 1.5f)
                    {
                        data.StuckTimer = 0f;
                        DebugLog.Warning($"[InteriorNavigator] NPC stuck, recomputing path. speed={data.Speed:F1}");
                        ComputePathToTarget(data);
                    }
                }
                else
                {
                    data.StuckTimer = 0f;
                }
            }
            _lastPositions[npc] = pos;

            UpdatePathFollow(npc, data, onComplete: () =>
            {
                if (data.ChaseTarget == null)
                {
                    // Directed mode: arrived at destination
                    data.OnArrival?.Invoke();
                    data.OnArrival = null;
                    // NPC stays until RecallNPC
                }
                // Chase mode: will re-pathfind next interval
            });
        }

        #endregion

        #region State: Exiting

        private void BeginExit(Component npc, TrackedNPC data)
        {
            Vector3 currentLocal = _buildingRoot.InverseTransformPoint(npc.transform.position);
            NavDoorwayInfo door = FindNearestExteriorDoorway(currentLocal);
            data.TargetDoorway = door;
            ComputeDoorwayPoints(data, door);

            // Pathfind to doorway interior point
            data.Path = _grid.FindPath(currentLocal, _buildingRoot.InverseTransformPoint(data.DoorwayInteriorWorld));
            data.PathIndex = 0;
            data.State = NPCNavState.Exiting;
            data.ChaseTarget = null;
        }

        #endregion

        #region Path Following

        private void UpdatePathFollow(Component npc, TrackedNPC data, Action onComplete)
        {
            if (data.Path == null || data.PathIndex >= data.Path.Count)
            {
                onComplete();
                return;
            }

            // Refresh speed each frame (catches walk→run transitions in chase mode)
            data.Speed = GetNPCSpeed(data);

            Vector3 target = data.Path[data.PathIndex];
            Vector3 pos = npc.transform.position;

            // Move at floor height
            float floorY = _buildingRoot.TransformPoint(Vector3.zero).y;
            target.y = floorY;
            Vector3 newPos = Vector3.MoveTowards(pos, target, data.Speed * Time.deltaTime);
            newPos.y = floorY;
            npc.transform.position = newPos;

            // Face movement direction
            RotateToward(npc.transform, target - pos);

            // Check waypoint arrival — advance through multiple waypoints per frame
            // if speed is high enough (prevents slow cell-by-cell crawl)
            float distSq = (newPos.x - target.x) * (newPos.x - target.x) +
                           (newPos.z - target.z) * (newPos.z - target.z);

            float threshold = (data.PathIndex == data.Path.Count - 1)
                ? Constants.InteriorNav.DestinationArrivalThreshold
                : Constants.InteriorNav.WaypointArrivalThreshold;

            if (distSq < threshold * threshold)
            {
                data.PathIndex++;
                // Skip ahead through close waypoints in the same frame
                while (data.PathIndex < data.Path.Count - 1)
                {
                    Vector3 next = data.Path[data.PathIndex];
                    next.y = floorY;
                    float nextDistSq = (newPos.x - next.x) * (newPos.x - next.x) +
                                       (newPos.z - next.z) * (newPos.z - next.z);
                    if (nextDistSq < threshold * threshold)
                        data.PathIndex++;
                    else
                        break;
                }
            }
        }

        private void ComputePathToTarget(TrackedNPC data)
        {
            Vector3 currentLocal = _buildingRoot.InverseTransformPoint(
                data.NpcComponent.transform.position);
            data.Path = _grid.FindPath(currentLocal, data.TargetLocal);
            data.PathIndex = 0;

            if (data.Path == null)
            {
                DebugLog.Warning("[InteriorNavigator] No path found to target " +
                                  $"({data.TargetLocal.x:F1}, {data.TargetLocal.z:F1})");
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Cross-platform GetComponent by <see cref="System.Type"/>.
        /// IL2CPP requires <c>Il2CppSystem.Type</c>; this converts automatically.
        /// </summary>
        private static Component? FindComponent(Component target, Type type)
        {
#if IL2CPP
            return target.GetComponent(Il2CppType.From(type));
#else
            return target.GetComponent(type);
#endif
        }

        /// <summary>
        /// Cross-platform GetComponentInChildren by <see cref="System.Type"/>.
        /// </summary>
        private static Component? FindComponentInChildren(Component target, Type type)
        {
#if IL2CPP
            return target.GetComponentInChildren(Il2CppType.From(type));
#else
            return target.GetComponentInChildren(type);
#endif
        }

        /// <summary>
        /// Cross-platform GetComponentInParent by <see cref="System.Type"/>.
        /// </summary>
        private static Component? FindComponentInParent(Component target, Type type)
        {
#if IL2CPP
            return target.GetComponentInParent(Il2CppType.From(type));
#else
            return target.GetComponentInParent(type);
#endif
        }

        private void RotateToward(Transform t, Vector3 direction)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) return;

            Quaternion targetRot = Quaternion.LookRotation(direction.normalized, Vector3.up);
            t.rotation = Quaternion.RotateTowards(
                t.rotation, targetRot,
                Constants.InteriorNav.RotationSpeed * Time.deltaTime);
        }

        private NavDoorwayInfo FindNearestExteriorDoorway(Vector3 localPos)
        {
            NavDoorwayInfo? best = null;
            float bestDist = float.MaxValue;
            foreach (NavDoorwayInfo d in _doorways)
            {
                if (d.IsInterior) continue;
                float dist = Vector3.Distance(localPos, d.Center);
                if (dist < bestDist) { bestDist = dist; best = d; }
            }
            return best!;
        }

        private void ComputeDoorwayPoints(TrackedNPC data, NavDoorwayInfo door)
        {
            // Exterior point: well outside the carving zone on surviving NavMesh.
            // Must be far enough that the NavMesh agent can path to it without
            // routing along the carving boundary (which causes corner-sticking).
            float extOffset = door.WallThickness / 2f + 2.5f;
            Vector3 extLocal = door.Center - door.InwardNormal * extOffset;
            extLocal.y = door.StairBasePosition.HasValue
                ? door.StairBasePosition.Value.y
                : -_foundationHeight;

            Vector3 extWorld = _buildingRoot.TransformPoint(extLocal);

            // Don't snap to NavMesh — SamplePosition can move the point around building
            // corners, causing the agent to route along the carving boundary and get stuck.
            // The agent's SetDestination internally snaps to the nearest reachable NavMesh.

            // Interior point: inside the wall on the floor
            float intOffset = door.WallThickness / 2f + 0.3f;
            Vector3 intLocal = door.Center + door.InwardNormal * intOffset;
            intLocal.y = 0f;

            data.DoorwayExteriorWorld = extWorld;
            data.DoorwayInteriorWorld = _buildingRoot.TransformPoint(intLocal);
        }

        private bool IsInsideBuilding(Vector3 localPos, float margin = 0f)
        {
            return localPos.x >= -margin && localPos.x <= _roomSize.x + margin &&
                   localPos.z >= -margin && localPos.z <= _roomSize.z + margin;
        }

        /// <summary>
        /// Check if any player is inside this building.
        /// Uses <see cref="Camera.allCameras"/> to support multiplayer.
        /// </summary>
        private bool IsPlayerInside() =>
            FindPlayerInside() != null;

        /// <summary>
        /// Find the first player transform inside this building, or null if none.
        /// Uses <see cref="Camera.allCameras"/> to support multiplayer — each player has a camera.
        /// </summary>
        private Transform? FindPlayerInside()
        {
            foreach (Camera cam in Camera.allCameras)
            {
                Vector3 playerLocal = _buildingRoot.InverseTransformPoint(cam.transform.position);
                if (IsInsideBuilding(playerLocal, margin: 1f))
                    return cam.transform;
            }
            return null;
        }

        private TrackedNPC CreateTrackedNPC(Component npc)
        {
            var tracked = new TrackedNPC { NpcComponent = npc };

            // Resolve NPCMovement and NavMeshAgent via reflection
            if (_npcMovementType != null)
            {
                Component? movement = FindComponent(npc, _npcMovementType);
                if (movement == null)
                    movement = FindComponentInChildren(npc, _npcMovementType);
                if (movement == null)
                    movement = FindComponentInParent(npc, _npcMovementType);

                if (movement != null)
                {
                    tracked.MovementRef = movement;
                    if (_agentAccessor.IsValid)
                        tracked.Agent = _agentAccessor.GetValue(movement) as NavMeshAgent;
                }
            }

            // Find the NPC's physics collider (CapsuleCollider on a child GameObject).
            // Disabled while inside to prevent NPCs from pushing the player.
            tracked.NpcCollider = npc.GetComponentInChildren<CapsuleCollider>();

            tracked.Speed = GetNPCSpeed(tracked);
            return tracked;
        }

        private float GetNPCSpeed(TrackedNPC data)
        {
            if (data.MovementRef == null) return 3.5f;

            float walkSpeed = 1.8f;
            float runSpeed = 7f;
            float scale = 0f;
            float multiplier = 1f;

            try
            {
                if (_walkSpeedAccessor.IsValid)
                {
                    object? val = _walkSpeedAccessor.GetValue(data.MovementRef);
                    if (val is float f) walkSpeed = f;
                }
                if (_runSpeedAccessor.IsValid)
                {
                    object? val = _runSpeedAccessor.GetValue(data.MovementRef);
                    if (val is float f) runSpeed = f;
                }
                if (_speedScaleAccessor.IsValid)
                {
                    object? val = _speedScaleAccessor.GetValue(data.MovementRef);
                    if (val is float f) scale = f;
                }
                if (_moveSpeedMultAccessor.IsValid)
                {
                    object? val = _moveSpeedMultAccessor.GetValue(data.MovementRef);
                    if (val is float f) multiplier = f;
                }
            }
            catch (Exception ex)
            {
                DebugLog.Warning($"[InteriorNavigator] Failed to read NPC speed via reflection: {ex.Message}");
            }

            return Mathf.Lerp(walkSpeed, runSpeed, scale) * multiplier;
        }

        private void ReleaseNPC(Component npc, TrackedNPC data, bool warpToExterior)
        {
            if (warpToExterior && data.DoorwayExteriorWorld != Vector3.zero)
                npc.transform.position = data.DoorwayExteriorWorld;

            // Re-enable collider before releasing back to game control
            if (data.NpcCollider != null)
                data.NpcCollider.enabled = true;

            EnableAgent(data);

            // Must remove from global set BEFORE invoking SetDestination
            // so the Harmony prefix lets the call through to the original method.
            _globallyManaged.Remove(npc);
            _removeQueue.Add(npc);

            // Resume navigation to pending exterior destination if one was set
            // (e.g. game called SetDestination(outside) while NPC was inside)
            if (data.PendingExteriorDestination.HasValue &&
                data.MovementRef != null &&
                _originalSetDestination != null)
            {
                try
                {
                    // Parameters: (Vector3 destination, Action<WalkResult> callback, float walkSpeedMult, float runSpeedMult)
                    _originalSetDestination.Invoke(
                        data.MovementRef,
                        new object?[] { data.PendingExteriorDestination.Value, null, 1f, 1f });
                }
                catch (Exception ex)
                {
                    DebugLog.Warning($"[InteriorNavigator] Failed to set pending destination: {ex.Message}");
                }
            }

            DebugLog.Info("[InteriorNavigator] NPC released from building.");
        }

        private void DisableAgent(TrackedNPC data)
        {
            if (data.MovementRef != null && _setAgentEnabled != null)
            {
                try { _setAgentEnabled.Invoke(data.MovementRef, new object[] { false }); }
                catch (Exception ex) { DebugLog.Warning($"[InteriorNavigator] DisableAgent failed: {ex.Message}"); }
            }
        }

        private void ClearHasDestination(TrackedNPC data)
        {
            if (data.MovementRef != null && _hasDestinationAccessor.IsValid)
            {
                try { _hasDestinationAccessor.SetValue(data.MovementRef, false); }
                catch (Exception ex) { DebugLog.Warning($"[InteriorNavigator] ClearHasDestination failed: {ex.Message}"); }
            }
        }

        private void EnableAgent(TrackedNPC data)
        {
            if (data.MovementRef != null && _setAgentEnabled != null)
            {
                try { _setAgentEnabled.Invoke(data.MovementRef, new object[] { true }); }
                catch (Exception ex) { DebugLog.Warning($"[InteriorNavigator] EnableAgent failed: {ex.Message}"); }
            }
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Clean up this navigator instance. Unregisters from the active buildings list,
        /// releases all NPCs, and unpatches Harmony when no buildings remain.
        /// Safe to call multiple times.
        /// </summary>
        public void Cleanup()
        {
            _activeBuildings.Remove(this);
            ReleaseAllNPCs();

            // Unpatch when no buildings remain
            if (_activeBuildings.Count == 0 && _harmony != null)
            {
                _harmony.UnpatchSelf();
                _harmony = null;
                _patchApplied = false;
                DebugLog.Info("[InteriorNavigator] Unpatched NPCMovement.SetDestination (no active buildings).");
            }
        }

        #endregion
    }
}
