using UnityEngine;
using MAPI.Core;
using MAPI.Gltf.Processing;

namespace MAPI.Gltf.Data
{
    /// <summary>
    /// Encapsulates all loading context for a GLTF import operation.
    /// Manages the GLTF root, buffer resolution, and Unity resource creation.
    /// </summary>
    internal sealed class GltfLoadContext
    {
        #region Fields

        private readonly GltfRoot _root;
        private readonly GltfBufferResolver _bufferResolver;
        private readonly GltfImportOptions _options;
        
        private List<Mesh> _meshes;
        private List<Material> _materials;
        private List<Texture2D?> _textures;
        private List<AnimationClip> _animations;
        private Dictionary<int, Transform> _nodeTransforms;

        #endregion

        #region Properties

        /// <summary>
        /// The parsed GLTF root object.
        /// </summary>
        public GltfRoot Root =>
            _root;

        /// <summary>
        /// The buffer resolver for accessing binary data.
        /// </summary>
        public GltfBufferResolver BufferResolver =>
            _bufferResolver;

        /// <summary>
        /// Import options controlling the loading behavior.
        /// </summary>
        public GltfImportOptions Options =>
            _options;

        /// <summary>
        /// Loaded meshes indexed by GLTF mesh index.
        /// </summary>
        public IReadOnlyList<Mesh> Meshes =>
            _meshes;

        /// <summary>
        /// Loaded materials indexed by GLTF material index.
        /// </summary>
        public IReadOnlyList<Material> Materials =>
            _materials;

        /// <summary>
        /// Loaded textures indexed by GLTF texture index.
        /// </summary>
        public IReadOnlyList<Texture2D?> Textures =>
            _textures;

        /// <summary>
        /// Loaded animations indexed by GLTF animation index.
        /// </summary>
        public IReadOnlyList<AnimationClip> Animations =>
            _animations;

        /// <summary>
        /// Map of GLTF node indices to Unity transforms.
        /// </summary>
        public IReadOnlyDictionary<int, Transform> NodeTransforms =>
            _nodeTransforms;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new load context for a GLTF import operation.
        /// </summary>
        /// <param name="root">The parsed GLTF root</param>
        /// <param name="bufferResolver">Buffer resolver for binary data</param>
        /// <param name="options">Import options</param>
        public GltfLoadContext(GltfRoot root, GltfBufferResolver bufferResolver, GltfImportOptions? options = null)
        {
            _root = root;
            _bufferResolver = bufferResolver;
            _options = options ?? new GltfImportOptions();
            
            _meshes = new List<Mesh>();
            _materials = new List<Material>();
            _textures = new List<Texture2D?>();
            _animations = new List<AnimationClip>();
            _nodeTransforms = new Dictionary<int, Transform>();
        }

        #endregion

        #region Resource Management

        /// <summary>
        /// Registers a mesh and tracks it for resource management.
        /// </summary>
        /// <param name="mesh">The mesh to register</param>
        public void RegisterMesh(Mesh mesh)
        {
            if (mesh == null) return;
            
            _meshes.Add(mesh);
            ResourceTracker.Register(mesh);
        }

        /// <summary>
        /// Registers a material and tracks it for resource management.
        /// </summary>
        /// <param name="material">The material to register</param>
        public void RegisterMaterial(Material material)
        {
            if (material == null) return;
            
            _materials.Add(material);
            ResourceTracker.Register(material);
        }

        /// <summary>
        /// Registers a texture and tracks it for resource management.
        /// </summary>
        /// <param name="texture">The texture to register</param>
        public void RegisterTexture(Texture2D? texture)
        {
            if (texture == null)
            {
                _textures.Add(null);
                return;
            }
            
            _textures.Add(texture);
            ResourceTracker.Register(texture);
        }

        /// <summary>
        /// Registers an animation clip and tracks it for resource management.
        /// </summary>
        /// <param name="clip">The animation clip to register</param>
        public void RegisterAnimation(AnimationClip clip)
        {
            if (clip == null) return;
            
            _animations.Add(clip);
            ResourceTracker.Register(clip);
        }

        /// <summary>
        /// Associates a node index with its Unity transform.
        /// </summary>
        /// <param name="nodeIndex">GLTF node index</param>
        /// <param name="transform">Unity transform</param>
        public void RegisterNodeTransform(int nodeIndex, Transform transform)
        {
            _nodeTransforms[nodeIndex] = transform;
        }

