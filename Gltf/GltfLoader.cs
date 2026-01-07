using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MAPI.Utils;

// Note: Requires Newtonsoft.Json. If not available, a simple JSON parser replacement is needed.
// Most Unity modding environments include Newtonsoft.Json.
using Newtonsoft.Json;

namespace MAPI.Gltf
{
    /// <summary>
    /// Loader for GLB (Binary GLTF) files
    /// </summary>
    public static class GltfLoader
    {
        private const uint GLTF_MAGIC = 0x46546C67; // "glTF"
        private const uint CHUNK_TYPE_JSON = 0x4E4F534A; // "JSON"
        private const uint CHUNK_TYPE_BIN = 0x004E4942; // "BIN"

        /// <summary>
        /// Parse a GLB byte array into a GltfRoot object
        /// </summary>
        /// <param name="glbBytes">Raw GLB file bytes</param>
        /// <returns>Parsed GltfRoot or null if invalid</returns>
        public static GltfRoot ParseGlb(byte[] glbBytes)
        {
            if (glbBytes == null || glbBytes.Length < 12)
            {
                DebugLog.Error("Invalid GLB data: too short");
                return null;
            }

            using (MemoryStream ms = new MemoryStream(glbBytes))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                // 1. Header
                uint magic = reader.ReadUInt32();
                if (magic != GLTF_MAGIC)
                {
                    DebugLog.Error($"Invalid GLB magic: {magic:X}");
                    return null;
                }

                uint version = reader.ReadUInt32();
                if (version != 2)
                {
                    DebugLog.Warning($"GLB version is {version}, expected 2. Loading may fail.");
                }

                uint length = reader.ReadUInt32();
                if (length != glbBytes.Length)
                {
                    DebugLog.Warning($"GLB length mismatch: Header says {length}, got {glbBytes.Length}");
                }

                GltfRoot root = null;
                byte[] binaryBuffer = null;

                // 2. Chunks
                while (ms.Position < ms.Length)
                {
                    if (ms.Position + 8 > ms.Length) break;

                    uint chunkLength = reader.ReadUInt32();
                    uint chunkType = reader.ReadUInt32();

                    if (ms.Position + chunkLength > ms.Length)
                    {
                        DebugLog.Error("GLB chunk incomplete");
                        break;
                    }

                    if (chunkType == CHUNK_TYPE_JSON)
                    {
                        byte[] jsonBytes = reader.ReadBytes((int)chunkLength);
                        string json = Encoding.UTF8.GetString(jsonBytes);
                        
                        try 
                        {
                            root = JsonConvert.DeserializeObject<GltfRoot>(json);
                        }
                        catch (Exception ex)
                        {
                            DebugLog.Error($"Failed to parse GLTF JSON: {ex.Message}");
                            return null;
                        }
                    }
                    else if (chunkType == CHUNK_TYPE_BIN)
                    {
                        binaryBuffer = reader.ReadBytes((int)chunkLength);
                    }
                    else
                    {
                        // Skip unknown chunks
                        ms.Seek(chunkLength, SeekOrigin.Current);
                    }
                }

                if (root != null && binaryBuffer != null && root.buffers != null && root.buffers.Count > 0)
                {
                    // Attach binary buffer to the first buffer definition
                    root.buffers[0].extra = binaryBuffer;
                }

                return root;
            }
        }
        
        /// <summary>
        /// Load a GLB model from bytes and convert to Unity GameObject
        /// </summary>
        /// <param name="glbBytes">Raw GLB bytes</param>
        /// <param name="shader">Shader to use for materials (defaults to Standard)</param>
        /// <returns>The root GameObject of the imported model</returns>
        public static GameObject LoadFromBytes(byte[] glbBytes, Shader shader = null)
        {
            GltfRoot root = ParseGlb(glbBytes);
            if (root == null) return null;

            byte[] binaryBuffer = root.buffers != null && root.buffers.Count > 0 ? root.buffers[0].extra : null;
            if (binaryBuffer == null)
            {
                DebugLog.Error("No binary buffer found in GLB");
                return null;
            }

            // Process textures
            List<Texture2D> textures = GltfTextureProcessor.ProcessTextures(root, binaryBuffer);

            // Process meshes
            List<GltfMeshResult> meshes = GltfMeshProcessor.ProcessMeshes(root, binaryBuffer);
            
            // Create root object
            GameObject modelRoot = new GameObject("GltfModel");
            
            // Build hierarchy and attach meshes
            GltfNodeProcessor.ProcessNodes(root, modelRoot, meshes, textures, shader);
            
            return modelRoot;
        }
    }
}
