using UnityEngine;
using S1MAPI.Core;
using S1MAPI.Utils;

namespace S1MAPI.ProceduralMesh.Generators.Organic
{
    /// <summary>
    /// Abstract base class for creating organic shape generators (animals, plants, etc.).
    /// Provides helper methods for common organic mesh patterns like rings and joints.
    /// </summary>
    /// <remarks>
    /// S1MAPI focuses on building/map construction. This class provides an extension point
    /// for mods that need organic shapes. Inherit from this class to create custom generators.
    /// 
    /// Example usage:
    /// <code>
    /// public class PetMeshGenerator : OrganicShapeGenerator
    /// {
    ///     protected override void GenerateGeometry(List{Vector3} vertices, List{int} triangles)
    ///     {
    ///         // Use helper methods like AddRing, AddJointRing, ConnectRings
    ///         AddRing(vertices, position: Vector3.zero, radius: 1f, segments: 12);
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class OrganicShapeGenerator
    {
        #region Fields

        private readonly string _name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new organic shape generator.
        /// </summary>
        /// <param name="name">Name for the generated mesh</param>
        protected OrganicShapeGenerator(string name)
        {
            _name = name;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Generate and return a complete mesh.
        /// </summary>
        /// <returns>The generated mesh</returns>
        public Mesh Generate()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            GenerateGeometry(vertices, triangles, uvs);

            Mesh mesh = new Mesh
            {
                name = _name,
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };

            if (uvs.Count > 0)
            {
                mesh.uv = uvs.ToArray();
            }

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            DebugLog.Info($"Generated organic shape mesh: {_name} ({vertices.Count} vertices)");

            return mesh;
        }

        /// <summary>
        /// Generate geometry directly into the provided lists.
        /// Useful for integrating with ProceduralMeshBuilder.
        /// </summary>
        /// <param name="vertices">List to add vertices to</param>
        /// <param name="triangles">List to add triangles to</param>
        /// <param name="uvs">List to add UVs to</param>
        /// <param name="offset">Position offset for all vertices</param>
        public void GenerateInto(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, Vector3 offset = default)
        {
            int startVertex = vertices.Count;
            
            GenerateGeometry(vertices, triangles, uvs);

            // Apply offset to newly added vertices
            if (offset != Vector3.zero)
            {
                for (int i = startVertex; i < vertices.Count; i++)
                {
                    vertices[i] += offset;
                }
            }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Override this method to implement your organic shape generation logic.
        /// Use the helper methods provided by this base class.
        /// </summary>
        /// <param name="vertices">List to add vertices to</param>
        /// <param name="triangles">List to add triangle indices to</param>
        /// <param name="uvs">List to add UV coordinates to (optional)</param>
        protected abstract void GenerateGeometry(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs);

        #endregion

        #region Protected Helper Methods

        /// <summary>
        /// Add a ring of vertices (circular cross-section).
        /// Useful for creating cylindrical body segments.
        /// </summary>
        /// <param name="vertices">List to add vertices to</param>
        /// <param name="position">Center position of the ring</param>
        /// <param name="radius">Radius of the ring</param>
        /// <param name="segments">Number of segments around the ring</param>
        /// <param name="ellipticalScale">Optional elliptical scaling (x, z)</param>
        protected void AddRing(
            List<Vector3> vertices,
            Vector3 position,
            float radius,
            int segments,
            Vector2 ellipticalScale = default)
        {
            if (ellipticalScale == default)
            {
                ellipticalScale = Vector2.one;
            }

            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * 2f * Mathf.PI;
                float x = Mathf.Cos(angle) * radius * ellipticalScale.x;
                float z = Mathf.Sin(angle) * radius * ellipticalScale.y;

                vertices.Add(position + new Vector3(x, 0f, z));
            }
        }

        /// <summary>
        /// Add a ring with asymmetric scaling (different top/bottom radii).
        /// Useful for organic body shapes.
        /// </summary>
        /// <param name="vertices">List to add vertices to</param>
        /// <param name="position">Center position of the ring</param>
        /// <param name="widthRadius">Width radius (X-axis)</param>
        /// <param name="heightRadiusTop">Height radius for upper half (Y-axis)</param>
        /// <param name="heightRadiusBottom">Height radius for lower half (Y-axis)</param>
        /// <param name="segments">Number of segments</param>
        /// <param name="asymmetry">Asymmetry factor (0 = symmetric)</param>
        protected void AddAsymmetricRing(
            List<Vector3> vertices,
            Vector3 position,
            float widthRadius,
            float heightRadiusTop,
            float heightRadiusBottom,
            int segments,
            float asymmetry = 0f)
        {
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * widthRadius;
                float sinAngle = Mathf.Sin(angle);
                float y;

                if (sinAngle > 0f)
                {
                    // Upper half - apply asymmetry
                    y = sinAngle * heightRadiusTop;
                    y *= 1f - asymmetry * Mathf.Abs(Mathf.Cos(angle));
                }
                else
                {
                    // Lower half
                    y = sinAngle * heightRadiusBottom;
                }

                vertices.Add(position + new Vector3(x, y, 0f));
            }
        }

        /// <summary>
        /// Connect two existing rings with triangles.
        /// </summary>
        /// <param name="triangles">List to add triangles to</param>
        /// <param name="startRingIndex">Vertex index of the first ring's start</param>
        /// <param name="endRingIndex">Vertex index of the second ring's start</param>
        /// <param name="segments">Number of segments in each ring</param>
        protected void ConnectRings(List<int> triangles, int startRingIndex, int endRingIndex, int segments)
        {
            for (int i = 0; i < segments; i++)
            {
                int v0 = startRingIndex + i;
                int v1 = startRingIndex + (i + 1) % segments;
                int v2 = endRingIndex + i;
                int v3 = endRingIndex + (i + 1) % segments;

                // Two triangles per quad
                triangles.Add(v0);
                triangles.Add(v2);
                triangles.Add(v1);

                triangles.Add(v1);
                triangles.Add(v2);
                triangles.Add(v3);
            }
        }

        /// <summary>
        /// Cap a ring with triangles (close one end).
        /// </summary>
        /// <param name="vertices">List to add center vertex to</param>
        /// <param name="triangles">List to add triangles to</param>
        /// <param name="ringStartIndex">Vertex index where the ring starts</param>
        /// <param name="segments">Number of segments in the ring</param>
        /// <param name="centerPosition">Position of the cap center</param>
        /// <param name="flipNormals">Flip triangle winding for opposite direction</param>
        protected void CapRing(
            List<Vector3> vertices,
            List<int> triangles,
            int ringStartIndex,
            int segments,
            Vector3 centerPosition,
            bool flipNormals = false)
        {
            int centerIndex = vertices.Count;
            vertices.Add(centerPosition);

            for (int i = 0; i < segments; i++)
            {
                if (flipNormals)
                {
                    triangles.Add(centerIndex);
                    triangles.Add(ringStartIndex + i);
                    triangles.Add(ringStartIndex + (i + 1) % segments);
                }
                else
                {
                    triangles.Add(centerIndex);
                    triangles.Add(ringStartIndex + (i + 1) % segments);
                    triangles.Add(ringStartIndex + i);
                }
            }
        }

        #endregion
    }
}
