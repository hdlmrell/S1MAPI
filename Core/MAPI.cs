using System;
using System.Reflection;
using UnityEngine;
using MAPI.ProceduralMesh;
using MAPI.Utils;

namespace MAPI.Core
{
    /// <summary>
    /// Main entry point and facade for the MAPI library
    /// Provides factory methods and convenience APIs for mod developers
    /// </summary>
    public static class MAPI
    {
        #region Properties
        
        /// <summary>
        /// Gets whether MAPI has been initialized
        /// </summary>
        public static bool IsInitialized =>
            MAPICore.IsInitialized;

        /// <summary>
        /// Gets the MAPI library version
        /// </summary>
        public static string Version =>
            MAPICore.Version;
        
        #endregion

        #region Initialization
        
        /// <summary>
        /// Initialize the MAPI library
        /// This should be called by consuming mods during their initialization
        /// </summary>
        /// <param name="defaultShader">Optional default shader for materials</param>
        public static void Initialize(Shader? defaultShader = null)
        {
            MAPICore.Initialize();

            if (defaultShader != null)
            {
                MaterialPresets.DefaultShader = defaultShader;
            }
        }

        /// <summary>
        /// Shutdown the MAPI library
        /// This should be called by consuming mods during their cleanup
        /// </summary>
        public static void Shutdown()
        {
            MAPICore.Shutdown();
        }
        
        #endregion

        #region Factory Methods - Mesh Generation
        
        /// <summary>
        /// Create a new procedural mesh builder
        /// </summary>
        /// <param name="name">Name for the mesh</param>
        /// <returns>A new ProceduralMeshBuilder instance</returns>
        public static ProceduralMeshBuilder CreateMesh(string name)
        {
            if (!IsInitialized)
            {
                DebugLog.Warning("MAPI is not initialized. Call MAPI.Initialize() first.");
            }

            return new ProceduralMeshBuilder(name);
        }

        /// <summary>
        /// Create a primitive GameObject (cube, sphere, cylinder, etc.)
        /// </summary>
        /// <param name="type">The type of primitive</param>
        /// <param name="name">Optional name for the GameObject</param>
        /// <returns>A GameObject with the primitive mesh</returns>
        public static GameObject CreatePrimitive(PrimitiveType type, string? name = null)
        {
            if (!IsInitialized)
            {
                DebugLog.Warning("MAPI is not initialized. Call MAPI.Initialize() first.");
            }

            GameObject primitive = GameObject.CreatePrimitive(type);
            if (!string.IsNullOrEmpty(name))
            {
                primitive.name = name;
            }

            return primitive;
        }
        
        #endregion

        #region Factory Methods - Building Construction
        
        /// <summary>
        /// Create a new building builder
        /// </summary>
        /// <param name="name">Name for the building</param>
        /// <returns>A new BuildingBuilder instance</returns>
        public static Building.BuildingBuilder CreateBuilding(string name)
        {
            if (!IsInitialized)
            {
                DebugLog.Warning("MAPI is not initialized. Call MAPI.Initialize() first.");
            }

            return new Building.BuildingBuilder(name);
        }

        /// <summary>
        /// Create a new semantic building builder for LLM-friendly construction
        /// Uses descriptive terms instead of precise coordinates
        /// </summary>
        /// <param name="name">Name for the building</param>
        /// <returns>A new SemanticBuildingBuilder instance</returns>
        public static Building.SemanticBuildingBuilder CreateSemanticBuilding(string name)
        {
            if (!IsInitialized)
            {
                DebugLog.Warning("MAPI is not initialized. Call MAPI.Initialize() first.");
            }

            return new Building.SemanticBuildingBuilder(name);
        }

        /// <summary>
        /// Create a new interior builder for furnishings
        /// </summary>
        /// <param name="name">Name for the interior</param>
        /// <returns>A new InteriorBuilder instance</returns>
        public static Building.InteriorBuilder CreateInterior(string name = "Interior")
        {
            if (!IsInitialized)
            {
                DebugLog.Warning("MAPI is not initialized. Call MAPI.Initialize() first.");
            }

            return new Building.InteriorBuilder(name);
        }
        
        #endregion

        #region Resource Loading
        
        /// <summary>
        /// Load a texture from an embedded resource
        /// </summary>
        /// <param name="resourceName">The fully qualified resource name</param>
        /// <param name="assembly">Optional assembly to load from (defaults to calling assembly)</param>
        /// <returns>The loaded texture, or null if loading failed</returns>
        public static Texture2D? LoadEmbeddedTexture(string resourceName, Assembly? assembly = null)
        {
            if (!IsInitialized)
            {
                DebugLog.Warning("MAPI is not initialized. Call MAPI.Initialize() first.");
            }

            return EmbeddedResourceLoader.LoadTexture(resourceName, assembly);
        }

