using MAPI.Utils;
using UnityEngine;

namespace MAPI.Extensions
{
    /// <summary>
    /// Extension methods for Mesh operations.
    /// </summary>
    public static class MeshExtensions
    {
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

        /// <summary>
        /// Check if mesh has valid data for rendering.
        /// </summary>
        public static bool IsValid(this Mesh mesh, bool checkVertexLimit = true)
        {
            if (mesh == null) return false;
            if (mesh.vertices.Length == 0) return false;
            if (mesh.triangles.Length == 0) return false;
            if (checkVertexLimit && mesh.vertices.Length > 65535)
            {
                DebugLog.Warning($"Mesh has {mesh.vertices.Length} vertices, exceeding Unity's 65535 limit");
                return false;
            }
            return true;
        }
    }
}
