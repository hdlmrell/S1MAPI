using System;
using UnityEngine;
using MAPI.Utils;

namespace MAPI.Core
{
    /// <summary>
    /// Main entry point and initialization for MAPI library
    /// This class handles library initialization and provides global configuration
    /// </summary>
    public static class MAPICore
    {
        private static bool _initialized = false;

        /// <summary>
        /// Gets whether MAPI has been initialized
        /// </summary>
        public static bool IsInitialized => _initialized;

        /// <summary>
        /// Gets the MAPI library version
        /// </summary>
        public static string Version => Constants.LIBRARY_VERSION;

        /// <summary>
        /// Initialize the MAPI library
        /// This should be called by consuming mods during their initialization
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
            {
                DebugLog.Warning("MAPI is already initialized");
                return;
            }

            try
            {
                DebugLog.Info($"Initializing {Constants.LIBRARY_NAME} v{Constants.LIBRARY_VERSION}");
                
                // Initialize subsystems
                InitializeSubsystems();

                _initialized = true;
                DebugLog.Info($"{Constants.LIBRARY_NAME} initialization complete");
            }
            catch (Exception ex)
            {
                DebugLog.Error($"Failed to initialize {Constants.LIBRARY_NAME}");
                DebugLog.Exception(ex);
                throw;
            }
        }

        /// <summary>
        /// Shutdown the MAPI library
        /// This should be called by consuming mods during their cleanup
        /// </summary>
        public static void Shutdown()
        {
            if (!_initialized)
            {
                DebugLog.Warning("MAPI is not initialized");
                return;
            }

            try
            {
                DebugLog.Info($"Shutting down {Constants.LIBRARY_NAME}");
                
                // Cleanup subsystems
                CleanupSubsystems();

                _initialized = false;
                DebugLog.Info($"{Constants.LIBRARY_NAME} shutdown complete");
            }
            catch (Exception ex)
            {
                DebugLog.Error($"Error during {Constants.LIBRARY_NAME} shutdown");
                DebugLog.Exception(ex);
            }
        }

        private static void InitializeSubsystems()
        {
            // Initialize various MAPI subsystems
            DebugLog.Info("Initializing subsystems...");
            
            // Initialize resource management
            ResourceTracker.Initialize();
            
            // Future: Initialize building system
            // Future: Initialize additional subsystems
        }

        private static void CleanupSubsystems()
        {
            // Cleanup various MAPI subsystems
            DebugLog.Info("Cleaning up subsystems...");
            
            // Cleanup resource management
            ResourceTracker.Shutdown();
            
            // Future: Cleanup building system
            // Future: Cleanup additional subsystems
        }
    }
}