        /// <summary>
        /// Gets the Unity transform for a GLTF node index.
        /// </summary>
        /// <param name="nodeIndex">GLTF node index</param>
        /// <returns>The transform, or null if not found</returns>
        public Transform? GetNodeTransform(int nodeIndex) =>
            _nodeTransforms.TryGetValue(nodeIndex, out Transform? t) ? t : null;

        #endregion

        #region Buffer Access

        /// <summary>
        /// Gets the buffer data for an accessor.
        /// </summary>
        /// <param name="accessorIndex">Index of the accessor</param>
        /// <returns>Raw byte data, or null on failure</returns>
        public byte[]? GetAccessorData(int accessorIndex)
        {
            if (_root.accessors == null || accessorIndex < 0 || accessorIndex >= _root.accessors.Count)
            {
                return null;
            }

            GltfAccessor accessor = _root.accessors[accessorIndex];

            if (accessor.type == null)
            {
                return null;
            }

            if (!accessor.bufferView.HasValue)
            {
                // Accessor with no buffer view returns zero-filled data
                int componentSize = GltfComponentType.GetSize(accessor.componentType);
                int componentCount = GltfAccessorType.GetComponentCount(accessor.type);
                return new byte[accessor.count * componentSize * componentCount];
            }

            return _bufferResolver.GetBufferViewData(_root, accessor.bufferView.Value);
        }

        /// <summary>
        /// Gets accessor metadata.
        /// </summary>
        /// <param name="accessorIndex">Index of the accessor</param>
        /// <returns>The accessor, or null if not found</returns>
        public GltfAccessor? GetAccessor(int accessorIndex)
        {
            if (_root.accessors == null || accessorIndex < 0 || accessorIndex >= _root.accessors.Count)
            {
                return null;
            }

            return _root.accessors[accessorIndex];
        }

        /// <summary>
        /// Gets a buffer view.
        /// </summary>
        /// <param name="bufferViewIndex">Index of the buffer view</param>
        /// <returns>The buffer view, or null if not found</returns>
        public GltfBufferView? GetBufferView(int bufferViewIndex)
        {
            if (_root.bufferViews == null || bufferViewIndex < 0 || bufferViewIndex >= _root.bufferViews.Count)
            {
                return null;
            }

            return _root.bufferViews[bufferViewIndex];
        }

        #endregion
    }

    /// <summary>
    /// Options for controlling GLTF import behavior.
    /// </summary>
    public sealed class GltfImportOptions
    {
        /// <summary>
        /// Shader to use for materials. If null, uses Standard shader.
        /// </summary>
        public Shader? Shader { get; set; }

        /// <summary>
        /// Whether to import animations.
        /// </summary>
        public bool ImportAnimations { get; set; } = true;

        /// <summary>
        /// Whether to import skinned meshes with bone weights.
        /// </summary>
        public bool ImportSkins { get; set; } = true;

        /// <summary>
        /// Whether to import morph targets (blend shapes).
        /// </summary>
        public bool ImportBlendShapes { get; set; } = true;

        /// <summary>
        /// Whether to import cameras.
        /// </summary>
        public bool ImportCameras { get; set; } = false;

        /// <summary>
        /// Whether to generate missing normals.
        /// </summary>
        public bool GenerateMissingNormals { get; set; } = true;

        /// <summary>
        /// Whether to generate missing tangents for normal mapping.
        /// </summary>
        public bool GenerateMissingTangents { get; set; } = true;

        /// <summary>
        /// Whether to set imported meshes readable (allows CPU access).
        /// </summary>
        public bool ReadableMeshes { get; set; } = false;

        /// <summary>
        /// Scale factor applied to all positions.
        /// </summary>
        public float ScaleFactor { get; set; } = 1.0f;

        /// <summary>
        /// Name for the root GameObject.
        /// </summary>
        public string RootName { get; set; } = "GltfModel";

        /// <summary>
        /// Multiplier for emissive color intensity.
        /// Values greater than 1.0 create HDR emission for neon/glow effects.
        /// Default is 1.0 (no change).
        /// </summary>
        public float EmissionIntensity { get; set; } = 1.0f;
    }
}
