using UnityEngine;

namespace MAPI.Building
{
    /// <summary>
    /// Base class for building construction components
    /// Provides common functionality for constructing buildings at runtime
    /// </summary>
    public abstract class BuildingComponent
    {
        /// <summary>
        /// Gets the name of this building component
        /// </summary>
        public abstract string ComponentName { get; }

        /// <summary>
        /// Initialize the building component
        /// </summary>
        public virtual void Initialize()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Build the component and return the root GameObject
        /// </summary>
        /// <returns>The root GameObject of the built component</returns>
        public abstract GameObject Build();

        /// <summary>
        /// Cleanup resources used by this component
        /// </summary>
        public virtual void Cleanup()
        {
            // Override in derived classes
        }
    }
}
