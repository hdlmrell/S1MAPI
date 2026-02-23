namespace S1MAPI.Utils
{
    /// <summary>
    /// Core constants for S1MAPI library
    /// </summary>
    internal static class Constants
    {
        public const string LIBRARY_NAME = "S1MAPI";
        public const string LIBRARY_VERSION = "1.0.0";
        public const string LIBRARY_AUTHOR = "Bars";

        /// <summary>
        /// Minimum supported Unity version
        /// </summary>
        public const string MIN_UNITY_VERSION = "2019.4";

        /// <summary>
        /// Recommended Unity version
        /// </summary>
        public const string RECOMMENDED_UNITY_VERSION = "2022.3.62f2";

        /// <summary>
        /// Layer names used by S1MAPI components
        /// </summary>
        public static class Layers
        {
            public const string DEFAULT = "Default";
            public const string IGNORE_RAYCAST = "Ignore Raycast";
        }

        /// <summary>
        /// Tag names used by S1MAPI components
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

            /// <summary>
            /// Scene material name for closed riser tread planks.
            /// </summary>
            public const string TreadWoodName = "wood brown";

            /// <summary>
            /// Scene material name for closed riser faces.
            /// </summary>
            public const string RiserWoodName = "wood_beige";

            /// <summary>
            /// Scene material name for open stringer tread planks.
            /// </summary>
            public const string StringerTreadWoodName = "mansion_brownwood_mat";

            /// <summary>
            /// Scene material name for open stringer diagonal beams.
            /// </summary>
            public const string StringerBeamWoodName = "wood brown";
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

            /// <summary>
            /// Default maximum step height for generated stairs.
            /// Kept below typical CharacterController stepOffset (~0.3m) for reliable climbing.
            /// </summary>
            public const float DefaultMaxStepHeight = 0.20f;

            /// <summary>
            /// Default step depth (tread) for generated stairs in meters.
            /// </summary>
            public const float DefaultStepDepth = 0.3f;
        }

        /// <summary>
        /// Window geometry and rendering constants.
        /// </summary>
        public static class Window
        {
            /// <summary>
            /// Default divider width between adjacent window panes in meters.
            /// </summary>
            public const float DefaultDividerWidth = 0.15f;

            /// <summary>
            /// Maximum individual pane width for multi-pane windows in meters.
            /// </summary>
            public const float MaxPaneWidth = 2.0f;

            /// <summary>
            /// Minimum individual pane width in meters. Pane count is auto-reduced if panes would be narrower.
            /// </summary>
            public const float MinPaneWidth = 0.3f;

            /// <summary>
            /// Minimum gap between window panes and wall edges in meters.
            /// </summary>
            public const float MinGap = 0.15f;

            /// <summary>
            /// Minimum horizontal margin reserved for side walls in a window section.
            /// </summary>
            public const float SideMargin = 0.5f;

            /// <summary>
            /// Minimum vertical margin reserved for header and sill in a window section.
            /// </summary>
            public const float VerticalMargin = 1.2f;

            /// <summary>
            /// Minimum side width required to place a window in a door side segment.
            /// </summary>
            public const float MinDoorSideWidth = 1.0f;

            /// <summary>
            /// Window frame depth in meters.
            /// </summary>
            public const float FrameDepth = 0.05f;

            /// <summary>
            /// Window frame member width in meters.
            /// </summary>
            public const float FrameWidth = 0.1f;

            /// <summary>
            /// Geometry threshold below which wall segments are not created.
            /// </summary>
            public const float SegmentThreshold = 0.01f;

            /// <summary>
            /// Maximum width of the solid strip next to a door in door-with-windows walls.
            /// </summary>
            public const float MaxDoorStripWidth = 0.5f;

            /// <summary>
            /// Minimum width of the solid strip next to a door in door-with-windows walls.
            /// </summary>
            public const float MinDoorStripWidth = 0.3f;

            /// <summary>
            /// Margin subtracted from the available side width when sizing the door strip.
            /// </summary>
            public const float DoorStripMargin = 0.4f;
        }

        /// <summary>
        /// Terrain and area clearing constants.
        /// </summary>
        public static class Terrain
        {
            /// <summary>
            /// Default padding around clearing bounds in meters.
            /// </summary>
            public const float DefaultClearingPadding = 2f;

            /// <summary>
            /// Default name patterns for vegetation and natural clutter.
            /// Used by the vegetation clearing pass to remove nature objects near buildings.
            /// </summary>
            public static readonly string[] DefaultVegetationKeywords =
            {
                "Rock", "Boulder", "Shrub", "Bush", "Tree rustle"
            };

            /// <summary>
            /// Name patterns for objects protected from footprint destruction.
            /// These typically extend far beyond the building and create visual gaps.
            /// </summary>
            public static readonly string[] DefaultProtectedKeywords =
            {
                "Road", "Sidewalk", "Wedge"
            };
        }

        /// <summary>
        /// GLTF file format constants
        /// </summary>
        public static class Gltf
        {
            /// <summary>
            /// GLB magic number ("glTF" in little-endian)
            /// </summary>
            public const uint GlbMagic = 0x46546C67;

            /// <summary>
            /// JSON chunk type identifier
            /// </summary>
            public const uint ChunkTypeJson = 0x4E4F534A;

            /// <summary>
            /// Binary chunk type identifier
            /// </summary>
            public const uint ChunkTypeBin = 0x004E4942;

            /// <summary>
            /// Current supported GLTF version
            /// </summary>
            public const int SupportedVersion = 2;

            /// <summary>
            /// Default name for imported models
            /// </summary>
            public const string DefaultModelName = "GltfModel";
        }
    }
}
