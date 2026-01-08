#pragma warning disable CS0649

namespace S1MAPI.Gltf.Data
{
    #region Core Structure

    /// <summary>
    /// Root object for a GLTF/GLB file containing scenes, nodes, meshes, and resources.
    /// Conforms to the GLTF 2.0 specification.
    /// </summary>
    [Serializable]
    internal sealed class GltfRoot
    {
        /// <summary>Metadata about the GLTF asset</summary>
        public GltfAsset? asset;
        /// <summary>Array of scenes available in the file</summary>
        public List<GltfScene>? scenes;
        /// <summary>Array of nodes defining the scene hierarchy</summary>
        public List<GltfNode>? nodes;
        /// <summary>Array of meshes defining geometry</summary>
        public List<GltfMesh>? meshes;
        /// <summary>Array of binary buffers containing mesh data</summary>
        public List<GltfBuffer>? buffers;
        /// <summary>Array of buffer views defining memory regions</summary>
        public List<GltfBufferView>? bufferViews;
        /// <summary>Array of accessors for typed data access</summary>
        public List<GltfAccessor>? accessors;
        /// <summary>Array of textures</summary>
        public List<GltfTexture>? textures;
        /// <summary>Array of images</summary>
        public List<GltfImage>? images;
        /// <summary>Array of samplers</summary>
        public List<GltfSampler>? samplers;
        /// <summary>Array of materials for rendering</summary>
        public List<GltfMaterial>? materials;
        /// <summary>Array of skins for skeletal animation</summary>
        public List<GltfSkin>? skins;
        /// <summary>Array of animations</summary>
        public List<GltfAnimation>? animations;
        /// <summary>Array of cameras</summary>
        public List<GltfCamera>? cameras;
        /// <summary>Default scene index to display</summary>
        public int scene;
        /// <summary>Extension data used by this asset</summary>
        public List<string>? extensionsUsed;
        /// <summary>Extension data required for loading</summary>
        public List<string>? extensionsRequired;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Metadata about the GLTF asset.
    /// </summary>
    [Serializable]
    internal sealed class GltfAsset
    {
        /// <summary>GLTF version (e.g., "2.0")</summary>
        public string? version;
        /// <summary>Minimum GLTF version required</summary>
        public string? minVersion;
        /// <summary>Generator tool that created this file</summary>
        public string? generator;
        /// <summary>Copyright information</summary>
        public string? copyright;
    }

    #endregion

    #region Scene and Nodes

    /// <summary>
    /// Defines a scene containing a list of root nodes.
    /// </summary>
    [Serializable]
    internal sealed class GltfScene
    {
        /// <summary>Name of the scene</summary>
        public string? name;
        /// <summary>Array of root node indices</summary>
        public List<int>? nodes;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Defines a node in the scene hierarchy.
    /// Nodes can have transforms, meshes, children, and skinning data.
    /// </summary>
    [Serializable]
    internal sealed class GltfNode
    {
        /// <summary>Name of the node</summary>
        public string? name;
        /// <summary>Index of the mesh attached to this node</summary>
        public int? mesh;
        /// <summary>Index of the skin for skeletal animation</summary>
        public int? skin;
        /// <summary>Index of the camera attached to this node</summary>
        public int? camera;
        /// <summary>Morph target weights for blend shapes</summary>
        public float[]? weights;
        /// <summary>Array of child node indices</summary>
        public List<int>? children;
        /// <summary>4x4 transformation matrix (column-major order)</summary>
        public float[]? matrix;
        /// <summary>Translation vector [x, y, z]</summary>
        public float[]? translation;
        /// <summary>Rotation quaternion [x, y, z, w]</summary>
        public float[]? rotation;
        /// <summary>Scale vector [x, y, z]</summary>
        public float[]? scale;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    #endregion

    #region Meshes and Primitives

    /// <summary>
    /// Defines a mesh containing one or more primitives.
    /// </summary>
    [Serializable]
    internal sealed class GltfMesh
    {
        /// <summary>Name of the mesh</summary>
        public string? name;
        /// <summary>Array of mesh primitives</summary>
        public List<GltfPrimitive>? primitives;
        /// <summary>Morph target weights</summary>
        public float[]? weights;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Defines a mesh primitive (triangle-based geometry).
    /// </summary>
    [Serializable]
    internal sealed class GltfPrimitive
    {
        /// <summary>Map of attribute names to accessor indices (POSITION, NORMAL, TEXCOORD_0, etc.)</summary>
        public Dictionary<string, int>? attributes;
        /// <summary>Index of the accessor containing indices</summary>
        public int? indices;
        /// <summary>Index of the material</summary>
        public int? material;
        /// <summary>Render mode (4 = TRIANGLES)</summary>
        public int mode = 4;
        /// <summary>Morph targets for blend shapes</summary>
        public List<Dictionary<string, int>>? targets;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    #endregion

