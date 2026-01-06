using System;
using System.Collections.Generic;
using UnityEngine;
using MAPI.Utils;

namespace MAPI.Gltf
{
    /// <summary>
    /// Processes GLTF mesh primitives and converts them to Unity meshes.
    /// Handles vertex data extraction, coordinate system conversion, and UV flipping.
    /// </summary>
    public static class GltfMeshProcessor
    {
        /// <summary>
        /// Process GLTF meshes and convert them to Unity meshes.
        /// </summary>
        /// <param name="gltf">The parsed GLTF root object</param>
        /// <param name="binaryBuffer">The binary buffer containing mesh data</param>
        /// <returns>List of Unity meshes</returns>
        public static List<Mesh> ProcessMeshes(GltfRoot gltf, byte[] binaryBuffer)
        {
            List<Mesh> meshes = new List<Mesh>();

            if (gltf.meshes == null) return meshes;

            foreach (GltfMesh gltfMesh in gltf.meshes)
            {
                // Unity Mesh combines primitives into submeshes, but here we might simplify
                // For now, let's assume one primitive per mesh or combine them
                // A robust implementation would handle multiple primitives as submeshes
                
                Mesh unityMesh = new Mesh();
                unityMesh.name = gltfMesh.name ?? "gltf_mesh";

                // We'll process the first primitive for simplicity in this version, 
                // or merge them if necessary. The analysis showed a simple loop.
                // Let's implement full multi-primitive support if possible, or just take the first one.
                
                if (gltfMesh.primitives != null && gltfMesh.primitives.Count > 0)
                {
                    // For MAPI v1, we focus on the first primitive as most simple models use one.
                    // Complex multi-material meshes would need submeshes.
                    GltfPrimitive primitive = gltfMesh.primitives[0];

                    // Positions
                    if (primitive.attributes.TryGetValue("POSITION", out int posIndex))
                    {
                        unityMesh.vertices = ReadVector3Array(gltf, binaryBuffer, posIndex, true);
                    }

                    // Normals
                    if (primitive.attributes.TryGetValue("NORMAL", out int normIndex))
                    {
                        unityMesh.normals = ReadVector3Array(gltf, binaryBuffer, normIndex, true);
                    }

                    // UVs
                    if (primitive.attributes.TryGetValue("TEXCOORD_0", out int uvIndex))
                    {
                        unityMesh.uv = ReadVector2Array(gltf, binaryBuffer, uvIndex, false); // UVs usually Y-flipped? GLTF is top-left origin?
                        // GLTF UV origin is top-left (0,0) -> (1,1). Unity is bottom-left. 
                        // Often requires V = 1 - V.
                        FlipUVs(unityMesh.uv);
                    }

                    // Tangents
                    if (primitive.attributes.TryGetValue("TANGENT", out int tanIndex))
                    {
                        unityMesh.tangents = ReadVector4Array(gltf, binaryBuffer, tanIndex, true);
                    }

                    // Indices / Triangles
                    if (primitive.indices.HasValue)
                    {
                        int[] indices = ReadIntArray(gltf, binaryBuffer, primitive.indices.Value);
                        // GLTF is CCW, Unity is CW? Or handled by X-inversion?
                        // Since we inverted X, triangle winding usually flips.
                        // We need to reverse triangle indices to maintain front-face.
                        FlipTriangles(indices);
                        unityMesh.triangles = indices;
                    }
                    
                    // Bone Weights (Joints/Weights)
                    if (primitive.attributes.TryGetValue("JOINTS_0", out int jointsIndex) &&
                        primitive.attributes.TryGetValue("WEIGHTS_0", out int weightsIndex))
                    {
                        unityMesh.boneWeights = ReadBoneWeights(gltf, binaryBuffer, jointsIndex, weightsIndex);
                    }

                    unityMesh.RecalculateBounds();
                }

                meshes.Add(unityMesh);
            }

            return meshes;
        }

