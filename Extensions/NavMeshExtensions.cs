using UnityEngine;
using UnityEngine.AI;

namespace S1MAPI.Extensions
{
    /// <summary>
    /// Extension methods for NavMesh operations.
    /// </summary>
    public static class NavMeshExtensions
    {
        /// <summary>
        /// Add or get a NavMeshObstacle component.
        /// </summary>
        public static NavMeshObstacle AddNavMeshObstacle(this GameObject gameObject, bool carving = true)
        {
            NavMeshObstacle obstacle = gameObject.GetOrAddComponent<NavMeshObstacle>();
            obstacle.carving = carving;
            return obstacle;
        }

        /// <summary>
        /// Configure NavMeshObstacle with fluent API.
        /// </summary>
        public static NavMeshObstacle WithCarving(this NavMeshObstacle obstacle, bool carving = true)
        {
            obstacle.carving = carving;
            return obstacle;
        }

        /// <summary>
        /// Configure NavMeshObstacle to carve only when stationary.
        /// </summary>
        public static NavMeshObstacle WithCarveOnlyStationary(this NavMeshObstacle obstacle, bool carveOnly = true)
        {
            obstacle.carveOnlyStationary = carveOnly;
            return obstacle;
        }
    }
}
