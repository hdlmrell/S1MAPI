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
        /// </summary>
        /// <returns>The instantiated GameObject or null if prefab not found</returns>
        public GameObject? InstantiateNetworked()
        {
            var instance = Instantiate();
            if (instance == null) return null;

            if (InstanceFinder.NetworkManager != null && InstanceFinder.NetworkManager.IsServer)
            {
                var netObj = instance.GetComponent<NetworkObject>();
                if (netObj != null)
                {
                    InstanceFinder.NetworkManager.ServerManager.Spawn(netObj);
                }
            }

            ResourceTracker.Register(instance);
            return instance;
        }

        #endregion

        #region Operators

        public override string ToString() => Name;

        public static implicit operator string(PrefabRef prefab) => prefab.Name;

        #endregion
    }
}
