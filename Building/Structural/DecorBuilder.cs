using S1MAPI.Building.Config;
using S1MAPI.ProceduralMesh;
using S1MAPI.Utils;
using UnityEngine;

namespace S1MAPI.Building.Structural
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

        private float _foundationClearanceX;
        private float _foundationClearanceZ;

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

            // Store clearance so AddStairs can auto-clear the foundation edge
            _foundationClearanceX = padding + expandX;
            _foundationClearanceZ = padding + expandZ;

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
        /// Add stairs from ground level up to the building floor on the specified wall.
        /// Supports multiple visual styles: Solid (concrete box steps), ClosedRiser (two-tone wood with risers),
        /// or OpenStringer (plank treads on diagonal stringer beams).
        /// </summary>
        /// <param name="wall">Which wall the stairs attach to</param>
        /// <param name="foundationHeight">Height of the foundation in meters</param>
        /// <param name="maxStepHeight">Maximum height per step. Lower values create more, shallower steps. (Solid only)</param>
        /// <param name="width">Step width in meters (Solid only)</param>
        /// <param name="stepDepth">Step depth (tread) in meters. Controls how far stairs extend outward. (Solid only)</param>
        /// <param name="color">Optional color override — defaults to floor color (Solid only)</param>
        /// <param name="material">Optional material override — defaults to floor material (Solid only)</param>
        /// <param name="style">Visual style of stairs to generate</param>
        /// <param name="flushWithFloor">If true, topmost step is flush with floor level. If false (default), topmost step is one step below floor. (Solid only)</param>
        /// <param name="gap">Vertical gap between foundation edge and top step. ClosedRiser/OpenStringer default to 0 (flush).</param>
        /// <returns>The stairs container GameObject</returns>
        public GameObject AddStairs(
            WallSide wall,
            float foundationHeight,
            float maxStepHeight = Constants.Spatial.DefaultMaxStepHeight,
            float width = 2.5f,
            float stepDepth = Constants.Spatial.DefaultStepDepth,
            Color? color = null,
            Material? material = null,
            StairStyle style = StairStyle.Solid,
            bool flushWithFloor = false,
            float gap = 0f)
        {
            return style switch
            {
                StairStyle.ClosedRiser => AddClosedRiserStairs(wall, foundationHeight, gap),
                StairStyle.OpenStringer => AddOpenStringerStairs(wall, foundationHeight, gap),
                _ => AddSolidStairs(wall, foundationHeight, maxStepHeight, width, stepDepth, color, material, flushWithFloor)
            };
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

        private GameObject AddSolidStairs(
            WallSide wall,
            float foundationHeight,
            float maxStepHeight,
            float width,
            float stepDepth,
            Color? color,
            Material? material,
            bool flushWithFloor = false)
        {
            GameObject container = BuildingUtilities.CreateFolder("Stairs", _parent);
            Color stepColor = color ?? _palette.FloorColor;
            int count = Mathf.Max(2, Mathf.CeilToInt(foundationHeight / maxStepHeight));
            float stepRise = foundationHeight / count;

            // Default: floor acts as the final step, so we generate count-1 visible steps.
            // Topmost step surface is at Y = -stepRise (one step below floor).
            // flushWithFloor: topmost step surface is at Y = 0 (flush with floor level).
            int visibleSteps = flushWithFloor ? count : count - 1;

            // Push steps outward past the foundation edge
            // Uses clearance set by AddFoundation (padding + expand), or 0.1m default
            float clearanceX = _foundationClearanceX > 0f ? _foundationClearanceX : 0.1f;
            float clearanceZ = _foundationClearanceZ > 0f ? _foundationClearanceZ : 0.1f;

            for (int i = 0; i < visibleSteps; i++)
            {
                float height = (i + 1) * stepRise;
                float yCenter = -foundationHeight + height / 2f;
                bool isNorthSouth = wall == WallSide.North || wall == WallSide.South;
                float clearance = isNorthSouth ? clearanceZ : clearanceX;
                float perpOffset = (visibleSteps - 1 - i) * stepDepth + stepDepth / 2f + clearance;

                Vector3 position;
                Vector3 size;

                switch (wall)
                {
                    case WallSide.North:
                        position = new Vector3(_roomSize.x / 2f, yCenter, _roomSize.z + perpOffset);
                        size = new Vector3(width, height, stepDepth);
                        break;
                    case WallSide.South:
                        position = new Vector3(_roomSize.x / 2f, yCenter, -perpOffset);
                        size = new Vector3(width, height, stepDepth);
                        break;
                    case WallSide.East:
                        position = new Vector3(_roomSize.x + perpOffset, yCenter, _roomSize.z / 2f);
                        size = new Vector3(stepDepth, height, width);
                        break;
                    case WallSide.West:
                        position = new Vector3(-perpOffset, yCenter, _roomSize.z / 2f);
                        size = new Vector3(stepDepth, height, width);
                        break;
                    default:
                        continue;
                }

                GameObject step = PrimitiveBuilder.CreateBox(
                    $"Step{i + 1}", position, size, stepColor, container.transform);

                Material? mat = material ?? _palette.FloorMaterial;
                if (mat != null)
                {
                    ApplyMaterial(step, mat);
                }
            }

            return container;
        }

        private GameObject AddClosedRiserStairs(WallSide wall, float foundationHeight, float gap)
        {
            GameObject container = BuildingUtilities.CreateFolder("Stairs_ClosedRiser", _parent);

            // Tread planks and riser faces use separate materials for two-tone look
            Material? treadMaterial = MaterialPresets.FindExistingMaterial(Constants.Materials.TreadWoodName);
            Material? riserMaterial = MaterialPresets.FindExistingMaterial(Constants.Materials.RiserWoodName);

            Color riserColor = new Color(0.85f, 0.80f, 0.72f);
            Color treadColor = new Color(0.45f, 0.35f, 0.25f);

            float width = 2.5f;
            float stepDepth = Constants.Spatial.DefaultStepDepth;
            float treadThickness = 0.05f;
            int count = Mathf.Max(2, Mathf.CeilToInt(foundationHeight / Constants.Spatial.DefaultMaxStepHeight));
            float stepRise = foundationHeight / count;
            // ClosedRiser: top step is flush with floor
            int visibleSteps = count;

            float clearanceX = _foundationClearanceX > 0f ? _foundationClearanceX : 0.1f;
            float clearanceZ = _foundationClearanceZ > 0f ? _foundationClearanceZ : 0.1f;

            for (int i = 0; i < visibleSteps; i++)
            {
                float height = (i + 1) * stepRise;
                float yCenter = -foundationHeight + height / 2f - gap;
                bool isNorthSouth = wall == WallSide.North || wall == WallSide.South;
                float clearance = isNorthSouth ? clearanceZ : clearanceX;
                float perpOffset = (visibleSteps - 1 - i) * stepDepth + stepDepth / 2f + clearance;

                // Tread sits ON TOP of riser (not embedded) to avoid z-fighting
                float treadY = -foundationHeight + height + treadThickness / 2f - gap;

                Vector3 riserPos, riserSize, treadPos, treadSize;

                // Tread overhang: slight lip past riser face
                float treadOverhangDepth = 0.04f;  // 4cm total depth overhang (2cm front + back)
                float treadOverhangWidth = 0.06f;  // 6cm total width overhang (3cm per side)

                switch (wall)
                {
                    case WallSide.North:
                        riserPos = new Vector3(_roomSize.x / 2f, yCenter, _roomSize.z + perpOffset);
                        riserSize = new Vector3(width, height, stepDepth);
                        treadPos = new Vector3(_roomSize.x / 2f, treadY, _roomSize.z + perpOffset);
                        treadSize = new Vector3(width + treadOverhangWidth, treadThickness, stepDepth + treadOverhangDepth);
                        break;
                    case WallSide.South:
                        riserPos = new Vector3(_roomSize.x / 2f, yCenter, -perpOffset);
                        riserSize = new Vector3(width, height, stepDepth);
                        treadPos = new Vector3(_roomSize.x / 2f, treadY, -perpOffset);
                        treadSize = new Vector3(width + treadOverhangWidth, treadThickness, stepDepth + treadOverhangDepth);
                        break;
                    case WallSide.East:
                        riserPos = new Vector3(_roomSize.x + perpOffset, yCenter, _roomSize.z / 2f);
                        riserSize = new Vector3(stepDepth, height, width);
                        treadPos = new Vector3(_roomSize.x + perpOffset, treadY, _roomSize.z / 2f);
                        treadSize = new Vector3(stepDepth + treadOverhangDepth, treadThickness, width + treadOverhangWidth);
                        break;
                    case WallSide.West:
                        riserPos = new Vector3(-perpOffset, yCenter, _roomSize.z / 2f);
                        riserSize = new Vector3(stepDepth, height, width);
                        treadPos = new Vector3(-perpOffset, treadY, _roomSize.z / 2f);
                        treadSize = new Vector3(stepDepth + treadOverhangDepth, treadThickness, width + treadOverhangWidth);
                        break;
                    default:
                        continue;
                }

                // Riser body (light brown wood)
                GameObject riser = PrimitiveBuilder.CreateBox(
                    $"Riser{i + 1}", riserPos, riserSize, riserColor, container.transform);
                if (riserMaterial != null) ApplyMaterial(riser, riserMaterial);

                // Tread plank on top (dark wood, slight overhang)
                GameObject tread = PrimitiveBuilder.CreateBox(
                    $"Tread{i + 1}", treadPos, treadSize, treadColor, container.transform);
                if (treadMaterial != null) ApplyMaterial(tread, treadMaterial);
            }

            DebugLog.Info($"[DecorBuilder] ClosedRiser stairs: {visibleSteps} steps, gap={gap:F2}, treadMat={treadMaterial?.name ?? "fallback"}, riserMat={riserMaterial?.name ?? "fallback"}");
            return container;
        }

        private GameObject AddOpenStringerStairs(WallSide wall, float foundationHeight, float gap)
        {
            GameObject container = BuildingUtilities.CreateFolder("Stairs_OpenStringer", _parent);

            // Separate materials for treads and stringer beams
            Material? strinTreadMaterial = MaterialPresets.FindExistingMaterial(Constants.Materials.StringerTreadWoodName);
            Color woodColor = new Color(0.55f, 0.45f, 0.35f);
            Material? strinBeamMaterial = MaterialPresets.FindExistingMaterial(Constants.Materials.StringerBeamWoodName);

            float width = 3.0f;
            float stepDepth = Constants.Spatial.DefaultStepDepth;
            float treadThickness = 0.07f;
            float beamWidth = 0.25f;   // Stringer beam cross-section width (4x4 post style)
            float beamHeight = 0.25f;  // Stringer beam cross-section height (4x4 post style)
            int count = Mathf.Max(2, Mathf.CeilToInt(foundationHeight / Constants.Spatial.DefaultMaxStepHeight));
            float stepRise = foundationHeight / count;
            // Top step sits one rise below floor level (the foundation/floor is the final surface)
            int visibleSteps = count - 1;

            float clearanceX = _foundationClearanceX > 0f ? _foundationClearanceX : 0.1f;
            float clearanceZ = _foundationClearanceZ > 0f ? _foundationClearanceZ : 0.1f;
            bool isNorthSouth = wall == WallSide.North || wall == WallSide.South;
            float clearance = isNorthSouth ? clearanceZ : clearanceX;

            // --- Treads (flat planks, no solid risers) ---
            float treadOverhang = 0.20f; // Treads extend past the stringer beams on each side
            for (int i = 0; i < visibleSteps; i++)
            {
                float stepTop = -foundationHeight + (i + 1) * stepRise - gap;
                float treadY = stepTop + treadThickness / 2f;
                float perpOffset = (visibleSteps - 1 - i) * stepDepth + stepDepth / 2f + clearance;

                Vector3 treadPos, treadSize;
                switch (wall)
                {
                    case WallSide.North:
                        treadPos = new Vector3(_roomSize.x / 2f, treadY, _roomSize.z + perpOffset);
                        treadSize = new Vector3(width + treadOverhang, treadThickness, stepDepth + 0.04f);
                        break;
                    case WallSide.South:
                        treadPos = new Vector3(_roomSize.x / 2f, treadY, -perpOffset);
                        treadSize = new Vector3(width + treadOverhang, treadThickness, stepDepth + 0.04f);
                        break;
                    case WallSide.East:
                        treadPos = new Vector3(_roomSize.x + perpOffset, treadY, _roomSize.z / 2f);
                        treadSize = new Vector3(stepDepth + 0.04f, treadThickness, width + treadOverhang);
                        break;
                    case WallSide.West:
                        treadPos = new Vector3(-perpOffset, treadY, _roomSize.z / 2f);
                        treadSize = new Vector3(stepDepth + 0.04f, treadThickness, width + treadOverhang);
                        break;
                    default:
                        continue;
                }

                GameObject tread = PrimitiveBuilder.CreateBox(
                    $"Tread{i + 1}", treadPos, treadSize, woodColor, container.transform);
                if (strinTreadMaterial != null) ApplyMaterial(tread, strinTreadMaterial);
            }

            // --- Stringer beams (diagonal supports on left and right sides) ---
            // Top end embeds into the foundation (at floor level), bottom end sinks into the ground.
            float bottomExtend = 0.30f;

            // Stringer runs from inside the foundation down to the ground
            float stringerBottomY = -foundationHeight - gap;
            // Push the top anchor well below floor level and behind the wall so the beam is fully hidden
            float stringerTopY = -gap + stepRise;               // One step above floor (hidden inside foundation)
            float topPerp = -beamHeight;                      // Behind the wall surface (into foundation)
            float bottomPerp = (visibleSteps - 1) * stepDepth + stepDepth / 2f + clearance;
            float stringerRun = bottomPerp - topPerp;
            float stringerRise = stringerTopY - stringerBottomY;

            // Stringer angle
            float angleRad = Mathf.Atan2(stringerRise, stringerRun);
            float angleDeg = angleRad * Mathf.Rad2Deg;

            float baseLength = Mathf.Sqrt(stringerRise * stringerRise + stringerRun * stringerRun);

            // The beam's actual endpoints along the slope:
            // - Top end: pull back 2 step rises from the anchor to hide inside foundation
            float topTrim = stepRise * 3.0f;
            // - Bottom end: extend into the ground
            float stringerLength = baseLength + bottomExtend - topTrim;

            // Compute the actual top and bottom endpoints of the trimmed/extended beam
            // Direction along slope (from top toward bottom): perp increases, Y decreases
            float dirPerp = Mathf.Cos(angleRad);
            float dirY = Mathf.Sin(angleRad);

            // Actual top end (trimmed): move topTrim along slope from the original top anchor
            float actualTopPerp = topPerp + topTrim * dirPerp;
            float actualTopY = stringerTopY - topTrim * dirY;
            // Actual bottom end (extended): move bottomExtend along slope past original bottom
            float actualBottomPerp = bottomPerp + bottomExtend * dirPerp;
            float actualBottomY = stringerBottomY - bottomExtend * dirY;

            // Midpoint is simply the average of the actual endpoints
            float midPerp = (actualTopPerp + actualBottomPerp) / 2f;
            float midY = (actualTopY + actualBottomY) / 2f;

            // Left/right offset: beams sit inside the tread width, treads overhang past them
            float sideOffset = width / 2f - beamWidth;

            for (int side = 0; side < 2; side++)
            {
                float signedOffset = (side == 0) ? -sideOffset : sideOffset;

                Vector3 beamPos;
                // Use Y as the long axis so the wood grain texture runs along the beam length
                Vector3 beamSize = new Vector3(beamWidth, stringerLength, beamHeight);
                Quaternion beamRot;

                switch (wall)
                {
                    case WallSide.North:
                        beamPos = new Vector3(_roomSize.x / 2f + signedOffset, midY, _roomSize.z + midPerp);
                        beamRot = Quaternion.Euler(angleDeg + 90f, 0f, 0f);
                        break;
                    case WallSide.South:
                        beamPos = new Vector3(_roomSize.x / 2f + signedOffset, midY, -midPerp);
                        beamRot = Quaternion.Euler(-angleDeg - 90f, 0f, 0f);
                        break;
                    case WallSide.East:
                        beamPos = new Vector3(_roomSize.x + midPerp, midY, _roomSize.z / 2f + signedOffset);
                        beamSize = new Vector3(beamHeight, stringerLength, beamWidth);
                        beamRot = Quaternion.Euler(0f, 0f, -angleDeg - 90f);
                        break;
                    case WallSide.West:
                        beamPos = new Vector3(-midPerp, midY, _roomSize.z / 2f + signedOffset);
                        beamSize = new Vector3(beamHeight, stringerLength, beamWidth);
                        beamRot = Quaternion.Euler(0f, 0f, angleDeg + 90f);
                        break;
                    default:
                        continue;
                }

                GameObject beam = PrimitiveBuilder.CreateBox(
                    $"Stringer{side + 1}", beamPos, beamSize, woodColor, container.transform);
                beam.transform.rotation = _parent.rotation * beamRot;
                if (strinBeamMaterial != null) ApplyMaterial(beam, strinBeamMaterial);
                else if (strinTreadMaterial != null) ApplyMaterial(beam, strinTreadMaterial);
            }

            DebugLog.Info($"[DecorBuilder] OpenStringer stairs: {visibleSteps} treads + 2 stringers, gap={gap:F2}, treadMat={strinTreadMaterial?.name ?? "fallback"}, beamMat={strinBeamMaterial?.name ?? "fallback"}");
            return container;
        }

        private static void ApplyMaterial(GameObject obj, Material material)
        {
            Renderer r = obj.GetComponent<Renderer>();
            if (r != null) r.material = material;
        }

        #endregion
    }
}
