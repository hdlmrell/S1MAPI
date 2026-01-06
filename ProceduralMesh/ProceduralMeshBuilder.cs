using System;
using System.Collections.Generic;
using UnityEngine;
using MAPI.Core;
using MAPI.Utils;

namespace MAPI.ProceduralMesh
{
    /// <summary>
    /// High-level builder for creating procedural meshes
    /// Provides fluent API for constructing meshes from basic shapes
    /// </summary>
    public class ProceduralMeshBuilder
    {
        #region Fields
        
        private readonly string _name;
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<int> _triangles = new List<int>();
        private readonly List<Vector2> _uvs = new List<Vector2>();
        private readonly List<Vector3> _normals = new List<Vector3>();
        private Material? _material;
        private bool _applyFlatShading = false;
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Create a new procedural mesh builder
        /// </summary>
        /// <param name="name">Name for the generated mesh</param>
        public ProceduralMeshBuilder(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "Mesh name cannot be null or empty");
            }

            _name = name;
        }
        
        #endregion

        #region Public API - Basic Shapes
        
        /// <summary>
        /// Add a box (cube) to the mesh
        /// </summary>
        /// <param name="center">Center position of the box</param>
        /// <param name="size">Size of the box</param>
        /// <returns>This builder for method chaining</returns>
        public ProceduralMeshBuilder AddBox(Vector3 center, Vector3 size)
        {
            int startVertex = _vertices.Count;

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

            _vertices.AddRange(corners);

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
                _triangles.Add(startVertex + tri);
            }

            DebugLog.Info($"Added box to mesh: {_name}");
            return this;
        }

        /// <summary>
        /// Add a sphere to the mesh
        /// </summary>
        /// <param name="center">Center position of the sphere</param>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="subdivisions">Number of subdivisions (higher = smoother)</param>
        /// <returns>This builder for method chaining</returns>
        public ProceduralMeshBuilder AddSphere(Vector3 center, float radius, int subdivisions = Constants.Mesh.DefaultSphereSubdivisions)
        {
            int startVertex = _vertices.Count;
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

                    _vertices.Add(center + position);
                    _uvs.Add(new Vector2((float)lon / longitudes, (float)lat / latitudes));
                }
            }

            for (int lat = 0; lat < latitudes; lat++)
            {
                for (int lon = 0; lon < longitudes; lon++)
                {
                    int current = startVertex + lat * (longitudes + 1) + lon;
                    int next = current + longitudes + 1;

                    _triangles.Add(current);
                    _triangles.Add(next);
                    _triangles.Add(current + 1);

                    _triangles.Add(current + 1);
                    _triangles.Add(next);
                    _triangles.Add(next + 1);
                }
            }

            DebugLog.Info($"Added sphere to mesh: {_name}");
            return this;
        }

        /// <summary>
        /// Add a cylinder to the mesh
        /// </summary>
        /// <param name="start">Start position (bottom center)</param>
        /// <param name="end">End position (top center)</param>
        /// <param name="radius">Radius of the cylinder</param>
        /// <param name="segments">Number of radial segments</param>
        /// <returns>This builder for method chaining</returns>
        public ProceduralMeshBuilder AddCylinder(Vector3 start, Vector3 end, float radius, int segments = Constants.Mesh.DefaultCylinderSegments)
        {
            int startVertex = _vertices.Count;
            Vector3 direction = (end - start).normalized;
            float height = Vector3.Distance(start, end);

            // Create perpendicular vectors for the circular cross-section
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);
            if (perpendicular.magnitude < 0.001f)
            {
                perpendicular = Vector3.Cross(direction, Vector3.right);
            }
            perpendicular.Normalize();
            Vector3 perpendicular2 = Vector3.Cross(direction, perpendicular).normalized;

            // Add center vertices
            _vertices.Add(start); // Bottom center
            _vertices.Add(end);   // Top center

            // Add rim vertices
            for (int i = 0; i <= segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                Vector3 offset = (perpendicular * cos + perpendicular2 * sin) * radius;

                _vertices.Add(start + offset); // Bottom rim
                _vertices.Add(end + offset);   // Top rim
            }

            // Bottom cap triangles
            for (int i = 0; i < segments; i++)
            {
                _triangles.Add(startVertex); // Bottom center
                _triangles.Add(startVertex + 2 + i * 2);
                _triangles.Add(startVertex + 2 + (i + 1) * 2);
            }

            // Top cap triangles
            for (int i = 0; i < segments; i++)
            {
                _triangles.Add(startVertex + 1); // Top center
                _triangles.Add(startVertex + 3 + (i + 1) * 2);
                _triangles.Add(startVertex + 3 + i * 2);
            }

            // Side triangles
            for (int i = 0; i < segments; i++)
            {
                int bottomCurrent = startVertex + 2 + i * 2;
                int bottomNext = startVertex + 2 + (i + 1) * 2;
                int topCurrent = bottomCurrent + 1;
                int topNext = bottomNext + 1;

                _triangles.Add(bottomCurrent);
                _triangles.Add(topCurrent);
                _triangles.Add(bottomNext);

                _triangles.Add(bottomNext);
                _triangles.Add(topCurrent);
                _triangles.Add(topNext);
            }

            DebugLog.Info($"Added cylinder to mesh: {_name}");
            return this;
        }

        /// <summary>
        /// Add a capsule to the mesh
        /// </summary>
        /// <param name="start">Start position (bottom)</param>
        /// <param name="end">End position (top)</param>
        /// <param name="radius">Radius of the capsule</param>
        /// <returns>This builder for method chaining</returns>
        public ProceduralMeshBuilder AddCapsule(Vector3 start, Vector3 end, float radius)
        {
            // Add cylinder for the middle section
            AddCylinder(start + Vector3.up * radius, end - Vector3.up * radius, radius, Constants.Mesh.DefaultCylinderSegments);

            // Add hemisphere for bottom
            AddSphere(start, radius, Constants.Mesh.DefaultCapsuleSubdivisions);

            // Add hemisphere for top
            AddSphere(end, radius, Constants.Mesh.DefaultCapsuleSubdivisions);

            DebugLog.Info($"Added capsule to mesh: {_name}");
            return this;
        }
        
        #endregion

        #region Public API - Material and Properties
        
        /// <summary>
        /// Set the material for the generated mesh
        /// </summary>
        /// <param name="material">The material to apply</param>
        /// <returns>This builder for method chaining</returns>
        public ProceduralMeshBuilder SetMaterial(Material material)
        {
            _material = material;
            return this;
        }

        /// <summary>
        /// Set a solid color for the mesh (creates a material internally)
        /// </summary>
        /// <param name="color">The color to apply</param>
        /// <returns>This builder for method chaining</returns>
        public ProceduralMeshBuilder SetColor(Color color)
        {
            _material = MaterialPresets.Opaque(color);
            return this;
        }

        /// <summary>
        /// Apply flat shading when building the mesh
        /// Creates a low-poly aesthetic with hard edges
        /// </summary>
        /// <returns>This builder for method chaining</returns>
        public ProceduralMeshBuilder ApplyFlatShading()
        {
            _applyFlatShading = true;
            return this;
        }
        
        #endregion

        #region Public API - Organic Shapes
        
        /// <summary>
        /// Add a segmented organic body to the mesh
        /// </summary>
        /// <param name="size">Overall size (x=width, y=height, z=length)</param>
        /// <param name="profile">Body profile defining the shape</param>
        /// <returns>This builder for method chaining</returns>
        public ProceduralMeshBuilder AddSegmentedBody(Vector3 size, BodyProfile profile)
        {
            Mesh bodyMesh = OrganicShapeBuilder.CreateSegmentedBody(size, profile, $"{_name}_Body");
            MergeMesh(bodyMesh);
            return this;
        }

        /// <summary>
        /// Add an articulated limb to the mesh
        /// </summary>
        /// <param name="profile">Limb profile defining joints and radii</param>
        /// <param name="position">Local position offset</param>
        /// <returns>This builder for method chaining</returns>
        public ProceduralMeshBuilder AddArticulatedLimb(LimbProfile profile, Vector3 position = default)
        {
            Mesh limbMesh = OrganicShapeBuilder.CreateArticulatedLimb(profile, $"{_name}_Limb");
            MergeMesh(limbMesh, position);
            return this;
        }

        /// <summary>
        /// Add a paw to the mesh
        /// </summary>
        /// <param name="width">Width of the paw</param>
        /// <param name="height">Height of the paw</param>
        /// <param name="length">Length of the paw</param>
        /// <param name="position">Local position offset</param>
        /// <returns>This builder for method chaining</returns>
        public ProceduralMeshBuilder AddPaw(float width, float height, float length, Vector3 position = default)
        {
            Mesh pawMesh = OrganicShapeBuilder.CreatePaw(width, height, length, $"{_name}_Paw");
            MergeMesh(pawMesh, position);
            return this;
        }

        /// <summary>
        /// Add an ear to the mesh
        /// </summary>
        /// <param name="radius">Size of the ear</param>
        /// <param name="position">Local position offset</param>
        /// <returns>This builder for method chaining</returns>
        public ProceduralMeshBuilder AddEar(float radius, Vector3 position = default)
        {
            Mesh earMesh = OrganicShapeBuilder.CreateEar(radius, $"{_name}_Ear");
            MergeMesh(earMesh, position);
            return this;
        }
        
        #endregion

        #region Public API - Build Methods
        
        /// <summary>
        /// Build the mesh and return it
        /// </summary>
        /// <returns>The constructed mesh</returns>
        public Mesh BuildMesh()
        {
            if (_vertices.Count == 0)
            {
                DebugLog.Warning($"Building empty mesh: {_name}");
            }

            Mesh mesh = new Mesh
            {
                name = _name,
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray()
            };

            if (_uvs.Count > 0)
            {
                mesh.uv = _uvs.ToArray();
            }

            if (_applyFlatShading)
            {
                MeshUtilities.ApplyFlatShading(mesh);
            }
            else
            {
                mesh.RecalculateNormals();
            }

            mesh.RecalculateBounds();
            ResourceTracker.Register(mesh);

            DebugLog.Info($"Built mesh: {_name} ({_vertices.Count} vertices, {_triangles.Count / 3} triangles)");
            return mesh;
        }

        /// <summary>
        /// Build a GameObject with the mesh and material applied
        /// </summary>
        /// <returns>The created GameObject with mesh components</returns>
        public GameObject Build()
        {
            Mesh mesh = BuildMesh();

            GameObject go = new GameObject(_name);
            MeshFilter filter = go.AddComponent<MeshFilter>();
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();

            filter.mesh = mesh;

            if (_material != null)
            {
                renderer.material = _material;
            }
            else
            {
                // Create a default material if none specified
                renderer.material = MaterialPresets.Opaque(Color.white);
            }

            DebugLog.Info($"Built GameObject: {_name}");
            return go;
        }
        
        #endregion

        #region Private Helper Methods
        
        /// <summary>
        /// Merge another mesh into this builder
        /// </summary>
        private void MergeMesh(Mesh mesh, Vector3 offset = default)
        {
            if (mesh == null) return;

            int vertexOffset = _vertices.Count;

            // Add vertices with offset
            foreach (Vector3 vertex in mesh.vertices)
            {
                _vertices.Add(vertex + offset);
            }

            // Add triangles with vertex offset
            foreach (int triangle in mesh.triangles)
            {
                _triangles.Add(triangle + vertexOffset);
            }

            // Add UVs if available
            if (mesh.uv != null && mesh.uv.Length > 0)
            {
                _uvs.AddRange(mesh.uv);
            }
        }
        
        #endregion
    }
}