        /// <summary>
        /// Load a sprite from an embedded resource
        /// </summary>
        /// <param name="resourceName">The fully qualified resource name</param>
        /// <param name="assembly">Optional assembly to load from (defaults to calling assembly)</param>
        /// <param name="pixelsPerUnit">Pixels per unit for the sprite</param>
        /// <returns>The loaded sprite, or null if loading failed</returns>
        public static Sprite? LoadEmbeddedSprite(string resourceName, Assembly? assembly = null, float pixelsPerUnit = Constants.Resources.DefaultPixelsPerUnit)
        {
            if (!IsInitialized)
            {
                DebugLog.Warning("MAPI is not initialized. Call MAPI.Initialize() first.");
            }

            return EmbeddedResourceLoader.LoadSprite(resourceName, assembly, pixelsPerUnit);
        }

        /// <summary>
        /// Load raw bytes from an embedded resource
        /// </summary>
        /// <param name="resourceName">The fully qualified resource name</param>
        /// <param name="assembly">Optional assembly to load from (defaults to calling assembly)</param>
        /// <returns>The resource bytes, or null if not found</returns>
        public static byte[]? LoadEmbeddedBytes(string resourceName, Assembly? assembly = null)
        {
            if (!IsInitialized)
            {
                DebugLog.Warning("MAPI is not initialized. Call MAPI.Initialize() first.");
            }

            return EmbeddedResourceLoader.LoadBytes(resourceName, assembly);
        }

        /// <summary>
        /// Load a GLTF/GLB model from an embedded resource or byte array
        /// </summary>
        /// <param name="glbBytes">Raw GLB bytes</param>
        /// <param name="shader">Optional shader to use for materials</param>
        /// <returns>The root GameObject of the loaded model</returns>
        public static GameObject LoadGltf(byte[] glbBytes, Shader? shader = null)
        {
            if (!IsInitialized)
            {
                DebugLog.Warning("MAPI is not initialized. Call MAPI.Initialize() first.");
            }

            return Gltf.GltfLoader.LoadFromBytes(glbBytes, shader);
        }

        /// <summary>
        /// Load a GLTF/GLB model from an embedded resource
        /// </summary>
        /// <param name="resourceName">The fully qualified resource name</param>
        /// <param name="assembly">Optional assembly to load from</param>
        /// <param name="shader">Optional shader to use for materials</param>
        /// <returns>The root GameObject of the loaded model</returns>
        public static GameObject LoadEmbeddedGltf(string resourceName, Assembly? assembly = null, Shader? shader = null)
        {
            byte[]? bytes = LoadEmbeddedBytes(resourceName, assembly);
            if (bytes == null) return null;
            return LoadGltf(bytes, shader);
        }
        
        #endregion

        #region Utility Methods
        
        /// <summary>
        /// Clone a GameObject with modification options
        /// </summary>
        /// <param name="original">The object to clone</param>
        /// <returns>An ObjectCloner builder</returns>
        public static ObjectCloner Clone(GameObject original)
        {
            return new ObjectCloner(original);
        }

        /// <summary>
        /// Register a Unity object for automatic cleanup
        /// </summary>
        /// <param name="resource">The resource to register</param>
        public static void RegisterResource(UnityEngine.Object resource)
        {
            ResourceTracker.Register(resource);
        }

        /// <summary>
        /// Snap a position to a grid
        /// </summary>
        /// <param name="position">The position to snap</param>
        /// <param name="gridSize">The grid cell size</param>
        /// <returns>The snapped position</returns>
        public static Vector3 SnapToGrid(Vector3 position, float gridSize = Constants.Spatial.DefaultGridSize)
        {
            return Building.BuildingUtilities.SnapToGrid(position, gridSize);
        }

        /// <summary>
        /// Create a material with the specified color
        /// </summary>
        /// <param name="color">The base color</param>
        /// <param name="transparent">Whether the material should be transparent</param>
        /// <param name="alpha">Alpha value if transparent</param>
        /// <returns>A new material</returns>
        public static Material CreateMaterial(Color color, bool transparent = false, float alpha = Constants.Materials.DefaultTransparencyAlpha)
        {
            if (transparent)
            {
                return MaterialPresets.Transparent(color, alpha);
            }
            else
            {
                return MaterialPresets.Opaque(color);
            }
        }
        
        #endregion
    }
}
