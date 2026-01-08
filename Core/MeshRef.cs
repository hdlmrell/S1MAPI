using UnityEngine;
using MAPI.Utils;
using System;

namespace MAPI.Core
{
    /// <summary>
    /// Reference to a static mesh asset in the game (no game logic, just visuals).
    /// Unlike PrefabRef, these are raw meshes without NetworkObject or interaction components.
    /// Use MAPI.S1.Meshes for known Schedule 1 mesh assets.
    /// </summary>
    public sealed class MeshRef
    {
        #region Properties

        /// <summary>The mesh asset name in the game</summary>
        public string Name { get; }

        /// <summary>Cached mesh instance</summary>
        private Mesh? _cachedMesh;

        /// <summary>Cached source GameObject (the original asset)</summary>
        private GameObject? _cachedSource;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a mesh reference.
        /// </summary>
        /// <param name="name">Exact mesh/GameObject name in the game assets (e.g., "SM_Prop_Vase_02")</param>
        public MeshRef(string name)
        {
            Name = name;
        }

        #endregion

        #region Public API - Finding

        /// <summary>
        /// Find the source GameObject containing this mesh.
        /// Searches through all loaded objects in the scene/resources.
        /// </summary>
        /// <returns>The source GameObject or null if not found</returns>
        public GameObject? FindSource()
        {
            if (_cachedSource != null) return _cachedSource;

            try
            {
                // Search all GameObjects including inactive ones
                var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (var obj in allObjects)
                {
                    if (obj.name == Name)
                    {
                        // Verify it has a mesh
                        var mf = obj.GetComponent<MeshFilter>();
                        if (mf != null && mf.sharedMesh != null)
                        {
                            _cachedSource = obj;
                            _cachedMesh = mf.sharedMesh;
                            return obj;
                        }
                    }
                }

                // Fallback: search by mesh name in MeshFilters
                var allMeshFilters = Resources.FindObjectsOfTypeAll<MeshFilter>();
                foreach (var mf in allMeshFilters)
                {
                    if (mf.sharedMesh != null && mf.sharedMesh.name == Name)
                    {
                        _cachedSource = mf.gameObject;
                        _cachedMesh = mf.sharedMesh;
                        return mf.gameObject;
                    }
                }
            }
            catch (System.Exception e)
            {
                DebugLog.Error($"[MeshRef] Error finding mesh '{Name}': {e.Message}");
            }

            return null;
        }

        /// <summary>
        /// Get the Mesh asset directly.
        /// </summary>
        /// <returns>The Mesh or null if not found</returns>
        public Mesh? GetMesh()
        {
            if (_cachedMesh != null) return _cachedMesh;
            FindSource();
            return _cachedMesh;
        }

        #endregion

        #region Public API - Instantiation

        /// <summary>
        /// Create a new GameObject with just the mesh visuals (no game components).
        /// </summary>
        /// <param name="name">Name for the new GameObject</param>
        /// <param name="parent">Optional parent transform</param>
        /// <param name="addCollider">Whether to add a BoxCollider (defaults to true)</param>
        /// <returns>New GameObject with MeshFilter and MeshRenderer, or null if mesh not found</returns>
        public GameObject? Instantiate(string? name = null, Transform? parent = null, bool addCollider = true)
        {
            var mesh = GetMesh();
            if (mesh == null)
            {
                DebugLog.Warning($"[MeshRef] Could not find mesh: {Name}");
                return null;
            }

            var source = FindSource();
            
            GameObject instance = new GameObject(name ?? Name);
            if (parent != null) instance.transform.SetParent(parent);

            // Add mesh components
            var mf = instance.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            var mr = instance.AddComponent<MeshRenderer>();
            
            // Copy materials from source if available
            if (source != null)
            {
                var sourceMr = source.GetComponent<MeshRenderer>();
                if (sourceMr != null)
                {
                    mr.sharedMaterials = sourceMr.sharedMaterials;
                }
            }

            // Add collider by default for physics interaction
            if (addCollider)
            {
                instance.AddComponent<BoxCollider>();
            }
            
            return instance;
        }