    #region Buffers and Accessors

    /// <summary>
    /// Defines a binary data buffer.
    /// </summary>
    [Serializable]
    internal sealed class GltfBuffer
    {
        /// <summary>Length of the buffer in bytes</summary>
        public int byteLength;
        /// <summary>URI to the buffer data (embedded base64 or external file)</summary>
        public string? uri;
        /// <summary>Name of the buffer</summary>
        public string? name;

        // Runtime only - not serialized from JSON
        /// <summary>Runtime: Loaded binary data bytes</summary>
        [NonSerialized]
        // ReSharper disable once InconsistentNaming
        public byte[]? Data;
    }

    /// <summary>
    /// Defines a view into a buffer.
    /// </summary>
    [Serializable]
    internal sealed class GltfBufferView
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
        /// <summary>Name of the buffer view</summary>
        public string? name;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Defines how to access binary data in a buffer view.
    /// </summary>
    [Serializable]
    internal sealed class GltfAccessor
    {
        /// <summary>Index of the buffer view</summary>
        public int? bufferView;
        /// <summary>Offset in bytes from the start of the buffer view</summary>
        public int byteOffset;
        /// <summary>Component type (5126=FLOAT, 5123=USHORT, etc.)</summary>
        public int componentType;
        /// <summary>Number of elements in the accessor</summary>
        public int count;
        /// <summary>Type of the elements (SCALAR, VEC2, VEC3, VEC4, MAT4)</summary>
        public string? type;
        /// <summary>Whether integer values are normalized to [0,1] or [-1,1]</summary>
        public bool? normalized;
        /// <summary>Minimum values (for validation/bounding)</summary>
        public float[]? min;
        /// <summary>Maximum values (for validation/bounding)</summary>
        public float[]? max;
        /// <summary>Sparse accessor data for sparse storage</summary>
        public GltfAccessorSparse? sparse;
        /// <summary>Name of the accessor</summary>
        public string? name;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Sparse storage of accessor data.
    /// </summary>
    [Serializable]
    internal sealed class GltfAccessorSparse
    {
        /// <summary>Number of entries stored in the sparse array</summary>
        public int count;
        /// <summary>Index of sparse entries to store</summary>
        public GltfAccessorSparseIndices? indices;
        /// <summary>Values to store at the sparse indices</summary>
        public GltfAccessorSparseValues? values;
    }

    /// <summary>
    /// Indices for sparse accessor data.
    /// </summary>
    [Serializable]
    internal sealed class GltfAccessorSparseIndices
    {
        /// <summary>Index of the buffer view containing indices</summary>
        public int bufferView;
        /// <summary>Offset in bytes</summary>
        public int byteOffset;
        /// <summary>Component type (5121=UBYTE, 5123=USHORT, 5125=UINT)</summary>
        public int componentType;
    }

    /// <summary>
    /// Values for sparse accessor data.
    /// </summary>
    [Serializable]
    internal sealed class GltfAccessorSparseValues
    {
        /// <summary>Index of the buffer view containing values</summary>
        public int bufferView;
        /// <summary>Offset in bytes</summary>
        public int byteOffset;
    }

    #endregion

    #region Materials

    /// <summary>
    /// Defines a material for rendering.
    /// </summary>
    [Serializable]
    internal sealed class GltfMaterial
    {
        /// <summary>Name of the material</summary>
        public string? name;
        /// <summary>PBR metallic-roughness properties</summary>
        public GltfPbrMetallicRoughness? pbrMetallicRoughness;
        /// <summary>Normal texture info</summary>
        public GltfNormalTextureInfo? normalTexture;
        /// <summary>Occlusion texture info</summary>
        public GltfOcclusionTextureInfo? occlusionTexture;
        /// <summary>Emissive texture info</summary>
        public GltfTextureInfo? emissiveTexture;
        /// <summary>Emissive color factor [r, g, b]</summary>
        public float[]? emissiveFactor;
        /// <summary>Alpha rendering mode (OPAQUE, MASK, BLEND)</summary>
        public string? alphaMode;
        /// <summary>Alpha cutoff threshold for MASK mode</summary>
        public float? alphaCutoff;
        /// <summary>Whether the material is double-sided</summary>
        public bool? doubleSided;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Defines PBR metallic-roughness material properties.
    /// </summary>
    [Serializable]
    internal sealed class GltfPbrMetallicRoughness
    {
        /// <summary>Base color factor [r, g, b, a]</summary>
        public float[]? baseColorFactor;
        /// <summary>Base color texture</summary>
        public GltfTextureInfo? baseColorTexture;
        /// <summary>Metallic factor (0-1)</summary>
        public float? metallicFactor;
        /// <summary>Roughness factor (0-1)</summary>
        public float? roughnessFactor;
        /// <summary>Metallic-roughness combined texture</summary>
        public GltfTextureInfo? metallicRoughnessTexture;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Reference to a texture with optional transform.
    /// </summary>
    [Serializable]
    internal sealed class GltfTextureInfo
    {
        /// <summary>Index of the texture</summary>
        public int index;
        /// <summary>Texture coordinate set (0 = TEXCOORD_0)</summary>
        public int? texCoord;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Normal texture info with scale.
    /// </summary>
    [Serializable]
    internal sealed class GltfNormalTextureInfo
    {
        /// <summary>Index of the texture</summary>
        public int index;
        /// <summary>Texture coordinate set</summary>
        public int? texCoord;
        /// <summary>Scale factor for the normal map</summary>
        public float? scale;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Occlusion texture info with strength.
    /// </summary>
    [Serializable]
    internal sealed class GltfOcclusionTextureInfo
    {
        /// <summary>Index of the texture</summary>
        public int index;
        /// <summary>Texture coordinate set</summary>
        public int? texCoord;
        /// <summary>Occlusion strength (0-1)</summary>
        public float? strength;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    #endregion

