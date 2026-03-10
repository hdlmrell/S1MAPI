using UnityEngine;

namespace S1MAPI.Building.Structural
{
    /// <summary>
    /// Optional material and color overrides for a single exterior wall side.
    /// When a value is null, the palette default is used.
    /// </summary>
    public sealed class WallAppearance
    {
        /// <summary>Optional wall color override. Null uses the palette default.</summary>
        public Color? Color { get; }

        /// <summary>Optional wall material override. Null uses the palette default.</summary>
        public Material? Material { get; }

        /// <summary>
        /// Create a wall appearance override.
        /// </summary>
        /// <param name="color">Optional color override (null = use palette)</param>
        /// <param name="material">Optional material override (null = use palette)</param>
        public WallAppearance(Color? color = null, Material? material = null)
        {
            Color = color;
            Material = material;
        }
    }
}
