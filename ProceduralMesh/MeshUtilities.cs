using UnityEngine;
using S1MAPI.Core;
using S1MAPI.Utils;

namespace S1MAPI.ProceduralMesh
{
    /// <summary>
    /// Utility functions for mesh merging and combination.
    /// For mesh modification and validation, see MeshExtensions in S1MAPI.Extensions namespace.
    /// </summary>
    public static class MeshUtilities
    {
        #region Constants
        
        private const int MaxVerticesPerMesh = Constants.Mesh.MaxVerticesPerMesh;
        
        #endregion

        #region Public API - Mesh Merging
        
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
    }
}