    #region Textures and Images

    /// <summary>
    /// Defines a texture, pointing to an image and a sampler.
    /// </summary>
    [Serializable]
    internal sealed class GltfTexture
    {
        /// <summary>Index of the sampler</summary>
        public int? sampler;
        /// <summary>Index of the image</summary>
        public int? source;
        /// <summary>Name of the texture</summary>
        public string? name;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Defines an image source.
    /// </summary>
    [Serializable]
    internal sealed class GltfImage
    {
        /// <summary>URI to external image data or base64 data URI</summary>
        public string? uri;
        /// <summary>MIME type of the image (image/jpeg, image/png)</summary>
        public string? mimeType;
        /// <summary>Index of the buffer view containing the image</summary>
        public int? bufferView;
        /// <summary>Name of the image</summary>
        public string? name;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Defines texture filtering and wrapping modes.
    /// </summary>
    [Serializable]
    internal sealed class GltfSampler
    {
        /// <summary>Magnification filter</summary>
        public int? magFilter;
        /// <summary>Minification filter</summary>
        public int? minFilter;
        /// <summary>S (U) wrapping mode</summary>
        public int? wrapS;
        /// <summary>T (V) wrapping mode</summary>
        public int? wrapT;
        /// <summary>Name of the sampler</summary>
        public string? name;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    #endregion

    #region Skinning

    /// <summary>
    /// Defines a skin for skeletal animation.
    /// </summary>
    [Serializable]
    internal sealed class GltfSkin
    {
        /// <summary>Name of the skin</summary>
        public string? name;
        /// <summary>Index of the accessor containing inverse bind matrices</summary>
        public int? inverseBindMatrices;
        /// <summary>Index of the root skeleton node</summary>
        public int? skeleton;
        /// <summary>Array of joint node indices</summary>
        public List<int>? joints;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    #endregion

    #region Animation

    /// <summary>
    /// Defines an animation with channels and samplers.
    /// </summary>
    [Serializable]
    internal sealed class GltfAnimation
    {
        /// <summary>Name of the animation</summary>
        public string? name;
        /// <summary>Animation channels (node property targets)</summary>
        public List<GltfAnimationChannel>? channels;
        /// <summary>Animation samplers (keyframe data)</summary>
        public List<GltfAnimationSampler>? samplers;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Defines an animation channel targeting a node property.
    /// </summary>
    [Serializable]
    internal sealed class GltfAnimationChannel
    {
        /// <summary>Index of the sampler</summary>
        public int sampler;
        /// <summary>Target node and property</summary>
        public GltfAnimationChannelTarget? target;
    }

    /// <summary>
    /// Target for an animation channel.
    /// </summary>
    [Serializable]
    internal sealed class GltfAnimationChannelTarget
    {
        /// <summary>Index of the target node</summary>
        public int? node;
        /// <summary>Property to animate (translation, rotation, scale, weights)</summary>
        public string? path;
    }

    /// <summary>
    /// Defines keyframe data for an animation.
    /// </summary>
    [Serializable]
    internal sealed class GltfAnimationSampler
    {
        /// <summary>Index of accessor containing keyframe times</summary>
        public int input;
        /// <summary>Interpolation mode (LINEAR, STEP, CUBICSPLINE)</summary>
        public string? interpolation;
        /// <summary>Index of accessor containing keyframe values</summary>
        public int output;
    }

    #endregion

    #region Camera

