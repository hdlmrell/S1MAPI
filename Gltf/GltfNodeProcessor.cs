using System.Collections.Generic;
using UnityEngine;
using MAPI.Utils;

namespace MAPI.Gltf
{
    public static class GltfNodeProcessor
    {
        public static List<Transform> ProcessNodes(GltfRoot gltf, GameObject root, List<Mesh> meshes, Shader shader)
        {
            List<Transform> nodes = new List<Transform>();
            Dictionary<int, Transform> nodeMap = new Dictionary<int, Transform>();

            // Create GameObjects
            for (int i = 0; i < gltf.nodes.Count; i++)
            {
                GltfNode node = gltf.nodes[i];
                GameObject go = new GameObject(node.name ?? $"node_{i}");
                nodeMap[i] = go.transform;
                nodes.Add(go.transform);

                // Apply Transforms
                if (node.matrix != null && node.matrix.Length == 16)
                {
                    // Decompose matrix approach
                    Matrix4x4 mat = new Matrix4x4();
                    for (int c = 0; c < 4; c++)
                    {
                        for (int r = 0; r < 4; r++)
                        {
                            mat[r, c] = node.matrix[c * 4 + r];
                        }
                    }
                    
                    // Extract TRS
                    Vector3 pos = mat.GetColumn(3);
                    Quaternion rot = mat.rotation;
                    Vector3 scale = mat.lossyScale;

                    // Convert Coordinate System: GLTF (Right-handed, Y-up) -> Unity (Left-handed, Y-up)
                    // Position: Invert X
                    go.transform.localPosition = new Vector3(-pos.x, pos.y, pos.z);
                    
                    // Rotation: Invert Y and Z (or X and W? Standard is -Y, -Z for Unity quaternion from GLTF)
                    // (x, y, z, w) -> (x, -y, -z, w)
                    go.transform.localRotation = new Quaternion(rot.x, -rot.y, -rot.z, rot.w);
                    
                    // Scale: No change usually
                    go.transform.localScale = scale;
                }
                else
                {
                    // TRS
                    if (node.translation != null)
                    {
                        go.transform.localPosition = new Vector3(-node.translation[0], node.translation[1], node.translation[2]);
                    }
                    
                    if (node.rotation != null)
                    {
                        go.transform.localRotation = new Quaternion(node.rotation[0], -node.rotation[1], -node.rotation[2], node.rotation[3]);
                    }
                    
                    if (node.scale != null)
                    {
                        go.transform.localScale = new Vector3(node.scale[0], node.scale[1], node.scale[2]);
                    }
                }

                // Mesh
                if (node.mesh.HasValue && node.mesh.Value < meshes.Count)
                {
                    Mesh mesh = meshes[node.mesh.Value];
                    MeshFilter mf = go.AddComponent<MeshFilter>();
                    mf.mesh = mesh;
                    MeshRenderer mr = go.AddComponent<MeshRenderer>();
                    
                    // Material assignment
                    // For MAPI v1, we use a default or specified shader. 
                    // Full material parsing is complex (textures, pbr), we'll do basic color if available or default.
                    // We need to look up the material from the mesh primitive if we want to be accurate.
                    // Since we flattened mesh primitives, this is a simplification.
                    
                    Material mat = new Material(shader ?? Shader.Find("Standard"));
                    mr.material = mat;
                }
            }

            // Hierarchy
            for (int i = 0; i < gltf.nodes.Count; i++)
            {
                GltfNode node = gltf.nodes[i];
                Transform parent = nodeMap[i];

                if (node.children != null)
                {
                    foreach (int childIndex in node.children)
                    {
                        if (nodeMap.TryGetValue(childIndex, out Transform child))
                        {
                            child.SetParent(parent, false);
                        }
                    }
                }
            }

            // Find roots (nodes without parents) and attach to root
            foreach (Transform t in nodes)
            {
                if (t.parent == null)
                {
                    t.SetParent(root.transform, false);
                }
            }

            // Skinning would go here (bind poses, bones), but sticking to static meshes for now as per "Phase 4" basics.
            // Full skinning is quite involved.

            return nodes;
        }
    }
}
