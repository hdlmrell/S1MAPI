using UnityEngine;

namespace MAPI.Gltf
{
    /// <summary>
    /// Static utility class for loading GLB/GLTF files.
    /// Provides convenient static methods that wrap GltfImporter.
    /// </summary>
    public static class GltfLoader
    {
        /// <summary>
        /// Load a GLB or GLTF model from bytes.
        /// Automatically detects format.
        /// </summary>
        /// <param name="data">GLB or JSON bytes</param>
        /// <param name="shader">Optional shader for materials</param>
        /// <returns>The root GameObject of the imported model</returns>
        public static GameObject? Load(byte[] data, Shader? shader = null)
        {
            GltfImporter importer = new GltfImporter();
            
            if (shader != null)
            {
                importer.SetShader(shader);
            }
            
            return importer.Load(data);
        }

        /// <summary>
        /// Load a GLB model from bytes.
        /// </summary>
        /// <param name="glbBytes">Raw GLB bytes</param>
        /// <param name="shader">Optional shader for materials</param>
        /// <returns>The root GameObject of the imported model</returns>
        public static GameObject? LoadGlb(byte[] glbBytes, Shader? shader = null)
        {
            GltfImporter importer = new GltfImporter();
            
            if (shader != null)
            {
                importer.SetShader(shader);
            }
            
            return importer.LoadGlb(glbBytes);
        }

        /// <summary>
        /// Load a GLTF/GLB model from a file path.
        /// </summary>
        /// <param name="filePath">Path to the GLTF or GLB file</param>
        /// <param name="shader">Optional shader for materials</param>
        /// <returns>The root GameObject of the imported model</returns>
        public static GameObject? LoadFromFile(string filePath, Shader? shader = null)
        {
            GltfImporter importer = new GltfImporter();
            
            if (shader != null)
            {
                importer.SetShader(shader);
            }
            
            return importer.LoadFromFile(filePath);
        }

        /// <summary>
        /// Load a GLTF model from JSON text.
        /// </summary>
        /// <param name="json">GLTF JSON string</param>
        /// <param name="basePath">Base directory for external resources</param>
        /// <param name="shader">Optional shader for materials</param>
        /// <returns>The root GameObject of the imported model</returns>
        public static GameObject? LoadFromJson(string json, string basePath, Shader? shader = null)
        {
            GltfImporter importer = new GltfImporter();
            
            if (shader != null)
            {
                importer.SetShader(shader);
            }
            
            return importer.LoadGltfJson(json, basePath);
        }
    }
}
