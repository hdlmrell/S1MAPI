using UnityEngine;
using MAPI.Core;
using MAPI.Utils;

namespace MAPI.ProceduralMesh
{
    /// <summary>
    /// Utility functions for mesh manipulation and optimization
    /// </summary>
    public static class MeshUtilities
    {
        #region Constants
        
        private const int MaxVerticesPerMesh = Constants.Mesh.MaxVerticesPerMesh;
        
        #endregion

        #region Public API - Mesh Modification
        
        /// <summary>
        /// Apply flat shading to a mesh by duplicating vertices
        /// Creates a low-poly aesthetic by ensuring each triangle has unique vertices
        /// </summary>
        /// <param name="mesh">The mesh to apply flat shading to</param>
        public static void ApplyFlatShading(Mesh mesh)
        {
            if (mesh == null)
            {
                DebugLog.Error("Cannot apply flat shading to null mesh");
                return;
            }

            Vector3[] oldVertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector3[] vertices = new Vector3[triangles.Length];
            Vector3[] normals = new Vector3[triangles.Length];
            Vector2[] uvs = mesh.uv.Length > 0 ? new Vector2[triangles.Length] : null;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v0 = oldVertices[triangles[i]];
                Vector3 v1 = oldVertices[triangles[i + 1]];
                Vector3 v2 = oldVertices[triangles[i + 2]];

                // Calculate face normal
                Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

                // Assign vertices and normals
                vertices[i] = v0;
                vertices[i + 1] = v1;
                vertices[i + 2] = v2;

                normals[i] = normal;
                normals[i + 1] = normal;
                normals[i + 2] = normal;

                // Update triangle indices
                triangles[i] = i;
                triangles[i + 1] = i + 1;
                triangles[i + 2] = i + 2;

                // Preserve UVs if they exist
                if (uvs != null && mesh.uv.Length > 0)
                {
                    uvs[i] = mesh.uv[triangles[i]];
                    uvs[i + 1] = mesh.uv[triangles[i + 1]];
                    uvs[i + 2] = mesh.uv[triangles[i + 2]];
                }
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            if (uvs != null)
            {
                mesh.uv = uvs;
            }

            mesh.RecalculateBounds();
            
            DebugLog.Info($"Applied flat shading to mesh: {mesh.name}");
        }

        /// <summary>
        /// Calculate normals for a mesh using a specific calculation mode
        /// </summary>
        /// <param name="mesh">The mesh to calculate normals for</param>
        /// <param name="mode">The normal calculation mode</param>
        public static void CalculateNormals(Mesh mesh, NormalCalculationMode mode = NormalCalculationMode.Smooth)
        {
            if (mesh == null)
            {
                DebugLog.Error("Cannot calculate normals for null mesh");
                return;
            }

            switch (mode)
            {
                case NormalCalculationMode.Smooth:
                    mesh.RecalculateNormals();
                    break;
                
                case NormalCalculationMode.Flat:
                    ApplyFlatShading(mesh);
                    break;
                
                default:
                    mesh.RecalculateNormals();
                    break;
            }
        }

        /// <summary>
        /// Merge multiple meshes into a single mesh
        /// </summary>
        /// <param name="meshes">The meshes to merge</param>
        /// <param name="offsets">Optional position offsets for each mesh (must match meshes length)</param>
        /// <param name="name">Optional name for the merged mesh</param>
        /// <returns>A new merged mesh, or null if merging failed</returns>
        public static Mesh? MergeMeshes(Mesh[] meshes, Vector3[]? offsets = null, string name = "MergedMesh")
        {
            if (meshes == null || meshes.Length == 0)
            {
                DebugLog.Error("Cannot merge null or empty mesh array");
                return null;
            }

            if (offsets != null && offsets.Length != meshes.Length)
            {
                DebugLog.Error($"Offsets array length ({offsets.Length}) must match meshes array length ({meshes.Length})");
                return null;
            }

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();

            for (int meshIndex = 0; meshIndex < meshes.Length; meshIndex++)
            {
                Mesh mesh = meshes[meshIndex];
                if (mesh == null)
                {
                    continue;
                }

                int vertexOffset = vertices.Count;
                Vector3 offset = (offsets != null && meshIndex < offsets.Length) ? offsets[meshIndex] : Vector3.zero;

                // Add vertices with offset
                foreach (Vector3 vertex in mesh.vertices)
                {
                    vertices.Add(vertex + offset);
                }

                normals.AddRange(mesh.normals);
                if (mesh.uv.Length > 0)
                {
                    uvs.AddRange(mesh.uv);
                }

                int[] meshTriangles = mesh.triangles;
                for (int i = 0; i < meshTriangles.Length; i++)
                {
                    triangles.Add(meshTriangles[i] + vertexOffset);
                }
            }

            if (vertices.Count > MaxVerticesPerMesh)
            {
                DebugLog.Warning($"Merged mesh has {vertices.Count} vertices, exceeding Unity's limit of {MaxVerticesPerMesh}");
            }

            Mesh mergedMesh = new Mesh
            {
                name = name,
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                normals = normals.ToArray()
            };

            if (uvs.Count > 0)
            {
                mergedMesh.uv = uvs.ToArray();
            }

            mergedMesh.RecalculateBounds();
            ResourceTracker.Register(mergedMesh);

            DebugLog.Info($"Merged {meshes.Length} meshes into: {name} ({vertices.Count} vertices)");
            return mergedMesh;
        }

        /// <summary>
        /// Merge a single mesh with a position offset into a new mesh
        /// </summary>
        /// <param name="mesh">The mesh to copy</param>
        /// <param name="offset">Position offset to apply</param>
        /// <param name="name">Optional name for the new mesh</param>
        /// <returns>A new mesh with offset applied</returns>
        public static Mesh? MergeMesh(Mesh mesh, Vector3 offset, string? name = null)
        {
            if (mesh == null)
            {
                DebugLog.Error("Cannot merge null mesh");
                return null;
            }

            return MergeMeshes(new[] { mesh }, new[] { offset }, name ?? $"{mesh.name}_Offset");
        }
        
        #endregion

        #region Public API - Mesh Validation
        
        /// <summary>
        /// Validate that a mesh is within Unity's limitations
        /// </summary>
        /// <param name="mesh">The mesh to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateMesh(Mesh mesh)
        {
            if (mesh == null)
            {
                DebugLog.Error("Mesh is null");
                return false;
            }

            if (mesh.vertices.Length == 0)
            {
                DebugLog.Error($"Mesh {mesh.name} has no vertices");
                return false;
            }

            if (mesh.triangles.Length == 0)
            {
                DebugLog.Error($"Mesh {mesh.name} has no triangles");
                return false;
            }

            if (mesh.vertices.Length > MaxVerticesPerMesh)
            {
                DebugLog.Error($"Mesh {mesh.name} has {mesh.vertices.Length} vertices, exceeding limit of {MaxVerticesPerMesh}");
                return false;
            }

            if (mesh.triangles.Length % 3 != 0)
            {
                DebugLog.Error($"Mesh {mesh.name} has invalid triangle count: {mesh.triangles.Length}");
                return false;
            }

            return true;
        }
        
        #endregion
    }

    /// <summary>
    /// Normal calculation modes for mesh processing
    /// </summary>
    public enum NormalCalculationMode
    {
        /// <summary>
        /// Smooth normals (averaged across shared vertices)
        /// </summary>
        Smooth,

        /// <summary>
        /// Flat normals (per-triangle, creates hard edges)
        /// </summary>
        Flat
    }
}
