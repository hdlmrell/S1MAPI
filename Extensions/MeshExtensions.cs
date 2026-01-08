using UnityEngine;
using S1MAPI.Core;
using S1MAPI.Utils;

namespace S1MAPI.Extensions
{
    /// <summary>
    /// Extension methods for Mesh operations.
    /// </summary>
    public static class MeshExtensions
    {
        #region Mesh Cloning and Properties
        
        /// <summary>
        /// Create a clone of this mesh.
        /// </summary>
        public static Mesh Clone(this Mesh mesh)
        {
            Mesh clone = new Mesh
            {
                name = $"{mesh.name}_Clone",
                vertices = mesh.vertices,
                triangles = mesh.triangles,
                normals = mesh.normals,
                tangents = mesh.tangents,
                uv = mesh.uv,
                uv2 = mesh.uv2,
                uv3 = mesh.uv3,
                uv4 = mesh.uv4,
                colors = mesh.colors,
                bounds = mesh.bounds
            };
            return clone;
        }

        /// <summary>
        /// Get the center point of the mesh based on bounds.
        /// </summary>
        public static Vector3 GetCenter(this Mesh mesh)
        {
            return mesh.bounds.center;
        }
        
        #endregion

        #region Mesh Validation
        
        /// <summary>
        /// Check if mesh has valid data for rendering.
        /// </summary>
        /// <param name="mesh">The mesh to validate</param>
        /// <param name="checkVertexLimit">Whether to check against Unity's vertex limit</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValid(this Mesh mesh, bool checkVertexLimit = true)
        {
            if (mesh == null) return false;
            if (mesh.vertices.Length == 0) return false;
            if (mesh.triangles.Length == 0) return false;
            if (checkVertexLimit && mesh.vertices.Length > Constants.Mesh.MaxVerticesPerMesh)
            {
                DebugLog.Warning($"Mesh has {mesh.vertices.Length} vertices, exceeding Unity's {Constants.Mesh.MaxVerticesPerMesh} limit");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validate that a mesh is within Unity's limitations with detailed error reporting.
        /// </summary>
        /// <param name="mesh">The mesh to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool Validate(this Mesh mesh)
        {
            if (mesh == null)
            {
                DebugLog.Error("Mesh is null");
                return false;
            }

            if (!mesh.IsValid())
            {
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

        #region Mesh Modification
        
        /// <summary>
        /// Apply flat shading to a mesh by duplicating vertices.
        /// Creates a low-poly aesthetic by ensuring each triangle has unique vertices.
        /// </summary>
        /// <param name="mesh">The mesh to apply flat shading to</param>
        public static void ApplyFlatShading(this Mesh mesh)
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
            Vector2[]? uvs = mesh.uv.Length > 0 ? new Vector2[triangles.Length] : null;

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
        /// Calculate normals for a mesh using a specific calculation mode.
        /// </summary>
        /// <param name="mesh">The mesh to calculate normals for</param>
        /// <param name="mode">The normal calculation mode</param>
        public static void CalculateNormals(this Mesh mesh, NormalCalculationMode mode = NormalCalculationMode.Smooth)
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
                    mesh.ApplyFlatShading();
                    break;
                
                default:
                    mesh.RecalculateNormals();
                    break;
            }
        }
        
        #endregion
    }

    /// <summary>
    /// Normal calculation modes for mesh processing.
    /// </summary>
    public enum NormalCalculationMode
    {
        /// <summary>
        /// Smooth normals (averaged across shared vertices).
        /// </summary>
        Smooth,

        /// <summary>
        /// Flat normals (per-triangle, creates hard edges).
        /// </summary>
        Flat
    }
}
