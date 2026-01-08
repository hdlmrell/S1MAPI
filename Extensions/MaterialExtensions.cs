using UnityEngine;

namespace MAPI.Extensions
{
    /// <summary>
    /// Extension methods for Material operations.
    /// </summary>
    public static class MaterialExtensions
    {
        /// <summary>
        /// Set color with alpha for transparent materials.
        /// </summary>
        public static Material SetColorAlpha(this Material material, float alpha)
        {
            Color color = material.color;
            color.a = alpha;
            material.color = color;
            return material;
        }

        /// <summary>
        /// Enable a shader keyword.
        /// </summary>
        public static Material EnableKeyword(this Material material, string keyword)
        {
            material.EnableKeyword(keyword);
            return material;
        }

        /// <summary>
        /// Disable a shader keyword.
        /// </summary>
        public static Material DisableKeyword(this Material material, string keyword)
        {
            material.DisableKeyword(keyword);
            return material;
        }
    }
}
