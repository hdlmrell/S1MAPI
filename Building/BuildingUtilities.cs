using S1MAPI.Extensions;
using UnityEngine;
using UnityEngine.AI;
using S1MAPI.Utils;

namespace S1MAPI.Building
{
    /// <summary>
    /// Utility functions for building construction and configuration
    /// </summary>
    public static class BuildingUtilities
    {
        #region Public API - NavMesh
        
        /// <summary>
        /// Add a NavMesh obstacle to a GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject to add the obstacle to</param>
        /// <param name="carving">Whether the obstacle should carve the NavMesh</param>
        /// <param name="carveOnlyStationary">Whether to only carve when stationary</param>
        public static void AddNavMeshObstacle(
            GameObject gameObject,
            bool carving = true,
            bool carveOnlyStationary = false)
        {
            if (gameObject == null)
            {
                DebugLog.Warning("Cannot add NavMeshObstacle to null GameObject");
                return;
            }

            NavMeshObstacle obstacle = gameObject.GetOrAddComponent<NavMeshObstacle>();
            obstacle.carving = carving;
            obstacle.carveOnlyStationary = carveOnlyStationary;

            DebugLog.Info($"Added NavMeshObstacle to: {gameObject.name}");
        }

        /// <summary>
        /// Add NavMesh obstacles to multiple GameObjects
        /// </summary>
        public static void AddNavMeshObstacles(GameObject[] gameObjects, bool carving = true)
        {
            if (gameObjects == null)
            {
                return;
            }

            foreach (GameObject go in gameObjects)
            {
                AddNavMeshObstacle(go, carving);
            }
        }
        
        #endregion

        #region Public API - Colliders
        
        /// <summary>
        /// Remove all colliders from a GameObject
        /// </summary>
        public static void RemoveColliders(GameObject gameObject, bool includeChildren = false)
        {
            if (gameObject == null)
            {
                return;
            }

            if (includeChildren)
            {
                Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                {
                    UnityEngine.Object.Destroy(collider);
                }
            }
            else
            {
                Collider collider = gameObject.GetComponent<Collider>();
                if (collider != null)
                {
                    UnityEngine.Object.Destroy(collider);
                }
            }
        }

        /// <summary>
        /// Setup collider for a primitive based on naming conventions
        /// Removes colliders from decorative or fallback objects
        /// </summary>
        public static void SetupCollider(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            string lowerName = gameObject.name.ToLower();

            // Remove colliders from decorative objects
            if (lowerName.Contains("fallback") || 
                lowerName.Contains("decorative") ||
                lowerName.Contains("rug") ||
                lowerName.Contains("mat"))
            {
                RemoveColliders(gameObject);
            }
        }
        
        #endregion

        #region Public API - Occlusion
        
        /// <summary>
        /// Apply occlusion settings to all renderers in a GameObject
        /// </summary>
        public static void ApplyOcclusionSettings(GameObject gameObject, bool allowOcclusion)
        {
            if (gameObject == null)
            {
                return;
            }

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.allowOcclusionWhenDynamic = allowOcclusion;
            }

            DebugLog.Info($"Set occlusion to {allowOcclusion} for {renderers.Length} renderers in {gameObject.name}");
        }
        
        #endregion

        #region Public API - Grid Snapping
        
        /// <summary>
        /// Snap a position to a grid
        /// </summary>
        /// <param name="position">The position to snap</param>
        /// <param name="gridSize">The grid cell size</param>
        /// <returns>The snapped position</returns>
        public static Vector3 SnapToGrid(Vector3 position, float gridSize = Constants.Spatial.DefaultGridSize)
        {
            return new Vector3(
                Mathf.Round(position.x / gridSize) * gridSize,
                Mathf.Round(position.y / gridSize) * gridSize,
                Mathf.Round(position.z / gridSize) * gridSize
            );
        }
        
        #endregion

        #region Public API - Hierarchy Organization
        
        /// <summary>
        /// Create an empty GameObject to serve as a folder/container
        /// </summary>
        public static GameObject CreateFolder(string name, Transform? parent = null)
        {
            GameObject folder = new GameObject(name);

            if (parent != null)
            {
                folder.transform.SetParent(parent);
                folder.transform.Reset();
            }

            return folder;
        }
        
        #endregion
    }
}
