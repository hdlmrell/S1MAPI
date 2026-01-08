using UnityEngine;

namespace MAPI.ProceduralMesh.Generators.Primitives
{
    /// <summary>
    /// Generates box (cube) mesh geometry.
    /// </summary>
    internal static class BoxGenerator
    {
        #region Public API

        /// <summary>
        /// Generate box vertices and triangles.
        /// </summary>
        /// <param name="vertices">List to add vertices to</param>
        /// <param name="triangles">List to add triangles to</param>
        /// <param name="center">Center position of the box</param>
        /// <param name="size">Size of the box</param>
        public static void Generate(
            List<Vector3> vertices,
            List<int> triangles,
            Vector3 center,
            Vector3 size)
        {
            int startVertex = vertices.Count;
            Vector3 halfSize = size * 0.5f;

            // Define 8 corners
            Vector3[] corners = new Vector3[]
            {
                center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), // 0
                center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z),  // 1
                center + new Vector3(halfSize.x, -halfSize.y, halfSize.z),   // 2
                center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z),  // 3
                center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z),  // 4
                center + new Vector3(halfSize.x, halfSize.y, -halfSize.z),   // 5
                center + new Vector3(halfSize.x, halfSize.y, halfSize.z),    // 6
                center + new Vector3(-halfSize.x, halfSize.y, halfSize.z)    // 7
            };

            vertices.AddRange(corners);

            // Define 12 triangles (2 per face)
            int[] boxTriangles = new int[]
            {
                // Bottom
                0, 2, 1, 0, 3, 2,
                // Top
                4, 5, 6, 4, 6, 7,
                // Front
                0, 1, 5, 0, 5, 4,
                // Back
                3, 7, 6, 3, 6, 2,
                // Left
                0, 4, 7, 0, 7, 3,
                // Right
                1, 2, 6, 1, 6, 5
            };

            foreach (int tri in boxTriangles)
            {
                triangles.Add(startVertex + tri);
            }
        }

        #endregion
    }
}
