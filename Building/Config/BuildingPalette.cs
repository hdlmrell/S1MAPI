using UnityEngine;
using MAPI.S1;

namespace MAPI.Building.Config
{
    /// <summary>
    /// Material and color palette for building construction.
    /// Use presets or create custom palettes for consistent theming.
    /// </summary>
    public sealed class BuildingPalette
    {
        #region Properties

        /// <summary>Floor material (optional, uses color if null)</summary>
        public Material? FloorMaterial { get; set; }
        
        /// <summary>Floor color (used if FloorMaterial is null)</summary>
        public Color FloorColor { get; set; } = new Color(0.7f, 0.7f, 0.7f);

        /// <summary>Wall material (optional, uses color if null)</summary>
        public Material? WallMaterial { get; set; }
        
        /// <summary>Wall color (used if WallMaterial is null)</summary>
        public Color WallColor { get; set; } = new Color(0.9f, 0.85f, 0.7f);

        /// <summary>Ceiling material (optional, uses color if null)</summary>
        public Material? CeilingMaterial { get; set; }
        
        /// <summary>Ceiling color (used if CeilingMaterial is null)</summary>
        public Color CeilingColor { get; set; } = Color.white;

        /// <summary>Trim/molding material (optional)</summary>
        public Material? TrimMaterial { get; set; }
        
        /// <summary>Trim color</summary>
        public Color TrimColor { get; set; } = new Color(0.6f, 0.3f, 0.2f);

        /// <summary>Pillar material (optional)</summary>
        public Material? PillarMaterial { get; set; }
        
        /// <summary>Pillar color</summary>
        public Color PillarColor { get; set; } = new Color(0.6f, 0.3f, 0.2f);

        /// <summary>Accent color for furniture and highlights</summary>
        public Color AccentColor { get; set; } = new Color(0.29f, 0.48f, 0.29f);

        /// <summary>Light color for ceiling lights</summary>
        public Color LightColor { get; set; } = new Color(1f, 0.95f, 0.8f);

        /// <summary>Light intensity</summary>
        public float LightIntensity { get; set; } = 1.0f;

        #endregion

        #region Static Presets

        /// <summary>
        /// Default neutral palette with beige walls and gray floors.
        /// </summary>
        public static BuildingPalette Default => new();

        /// <summary>
        /// Modern industrial palette with concrete and metal.
        /// </summary>
        public static BuildingPalette Industrial => new()
        {
            FloorMaterial = Materials.ConcreteLightGrey,
            FloorColor = new Color(0.75f, 0.75f, 0.75f),
            WallMaterial = Materials.GraniteDullSalmonLighter,
            WallColor = new Color(0.94f, 0.94f, 0.94f),
            CeilingMaterial = Materials.ConcreteLightGrey,
            CeilingColor = new Color(0.94f, 0.94f, 0.94f),
            TrimMaterial = Materials.BrickBrickColored,
            TrimColor = new Color(0.6f, 0.3f, 0.2f),
            PillarMaterial = Materials.BrickBrickColored,
            PillarColor = new Color(0.6f, 0.3f, 0.2f),
            AccentColor = new Color(0.29f, 0.48f, 0.29f),
            LightColor = new Color(1f, 0.98f, 0.95f),
            LightIntensity = 1.2f
        };

        /// <summary>
        /// Clean modern palette with white walls and polished floors.
        /// </summary>
        public static BuildingPalette Modern => new()
        {
            FloorColor = new Color(0.85f, 0.85f, 0.85f),
            WallColor = Color.white,
            CeilingColor = Color.white,
            TrimColor = new Color(0.1f, 0.1f, 0.1f),
            PillarColor = new Color(0.1f, 0.1f, 0.1f),
            AccentColor = new Color(0.2f, 0.4f, 0.8f),
            LightColor = new Color(1f, 1f, 1f),
            LightIntensity = 1.0f
        };

        /// <summary>
        /// Warm wooden palette for cozy interiors.
        /// </summary>
        public static BuildingPalette Rustic => new()
        {
            FloorMaterial = Materials.WoodMediumBrown,
            FloorColor = new Color(0.4f, 0.25f, 0.1f),
            WallColor = new Color(0.95f, 0.9f, 0.8f),
            CeilingColor = new Color(0.9f, 0.85f, 0.75f),
            TrimColor = new Color(0.35f, 0.2f, 0.1f),
            PillarColor = new Color(0.35f, 0.2f, 0.1f),
            AccentColor = new Color(0.6f, 0.3f, 0.15f),
            LightColor = new Color(1f, 0.9f, 0.7f),
            LightIntensity = 0.9f
        };

        /// <summary>
        /// Dark sleek palette for nightclubs or modern retail.
        /// </summary>
        public static BuildingPalette Dark => new()
        {
            FloorMaterial = Materials.MetalDarkGrey,
            FloorColor = new Color(0.15f, 0.15f, 0.15f),
            WallColor = new Color(0.2f, 0.2f, 0.2f),
            CeilingColor = new Color(0.1f, 0.1f, 0.1f),
            TrimColor = new Color(0.05f, 0.05f, 0.05f),
            PillarColor = new Color(0.05f, 0.05f, 0.05f),
            AccentColor = new Color(0.8f, 0.2f, 0.4f),
            LightColor = new Color(0.9f, 0.7f, 1f),
            LightIntensity = 0.8f
        };

        #endregion

        #region Builder Pattern

        /// <summary>
        /// Create a copy of this palette for modification.
        /// </summary>
        public BuildingPalette Clone() => new()
        {
            FloorMaterial = FloorMaterial,
            FloorColor = FloorColor,
            WallMaterial = WallMaterial,
            WallColor = WallColor,
            CeilingMaterial = CeilingMaterial,
            CeilingColor = CeilingColor,
            TrimMaterial = TrimMaterial,
            TrimColor = TrimColor,
            PillarMaterial = PillarMaterial,
            PillarColor = PillarColor,
            AccentColor = AccentColor,
            LightColor = LightColor,
            LightIntensity = LightIntensity
        };

        /// <summary>
        /// Set accent color and return this palette for chaining.
        /// </summary>
        public BuildingPalette WithAccent(Color accent)
        {
            AccentColor = accent;
            return this;
        }

        /// <summary>
        /// Set floor material and return this palette for chaining.
        /// </summary>
        public BuildingPalette WithFloor(Material material)
        {
            FloorMaterial = material;
            return this;
        }

        /// <summary>
        /// Set wall material and return this palette for chaining.
        /// </summary>
        public BuildingPalette WithWalls(Material material)
        {
            WallMaterial = material;
            return this;
        }

        #endregion
    }
}
