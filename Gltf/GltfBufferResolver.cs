using System;
using System.IO;
using System.Text;
using MAPI.Utils;

namespace MAPI.Gltf
{
    /// <summary>
    /// Resolves and loads buffer data from various sources: GLB binary chunks, external files, or base64 data URIs.
    /// Implements the single responsibility of buffer data resolution.
    /// </summary>
    internal sealed class GltfBufferResolver
    {
        #region Fields

        private readonly string? _basePath;
        private readonly byte[]? _glbBinaryChunk;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a buffer resolver for GLB files with an embedded binary chunk.
        /// </summary>
        /// <param name="glbBinaryChunk">The binary chunk from the GLB file</param>
        public GltfBufferResolver(byte[] glbBinaryChunk)
        {
            _glbBinaryChunk = glbBinaryChunk;
            _basePath = null;
        }

        /// <summary>
        /// Creates a buffer resolver for GLTF files with external buffer references.
        /// </summary>
        /// <param name="basePath">Base directory for resolving relative URIs</param>
        public GltfBufferResolver(string? basePath)
        {
            _basePath = basePath;
            _glbBinaryChunk = null;
        }

        /// <summary>
        /// Creates a buffer resolver for GLTF files with both base path and optional binary chunk.
        /// </summary>
        /// <param name="basePath">Base directory for resolving relative URIs</param>
        /// <param name="glbBinaryChunk">Optional binary chunk (for embedded-buffer GLTF or GLB)</param>
        public GltfBufferResolver(string? basePath, byte[]? glbBinaryChunk)
        {
            _basePath = basePath;
            _glbBinaryChunk = glbBinaryChunk;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Resolves all buffers in a GLTF root, loading their binary data.
        /// </summary>
        /// <param name="gltf">The GLTF root object</param>
        /// <returns>True if all buffers were resolved successfully</returns>
        public bool ResolveBuffers(GltfRoot gltf)
        {
            if (gltf.buffers == null || gltf.buffers.Count == 0)
            {
                return true;
            }

            for (int i = 0; i < gltf.buffers.Count; i++)
            {
                GltfBuffer buffer = gltf.buffers[i];

                if (buffer.Data != null)
                {
                    // Already resolved
                    continue;
                }

            byte[]? data = ResolveBuffer(buffer, i);
            if (data == null)
                {
                    DebugLog.Error($"Failed to resolve buffer {i}");
                    return false;
                }

                buffer.Data = data;
            }

            return true;
        }

        /// <summary>
        /// Resolves a single buffer's binary data.
        /// </summary>
        /// <param name="buffer">The buffer definition</param>
        /// <param name="bufferIndex">Index of the buffer (used for GLB binary chunk detection)</param>
        /// <returns>The loaded binary data, or null on failure</returns>
        public byte[]? ResolveBuffer(GltfBuffer buffer, int bufferIndex)
        {
            // Case 1: GLB binary chunk (first buffer with no URI)
            if (string.IsNullOrEmpty(buffer.uri))
            {
                if (bufferIndex == 0 && _glbBinaryChunk != null)
                {
                    return _glbBinaryChunk;
                }

                DebugLog.Error($"Buffer {bufferIndex} has no URI and no GLB binary chunk available");
                return null;
            }

            // Case 2: Base64 data URI
            if (IsDataUri(buffer.uri))
            {
                return DecodeDataUri(buffer.uri);
            }

            // Case 3: External file
            return LoadExternalFile(buffer.uri);
        }

        /// <summary>
        /// Gets buffer data at the specified view.
        /// </summary>
        /// <param name="gltf">The GLTF root object</param>
        /// <param name="bufferViewIndex">Index of the buffer view</param>
        /// <returns>Byte array segment for the buffer view, or null on failure</returns>
        public byte[]? GetBufferViewData(GltfRoot gltf, int bufferViewIndex)
        {
            if (gltf.bufferViews == null || bufferViewIndex < 0 || bufferViewIndex >= gltf.bufferViews.Count)
            {
                return null;
            }

            GltfBufferView view = gltf.bufferViews[bufferViewIndex];
            
            if (gltf.buffers == null || view.buffer < 0 || view.buffer >= gltf.buffers.Count)
            {
                return null;
            }

            GltfBuffer buffer = gltf.buffers[view.buffer];
            
            if (buffer.Data == null)
            {
                DebugLog.Error($"Buffer {view.buffer} not resolved");
                return null;
            }

            int offset = view.byteOffset;
            int length = view.byteLength;

            if (offset + length > buffer.Data.Length)
            {
                DebugLog.Error($"Buffer view {bufferViewIndex} exceeds buffer bounds");
                return null;
            }

            byte[] result = new byte[length];
            Array.Copy(buffer.Data, offset, result, 0, length);
            return result;
        }

        #endregion

        #region Private Methods

        private static bool IsDataUri(string uri) =>
            uri != null && uri.StartsWith("data:", StringComparison.OrdinalIgnoreCase);

        private static byte[]? DecodeDataUri(string uri)
        {
            // Format: data:[<mediatype>][;base64],<data>
            int commaIndex = uri.IndexOf(',');
            if (commaIndex < 0)
            {
                DebugLog.Error("Invalid data URI: missing comma separator");
                return null;
            }

            string header = uri.Substring(0, commaIndex);
            string data = uri.Substring(commaIndex + 1);

            bool isBase64 = header.Contains(";base64");

            try
            {
                if (isBase64)
                {
                    return Convert.FromBase64String(data);
                }
                else
                {
                    // URL-encoded data
                    string decoded = Uri.UnescapeDataString(data);
                    return Encoding.UTF8.GetBytes(decoded);
                }
            }
            catch (Exception ex)
            {
                DebugLog.Error($"Failed to decode data URI: {ex.Message}");
                return null;
            }
        }

        private byte[]? LoadExternalFile(string uri)
        {
            if (string.IsNullOrEmpty(_basePath))
            {
                DebugLog.Error($"Cannot load external file '{uri}': no base path specified");
                return null;
            }

            try
            {
                // Decode URI components and sanitize
                string decodedUri = Uri.UnescapeDataString(uri);
                
                // Combine with base path
                string fullPath = Path.Combine(_basePath, decodedUri);
                
                // Normalize path separators
                fullPath = Path.GetFullPath(fullPath);

                // Security check: ensure the path is within the base directory
                string normalizedBase = Path.GetFullPath(_basePath);
                if (!fullPath.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase))
                {
                    DebugLog.Error($"Security: external file '{uri}' resolves outside base directory");
                    return null;
                }

                if (!File.Exists(fullPath))
                {
                    DebugLog.Error($"External file not found: {fullPath}");
                    return null;
                }

                return File.ReadAllBytes(fullPath);
            }
            catch (Exception ex)
            {
                DebugLog.Error($"Failed to load external file '{uri}': {ex.Message}");
                return null;
            }
        }

        #endregion
    }
}
