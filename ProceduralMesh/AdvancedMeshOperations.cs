using UnityEngine;
using MAPI.Utils;

namespace MAPI.ProceduralMesh
{
    /// <summary>
    /// Advanced operations for mesh manipulation and analysis
    /// </summary>
    public static class AdvancedMeshOperations
    {
        #region Mesh Optimization
        
        /// <summary>
        /// Remove duplicate vertices from a mesh (weld vertices)
        /// </summary>
        /// <param name="mesh">The mesh to optimize</param>
        /// <param name="threshold">Distance threshold for merging vertices</param>
        /// <returns>A new optimized mesh</returns>
        public static Mesh WeldVertices(Mesh mesh, float threshold = Constants.Mesh.VertexWeldThreshold)
        {
            if (mesh == null) return null;

            Vector3[] oldVertices = mesh.vertices;
            Vector3[] oldNormals = mesh.normals;
            Vector2[] oldUVs = mesh.uv;
            int[] oldTriangles = mesh.triangles;

            List<Vector3> newVertices = new List<Vector3>();
            List<Vector3> newNormals = new List<Vector3>();
            List<Vector2> newUVs = new List<Vector2>();
            
            // Map old vertex index to new vertex index
            int[] vertexMap = new int[oldVertices.Length];
            
            // Dictionary to find existing vertices: Hash of position -> List of indices
            // Simple grid hashing or just spatial distance check
            
            // Brute force for simplicity in v1, or optimized spatial hash?
            // Let's use a spatial bucket approach for performance
            
            for (int i = 0; i < oldVertices.Length; i++)
            {
                Vector3 v = oldVertices[i];
                bool found = false;
                
                // Check against recent vertices? Or all?
                // For exact welding (threshold ~0), dictionary is fast.
                // For threshold > 0, we need distance check.
                
                // Simplified O(N^2) for now - optimize later if needed
                for (int j = 0; j < newVertices.Count; j++)
                {
                    if (Vector3.Distance(v, newVertices[j]) <= threshold)
                    {
                        // Check normal and UV similarity too? Usually welding ignores attributes if position matches
                        // But for sharp edges we might want to split. 
                        // Assuming basic geometric weld here.
                        
                        vertexMap[i] = j;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    vertexMap[i] = newVertices.Count;
                    newVertices.Add(v);
                    if (i < oldNormals.Length) newNormals.Add(oldNormals[i]);
                    if (i < oldUVs.Length) newUVs.Add(oldUVs[i]);
                }
            }

            // Remap triangles
            int[] newTriangles = new int[oldTriangles.Length];
            for (int i = 0; i < oldTriangles.Length; i++)
            {
                newTriangles[i] = vertexMap[oldTriangles[i]];
            }

            Mesh newMesh = new Mesh();
            newMesh.name = $"{mesh.name}_Welded";
            newMesh.vertices = newVertices.ToArray();
            newMesh.triangles = newTriangles;
            
            if (newNormals.Count == newVertices.Count) newMesh.normals = newNormals.ToArray();
            if (newUVs.Count == newVertices.Count) newMesh.uv = newUVs.ToArray();
            
            newMesh.RecalculateBounds();
            newMesh.RecalculateNormals(); // Recompute normals after welding
            
            return newMesh;
        }
        
        #endregion

        #region Mesh Analysis
        
        /// <summary>
        /// Calculate the surface area of the mesh
        /// </summary>
        public static float CalculateSurfaceArea(Mesh mesh)
        {
            if (mesh == null) return 0f;

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            float area = 0f;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v0 = vertices[triangles[i]];
                Vector3 v1 = vertices[triangles[i + 1]];
                Vector3 v2 = vertices[triangles[i + 2]];

                area += Vector3.Cross(v1 - v0, v2 - v0).magnitude * 0.5f;
            }

            return area;
        }

        /// <summary>
        /// Calculate the approximate volume of a closed mesh
        /// </summary>
        public static float CalculateVolume(Mesh mesh)
        {
            if (mesh == null) return 0f;

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            float volume = 0f;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v0 = vertices[triangles[i]];
                Vector3 v1 = vertices[triangles[i + 1]];
                Vector3 v2 = vertices[triangles[i + 2]];

                volume += SignedVolumeOfTriangle(v0, v1, v2);
            }

            return Mathf.Abs(volume);
        }

        private static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float v321 = p3.x * p2.y * p1.z;
            float v231 = p2.x * p3.y * p1.z;
            float v312 = p3.x * p1.y * p2.z;
            float v132 = p1.x * p3.y * p2.z;
            float v213 = p2.x * p1.y * p3.z;
            float v123 = p1.x * p2.y * p3.z;
            return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
        }
        
        #endregion

        #region Mesh Modification
        
        /// <summary>
        /// Subdivide the mesh (simple midpoint subdivision)
        /// </summary>
        public static Mesh Subdivide(Mesh mesh, int levels = 1)
        {
            if (levels <= 0) return InstantiateMesh(mesh);
            
            // Basic implementation: split every triangle into 4
            // v0
            // | \
            // m0--m2
            // | \ | \
            // v1--m1--v2
            
            // This is complex to implement robustly in one pass.
            // Placeholder for now.
            DebugLog.Warning("Mesh subdivision not fully implemented yet.");
            return InstantiateMesh(mesh);
        }

        private static Mesh InstantiateMesh(Mesh mesh)
        {
            Mesh newMesh = new Mesh();
            newMesh.vertices = mesh.vertices;
            newMesh.triangles = mesh.triangles;
            newMesh.normals = mesh.normals;
            newMesh.uv = mesh.uv;
            return newMesh;
        }
        
        #endregion
    }
}
