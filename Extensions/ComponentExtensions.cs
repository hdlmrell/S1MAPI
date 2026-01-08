using UnityEngine;

namespace S1MAPI.Extensions
{
    /// <summary>
    /// Extension methods for Component operations.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Get component or null if not found (no exception thrown).
        /// </summary>
        public static T? GetComponent<T>(this Component component) where T : class
        {
            return component.GetComponent<T>();
        }

        /// <summary>
        /// Enable this component.
        /// </summary>
        public static T Enable<T>(this T component) where T : Behaviour
        {
            component.enabled = true;
            return component;
        }

        /// <summary>
        /// Disable this component.
        /// </summary>
        public static T Disable<T>(this T component) where T : Behaviour
        {
            component.enabled = false;
            return component;
        }

        /// <summary>
        /// Get component by type name without requiring type reference.
        /// </summary>
        public static Component? GetComponentByName(this Component component, string componentName, bool includeInactive = false)
        {
            var components = component.GetComponentsInChildren<Component>(includeInactive);
            foreach (var c in components)
            {
                if (c.GetType().Name == componentName)
                {
                    return c;
                }
            }
            return null;
        }
    }
}
