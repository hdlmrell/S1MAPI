using UnityEngine;
using MAPI.Utils;

namespace MAPI.Core
{
    /// <summary>
    /// Utility for enabling and configuring components on instantiated prefabs.
    /// Provides a clean way to activate game-specific components (like ATM, VendingMachine, etc.)
    /// without requiring MAPI to reference ScheduleOne types.
    /// </summary>
    public static class ComponentEnabler
    {
        /// <summary>
        /// Enable all MonoBehaviour components on a GameObject and its children.
        /// Useful for activating game logic components after instantiation.
        /// </summary>
        /// <param name="gameObject">The GameObject to enable components on</param>
        /// <param name="recursive">Whether to enable components on children (default: true)</param>
        public static void EnableAllComponents(GameObject gameObject, bool recursive = true)
        {
            if (gameObject == null)
            {
                DebugLog.Warning("[ComponentEnabler] Cannot enable components on null GameObject");
                return;
            }

            if (recursive)
            {
                var components = gameObject.GetComponentsInChildren<MonoBehaviour>(includeInactive: true);
                foreach (var component in components)
                {
                    if (component != null)
                    {
                        component.enabled = true;
                    }
                }
            }
            else
            {
                var components = gameObject.GetComponents<MonoBehaviour>();
                foreach (var component in components)
                {
                    if (component != null)
                    {
                        component.enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Enable specific component types by name.
        /// Useful when you only want to enable certain components (e.g., "ATM", "VendingMachine").
        /// </summary>
        /// <param name="gameObject">The GameObject to enable components on</param>
        /// <param name="componentNames">Array of component type names to enable</param>
        /// <param name="recursive">Whether to search children (default: true)</param>
        public static void EnableComponentsByName(GameObject gameObject, string[] componentNames, bool recursive = true)
        {
            if (gameObject == null)
            {
                DebugLog.Warning("[ComponentEnabler] Cannot enable components on null GameObject");
                return;
            }

            if (componentNames == null || componentNames.Length == 0)
            {
                DebugLog.Warning("[ComponentEnabler] No component names provided");
                return;
            }

            var components = recursive 
                ? gameObject.GetComponentsInChildren<MonoBehaviour>(includeInactive: true)
                : gameObject.GetComponents<MonoBehaviour>();

            foreach (var component in components)
            {
                if (component == null) continue;

                string componentTypeName = component.GetType().Name;
                foreach (var name in componentNames)
                {
                    if (componentTypeName == name)
                    {
                        component.enabled = true;
                        DebugLog.Info($"[ComponentEnabler] Enabled component: {componentTypeName}");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Get a component by name without requiring a type reference.
        /// Returns the component as a generic MonoBehaviour.
        /// </summary>
        /// <param name="gameObject">The GameObject to search</param>
        /// <param name="componentName">Name of the component type</param>
        /// <param name="recursive">Whether to search children (default: true)</param>
        /// <returns>The component if found, null otherwise</returns>
        public static MonoBehaviour? GetComponentByName(GameObject gameObject, string componentName, bool recursive = true)
        {
            if (gameObject == null) return null;

            var components = recursive
                ? gameObject.GetComponentsInChildren<MonoBehaviour>(includeInactive: true)
                : gameObject.GetComponents<MonoBehaviour>();

            foreach (var component in components)
            {
                if (component != null && component.GetType().Name == componentName)
                {
                    return component;
                }
            }

            return null;
        }

        /// <summary>
        /// Enable a component and optionally invoke a setup method on it.
        /// Uses reflection to call the method without requiring type knowledge.
        /// </summary>
        /// <param name="gameObject">The GameObject containing the component</param>
        /// <param name="componentName">Name of the component type</param>
        /// <param name="setupMethodName">Optional method name to invoke after enabling</param>
        /// <param name="recursive">Whether to search children (default: true)</param>
        /// <returns>True if component was found and enabled</returns>
        public static bool EnableAndSetup(GameObject gameObject, string componentName, string? setupMethodName = null, bool recursive = true)
        {
            var component = GetComponentByName(gameObject, componentName, recursive);
            if (component == null)
            {
                DebugLog.Warning($"[ComponentEnabler] Component '{componentName}' not found on {gameObject.name}");
                return false;
            }

            component.enabled = true;

            // Optionally invoke a setup method
            if (!string.IsNullOrEmpty(setupMethodName))
            {
                var method = component.GetType().GetMethod(setupMethodName, 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);

                if (method != null)
                {
                    try
                    {
                        method.Invoke(component, null);
                        DebugLog.Info($"[ComponentEnabler] Invoked {setupMethodName} on {componentName}");
                    }
                    catch (System.Exception e)
                    {
                        DebugLog.Error($"[ComponentEnabler] Failed to invoke {setupMethodName}: {e.Message}");
                    }
                }
                else
                {
                    DebugLog.Warning($"[ComponentEnabler] Method '{setupMethodName}' not found on {componentName}");
                }
            }

            return true;
        }
    }
}
