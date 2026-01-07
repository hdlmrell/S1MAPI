using System.Collections.Generic;
using UnityEngine;
using MAPI.Utils;

namespace MAPI.Gltf
{
    /// <summary>
    /// Processes GLTF textures and images.
    /// </summary>
    public static class GltfTextureProcessor
    {
        /// <summary>
        /// Process GLTF textures and convert them to Unity Textures.
        /// </summary>
        /// <param name="gltf">The parsed GLTF root object</param>
        /// <param name="binaryBuffer">The binary buffer containing image data</param>
        /// <returns>List of Unity textures</returns>
        public static List<Texture2D?> ProcessTextures(GltfRoot gltf, byte[]? binaryBuffer)
        {
            List<Texture2D?> textures = new List<Texture2D?>();

            if (gltf.textures == null) return textures;

            foreach (GltfTexture gltfTex in gltf.textures)
            {
                if (gltfTex.source.HasValue && gltf.images != null && gltfTex.source.Value < gltf.images.Count)
                {
                    GltfImage image = gltf.images[gltfTex.source.Value];
                    Texture2D? tex = LoadImage(gltf, image, binaryBuffer);
                    if (tex != null)
                    {
                        tex.name = gltfTex.name ?? image.name ?? $"texture_{textures.Count}";
                        
                        // Apply sampler settings if available
                        if (gltfTex.sampler.HasValue && gltf.samplers != null && gltfTex.sampler.Value < gltf.samplers.Count)
                        {
                            ApplySampler(tex, gltf.samplers[gltfTex.sampler.Value]);
                        }
                        
                        textures.Add(tex);
                    }
                    else
                    {
                        // Add null placeholder to keep indices aligned
                        textures.Add(null);
                        DebugLog.Warning($"Failed to load texture index {textures.Count - 1}");
                    }
                }
                else
                {
                    textures.Add(null);
                }
            }

            return textures;
        }

        private static Texture2D? LoadImage(GltfRoot gltf, GltfImage image, byte[]? binaryBuffer)
        {
            if (image.bufferView.HasValue)
            {
                // Load from binary buffer
                if (gltf.bufferViews != null && image.bufferView.Value < gltf.bufferViews.Count)
                {
                    GltfBufferView view = gltf.bufferViews[image.bufferView.Value];
                    
                    // Validate buffer index (usually 0 for GLB binary chunk)
                    if (view.buffer == 0 && binaryBuffer != null)
                    {
                        int offset = view.byteOffset;
                        int length = view.byteLength;
                        
                        if (offset + length <= binaryBuffer.Length)
                        {
                            byte[] imageBytes = new byte[length];
                            System.Array.Copy(binaryBuffer, offset, imageBytes, 0, length);
                            
                            Texture2D tex = new Texture2D(2, 2);
                            if (tex.LoadImage(imageBytes))
                            {
                                return tex;
                            }
                        }
                    }
                }
            }
            // URI loading is not supported in this version for safety/simplicity
            // (most GLBs embed textures anyway)
            
            return null;
        }

        private static void ApplySampler(Texture2D tex, GltfSampler sampler)
        {
            // Wrap modes
            if (sampler.wrapS.HasValue) tex.wrapModeU = GetWrapMode(sampler.wrapS.Value);
            if (sampler.wrapT.HasValue) tex.wrapModeV = GetWrapMode(sampler.wrapT.Value);
            
            // Filter modes
            if (sampler.minFilter.HasValue || sampler.magFilter.HasValue)
            {
                tex.filterMode = GetFilterMode(sampler.minFilter ?? 0, sampler.magFilter ?? 0);
            }
        }

        private static TextureWrapMode GetWrapMode(int mode)
        {
            // 33071 = CLAMP_TO_EDGE
            // 33648 = MIRRORED_REPEAT
            // 10497 = REPEAT
            switch (mode)
            {
                case 33071: return TextureWrapMode.Clamp;
                case 33648: return TextureWrapMode.Mirror;
                case 10497: return TextureWrapMode.Repeat;
                default: return TextureWrapMode.Repeat;
            }
        }

        private static FilterMode GetFilterMode(int min, int mag)
        {
            // Simplified mapping
            // NEAREST = 9728
            // LINEAR = 9729
            // Mipmap modes...
            
            if (min == 9728 || mag == 9728) return FilterMode.Point;
            return FilterMode.Bilinear; 
        }
    }
}
