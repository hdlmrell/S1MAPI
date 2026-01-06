using System.Collections.Generic;
using UnityEngine;
using MAPI.Utils;

namespace MAPI.ProceduralMesh.Generators
{
    /// <summary>
    /// Generates cylinder mesh geometry.
    /// </summary>
    internal static class CylinderGenerator
    {
        #region Public API

        /// <summary>
        /// Generate cylinder vertices and triangles.
        /// </summary>
        /// <param name="vertices">List to add vertices to</param>
        /// <param name="triangles">List to add triangles to</param>
        /// <param name="start">Start position (bottom center)</param>
        /// <param name="end">End position (top center)</param>
        /// <param name="radius">Radius of the cylinder</param>
        /// <param name="segments">Number of radial segments</param>
        public static void Generate(
            List<Vector3> vertices,
            List<int> triangles,
            Vector3 start,
            Vector3 end,
            float radius,
            int segments = Constants.Mesh.DefaultCylinderSegments)
        {
            int startVertex = vertices.Count;
            Vector3 direction = (end - start).normalized;

            // Create perpendicular vectors for the circular cross-section
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);
            if (perpendicular.magnitude < 0.001f)
            {
                perpendicular = Vector3.Cross(direction, Vector3.right);
            }
            perpendicular.Normalize();
            Vector3 perpendicular2 = Vector3.Cross(direction, perpendicular).normalized;

            // Add center vertices
            vertices.Add(start); // Bottom center
            vertices.Add(end);   // Top center

            // Add rim vertices
            for (int i = 0; i <= segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                Vector3 offset = (perpendicular * cos + perpendicular2 * sin) * radius;

                vertices.Add(start + offset); // Bottom rim
                vertices.Add(end + offset);   // Top rim
            }

            // Bottom cap triangles
            for (int i = 0; i < segments; i++)
            {
                triangles.Add(startVertex); // Bottom center
                triangles.Add(startVertex + 2 + i * 2);
                triangles.Add(startVertex + 2 + (i + 1) * 2);
            }

            // Top cap triangles
            for (int i = 0; i < segments; i++)
            {
                triangles.Add(startVertex + 1); // Top center
                triangles.Add(startVertex + 3 + (i + 1) * 2);
                triangles.Add(startVertex + 3 + i * 2);
            }

            // Side triangles
            for (int i = 0; i < segments; i++)
            {
                int bottomCurrent = startVertex + 2 + i * 2;
                int bottomNext = startVertex + 2 + (i + 1) * 2;
                int topCurrent = bottomCurrent + 1;
                int topNext = bottomNext + 1;

                triangles.Add(bottomCurrent);
                triangles.Add(topCurrent);
                triangles.Add(bottomNext);

                triangles.Add(bottomNext);
                triangles.Add(topCurrent);
                triangles.Add(topNext);
            }
        }

        #endregion
    }
}
