using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MAPI.Utils;

namespace MAPI.Core
{
    /// <summary>
    /// Automatic resource tracking and cleanup system.
    /// Prevents memory leaks by tracking created resources and cleaning them up when needed.
    /// </summary>
    /// <remarks>
    /// Resources registered with this tracker will be automatically destroyed when scenes unload
    /// or when the application quits. Use RegisterForScene to associate resources with specific scenes.
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

        /// <summary>
        /// INTERNAL: Whether the tracker has been initialized.
        /// </summary>
        internal static bool _initialized = false;

        /// <summary>
        /// INTERNAL: Initialize the resource tracker.
        /// Called by MAPICore during library initialization.
        /// </summary>
        internal static void Initialize()
        {
            if (_initialized)
            {
                DebugLog.Warning("ResourceTracker is already initialized");
                return;
            }

            SceneManager.sceneUnloaded += OnSceneUnloaded;
            Application.quitting += OnApplicationQuitting;

            _initialized = true;
            DebugLog.Info("ResourceTracker initialized");
        }

        /// <summary>
        /// INTERNAL: Shutdown the resource tracker.
        /// Called by MAPICore during library shutdown.
        /// </summary>
        internal static void Shutdown()
        {
            if (!_initialized)
            {
                return;
            }

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            Application.quitting -= OnApplicationQuitting;

            CleanupAll();

            _initialized = false;
            DebugLog.Info("ResourceTracker shutdown");
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Event triggered when a scene is being cleaned up.
        /// </summary>
        public static event Action<Scene> OnSceneCleanup;

        /// <summary>
        /// Event triggered when the application is quitting.
        /// </summary>
        public static event Action OnApplicationQuit;

        /// <summary>
        /// Gets the total number of tracked resources.
        /// </summary>
        public static int TrackedResourceCount =>
            _trackedResources.Count;

        /// <summary>
        /// Register a resource for tracking.
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
        /// Clean up all tracked resources.
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
        /// Clean up resources associated with a specific scene.
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

            OnSceneCleanup?.Invoke(scene);

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

        #region Private Members

        /// <summary>
        /// INTERNAL: Handler for scene unload events.
        /// </summary>
        private static void OnSceneUnloaded(Scene scene)
        {
            CleanupScene(scene);
        }

        /// <summary>
        /// INTERNAL: Handler for application quit event.
        /// </summary>
        private static void OnApplicationQuitting()
        {
            OnApplicationQuit?.Invoke();
            CleanupAll();
        }

        #endregion
    }
}
