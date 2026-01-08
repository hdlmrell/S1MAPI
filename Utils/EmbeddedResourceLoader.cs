using System.Reflection;
using UnityEngine;

namespace MAPI.Utils
{
    /// <summary>
    /// Unified resource loading from embedded assembly resources
    /// Supports loading textures, sprites, and raw bytes from manifest resources
    /// </summary>
    public static class EmbeddedResourceLoader
    {
        #region Public API - Resource Loading
        
        /// <summary>
        /// Load raw bytes from an embedded resource
        /// </summary>
        /// <param name="resourceName">The fully qualified resource name</param>
        /// <param name="assembly">Optional assembly to load from (defaults to calling assembly)</param>
        /// <returns>The resource bytes, or null if not found</returns>
        public static byte[]? LoadBytes(string resourceName, Assembly? assembly = null)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                DebugLog.Error("Resource name cannot be null or empty");
                return null;
            }

            Assembly targetAssembly = assembly ?? Assembly.GetCallingAssembly();

            try
            {
                using (Stream? stream = targetAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        DebugLog.Warning($"Embedded resource not found: {resourceName}");
                        return null;
                    }

                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);

                    DebugLog.Info($"Loaded {buffer.Length} bytes from embedded resource: {resourceName}");
                    return buffer;
                }
            }
            catch (Exception ex)
            {
                DebugLog.Error($"Failed to load embedded resource: {resourceName}");
                DebugLog.Exception(ex);
                return null;
            }
        }

        /// <summary>
        /// Load a texture from an embedded resource
        /// </summary>
        /// <param name="resourceName">The fully qualified resource name (PNG or JPG)</param>
        /// <param name="assembly">Optional assembly to load from (defaults to calling assembly)</param>
        /// <returns>The loaded texture, or null if loading failed</returns>
        public static Texture2D? LoadTexture(string resourceName, Assembly? assembly = null)
        {
            byte[]? imageBytes = LoadBytes(resourceName, assembly);
            if (imageBytes == null)
            {
                return null;
            }

            Texture2D texture = new Texture2D(2, 2);
            if (!texture.LoadImage(imageBytes))
            {
                DebugLog.Error($"Failed to load image data from resource: {resourceName}");
                UnityEngine.Object.Destroy(texture);
                return null;
            }

            texture.name = $"MAPI_{Path.GetFileNameWithoutExtension(resourceName)}";

            DebugLog.Info($"Loaded texture: {texture.name} ({texture.width}x{texture.height})");
            return texture;
        }

        /// <summary>
        /// Load a sprite from an embedded resource
        /// </summary>
        /// <param name="resourceName">The fully qualified resource name (PNG or JPG)</param>
        /// <param name="assembly">Optional assembly to load from (defaults to calling assembly)</param>
        /// <param name="pixelsPerUnit">Pixels per unit for the sprite (default 100)</param>
        /// <returns>The loaded sprite, or null if loading failed</returns>
        public static Sprite? LoadSprite(string resourceName, Assembly? assembly = null, float pixelsPerUnit = 100f)
        {
            Texture2D? texture = LoadTexture(resourceName, assembly);
            if (texture == null)
            {
                return null;
            }

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                pixelsPerUnit
            );

            sprite.name = $"MAPI_{Path.GetFileNameWithoutExtension(resourceName)}_Sprite";

            DebugLog.Info($"Created sprite from texture: {sprite.name}");
            return sprite;
        }
        
        #endregion

        #region Public API - Utility Methods
        
        /// <summary>
        /// Check if an embedded resource exists
        /// </summary>
        /// <param name="resourceName">The fully qualified resource name</param>
        /// <param name="assembly">Optional assembly to check (defaults to calling assembly)</param>
        /// <returns>True if the resource exists, false otherwise</returns>
        public static bool ResourceExists(string resourceName, Assembly? assembly = null)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                return false;
            }

            Assembly targetAssembly = assembly ?? Assembly.GetCallingAssembly();
            
            using (Stream? stream = targetAssembly.GetManifestResourceStream(resourceName))
            {
                return stream != null;
            }
        }

        /// <summary>
        /// List all embedded resource names in an assembly
        /// </summary>
        /// <param name="assembly">Optional assembly to check (defaults to calling assembly)</param>
        /// <returns>Array of resource names</returns>
        public static string[] ListResources(Assembly? assembly = null)
        {
            Assembly targetAssembly = assembly ?? Assembly.GetCallingAssembly();
            return targetAssembly.GetManifestResourceNames();
        }
        
        #endregion
    }
}
