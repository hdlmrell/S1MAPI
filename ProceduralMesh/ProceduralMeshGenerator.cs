using UnityEngine;

namespace MAPI.ProceduralMesh
{
    /// <summary>
    /// Base class for procedural mesh generation
    /// Provides common functionality for creating meshes at runtime
    /// </summary>
    public abstract class ProceduralMeshGenerator
    {
        /// <summary>
        /// Generate a mesh with the specified parameters
        /// </summary>
        /// <returns>The generated mesh</returns>
        public abstract Mesh Generate();

        /// <summary>
        /// Create a game object with the generated mesh
        /// </summary>
        /// <param name="name">Name for the created GameObject</param>
        /// <param name="material">Material to apply to the mesh renderer</param>
        /// <returns>The created GameObject with mesh components</returns>
        public virtual GameObject CreateGameObject(string name, Material? material = null)
        {
            Mesh mesh = Generate();
            
            GameObject go = new GameObject(name);
            MeshFilter filter = go.AddComponent<MeshFilter>();
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            
            filter.mesh = mesh;
            
            if (material != null)
            {
                renderer.material = material;
            }
            
            return go;
        }

        /// <summary>
        /// Calculate normals for a mesh
        /// </summary>
        protected void CalculateNormals(Mesh mesh)
        {
            mesh.RecalculateNormals();
        }

        /// <summary>
        /// Calculate bounds for a mesh
        /// </summary>
        protected void CalculateBounds(Mesh mesh)
        {
            mesh.RecalculateBounds();
        }

        /// <summary>
        /// Optimize the mesh for rendering
        /// </summary>
        protected void OptimizeMesh(Mesh mesh)
        {
            mesh.Optimize();
        }
    }
}
