using UnityEngine;
using MAPI.S1;

namespace MAPI.Building.Config
{
    /// <summary>
    /// Configuration for building dimensions and style.
    /// Use presets or create custom configs for different building types.
    /// </summary>
    public sealed class BuildingConfig
    {
        #region Properties

        /// <summary>Room width in meters (X axis)</summary>
        public float Width { get; set; } = 8f;

        /// <summary>Room height in meters (Y axis)</summary>
        public float Height { get; set; } = 3f;

        /// <summary>Room depth in meters (Z axis)</summary>
        public float Depth { get; set; } = 6f;

        /// <summary>Wall thickness in meters</summary>
        public float WallThickness { get; set; } = 0.2f;

        /// <summary>Floor thickness in meters</summary>
        public float FloorThickness { get; set; } = 0.1f;

        /// <summary>Ceiling thickness in meters</summary>
        public float CeilingThickness { get; set; } = 0.1f;

        /// <summary>Material and color palette</summary>
        public BuildingPalette Palette { get; set; } = new();

        #endregion

        #region Computed Properties

        /// <summary>Room dimensions as Vector3(Width, Height, Depth)</summary>
        public Vector3 Size => new(Width, Height, Depth);

        /// <summary>Room center point</summary>
        public Vector3 Center => new(Width / 2f, Height / 2f, Depth / 2f);

        #endregion

        #region Static Presets

        /// <summary>
        /// Default medium room (8m x 3m x 6m).
        /// </summary>
        public static BuildingConfig Default => new();

        /// <summary>
        /// Small room suitable for closets or bathrooms (3m x 2.4m x 3m).
        /// </summary>
        public static BuildingConfig Tiny => new()
        {
            Width = 3f,
            Height = 2.4f,
            Depth = 3f
        };

        /// <summary>
        /// Small room suitable for offices or bedrooms (5m x 3m x 4m).
        /// </summary>
        public static BuildingConfig Small => new()
        {
            Width = 5f,
            Height = 3f,
            Depth = 4f
        };

        /// <summary>
        /// Medium room for living spaces (8m x 3m x 6m).
        /// </summary>
        public static BuildingConfig Medium => new()
        {
            Width = 8f,
            Height = 3f,
            Depth = 6f
        };

        /// <summary>
        /// Large room for retail or open offices (12m x 4m x 10m).
        /// </summary>
        public static BuildingConfig Large => new()
        {
            Width = 12f,
            Height = 4f,
            Depth = 10f
        };

        /// <summary>
        /// Huge room for warehouses or showrooms (20m x 6m x 15m).
        /// </summary>
        public static BuildingConfig Huge => new()
        {
            Width = 20f,
            Height = 6f,
            Depth = 15f
        };

        /// <summary>
        /// Warehouse style (20m x 6m x 15m) with industrial palette.
        /// </summary>
        public static BuildingConfig Warehouse => new()
        {
            Width = 20f,
            Height = 6f,
            Depth = 15f,
            Palette = new BuildingPalette
            {
                FloorMaterial = Materials.ConcreteLightGrey,
                FloorColor = new Color(0.75f, 0.75f, 0.75f),
                WallMaterial = Materials.GraniteDullSalmonLighter,
                WallColor = new Color(0.94f, 0.94f, 0.94f),
                CeilingMaterial = Materials.ConcreteLightGrey,
                CeilingColor = new Color(0.94f, 0.94f, 0.94f),
                TrimMaterial = Materials.BrickWallRed,
                TrimColor = new Color(0.6f, 0.3f, 0.2f),
                PillarMaterial = Materials.BrickWallRed,
                PillarColor = new Color(0.6f, 0.3f, 0.2f),
                AccentColor = new Color(0.29f, 0.48f, 0.29f),
                LightColor = new Color(1f, 0.98f, 0.95f),
                LightIntensity = 1.2f
            }
        };

        #endregion

        #region Builder Pattern

        /// <summary>
        /// Create a copy of this config for modification.
        /// </summary>
        public BuildingConfig Clone() => new()
        {
            Width = Width,
            Height = Height,
            Depth = Depth,
            WallThickness = WallThickness,
            FloorThickness = FloorThickness,
            CeilingThickness = CeilingThickness,
            Palette = Palette.Clone()
        };

        /// <summary>
        /// Set dimensions and return this config for chaining.
        /// </summary>
        public BuildingConfig WithDimensions(float width, float height, float depth)
        {
            Width = width;
            Height = height;
            Depth = depth;
            return this;
        }

        /// <summary>
        /// Set palette and return this config for chaining.
        /// </summary>
        public BuildingConfig WithPalette(BuildingPalette palette)
        {
            Palette = palette;
            return this;
        }

        /// <summary>
        /// Set wall thickness and return this config for chaining.
        /// </summary>
        public BuildingConfig WithWallThickness(float thickness)
        {
            WallThickness = thickness;
            return this;
        }

        #endregion
    }
}