        private static void FlipUVs(Vector2[] uvs)
        {
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i].y = 1.0f - uvs[i].y;
            }
        }

        private static void FlipTriangles(int[] indices)
        {
            for (int i = 0; i < indices.Length; i += 3)
            {
                int temp = indices[i];
                indices[i] = indices[i + 2];
                indices[i + 2] = temp;
            }
        }

        private static Vector3[] ReadVector3Array(GltfRoot gltf, byte[] buffer, int accessorIndex, bool convertCoordinate)
        {
            GltfAccessor accessor = gltf.accessors[accessorIndex];
            GltfBufferView view = gltf.bufferViews[accessor.bufferView];
            
            int count = accessor.count;
            int startOffset = view.byteOffset + accessor.byteOffset;
            int stride = view.byteStride ?? 12; // 3 * float(4)

            Vector3[] result = new Vector3[count];

            for (int i = 0; i < count; i++)
            {
                int offset = startOffset + i * stride;
                float x = BitConverter.ToSingle(buffer, offset);
                float y = BitConverter.ToSingle(buffer, offset + 4);
                float z = BitConverter.ToSingle(buffer, offset + 8);

                if (convertCoordinate)
                {
                    // GLTF (Right-handed, Y-up) -> Unity (Left-handed, Y-up)
                    // Invert X
                    result[i] = new Vector3(-x, y, z);
                }
                else
                {
                    result[i] = new Vector3(x, y, z);
                }
            }

            return result;
        }

        private static Vector2[] ReadVector2Array(GltfRoot gltf, byte[] buffer, int accessorIndex, bool flipY)
        {
            GltfAccessor accessor = gltf.accessors[accessorIndex];
            GltfBufferView view = gltf.bufferViews[accessor.bufferView];
            
            int count = accessor.count;
            int startOffset = view.byteOffset + accessor.byteOffset;
            int stride = view.byteStride ?? 8; // 2 * float(4)

            Vector2[] result = new Vector2[count];

            for (int i = 0; i < count; i++)
            {
                int offset = startOffset + i * stride;
                float x = BitConverter.ToSingle(buffer, offset);
                float y = BitConverter.ToSingle(buffer, offset + 4);

                result[i] = new Vector2(x, y);
            }

            return result;
        }

        private static Vector4[] ReadVector4Array(GltfRoot gltf, byte[] buffer, int accessorIndex, bool convertCoordinate)
        {
            GltfAccessor accessor = gltf.accessors[accessorIndex];
            GltfBufferView view = gltf.bufferViews[accessor.bufferView];
            
            int count = accessor.count;
            int startOffset = view.byteOffset + accessor.byteOffset;
            int stride = view.byteStride ?? 16; // 4 * float(4)

            Vector4[] result = new Vector4[count];

            for (int i = 0; i < count; i++)
            {
                int offset = startOffset + i * stride;
                float x = BitConverter.ToSingle(buffer, offset);
                float y = BitConverter.ToSingle(buffer, offset + 4);
                float z = BitConverter.ToSingle(buffer, offset + 8);
                float w = BitConverter.ToSingle(buffer, offset + 12);

                if (convertCoordinate)
                {
                    // Tangents might need inversion similar to rotation/position
                    // Usually X inverted matches position inversion
                    result[i] = new Vector4(-x, y, z, w);
                }
                else
                {
                    result[i] = new Vector4(x, y, z, w);
                }
            }

            return result;
        }

        private static int[] ReadIntArray(GltfRoot gltf, byte[] buffer, int accessorIndex)
        {
            GltfAccessor accessor = gltf.accessors[accessorIndex];
            GltfBufferView view = gltf.bufferViews[accessor.bufferView];
            
            int count = accessor.count;
            int startOffset = view.byteOffset + accessor.byteOffset;
            int[] result = new int[count];

            // Component Type:
            // 5120 (BYTE), 5121 (UNSIGNED_BYTE)
            // 5122 (SHORT), 5123 (UNSIGNED_SHORT)
            // 5125 (UNSIGNED_INT)

            int stride = view.byteStride ?? GetComponentSize(accessor.componentType);

            for (int i = 0; i < count; i++)
            {
                int offset = startOffset + i * stride;
                switch (accessor.componentType)
                {
                    case 5121: // UNSIGNED_BYTE
                        result[i] = buffer[offset];
                        break;
                    case 5123: // UNSIGNED_SHORT
                        result[i] = BitConverter.ToUInt16(buffer, offset);
                        break;
                    case 5125: // UNSIGNED_INT
                        result[i] = (int)BitConverter.ToUInt32(buffer, offset);
                        break;
                    default:
                        DebugLog.Warning($"Unsupported component type for indices: {accessor.componentType}");
                        break;
                }
            }

            return result;
        }

        private static BoneWeight[] ReadBoneWeights(GltfRoot gltf, byte[] buffer, int jointsIndex, int weightsIndex)
        {
            // Read Joints (usually USHORT)
            // Read Weights (usually FLOAT or normalized types)
            
            // This is a simplified implementation assuming standard 4 weights per vertex
            
            GltfAccessor jAcc = gltf.accessors[jointsIndex];
            GltfAccessor wAcc = gltf.accessors[weightsIndex];
            
            // Assuming standard count match
            int count = jAcc.count;
            BoneWeight[] weights = new BoneWeight[count];
            
            // Read all raw values first? No, read per vertex.
            // Simplified: Assume JOINTS is USHORT vec4, WEIGHTS is FLOAT vec4
            
            int jStart = gltf.bufferViews[jAcc.bufferView].byteOffset + jAcc.byteOffset;
            int wStart = gltf.bufferViews[wAcc.bufferView].byteOffset + wAcc.byteOffset;
            
            // Assuming tight packing for simplicity in this version
            
            for (int i = 0; i < count; i++)
            {
                BoneWeight bw = new BoneWeight();
                
                // Read 4 joints
                // Careful with component types here. JOINTS is typically 5123 (USHORT) or 5121 (UBYTE)
                int jOffset = jStart + i * (jAcc.componentType == 5123 ? 8 : 4);
                
                if (jAcc.componentType == 5123)
                {
                    bw.boneIndex0 = BitConverter.ToUInt16(buffer, jOffset);
                    bw.boneIndex1 = BitConverter.ToUInt16(buffer, jOffset + 2);
                    bw.boneIndex2 = BitConverter.ToUInt16(buffer, jOffset + 4);
                    bw.boneIndex3 = BitConverter.ToUInt16(buffer, jOffset + 6);
                }
                else // UBYTE
                {
                    bw.boneIndex0 = buffer[jOffset];
                    bw.boneIndex1 = buffer[jOffset + 1];
                    bw.boneIndex2 = buffer[jOffset + 2];
                    bw.boneIndex3 = buffer[jOffset + 3];
                }

                // Read 4 weights
                // WEIGHTS is typically 5126 (FLOAT)
                int wOffset = wStart + i * 16; 
                bw.weight0 = BitConverter.ToSingle(buffer, wOffset);
                bw.weight1 = BitConverter.ToSingle(buffer, wOffset + 4);
                bw.weight2 = BitConverter.ToSingle(buffer, wOffset + 8);
                bw.weight3 = BitConverter.ToSingle(buffer, wOffset + 12);
                
                weights[i] = bw;
            }

            return weights;
        }

        private static int GetComponentSize(int componentType)
        {
            switch (componentType)
            {
                case 5120: return 1;
                case 5121: return 1;
                case 5122: return 2;
                case 5123: return 2;
                case 5125: return 4;
                case 5126: return 4;
                default: return 1;
            }
        }
    }
}
