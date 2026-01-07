using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; // For IndexFormat
using MAPI.Utils;

#if IL2CPP
using Il2CppInterop.Runtime.InteropTypes.Arrays;
#endif

namespace MAPI.Gltf
{
    public class GltfMeshResult
    {
        public Mesh mesh;
        public int[] materialIndices;
    }

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
        /// <returns>List of processed meshes with material indices</returns>
        public static List<GltfMeshResult> ProcessMeshes(GltfRoot gltf, byte[] binaryBuffer)
        {
            List<GltfMeshResult> results = new List<GltfMeshResult>();

            if (gltf.meshes == null) return results;

            foreach (GltfMesh gltfMesh in gltf.meshes)
            {
                Mesh unityMesh = new Mesh();
                unityMesh.name = gltfMesh.name ?? "gltf_mesh";
                
                // Allow large meshes
                unityMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                List<Vector3> allVertices = new List<Vector3>();
                List<Vector3> allNormals = new List<Vector3>();
                List<Vector2> allUvs = new List<Vector2>();
                List<Vector4> allTangents = new List<Vector4>();
                List<BoneWeight> allBoneWeights = new List<BoneWeight>();
                List<int[]> allSubmeshIndices = new List<int[]>();
                List<int> materialIndices = new List<int>();

                int vertexOffset = 0;

                if (gltfMesh.primitives != null)
                {
                    foreach (GltfPrimitive primitive in gltfMesh.primitives)
                    {
                        // Positions
                        int vertexCount = 0;
                        if (primitive.attributes.TryGetValue("POSITION", out int posIndex))
                        {
                            Vector3[] verts = ReadVector3Array(gltf, binaryBuffer, posIndex, true);
                            vertexCount = verts.Length;
                            allVertices.AddRange(verts);
                        }

                        // Normals
                        if (primitive.attributes.TryGetValue("NORMAL", out int normIndex))
                        {
                            allNormals.AddRange(ReadVector3Array(gltf, binaryBuffer, normIndex, true));
                        }

                        // UVs
                        if (primitive.attributes.TryGetValue("TEXCOORD_0", out int uvIndex))
                        {
                            Vector2[] uvs = ReadVector2Array(gltf, binaryBuffer, uvIndex, false);
                            FlipUVs(uvs);
                            allUvs.AddRange(uvs);
                        }

                        // Tangents
                        if (primitive.attributes.TryGetValue("TANGENT", out int tanIndex))
                        {
                            allTangents.AddRange(ReadVector4Array(gltf, binaryBuffer, tanIndex, true));
                        }

                        // Bone Weights
                        if (primitive.attributes.TryGetValue("JOINTS_0", out int jointsIndex) &&
                            primitive.attributes.TryGetValue("WEIGHTS_0", out int weightsIndex))
                        {
                            allBoneWeights.AddRange(ReadBoneWeights(gltf, binaryBuffer, jointsIndex, weightsIndex));
                        }

                        // Indices
                        if (primitive.indices.HasValue)
                        {
                            int[] indices = ReadIntArray(gltf, binaryBuffer, primitive.indices.Value);
                            FlipTriangles(indices);
                            
                            // Apply offset for combined mesh
                            if (vertexOffset > 0)
                            {
                                for (int k = 0; k < indices.Length; k++)
                                {
                                    indices[k] += vertexOffset;
                                }
                            }
                            
                            allSubmeshIndices.Add(indices);
                        }
                        else
                        {
                            // Non-indexed geometry not supported in this simplified version
                             DebugLog.Warning($"Non-indexed primitive in mesh '{unityMesh.name}'. Skipping.");
                             allSubmeshIndices.Add(new int[0]);
                        }

                        materialIndices.Add(primitive.material ?? -1); // -1 means default material
                        vertexOffset += vertexCount;
                    }
                }

                // Assign to Unity Mesh
#if IL2CPP
                unityMesh.SetVertices(allVertices.ToIl2CppList());
                if (allNormals.Count > 0) unityMesh.SetNormals(allNormals.ToIl2CppList());
                if (allUvs.Count > 0) unityMesh.SetUVs(0, allUvs.ToIl2CppList());
                if (allTangents.Count > 0) unityMesh.SetTangents(allTangents.ToIl2CppList());
#else
                unityMesh.SetVertices(allVertices);
                if (allNormals.Count > 0) unityMesh.SetNormals(allNormals);
                if (allUvs.Count > 0) unityMesh.SetUVs(0, allUvs);
                if (allTangents.Count > 0) unityMesh.SetTangents(allTangents);
#endif
                if (allBoneWeights.Count > 0) unityMesh.boneWeights = allBoneWeights.ToArray();

                unityMesh.subMeshCount = allSubmeshIndices.Count;
                for (int i = 0; i < allSubmeshIndices.Count; i++)
                {
                    unityMesh.SetTriangles(allSubmeshIndices[i], i);
                }

                unityMesh.RecalculateBounds();
                
                results.Add(new GltfMeshResult
                {
                    mesh = unityMesh,
                    materialIndices = materialIndices.ToArray()
                });
            }

            return results;
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
            if (!accessor.bufferView.HasValue)
            {
                return new Vector3[accessor.count];
            }
            GltfBufferView view = gltf.bufferViews[accessor.bufferView.Value];
            
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
            if (!accessor.bufferView.HasValue)
            {
                return new Vector2[accessor.count];
            }
            GltfBufferView view = gltf.bufferViews[accessor.bufferView.Value];
            
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
            if (!accessor.bufferView.HasValue)
            {
                return new Vector4[accessor.count];
            }
            GltfBufferView view = gltf.bufferViews[accessor.bufferView.Value];
            
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
            if (!accessor.bufferView.HasValue)
            {
                return new int[accessor.count];
            }
            GltfBufferView view = gltf.bufferViews[accessor.bufferView.Value];
            
            int count = accessor.count;
            int startOffset = view.byteOffset + accessor.byteOffset;
            int[] result = new int[count];

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
            GltfAccessor jAcc = gltf.accessors[jointsIndex];
            GltfAccessor wAcc = gltf.accessors[weightsIndex];
            
            if (!jAcc.bufferView.HasValue || !wAcc.bufferView.HasValue)
            {
                return new BoneWeight[jAcc.count];
            }
            
            int count = jAcc.count;
            BoneWeight[] weights = new BoneWeight[count];
            
            int jStart = gltf.bufferViews[jAcc.bufferView.Value].byteOffset + jAcc.byteOffset;
            int wStart = gltf.bufferViews[wAcc.bufferView.Value].byteOffset + wAcc.byteOffset;
            
            for (int i = 0; i < count; i++)
            {
                BoneWeight bw = new BoneWeight();
                
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