        /// <summary>
        /// Create a new GameObject with the mesh at a specific position/rotation.
        /// </summary>
        /// <param name="name">Name for the new GameObject</param>
        /// <param name="localPosition">Local position</param>
        /// <param name="localRotation">Local rotation</param>
        /// <param name="parent">Parent transform</param>
        /// <param name="addCollider">Whether to add a BoxCollider (defaults to true)</param>
        /// <returns>New GameObject with mesh, or null if mesh not found</returns>
        public GameObject? Instantiate(string name, Vector3 localPosition, Quaternion localRotation, Transform? parent = null, bool addCollider = true)
        {
            var instance = Instantiate(name, parent, addCollider);
            if (instance == null) return null;

            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = localRotation;

            return instance;
        }

        /// <summary>
        /// Create a new GameObject with the mesh, applying a custom material.
        /// </summary>
        /// <param name="name">Name for the new GameObject</param>
        /// <param name="material">Material to apply</param>
        /// <param name="parent">Optional parent transform</param>
        /// <param name="addCollider">Whether to add a BoxCollider (defaults to true)</param>
        /// <returns>New GameObject with mesh and custom material, or null if mesh not found</returns>
        public GameObject? InstantiateWithMaterial(string name, Material material, Transform? parent = null, bool addCollider = true)
        {
            var instance = Instantiate(name, parent, addCollider);
            if (instance == null) return null;

            var mr = instance.GetComponent<MeshRenderer>();
            if (mr != null) mr.material = material;

            return instance;
        }

        /// <summary>
        /// Clone the entire source GameObject (mesh + hierarchy but NOT game-specific components).
        /// This copies the visual structure but strips NetworkObject, interaction scripts, etc.
        /// </summary>
        /// <param name="name">Name for the clone</param>
        /// <param name="parent">Optional parent transform</param>
        /// <param name="stripColliders">Whether to remove colliders (default: true)</param>
        /// <returns>Cloned GameObject or null if source not found</returns>
        public GameObject? CloneVisuals(string? name = null, Transform? parent = null, bool stripColliders = true)
        {
            var source = FindSource();
            if (source == null)
            {
                DebugLog.Warning($"[MeshRef] Could not find source for cloning: {Name}");
                return null;
            }

            GameObject clone = UnityEngine.Object.Instantiate(source);
            clone.name = name ?? $"{Name}_Clone";

            // Strip colliders if requested
            if (stripColliders)
            {
                foreach (Collider c in clone.GetComponentsInChildren<Collider>(true))
                {
                    UnityEngine.Object.Destroy(c);
                }
            }

            if (parent != null) clone.transform.SetParent(parent);

            return clone;
        }

        #endregion

        #region Static Helpers

        /// <summary>
        /// Quick static helper to clone an object with modifications.
        /// </summary>
        public static GameObject? Clone(GameObject original, Vector3 position, Quaternion rotation, Transform? parent = null, bool stripColliders = true)
        {
            if (original == null)
            {
                DebugLog.Error("Cannot clone null object");
                return null;
            }

            GameObject clone = UnityEngine.Object.Instantiate(original, position, rotation, parent);
            clone.name = $"{original.name}_Clone";

            if (stripColliders)
            {
                foreach (Collider c in clone.GetComponentsInChildren<Collider>(true))
                {
                    UnityEngine.Object.Destroy(c);
                }
            }

            return clone;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Returns the mesh name as a string.
        /// </summary>
        public override string ToString() => Name;

        /// <summary>
        /// Implicitly converts MeshRef to its underlying mesh name string.
        /// </summary>
        /// <param name="mesh">The mesh reference to convert.</param>
        public static implicit operator string(MeshRef mesh) => mesh.Name;

        #endregion
    }
}
