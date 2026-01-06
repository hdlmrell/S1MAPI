namespace MAPI.Utils
{
    /// <summary>
    /// Core constants for MAPI library
    /// </summary>
    public static class Constants
    {
        public const string LIBRARY_NAME = "MAPI";
        public const string LIBRARY_VERSION = "0.1.0";
        public const string LIBRARY_AUTHOR = "MAPI Contributors";

        /// <summary>
        /// Minimum supported Unity version
        /// </summary>
        public const string MIN_UNITY_VERSION = "2019.4";

        /// <summary>
        /// Recommended Unity version
        /// </summary>
        public const string RECOMMENDED_UNITY_VERSION = "2020.1";

        /// <summary>
        /// Layer names used by MAPI components
        /// </summary>
        public static class Layers
        {
            public const string DEFAULT = "Default";
            public const string IGNORE_RAYCAST = "Ignore Raycast";
        }

    /// <summary>
    /// Tag names used by MAPI components
    /// </summary>
    public static class Tags
    {
        public const string UNTAGGED = "Untagged";
    }

    /// <summary>
    /// Mesh generation and processing constants
    /// </summary>
    public static class Mesh
    {
        /// <summary>
        /// Maximum vertices per mesh allowed by Unity
        /// </summary>
        public const int MaxVerticesPerMesh = 65535;

        /// <summary>
        /// Threshold distance for welding vertices
        /// </summary>
        public const float VertexWeldThreshold = 0.001f;

        /// <summary>
        /// Default number of radial segments for cylinders
        /// </summary>
        public const int DefaultCylinderSegments = 12;

        /// <summary>
        /// Default subdivision count for sphere generation
        /// </summary>
        public const int DefaultSphereSubdivisions = 8;

        /// <summary>
        /// Default subdivision count for capsule hemispheres
        /// </summary>
        public const int DefaultCapsuleSubdivisions = 6;
    }

    /// <summary>
    /// Material and rendering constants
    /// </summary>
    public static class Materials
    {
        /// <summary>
        /// Default alpha value for transparent materials
        /// </summary>
        public const float DefaultTransparencyAlpha = 0.5f;

        /// <summary>
        /// Alpha value for window/glass materials
        /// </summary>
        public const float WindowGlassAlpha = 0.35f;

        /// <summary>
        /// Alpha value for general glass materials
        /// </summary>
        public const float GlassAlpha = 0.3f;
    }

    /// <summary>
    /// Resource loading constants
    /// </summary>
    public static class Resources
    {
        /// <summary>
        /// Default pixels per unit for sprite creation
        /// </summary>
        public const float DefaultPixelsPerUnit = 100f;
    }

    /// <summary>
    /// Building and spatial constants
    /// </summary>
    public static class Spatial
    {
        /// <summary>
        /// Default grid cell size for snapping operations
        /// </summary>
        public const float DefaultGridSize = 0.5f;
    }
}
}
