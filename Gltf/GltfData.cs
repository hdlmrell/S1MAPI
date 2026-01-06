using System.Collections.Generic;

namespace MAPI.Gltf
{
    // Minimal GLTF data structures for JSON deserialization
    
    [System.Serializable]
    public class GltfRoot
    {
        public List<GltfScene> scenes;
        public List<GltfNode> nodes;
        public List<GltfMesh> meshes;
        public List<GltfBuffer> buffers;
        public List<GltfBufferView> bufferViews;
        public List<GltfAccessor> accessors;
        public List<GltfMaterial> materials;
        public List<GltfSkin> skins;
        public int scene; // Default scene index
    }

    [System.Serializable]
    public class GltfScene
    {
        public string name;
        public List<int> nodes;
    }

    [System.Serializable]
    public class GltfNode
    {
        public string name;
        public int? mesh;
        public int? skin;
        public List<int> children;
        public float[] matrix;
        public float[] translation;
        public float[] rotation;
        public float[] scale;
    }

    [System.Serializable]
    public class GltfMesh
    {
        public string name;
        public List<GltfPrimitive> primitives;
    }

    [System.Serializable]
    public class GltfPrimitive
    {
        public Dictionary<string, int> attributes;
        public int? indices;
        public int? material;
        public int mode = 4; // TRIANGLES
        public List<Dictionary<string, int>> targets; // Morph targets
    }

    [System.Serializable]
    public class GltfBuffer
    {
        public int byteLength;
        public string uri;
        
        // Runtime only - not serialized
        public byte[] extra; 
    }

    [System.Serializable]
    public class GltfBufferView
    {
        public int buffer;
        public int byteOffset;
        public int byteLength;
        public int? byteStride;
        public int? target;
    }

    [System.Serializable]
    public class GltfAccessor
    {
        public int bufferView;
        public int byteOffset;
        public int componentType; // 5126=FLOAT, 5123=USHORT, etc.
        public int count;
        public string type; // "SCALAR", "VEC3", etc.
        public float[] min;
        public float[] max;
    }

    [System.Serializable]
    public class GltfMaterial
    {
        public string name;
        public GltfPbrMetallicRoughness pbrMetallicRoughness;
    }

    [System.Serializable]
    public class GltfPbrMetallicRoughness
    {
        public float[] baseColorFactor;
        public GltfTextureInfo baseColorTexture;
        public float? metallicFactor;
        public float? roughnessFactor;
    }

    [System.Serializable]
    public class GltfTextureInfo
    {
        public int index;
    }

    [System.Serializable]
    public class GltfSkin
    {
        public int? inverseBindMatrices;
        public int? skeleton;
        public List<int> joints;
    }
}
