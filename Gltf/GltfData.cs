using System.Collections.Generic;

namespace MAPI.Gltf
{
    // Minimal GLTF data structures for JSON deserialization

    /// <summary>
    /// Root object for a GLTF/GLB file containing scenes, nodes, meshes, and resources.
    /// </summary>
    [System.Serializable]
    public class GltfRoot
    {
        /// <summary>Array of scenes available in the file</summary>
        public List<GltfScene> scenes;
        /// <summary>Array of nodes defining the scene hierarchy</summary>
        public List<GltfNode> nodes;
        /// <summary>Array of meshes defining geometry</summary>
        public List<GltfMesh> meshes;
        /// <summary>Array of binary buffers containing mesh data</summary>
        public List<GltfBuffer> buffers;
        /// <summary>Array of buffer views defining memory regions</summary>
        public List<GltfBufferView> bufferViews;
        /// <summary>Array of accessors for typed data access</summary>
        public List<GltfAccessor> accessors;
        /// <summary>Array of materials for rendering</summary>
        public List<GltfMaterial> materials;
        /// <summary>Array of skins for skeletal animation</summary>
        public List<GltfSkin> skins;
        /// <summary>Default scene index to display</summary>
        public int scene;
    }

    /// <summary>
    /// Defines a scene containing a list of root nodes.
    /// </summary>
    [System.Serializable]
    public class GltfScene
    {
        /// <summary>Name of the scene</summary>
        public string name;
        /// <summary>Array of root node indices</summary>
        public List<int> nodes;
    }

    /// <summary>
    /// Defines a node in the scene hierarchy.
    /// Nodes can have transforms, meshes, children, and skinning data.
    /// </summary>
    [System.Serializable]
    public class GltfNode
    {
        /// <summary>Name of the node</summary>
        public string name;
        /// <summary>Index of the mesh attached to this node</summary>
        public int? mesh;
        /// <summary>Index of the skin for skeletal animation</summary>
        public int? skin;
        /// <summary>Array of child node indices</summary>
        public List<int> children;
        /// <summary>4x4 transformation matrix (row-major order)</summary>
        public float[] matrix;
        /// <summary>Translation vector [x, y, z]</summary>
        public float[] translation;
        /// <summary>Rotation quaternion [x, y, z, w]</summary>
        public float[] rotation;
        /// <summary>Scale vector [x, y, z]</summary>
        public float[] scale;
    }

    /// <summary>
    /// Defines a mesh containing one or more primitives.
    /// </summary>
    [System.Serializable]
    public class GltfMesh
    {
        /// <summary>Name of the mesh</summary>
        public string name;
        /// <summary>Array of mesh primitives</summary>
        public List<GltfPrimitive> primitives;
    }

    /// <summary>
    /// Defines a mesh primitive (triangle-based geometry).
    /// </summary>
    [System.Serializable]
    public class GltfPrimitive
    {
        /// <summary>Map of attribute names to accessor indices (POSITION, NORMAL, TEXCOORD_0, etc.)</summary>
        public Dictionary<string, int> attributes;
        /// <summary>Index of the accessor containing indices</summary>
        public int? indices;
        /// <summary>Index of the material</summary>
        public int? material;
        /// <summary>Render mode (4 = TRIANGLES)</summary>
        public int mode = 4;
        /// <summary>Morph targets for blend shapes</summary>
        public List<Dictionary<string, int>> targets;
    }

    /// <summary>
    /// Defines a binary data buffer.
    /// </summary>
    [System.Serializable]
    public class GltfBuffer
    {
        /// <summary>Length of the buffer in bytes</summary>
        public int byteLength;
        /// <summary>URI to the buffer data (embedded base64 or external file)</summary>
        public string uri;
        
        // Runtime only - not serialized
        /// <summary>Runtime: Loaded binary data bytes</summary>
        public byte[] extra; 
    }

    /// <summary>
    /// Defines a view into a buffer.
    /// </summary>
    [System.Serializable]
    public class GltfBufferView
    {
        /// <summary>Index of the buffer</summary>
        public int buffer;
        /// <summary>Offset in bytes from the start of the buffer</summary>
        public int byteOffset;
        /// <summary>Length in bytes</summary>
        public int byteLength;
        /// <summary>Stride in bytes between elements (null for tightly packed)</summary>
        public int? byteStride;
        /// <summary>Target buffer type (34962 = ARRAY_BUFFER, 34963 = ELEMENT_ARRAY_BUFFER)</summary>
        public int? target;
    }

    /// <summary>
    /// Defines how to access binary data in a buffer view.
    /// </summary>
    [System.Serializable]
    public class GltfAccessor
    {
        /// <summary>Index of the buffer view</summary>
        public int bufferView;
        /// <summary>Offset in bytes from the start of the buffer view</summary>
        public int byteOffset;
        /// <summary>Component type (5126=FLOAT, 5123=USHORT, etc.)</summary>
        public int componentType;
        /// <summary>Number of elements in the accessor</summary>
        public int count;
        /// <summary>Type of the elements (SCALAR, VEC2, VEC3, VEC4, MAT4)</summary>
        public string type;
        /// <summary>Minimum values (for validation/bounding)</summary>
        public float[] min;
        /// <summary>Maximum values (for validation/bounding)</summary>
        public float[] max;
    }

    /// <summary>
    /// Defines a material for rendering.
    /// </summary>
    [System.Serializable]
    public class GltfMaterial
    {
        /// <summary>Name of the material</summary>
        public string name;
        /// <summary>PBR metallic-roughness properties</summary>
        public GltfPbrMetallicRoughness pbrMetallicRoughness;
    }

    /// <summary>
    /// Defines PBR metallic-roughness material properties.
    /// </summary>
    [System.Serializable]
    public class GltfPbrMetallicRoughness
    {
        /// <summary>Base color factor [r, g, b, a]</summary>
        public float[] baseColorFactor;
        /// <summary>Base color texture</summary>
        public GltfTextureInfo baseColorTexture;
        /// <summary>Metallic factor (0-1)</summary>
        public float? metallicFactor;
        /// <summary>Roughness factor (0-1)</summary>
        public float? roughnessFactor;
    }

    /// <summary>
    /// Defines a texture and its source sampler.
    /// </summary>
    [System.Serializable]
    public class GltfTextureInfo
    {
        /// <summary>Index of the texture</summary>
        public int index;
    }

    /// <summary>
    /// Defines a skin for skeletal animation.
    /// </summary>
    [System.Serializable]
    public class GltfSkin
    {
        /// <summary>Index of the accessor containing inverse bind matrices</summary>
        public int? inverseBindMatrices;
        /// <summary>Index of the root skeleton node</summary>
        public int? skeleton;
        /// <summary>Array of joint node indices</summary>
        public List<int> joints;
    }
}
