using MAPI.Building.Config;
using MAPI.ProceduralMesh;
using UnityEngine;

namespace MAPI.Building.Interior
{
    /// <summary>
    /// Creates decorative building elements like trim, pillars, and foundations.
    /// Extracted from SemanticBuildingBuilder for SRP compliance.
    /// </summary>
    public sealed class DecorBuilder
    {
        #region Fields

        private readonly Transform _parent;
        private readonly Vector3 _roomSize;
        private readonly BuildingPalette _palette;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new decor builder.
        /// </summary>
        /// <param name="parent">Parent transform for decor elements</param>
        /// <param name="roomSize">Room dimensions</param>
        /// <param name="palette">Material and color palette</param>
        public DecorBuilder(Transform parent, Vector3 roomSize, BuildingPalette palette)
        {
            _parent = parent;
            _roomSize = roomSize;
            _palette = palette;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Add decorative trim around the top of the building.
        /// </summary>
        /// <param name="height">Height of the trim in meters</param>
        /// <param name="material">Optional material override</param>
        /// <returns>The trim container GameObject</returns>
        public GameObject AddRoofTrim(float height = 0.3f, Material? material = null)
        {
            float wallThickness = 0.2f;
            float trimDepth = wallThickness + 0.1f;
            GameObject container = BuildingUtilities.CreateFolder("RoofTrim", _parent);

            Color color = _palette.TrimColor;
            float yPos = _roomSize.y - height / 2f;

            // North
            GameObject north = PrimitiveBuilder.CreateBox("NorthTrim",
                new Vector3(_roomSize.x / 2f, yPos, _roomSize.z),
                new Vector3(_roomSize.x + trimDepth, height, trimDepth),
                color, container.transform);

            // South
            GameObject south = PrimitiveBuilder.CreateBox("SouthTrim",
                new Vector3(_roomSize.x / 2f, yPos, 0f),
                new Vector3(_roomSize.x + trimDepth, height, trimDepth),
                color, container.transform);

            // East
            GameObject east = PrimitiveBuilder.CreateBox("EastTrim",
                new Vector3(_roomSize.x, yPos, _roomSize.z / 2f),
                new Vector3(trimDepth, height, _roomSize.z - trimDepth),
                color, container.transform);

            // West
            GameObject west = PrimitiveBuilder.CreateBox("WestTrim",
                new Vector3(0f, yPos, _roomSize.z / 2f),
                new Vector3(trimDepth, height, _roomSize.z - trimDepth),
                color, container.transform);

            // Apply material
            Material? mat = material ?? _palette.TrimMaterial;
            if (mat != null)
            {
                ApplyMaterial(north, mat);
                ApplyMaterial(south, mat);
                ApplyMaterial(east, mat);
                ApplyMaterial(west, mat);
            }

            return container;
        }

        /// <summary>
        /// Add a secondary trim above the main roof trim (parapet style).
        /// </summary>
        /// <param name="height">Height of the trim in meters</param>
        /// <param name="material">Optional material override</param>
        /// <returns>The trim container GameObject</returns>
        public GameObject AddSecondaryRoofTrim(float height = 0.15f, Material? material = null)
        {
            float wallThickness = 0.2f;
            float trimDepth = wallThickness + 0.1f;
            GameObject container = BuildingUtilities.CreateFolder("SecondaryRoofTrim", _parent);

            Color color = _palette.AccentColor;
            // Position above the roof line (AddRoofTrim ends at _roomSize.y)
            float yPos = _roomSize.y + height / 2f;

            // North
            GameObject north = PrimitiveBuilder.CreateBox("NorthTrim",
                new Vector3(_roomSize.x / 2f, yPos, _roomSize.z),
                new Vector3(_roomSize.x + trimDepth, height, trimDepth),
                color, container.transform);

            // South
            GameObject south = PrimitiveBuilder.CreateBox("SouthTrim",
                new Vector3(_roomSize.x / 2f, yPos, 0f),
                new Vector3(_roomSize.x + trimDepth, height, trimDepth),
                color, container.transform);

            // East
            GameObject east = PrimitiveBuilder.CreateBox("EastTrim",
                new Vector3(_roomSize.x, yPos, _roomSize.z / 2f),
                new Vector3(trimDepth, height, _roomSize.z - trimDepth),
                color, container.transform);

            // West
            GameObject west = PrimitiveBuilder.CreateBox("WestTrim",
                new Vector3(0f, yPos, _roomSize.z / 2f),
                new Vector3(trimDepth, height, _roomSize.z - trimDepth),
                color, container.transform);

            // Apply material
            Material? mat = material ?? _palette.AccentMaterial ?? _palette.TrimMaterial;
            if (mat != null)
            {
                ApplyMaterial(north, mat);
                ApplyMaterial(south, mat);
                ApplyMaterial(east, mat);
                ApplyMaterial(west, mat);
            }

            return container;
        }

        /// <summary>
        /// Add structural pillars at the four corners of the building.
        /// </summary>
        /// <param name="width">Pillar width in meters</param>
        /// <param name="material">Optional material override</param>
        /// <returns>The pillars container GameObject</returns>
        public GameObject AddCornerPillars(float width = 0.4f, Material? material = null)
        {
            GameObject container = BuildingUtilities.CreateFolder("CornerPillars", _parent);

            Color color = _palette.PillarColor;
            float height = _roomSize.y;
            float offset = width / 2f;

            // Northeast
            GameObject ne = PrimitiveBuilder.CreateBox("Pillar_NE",
                new Vector3(_roomSize.x + offset - 0.1f, height / 2f, _roomSize.z + offset - 0.1f),
                new Vector3(width, height, width),
                color, container.transform);

            // Northwest
            GameObject nw = PrimitiveBuilder.CreateBox("Pillar_NW",
                new Vector3(0f - offset + 0.1f, height / 2f, _roomSize.z + offset - 0.1f),
                new Vector3(width, height, width),
                color, container.transform);

            // Southeast
            GameObject se = PrimitiveBuilder.CreateBox("Pillar_SE",
                new Vector3(_roomSize.x + offset - 0.1f, height / 2f, 0f - offset + 0.1f),
                new Vector3(width, height, width),
                color, container.transform);

            // Southwest
            GameObject sw = PrimitiveBuilder.CreateBox("Pillar_SW",
                new Vector3(0f - offset + 0.1f, height / 2f, 0f - offset + 0.1f),
                new Vector3(width, height, width),
                color, container.transform);

            // Apply material
            Material? mat = material ?? _palette.PillarMaterial;
            if (mat != null)
            {
                ApplyMaterial(ne, mat);
                ApplyMaterial(nw, mat);
                ApplyMaterial(se, mat);
                ApplyMaterial(sw, mat);
            }

            return container;
        }

        /// <summary>
        /// Add a solid foundation block beneath the building.
        /// </summary>
        /// <param name="height">Depth of the foundation in meters</param>
        /// <param name="expandX">Extra width expansion beyond room bounds</param>
        /// <param name="expandZ">Extra depth expansion beyond room bounds</param>
        /// <param name="color">Optional color override</param>
        /// <param name="material">Optional material override</param>
        /// <returns>The foundation GameObject</returns>
        public GameObject AddFoundation(float height = 2.0f, float expandX = 0f, float expandZ = 0f, Color? color = null, Material? material = null)
        {
            Color foundationColor = color ?? new Color(0.4f, 0.4f, 0.4f);
            GameObject container = BuildingUtilities.CreateFolder("Foundation", _parent);

            float padding = 0.1f;
            float yOffset = -0.001f; // Avoid z-fighting with floor

            float width = _roomSize.x + padding * 2 + expandX * 2;
            float depth = _roomSize.z + padding * 2 + expandZ * 2;

            GameObject foundation = PrimitiveBuilder.CreateBox("FoundationBlock",
                new Vector3(_roomSize.x / 2f, -height / 2f + yOffset, _roomSize.z / 2f),
                new Vector3(width, height, depth),
                foundationColor,
                container.transform);

            if (material != null)
            {
                ApplyMaterial(foundation, material);
            }

            return container;
        }

        /// <summary>
        /// Add base molding around the bottom of the building.
        /// </summary>
        /// <param name="height">Molding height in meters</param>
        /// <param name="depth">Molding depth in meters</param>
        /// <param name="material">Optional material override</param>
        /// <returns>The molding container GameObject</returns>
        public GameObject AddBaseMolding(float height = 0.3f, float depth = 0.1f, Material? material = null)
        {
            float halfWidth = _roomSize.x / 2f;
            float halfDepth = _roomSize.z / 2f;
            GameObject container = BuildingUtilities.CreateFolder("BaseMolding", _parent);

            Color color = _palette.TrimColor;
            
            // Back (North)
            GameObject back = PrimitiveBuilder.CreateBox(
                "BaseMolding_North",
                new Vector3(halfWidth, height / 2f, _roomSize.z + depth / 2f),
                new Vector3(_roomSize.x + depth * 2f, height, depth),
                color,
                container.transform
            );

            // Left (West)
            GameObject left = PrimitiveBuilder.CreateBox(
                "BaseMolding_West",
                new Vector3(-depth / 2f, height / 2f, halfDepth),
                new Vector3(depth, height, _roomSize.z),
                color,
                container.transform
            );

            // Right (East)
            GameObject right = PrimitiveBuilder.CreateBox(
                "BaseMolding_East",
                new Vector3(_roomSize.x + depth / 2f, height / 2f, halfDepth),
                new Vector3(depth, height, _roomSize.z),
                color,
                container.transform
            );

            // Front (South)
            GameObject front = PrimitiveBuilder.CreateBox(
                "BaseMolding_South",
                new Vector3(halfWidth, height / 2f, -depth / 2f),
                new Vector3(_roomSize.x + depth * 2f, height, depth),
                color,
                container.transform
            );

            // Apply material
            Material? mat = material ?? _palette.TrimMaterial;
            if (mat != null)
            {
                ApplyMaterial(back, mat);
                ApplyMaterial(left, mat);
                ApplyMaterial(right, mat);
                ApplyMaterial(front, mat);
            }

            return container;
        }

        /// <summary>
        /// Add floor to the room.
        /// </summary>
        /// <param name="thickness">Floor thickness in meters</param>
        /// <returns>The floor GameObject</returns>
        public GameObject AddFloor(float thickness = 0.1f)
        {
            GameObject floor = PrimitiveBuilder.CreateBox("Floor",
                new Vector3(_roomSize.x / 2f, -thickness / 2f, _roomSize.z / 2f),
                new Vector3(_roomSize.x, thickness, _roomSize.z),
                _palette.FloorColor,
                _parent);

            if (_palette.FloorMaterial != null)
            {
                ApplyMaterial(floor, _palette.FloorMaterial);
            }

            return floor;
        }

        /// <summary>
        /// Add ceiling to the room.
        /// </summary>
        /// <param name="thickness">Ceiling thickness in meters</param>
        /// <returns>The ceiling GameObject</returns>
        public GameObject AddCeiling(float thickness = 0.1f)
        {
            GameObject ceiling = PrimitiveBuilder.CreateBox("Ceiling",
                new Vector3(_roomSize.x / 2f, _roomSize.y + thickness / 2f, _roomSize.z / 2f),
                new Vector3(_roomSize.x, thickness, _roomSize.z),
                _palette.CeilingColor,
                _parent);

            if (_palette.CeilingMaterial != null)
            {
                ApplyMaterial(ceiling, _palette.CeilingMaterial);
            }

            return ceiling;
        }

        #endregion

        #region Private Methods

        private static void ApplyMaterial(GameObject obj, Material material)
        {
            Renderer r = obj.GetComponent<Renderer>();
            if (r != null) r.material = material;
        }

        #endregion
    }
}