    /// <summary>
    /// Defines a camera (perspective or orthographic).
    /// </summary>
    [Serializable]
    internal sealed class GltfCamera
    {
        /// <summary>Name of the camera</summary>
        public string? name;
        /// <summary>Camera type (perspective or orthographic)</summary>
        public string? type;
        /// <summary>Perspective camera properties</summary>
        public GltfCameraPerspective? perspective;
        /// <summary>Orthographic camera properties</summary>
        public GltfCameraOrthographic? orthographic;
        /// <summary>Custom extensions dictionary</summary>
        public Dictionary<string, object>? extensions;
        /// <summary>Application-specific extras</summary>
        public object? extras;
    }

    /// <summary>
    /// Perspective camera properties.
    /// </summary>
    [Serializable]
    internal sealed class GltfCameraPerspective
    {
        /// <summary>Aspect ratio (width/height)</summary>
        public float? aspectRatio;
        /// <summary>Vertical field of view in radians</summary>
        public float yfov;
        /// <summary>Near clipping plane distance</summary>
        public float znear;
        /// <summary>Far clipping plane distance (optional for infinite projection)</summary>
        public float? zfar;
    }

    /// <summary>
    /// Orthographic camera properties.
    /// </summary>
    [Serializable]
    internal sealed class GltfCameraOrthographic
    {
        /// <summary>Horizontal magnification</summary>
        public float xmag;
        /// <summary>Vertical magnification</summary>
        public float ymag;
        /// <summary>Near clipping plane distance</summary>
        public float znear;
        /// <summary>Far clipping plane distance</summary>
        public float zfar;
    }

    #endregion

    #region Enumerations

    /// <summary>
    /// GLTF component type constants.
    /// </summary>
    internal static class GltfComponentType
    {
        public const int Byte = 5120;
        public const int UnsignedByte = 5121;
        public const int Short = 5122;
        public const int UnsignedShort = 5123;
        public const int UnsignedInt = 5125;
        public const int Float = 5126;

        /// <summary>
        /// Gets the size in bytes for a component type.
        /// </summary>
        public static int GetSize(int componentType) =>
            componentType switch
            {
                Byte => 1,
                UnsignedByte => 1,
                Short => 2,
                UnsignedShort => 2,
                UnsignedInt => 4,
                Float => 4,
                _ => 1
            };
    }

    /// <summary>
    /// GLTF accessor type constants.
    /// </summary>
    internal static class GltfAccessorType
    {
        public const string Scalar = "SCALAR";
        public const string Vec2 = "VEC2";
        public const string Vec3 = "VEC3";
        public const string Vec4 = "VEC4";
        public const string Mat2 = "MAT2";
        public const string Mat3 = "MAT3";
        public const string Mat4 = "MAT4";

        /// <summary>
        /// Gets the number of components for an accessor type.
        /// </summary>
        public static int GetComponentCount(string type) =>
            type switch
            {
                Scalar => 1,
                Vec2 => 2,
                Vec3 => 3,
                Vec4 => 4,
                Mat2 => 4,
                Mat3 => 9,
                Mat4 => 16,
                _ => 1
            };
    }

    /// <summary>
    /// GLTF primitive mode constants.
    /// </summary>
    internal static class GltfPrimitiveMode
    {
        public const int Points = 0;
        public const int Lines = 1;
        public const int LineLoop = 2;
        public const int LineStrip = 3;
        public const int Triangles = 4;
        public const int TriangleStrip = 5;
        public const int TriangleFan = 6;
    }

    /// <summary>
    /// GLTF alpha mode constants.
    /// </summary>
    internal static class GltfAlphaMode
    {
        public const string Opaque = "OPAQUE";
        public const string Mask = "MASK";
        public const string Blend = "BLEND";
    }

    /// <summary>
    /// GLTF texture filter constants.
    /// </summary>
    internal static class GltfTextureFilter
    {
        public const int Nearest = 9728;
        public const int Linear = 9729;
        public const int NearestMipmapNearest = 9984;
        public const int LinearMipmapNearest = 9985;
        public const int NearestMipmapLinear = 9986;
        public const int LinearMipmapLinear = 9987;
    }

    /// <summary>
    /// GLTF texture wrap mode constants.
    /// </summary>
    internal static class GltfTextureWrap
    {
        public const int ClampToEdge = 33071;
        public const int MirroredRepeat = 33648;
        public const int Repeat = 10497;
    }

    /// <summary>
    /// GLTF animation interpolation constants.
    /// </summary>
    internal static class GltfInterpolation
    {
        public const string Linear = "LINEAR";
        public const string Step = "STEP";
        public const string CubicSpline = "CUBICSPLINE";
    }

    /// <summary>
    /// GLTF animation path constants.
    /// </summary>
    internal static class GltfAnimationPath
    {
        public const string Translation = "translation";
        public const string Rotation = "rotation";
        public const string Scale = "scale";
        public const string Weights = "weights";
    }

    #endregion
}

#pragma warning restore CS0649
