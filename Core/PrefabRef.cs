using System;
using System.Reflection;
using UnityEngine;
using MAPI.Utils;

#if IL2CPP
using Il2CppFishNet;
using Il2CppFishNet.Managing;
using Il2CppFishNet.Managing.Object;
using Il2CppFishNet.Object;
#else
using FishNet;
using FishNet.Managing.Object;
using FishNet.Object;
#endif

namespace MAPI.Core
{
    /// <summary>
    /// Reference to a network-spawnable prefab in the game.
    /// These are full GameObjects with components (NetworkObject, interaction scripts, etc.).
    /// For static mesh-only assets (decoration), use MeshRef instead.
    /// Use MAPI.S1.Prefabs for known Schedule 1 prefab assets.
    /// </summary>
    public sealed class PrefabRef
    {
        #region Properties

        /// <summary>The prefab name as registered in FishNet</summary>
        public string Name { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Create a prefab reference.
        /// </summary>
        /// <param name="name">Exact prefab name in FishNet registry</param>
        public PrefabRef(string name)
        {
            Name = name;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Find the prefab GameObject from FishNet's spawnable prefabs.
        /// </summary>
        /// <returns>The prefab GameObject or null if not found</returns>
        public GameObject? Find()
        {
            try
            {
                var networkManager = InstanceFinder.NetworkManager;
                if (networkManager == null) return null;

                var spawnablePrefabs = networkManager.GetPrefabObjects<PrefabObjects>(0, false);
                if (spawnablePrefabs == null) return null;

                int count = spawnablePrefabs.GetObjectCount();
                for (int i = 0; i < count; i++)
                {
                    NetworkObject obj = spawnablePrefabs.GetObject(true, i);
                    if (obj != null && obj.gameObject != null && obj.gameObject.name == Name)
                    {
                        return obj.gameObject;
                    }
                }
            }
            catch (System.Exception e)
            {
                DebugLog.Error($"[PrefabRef] Error finding prefab '{Name}': {e.Message}");
            }
            return null;
        }

        /// <summary>
        /// Instantiate this prefab locally (no network spawning).
        /// </summary>
        /// <returns>The instantiated GameObject or null if prefab not found</returns>
        public GameObject? Instantiate()
        {
            var prefab = Find();
            if (prefab == null)
            {
                DebugLog.Warning($"[PrefabRef] Could not find prefab: {Name}");
                return null;
            }
            return UnityEngine.Object.Instantiate(prefab);
        }

        /// <summary>
        /// Instantiate and spawn on the network (server only).
        /// Instantiates the prefab as inactive to prevent Awake() from running with uninitialized 
        /// network state, initializes any GUID fields, spawns it on the network, then activates it.
        /// This fixes issues with prefabs like ATM that require valid GUIDs in Awake().
        /// </summary>
        /// <returns>The instantiated GameObject or null if prefab not found</returns>
        public GameObject? InstantiateNetworked()
        {
            var prefab = Find();
            if (prefab == null)
            {
                DebugLog.Warning($"[PrefabRef] Could not find prefab: {Name}");
                return null;
            }

            // Store original active state
            bool originalState = prefab.activeSelf;
            
            // Temporarily disable the prefab to prevent Awake() during instantiation
            prefab.SetActive(false);

            // Instantiate with components inactive
            GameObject? instance = UnityEngine.Object.Instantiate(prefab);

            // Restore prefab's original state
            prefab.SetActive(originalState);

            if (instance == null) return null;

            // Initialize GUID fields on components before Awake() runs
            // This fixes prefabs like ATM that parse GUIDs in Awake()
            InitializeGuidFields(instance);

            // Spawn on network (assigns network GUID and calls OnStartServer)
            if (InstanceFinder.NetworkManager != null && InstanceFinder.NetworkManager.IsServer)
            {
                var netObj = instance.GetComponent<NetworkObject>();
                if (netObj != null)
                {
                    InstanceFinder.NetworkManager.ServerManager.Spawn(netObj);
                }
            }

            // Now activate the instance - Awake() will run with valid GUIDs
            instance.SetActive(true);

            ResourceTracker.Register(instance);
            return instance;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize GUID/guid string fields on all components to prevent parse errors in Awake().
        /// Uses reflection to find and populate empty GUID fields without requiring ScheduleOne references.
        /// </summary>
        private static void InitializeGuidFields(GameObject instance)
        {
            var components = instance.GetComponentsInChildren<MonoBehaviour>(includeInactive: true);
            foreach (var component in components)
            {
                if (component == null) continue;

                var type = component.GetType();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var field in fields)
                {
                    try
                    {
                        // Handle string fields named "guid" or "GUID" that are empty or contain empty GUID
                        if (field.FieldType == typeof(string) && 
                            (field.Name.Equals("guid", StringComparison.OrdinalIgnoreCase) ||
                             field.Name.Contains("guid", StringComparison.OrdinalIgnoreCase)))
                        {
                            var value = field.GetValue(component) as string;
                            if (string.IsNullOrEmpty(value) || value == "00000000-0000-0000-0000-000000000000")
                            {
                                field.SetValue(component, Guid.NewGuid().ToString());
                                DebugLog.Info($"[PrefabRef] Initialized GUID field '{field.Name}' on {type.Name}");
                            }
                        }
                        // Handle System.Guid fields
                        else if (field.FieldType == typeof(Guid))
                        {
                            if (field.GetValue(component) is Guid value)
                            {
                                if (value == Guid.Empty)
                                {
                                    field.SetValue(component, Guid.NewGuid());
                                    DebugLog.Info($"[PrefabRef] Initialized Guid field '{field.Name}' on {type.Name}");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        DebugLog.Warning($"[PrefabRef] Failed to initialize field '{field.Name}' on {type.Name}: {e.Message}");
                    }
                }
            }
        }

        #endregion

        #region Operators

        public override string ToString() => Name;

        public static implicit operator string(PrefabRef prefab) => prefab.Name;

        #endregion
    }
}
