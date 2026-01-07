using UnityEngine;
using MAPI.S1;

namespace MAPI.Building.Config
{
    /// <summary>
    /// Material and color palette for building construction.
    /// Create custom palettes for consistent theming.
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
        
        // <summary>Accent material for furniture and highlights</summary>
        public Material? AccentMaterial { get; set; }

        /// <summary>Light color for ceiling lights</summary>
        public Color LightColor { get; set; } = new Color(1f, 0.95f, 0.8f);

        /// <summary>Light intensity</summary>
        public float LightIntensity { get; set; } = 1.0f;

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
            AccentMaterial = AccentMaterial,
            LightColor = LightColor,
            LightIntensity = LightIntensity
        };

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
