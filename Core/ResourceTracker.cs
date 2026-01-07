using UnityEngine;
using UnityEngine.SceneManagement;
using MAPI.Utils;

namespace MAPI.Core
{
    /// <summary>
    /// Optional resource tracking system for manual cleanup control.
    /// Provides utility methods to track and cleanup resources if needed.
    /// </summary>
    /// <remarks>
    /// Unity automatically cleans up resources on scene unload and application quit.
    /// This tracker is optional - use it only if you need explicit cleanup control.
    /// Register() calls are lightweight and can be used for debugging resource usage.
    /// </remarks>
    public static class ResourceTracker
    {
        #region Internal Members

        /// <summary>
        /// INTERNAL: Set of all tracked resources.
        /// </summary>
        internal static readonly HashSet<UnityEngine.Object> _trackedResources = new HashSet<UnityEngine.Object>();

        /// <summary>
        /// INTERNAL: Resources associated with specific scenes.
        /// </summary>
        internal static readonly Dictionary<Scene, HashSet<UnityEngine.Object>> _sceneResources = new Dictionary<Scene, HashSet<UnityEngine.Object>>();

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the total number of tracked resources.
        /// </summary>
        public static int TrackedResourceCount =>
            _trackedResources.Count;

        /// <summary>
        /// Register a resource for tracking.
        /// Useful for debugging or when you need manual cleanup control.
        /// </summary>
        /// <param name="resource">The resource to track</param>
        public static void Register(UnityEngine.Object resource)
        {
            if (resource == null)
            {
                DebugLog.Warning("Attempted to register null resource");
                return;
            }

            if (_trackedResources.Add(resource))
            {
                DebugLog.Info($"Registered resource: {resource.name} ({resource.GetType().Name})");
            }
        }

        /// <summary>
        /// Register a resource for tracking and associate it with a scene.
        /// </summary>
        /// <param name="resource">The resource to track</param>
        /// <param name="scene">The scene to associate with</param>
        public static void RegisterForScene(UnityEngine.Object resource, Scene scene)
        {
            if (resource == null)
            {
                DebugLog.Warning("Attempted to register null resource");
                return;
            }

            Register(resource);

            if (!_sceneResources.ContainsKey(scene))
            {
                _sceneResources[scene] = new HashSet<UnityEngine.Object>();
            }

            _sceneResources[scene].Add(resource);
        }

        /// <summary>
        /// Unregister a resource from tracking.
        /// </summary>
        /// <param name="resource">The resource to unregister</param>
        public static void Unregister(UnityEngine.Object resource)
        {
            if (resource == null)
            {
                return;
            }

            if (_trackedResources.Remove(resource))
            {
                DebugLog.Info($"Unregistered resource: {resource.name} ({resource.GetType().Name})");

                // Remove from scene resources
                foreach (HashSet<UnityEngine.Object> sceneEntry in _sceneResources.Values)
                {
                    sceneEntry.Remove(resource);
                }
            }
        }

        /// <summary>
        /// Manually clean up all tracked resources.
        /// Note: Unity already handles cleanup automatically - use only if you need explicit control.
        /// </summary>
        public static void CleanupAll()
        {
            DebugLog.Info($"Cleaning up {_trackedResources.Count} tracked resources");

            int destroyedCount = 0;
            foreach (UnityEngine.Object resource in _trackedResources)
            {
                if (resource != null)
                {
                    UnityEngine.Object.Destroy(resource);
                    destroyedCount++;
                }
            }

            _trackedResources.Clear();
            _sceneResources.Clear();

            DebugLog.Info($"Cleaned up {destroyedCount} resources");
        }

        /// <summary>
        /// Manually clean up resources associated with a specific scene.
        /// Note: Unity already handles cleanup automatically - use only if you need explicit control.
        /// </summary>
        /// <param name="scene">The scene to clean up</param>
        public static void CleanupScene(Scene scene)
        {
            if (!_sceneResources.TryGetValue(scene, out HashSet<UnityEngine.Object> resources))
            {
                return;
            }

            DebugLog.Info($"Cleaning up {resources.Count} resources for scene: {scene.name}");

            int destroyedCount = 0;
            foreach (UnityEngine.Object resource in resources)
            {
                if (resource != null)
                {
                    UnityEngine.Object.Destroy(resource);
                    _trackedResources.Remove(resource);
                    destroyedCount++;
                }
            }

            _sceneResources.Remove(scene);

            DebugLog.Info($"Cleaned up {destroyedCount} resources from scene: {scene.name}");
        }

        /// <summary>
        /// Check if a resource is being tracked.
        /// </summary>
        /// <param name="resource">The resource to check</param>
        /// <returns>True if the resource is tracked, false otherwise</returns>
        public static bool IsTracked(UnityEngine.Object resource) =>
            resource != null && _trackedResources.Contains(resource);

        #endregion
    }
}
