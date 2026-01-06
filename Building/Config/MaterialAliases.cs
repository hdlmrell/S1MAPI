using UnityEngine;
using MAPI.S1;
using MAPI.Utils;

namespace MAPI.Building
{
    /// <summary>
    /// Short alias for accessing game materials.
    /// Import with: using static MAPI.Building.Mat;
    /// </summary>
    /// <example>
    /// using static MAPI.Building.Mat;
    /// 
    /// builder.AddFloor(material: Concrete);
    /// builder.AddWalls(material: Granite);
    /// </example>
    public static class Mat
    {
        /// <summary>Light grey concrete material</summary>
        public static Material Concrete => Materials.ConcreteLightGrey;

        /// <summary>Medium brown wood material</summary>
        public static Material Wood => Materials.WoodMediumBrown;

        /// <summary>Dull salmon granite material</summary>
        public static Material Granite => Materials.GraniteDullSalmonLighter;

        /// <summary>Brick colored material</summary>
        public static Material Brick => Materials.BrickBrickColored;

        /// <summary>Dark grey metal material</summary>
        public static Material Metal => Materials.MetalDarkGrey;

        /// <summary>Clear glass material (laundromat style)</summary>
        public static Material Glass => Materials.LaundromatGlass;

        /// <summary>
        /// Create a solid color material.
        /// </summary>
        public static Material Color(Color color) => MaterialPresets.Opaque(color);

        /// <summary>
        /// Create an emissive (glowing) material.
        /// </summary>
        public static Material Emissive(Color color, float intensity = 1f) => MaterialPresets.Emissive(color, intensity);

        /// <summary>
        /// Create a transparent glass material.
        /// </summary>
        public static Material TintedGlass(Color tint) => MaterialPresets.Glass(tint);

        /// <summary>
        /// Find a material by name from the game's loaded assets.
        /// </summary>
        public static Material? Find(string name) => MaterialPresets.FindExistingMaterial(name);
    }

    /// <summary>
    /// Short alias for common colors used in buildings.
    /// Import with: using static MAPI.Building.Colors;
    /// </summary>
    public static class Colors
    {
        // Neutrals
        public static Color White => UnityEngine.Color.white;
        public static Color Black => UnityEngine.Color.black;
        public static Color Gray => new(0.5f, 0.5f, 0.5f);
        public static Color LightGray => new(0.75f, 0.75f, 0.75f);
        public static Color DarkGray => new(0.25f, 0.25f, 0.25f);
        public static Color Charcoal => new(0.16f, 0.16f, 0.16f);

        // Woods
        public static Color WoodLight => new(0.65f, 0.45f, 0.25f);
        public static Color WoodMedium => new(0.55f, 0.35f, 0.2f);
        public static Color WoodDark => new(0.35f, 0.2f, 0.1f);

        // Concrete/Stone
        public static Color ConcreteLight => new(0.75f, 0.75f, 0.75f);
        public static Color ConcreteDark => new(0.4f, 0.4f, 0.4f);
        public static Color Slate => new(0.94f, 0.94f, 0.94f);

        // Accent colors
        public static Color KushGreen => new(0.29f, 0.48f, 0.29f);
        public static Color Bronze => new(0.8f, 0.5f, 0.2f);
        public static Color MatteBlack => new(0.1f, 0.1f, 0.1f);
        public static Color Leather => new(0.2f, 0.15f, 0.1f);

        // Brick
        public static Color BrickRed => new(0.6f, 0.3f, 0.2f);
        public static Color BrickTan => new(0.75f, 0.6f, 0.45f);

        // Wall colors
        public static Color Beige => new(0.9f, 0.85f, 0.7f);
        public static Color Cream => new(0.95f, 0.9f, 0.8f);
        public static Color Salmon => new(0.8f, 0.55f, 0.5f);
    }
}
