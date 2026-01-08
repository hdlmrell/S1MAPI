using UnityEngine;
using MAPI.Utils;

namespace MAPI.ProceduralMesh.Generators.Primitives
{
    /// <summary>
    /// Generates sphere mesh geometry using UV sphere algorithm.
    /// </summary>
    internal static class SphereGenerator
    {
        #region Public API

        /// <summary>
        /// Generate sphere vertices, triangles, and UVs.
        /// </summary>
        /// <param name="vertices">List to add vertices to</param>
        /// <param name="triangles">List to add triangles to</param>
        /// <param name="uvs">List to add UVs to</param>
        /// <param name="center">Center position of the sphere</param>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="subdivisions">Number of subdivisions (higher = smoother)</param>
        public static void Generate(
            List<Vector3> vertices,
            List<int> triangles,
            List<Vector2> uvs,
            Vector3 center,
            float radius,
            int subdivisions = Constants.Mesh.DefaultSphereSubdivisions)
        {
            int startVertex = vertices.Count;
            int longitudes = subdivisions * 2;
            int latitudes = subdivisions;

            for (int lat = 0; lat <= latitudes; lat++)
            {
                float theta = lat * Mathf.PI / latitudes;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);

                for (int lon = 0; lon <= longitudes; lon++)
                {
                    float phi = lon * 2 * Mathf.PI / longitudes;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);

                    Vector3 position = new Vector3(
                        radius * sinTheta * cosPhi,
                        radius * cosTheta,
                        radius * sinTheta * sinPhi
                    );

                    vertices.Add(center + position);
                    uvs.Add(new Vector2((float)lon / longitudes, (float)lat / latitudes));
                }
            }

            for (int lat = 0; lat < latitudes; lat++)
            {
                for (int lon = 0; lon < longitudes; lon++)
                {
                    int current = startVertex + lat * (longitudes + 1) + lon;
                    int next = current + longitudes + 1;

                    triangles.Add(current);
                    triangles.Add(next);
                    triangles.Add(current + 1);

                    triangles.Add(current + 1);
                    triangles.Add(next);
                    triangles.Add(next + 1);
                }
            }
        }

        #endregion
    }
}
