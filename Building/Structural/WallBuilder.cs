using S1MAPI.Building.Config;
using S1MAPI.ProceduralMesh;
using UnityEngine;
using S1MAPI.S1;

namespace S1MAPI.Building.Structural
{
    /// <summary>
    /// Specifies which side of the room a wall is on.
    /// </summary>
    public enum WallSide
    {
        /// <summary>The north wall (positive Z direction).</summary>
        North,
        /// <summary>The south wall (zero or negative Z direction).</summary>
        South,
        /// <summary>The east wall (positive X direction).</summary>
        East,
        /// <summary>The west wall (zero or negative X direction).</summary>
        West
    }

    /// <summary>
    /// Visual style for generated stairs.
    /// </summary>
    public enum StairStyle
    {
        /// <summary>Solid concrete box steps (default).</summary>
        Solid,
        /// <summary>Two-tone wood stairs with risers and tread planks (closed riser style).</summary>
        ClosedRiser,
        /// <summary>Open plank treads on diagonal stringer beams (open stringer style).</summary>
        OpenStringer
    }

    /// <summary>
    /// Type of opening in a wall.
    /// </summary>
    public enum WallOpeningType
    {
        /// <summary>No opening (solid wall).</summary>
        None,
        /// <summary>A door opening.</summary>
        Door,
        /// <summary>A window opening.</summary>
        Window
    }

    /// <summary>
    /// Configuration for a wall opening (door or window).
    /// </summary>
    public sealed class WallOpening
    {
        /// <summary>The type of opening (door, window, or none).</summary>
        public WallOpeningType Type { get; set; } = WallOpeningType.None;
        /// <summary>The width of the opening in meters.</summary>
        public float Width { get; set; } = 2.0f;
        /// <summary>The height of the opening in meters.</summary>
        public float Height { get; set; } = 2.2f;
        /// <summary>The bottom offset (sill height) in meters. Used for windows.</summary>
        public float BottomOffset { get; set; } = 0f;
        /// <summary>Optional window for the left side segment of a door wall.</summary>
        public WallOpening? LeftWindow { get; set; }
        /// <summary>Optional window for the right side segment of a door wall.</summary>
        public WallOpening? RightWindow { get; set; }

        /// <summary>
        /// Creates a door opening configuration.
        /// </summary>
        /// <param name="width">The door width in meters (default 2.0).</param>
        /// <param name="height">The door height in meters (default 2.2).</param>
        /// <returns>A new WallOpening configured as a door.</returns>
        public static WallOpening Door(float width = 2.0f, float height = 2.2f) => new()
        {
            Type = WallOpeningType.Door,
            Width = width,
            Height = height,
            BottomOffset = 0f
        };

        /// <summary>
        /// Creates a window opening configuration.
        /// </summary>
        /// <param name="width">The window width in meters (default 2.5).</param>
        /// <param name="height">The window height in meters (default 2.0).</param>
        /// <param name="sillHeight">The sill height from the floor in meters (default 0.8).</param>
        /// <returns>A new WallOpening configured as a window.</returns>
        public static WallOpening Window(float width = 2.5f, float height = 2.0f, float sillHeight = 0.8f) => new()
        {
            Type = WallOpeningType.Window,
            Width = width,
            Height = height,
            BottomOffset = sillHeight
        };

        /// <summary>
        /// Creates a door opening with windows on the left and/or right side segments.
        /// </summary>
        /// <param name="doorWidth">The door width in meters (default 2.0).</param>
        /// <param name="doorHeight">The door height in meters (default 2.2).</param>
        /// <param name="leftWindow">Window config for left side, or null for no window.</param>
        /// <param name="rightWindow">Window config for right side, or null for no window.</param>
        /// <returns>A new WallOpening configured as a door with side windows.</returns>
        public static WallOpening DoorWithWindows(
            float doorWidth = 2.0f, float doorHeight = 2.2f,
            WallOpening? leftWindow = null, WallOpening? rightWindow = null)
        {
            // When neither window is specified, use defaults on both sides
            if (leftWindow == null && rightWindow == null)
            {
                WallOpening defaultWindow = Window(width: 1.5f, height: 1.5f, sillHeight: 0.8f);
                leftWindow = defaultWindow;
                rightWindow = defaultWindow;
            }

            return new WallOpening
            {
                Type = WallOpeningType.Door,
                Width = doorWidth,
                Height = doorHeight,
                BottomOffset = 0f,
                LeftWindow = leftWindow,
                RightWindow = rightWindow
            };
        }
    }

    /// <summary>
    /// Builds walls with optional openings (doors, windows).
    /// Extracted from SemanticBuildingBuilder for SRP compliance.
    /// </summary>
    public sealed class WallBuilder
    {
        #region Fields

        private readonly Transform _parent;
        private readonly Vector3 _roomSize;
        private readonly float _wallThickness;
        private readonly BuildingPalette _palette;
        private GameObject? _wallsContainer;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new wall builder.
        /// </summary>
        /// <param name="parent">Parent transform for walls</param>
        /// <param name="roomSize">Room dimensions (width, height, depth)</param>
        /// <param name="wallThickness">Wall thickness in meters</param>
        /// <param name="palette">Material and color palette</param>
        public WallBuilder(Transform parent, Vector3 roomSize, float wallThickness, BuildingPalette palette)
        {
            _parent = parent;
            _roomSize = roomSize;
            _wallThickness = wallThickness;
            _palette = palette;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Build all four walls with specified openings.
        /// </summary>
        /// <param name="northOpening">Opening for north wall (or null for solid)</param>
        /// <param name="southOpening">Opening for south wall (or null for solid)</param>
        /// <param name="eastOpening">Opening for east wall (or null for solid)</param>
        /// <param name="westOpening">Opening for west wall (or null for solid)</param>
        /// <returns>The walls container GameObject</returns>
        public GameObject BuildWalls(
            WallOpening? northOpening = null,
            WallOpening? southOpening = null,
            WallOpening? eastOpening = null,
            WallOpening? westOpening = null)
        {
            _wallsContainer = BuildingUtilities.CreateFolder("Walls", _parent);

            // North wall (at Z = depth)
            BuildWall(WallSide.North, northOpening);

            // South wall (at Z = 0)
            BuildWall(WallSide.South, southOpening);

            // East wall (at X = width)
            BuildWall(WallSide.East, eastOpening);

            // West wall (at X = 0)
            BuildWall(WallSide.West, westOpening);

            return _wallsContainer;
        }

        /// <summary>
        /// Build a single wall with optional opening.
        /// </summary>
        /// <param name="side">Which side of the room</param>
        /// <param name="opening">Opening configuration (or null for solid wall)</param>
        /// <returns>The wall GameObject(s)</returns>
        public GameObject BuildWall(WallSide side, WallOpening? opening = null)
        {
            _wallsContainer ??= BuildingUtilities.CreateFolder("Walls", _parent);

            var (position, size, isVertical) = GetWallTransform(side);
            string wallName = $"{side}Wall";

            if (opening == null || opening.Type == WallOpeningType.None)
            {
                return CreateSolidWall(wallName, position, size);
            }

            return opening.Type switch
            {
                WallOpeningType.Door when opening.LeftWindow != null || opening.RightWindow != null
                    => CreateWallWithDoorAndWindows(wallName, position, size, opening, isVertical),
                WallOpeningType.Door => CreateWallWithDoor(wallName, position, size, opening, isVertical),
                WallOpeningType.Window => CreateWallWithWindow(wallName, position, size, opening, isVertical),
                _ => CreateSolidWall(wallName, position, size)
            };
        }

        #endregion

        #region Private Methods

        private (Vector3 position, Vector3 size, bool isVertical) GetWallTransform(WallSide side)
        {
            return side switch
            {
                WallSide.North => (
                    new Vector3(_roomSize.x / 2f, _roomSize.y / 2f, _roomSize.z),
                    new Vector3(_roomSize.x, _roomSize.y, _wallThickness),
                    false
                ),
                WallSide.South => (
                    new Vector3(_roomSize.x / 2f, _roomSize.y / 2f, 0f),
                    new Vector3(_roomSize.x, _roomSize.y, _wallThickness),
                    false
                ),
                WallSide.East => (
                    new Vector3(_roomSize.x, _roomSize.y / 2f, _roomSize.z / 2f),
                    new Vector3(_wallThickness, _roomSize.y, _roomSize.z),
                    true
                ),
                WallSide.West => (
                    new Vector3(0f, _roomSize.y / 2f, _roomSize.z / 2f),
                    new Vector3(_wallThickness, _roomSize.y, _roomSize.z),
                    true
                ),
                _ => throw new System.ArgumentException($"Unknown wall side: {side}")
            };
        }

        private GameObject CreateSolidWall(string name, Vector3 position, Vector3 size)
        {
            GameObject wall = PrimitiveBuilder.CreateBox(name, position, size, _palette.WallColor, _wallsContainer!.transform);
            ApplyWallMaterial(wall);
            return wall;
        }

        private GameObject CreateWallWithDoor(string name, Vector3 wallCenter, Vector3 wallSize, WallOpening opening, bool isVertical)
        {
            GameObject container = BuildingUtilities.CreateFolder(name, _wallsContainer!.transform);

            float wallWidth = isVertical ? wallSize.z : wallSize.x;
            float wallHeight = wallSize.y;
            float doorWidth = opening.Width;
            float doorHeight = opening.Height;
            float sideWallWidth = (wallWidth - doorWidth) / 2f;

            Vector3 leftOffset = isVertical ? Vector3.back * (doorWidth / 2f + sideWallWidth / 2f) : Vector3.left * (doorWidth / 2f + sideWallWidth / 2f);
            Vector3 rightOffset = isVertical ? Vector3.forward * (doorWidth / 2f + sideWallWidth / 2f) : Vector3.right * (doorWidth / 2f + sideWallWidth / 2f);

            // Left segment
            Vector3 leftSize = isVertical
                ? new Vector3(_wallThickness, wallHeight, sideWallWidth)
                : new Vector3(sideWallWidth, wallHeight, _wallThickness);
            GameObject left = PrimitiveBuilder.CreateBox($"{name}_Left", wallCenter + leftOffset, leftSize, _palette.WallColor, container.transform);
            ApplyWallMaterial(left);

            // Right segment
            Vector3 rightSize = leftSize;
            GameObject right = PrimitiveBuilder.CreateBox($"{name}_Right", wallCenter + rightOffset, rightSize, _palette.WallColor, container.transform);
            ApplyWallMaterial(right);

            // Top segment (wall above door)
            float topHeight = wallHeight - doorHeight;
            if (topHeight > 0f)
            {
                Vector3 topSize = isVertical
                    ? new Vector3(_wallThickness, topHeight, doorWidth)
                    : new Vector3(doorWidth, topHeight, _wallThickness);
                float topCenterY = wallHeight / 2f - topHeight / 2f;
                Vector3 topOffset = Vector3.up * topCenterY;
                GameObject top = PrimitiveBuilder.CreateBox($"{name}_Top", wallCenter + topOffset, topSize, _palette.WallColor, container.transform);
                ApplyWallMaterial(top);
            }

            return container;
        }

        private GameObject CreateWallWithDoorAndWindows(string name, Vector3 wallCenter, Vector3 wallSize, WallOpening opening, bool isVertical)
        {
            GameObject container = BuildingUtilities.CreateFolder(name, _wallsContainer!.transform);

            float wallWidth = isVertical ? wallSize.z : wallSize.x;
            float wallHeight = wallSize.y;
            float doorWidth = opening.Width;
            float doorHeight = opening.Height;
            float fullSideWidth = (wallWidth - doorWidth) / 2f;

            // Left side (may contain a window)
            BuildDoorSideSegment(name, "_Left", wallCenter, doorWidth, wallHeight,
                fullSideWidth, opening.LeftWindow, isVertical, true, container.transform);

            // Right side (may contain a window)
            BuildDoorSideSegment(name, "_Right", wallCenter, doorWidth, wallHeight,
                fullSideWidth, opening.RightWindow, isVertical, false, container.transform);

            // Top segment (wall above door) — same as CreateWallWithDoor
            float topHeight = wallHeight - doorHeight;
            if (topHeight > 0f)
            {
                Vector3 topSize = isVertical
                    ? new Vector3(_wallThickness, topHeight, doorWidth)
                    : new Vector3(doorWidth, topHeight, _wallThickness);
                float topCenterY = wallHeight / 2f - topHeight / 2f;
                Vector3 topOffset = Vector3.up * topCenterY;
                GameObject top = PrimitiveBuilder.CreateBox($"{name}_Top", wallCenter + topOffset, topSize, _palette.WallColor, container.transform);
                ApplyWallMaterial(top);
            }

            return container;
        }

        private void BuildDoorSideSegment(
            string wallName, string suffix, Vector3 wallCenter,
            float doorWidth, float wallHeight, float fullSideWidth,
            WallOpening? sideWindow, bool isVertical, bool isLeftSide, Transform parent)
        {
            float dirSign = isLeftSide ? -1f : 1f;

            if (sideWindow == null || fullSideWidth < 1.0f)
            {
                // No window — create single solid segment (same as CreateWallWithDoor)
                Vector3 offset = isVertical
                    ? new Vector3(0f, 0f, dirSign * (doorWidth / 2f + fullSideWidth / 2f))
                    : new Vector3(dirSign * (doorWidth / 2f + fullSideWidth / 2f), 0f, 0f);
                Vector3 size = isVertical
                    ? new Vector3(_wallThickness, wallHeight, fullSideWidth)
                    : new Vector3(fullSideWidth, wallHeight, _wallThickness);
                GameObject solid = PrimitiveBuilder.CreateBox($"{wallName}{suffix}", wallCenter + offset, size, _palette.WallColor, parent);
                ApplyWallMaterial(solid);
                return;
            }

            // With window: small strip adjacent to the door (for InsetDoorWallSegments), window section gets the rest
            float stripWidth = Mathf.Min(0.5f, fullSideWidth - sideWindow.Width - 0.4f);
            stripWidth = Mathf.Max(0.3f, stripWidth);
            float windowSectionWidth = fullSideWidth - stripWidth;

            // Solid strip next to the door (keeps {wallName}_Left / _Right name for InsetDoorWallSegments)
            float stripCenterOffset = doorWidth / 2f + stripWidth / 2f;
            Vector3 stripOffset = isVertical
                ? new Vector3(0f, 0f, dirSign * stripCenterOffset)
                : new Vector3(dirSign * stripCenterOffset, 0f, 0f);
            Vector3 stripSize = isVertical
                ? new Vector3(_wallThickness, wallHeight, stripWidth)
                : new Vector3(stripWidth, wallHeight, _wallThickness);
            GameObject strip = PrimitiveBuilder.CreateBox($"{wallName}{suffix}", wallCenter + stripOffset, stripSize, _palette.WallColor, parent);
            ApplyWallMaterial(strip);

            // Window section in the remaining area (no overlap with strip so InsetDoorWallSegments works)
            float winSectionCenterOffset = doorWidth / 2f + stripWidth + windowSectionWidth / 2f;
            Vector3 winSectionCenter = wallCenter + (isVertical
                ? new Vector3(0f, 0f, dirSign * winSectionCenterOffset)
                : new Vector3(dirSign * winSectionCenterOffset, 0f, 0f));

            // Shift window toward door by stripWidth/2 so it centers in the full side width
            float windowOffset = -dirSign * stripWidth / 2f;
            CreateWindowInSection($"{wallName}{suffix}Win", winSectionCenter, windowSectionWidth, wallHeight,
                sideWindow, isVertical, parent, windowOffset);
        }

        private GameObject CreateWallWithWindow(string name, Vector3 wallCenter, Vector3 wallSize, WallOpening opening, bool isVertical)
        {
            GameObject container = BuildingUtilities.CreateFolder(name, _wallsContainer!.transform);
            float wallWidth = isVertical ? wallSize.z : wallSize.x;
            CreateWindowInSection(name, wallCenter, wallWidth, wallSize.y, opening, isVertical, container.transform);
            return container;
        }

        private void CreateWindowInSection(
            string namePrefix, Vector3 sectionCenter,
            float sectionWidth, float sectionHeight,
            WallOpening window, bool isVertical, Transform parent,
            float windowOffset = 0f)
        {
            float windowWidth = Mathf.Min(window.Width, sectionWidth - 0.5f);
            float windowHeight = Mathf.Min(window.Height, sectionHeight - 1.2f);
            float windowBottom = window.BottomOffset;

            float topHeight = sectionHeight - (windowBottom + windowHeight);
            float sideWidth = (sectionWidth - windowWidth) / 2f;
            float halfHeight = sectionHeight / 2f;
            float windowCenterY = (windowBottom + windowHeight / 2f) - halfHeight;

            // Window center shifted along wall axis (positive = +Z for vertical, +X for horizontal)
            Vector3 winShift = isVertical
                ? new Vector3(0f, 0f, windowOffset)
                : new Vector3(windowOffset, 0f, 0f);
            Vector3 windowCenter = sectionCenter + winShift;

            // Bottom segment (sill) — full section width, no shift
            if (windowBottom > 0.01f)
            {
                Vector3 bottomSize = isVertical
                    ? new Vector3(_wallThickness, windowBottom, sectionWidth)
                    : new Vector3(sectionWidth, windowBottom, _wallThickness);
                Vector3 bottomOffset = Vector3.down * (halfHeight - windowBottom / 2f);
                GameObject bottom = PrimitiveBuilder.CreateBox($"{namePrefix}_Bottom", sectionCenter + bottomOffset, bottomSize, _palette.WallColor, parent);
                ApplyWallMaterial(bottom);
            }

            // Top segment (header) — full section width, no shift
            if (topHeight > 0.01f)
            {
                Vector3 topSize = isVertical
                    ? new Vector3(_wallThickness, topHeight, sectionWidth)
                    : new Vector3(sectionWidth, topHeight, _wallThickness);
                Vector3 topOffset = Vector3.up * (halfHeight - topHeight / 2f);
                GameObject top = PrimitiveBuilder.CreateBox($"{namePrefix}_Top", sectionCenter + topOffset, topSize, _palette.WallColor, parent);
                ApplyWallMaterial(top);
            }

            // Side segments — asymmetric widths when window is offset
            // For isVertical: +Z side = sideWidth - offset, -Z side = sideWidth + offset
            // For non-vertical: -X side = sideWidth + offset, +X side = sideWidth - offset
            float posSideWidth = sideWidth - windowOffset;
            float negSideWidth = sideWidth + windowOffset;
            float leftSideWidth = isVertical ? posSideWidth : negSideWidth;
            float rightSideWidth = isVertical ? negSideWidth : posSideWidth;

            if (leftSideWidth > 0.01f)
            {
                Vector3 leftSize = isVertical
                    ? new Vector3(_wallThickness, windowHeight, leftSideWidth)
                    : new Vector3(leftSideWidth, windowHeight, _wallThickness);
                Vector3 leftOffset = isVertical
                    ? new Vector3(0f, windowCenterY, windowWidth / 2f + leftSideWidth / 2f)
                    : new Vector3(-(windowWidth / 2f + leftSideWidth / 2f), windowCenterY, 0f);
                GameObject leftSide = PrimitiveBuilder.CreateBox($"{namePrefix}_Left", windowCenter + leftOffset, leftSize, _palette.WallColor, parent);
                ApplyWallMaterial(leftSide);
            }

            if (rightSideWidth > 0.01f)
            {
                Vector3 rightSize = isVertical
                    ? new Vector3(_wallThickness, windowHeight, rightSideWidth)
                    : new Vector3(rightSideWidth, windowHeight, _wallThickness);
                Vector3 rightOffset = isVertical
                    ? new Vector3(0f, windowCenterY, -(windowWidth / 2f + rightSideWidth / 2f))
                    : new Vector3(windowWidth / 2f + rightSideWidth / 2f, windowCenterY, 0f);
                GameObject rightSide = PrimitiveBuilder.CreateBox($"{namePrefix}_Right", windowCenter + rightOffset, rightSize, _palette.WallColor, parent);
                ApplyWallMaterial(rightSide);
            }

            // Window frame — at shifted center
            CreateWindowFrame(parent, windowCenter, windowWidth, windowHeight, windowCenterY, isVertical);

            // Glass pane — at shifted center
            Vector3 glassSize = isVertical
                ? new Vector3(_wallThickness * 0.2f, windowHeight - 0.1f, windowWidth - 0.1f)
                : new Vector3(windowWidth - 0.1f, windowHeight - 0.1f, _wallThickness * 0.2f);
            GameObject glass = PrimitiveBuilder.CreateBox($"{namePrefix}_WindowGlass",
                windowCenter + new Vector3(0f, windowCenterY, 0f), glassSize,
                new Color(0.7f, 0.9f, 1f), parent);

            Material glassMat = Materials.LaundromatGlass;
            if (glassMat != null)
            {
                Renderer r = glass.GetComponent<Renderer>();
                if (r != null) r.material = glassMat;
            }
        }

        private void CreateWindowFrame(Transform parent, Vector3 wallCenter, float windowWidth, float windowHeight, float windowCenterY, bool isVertical)
        {
            Color frameColor = new Color(0.1f, 0.1f, 0.1f);
            float frameDepth = 0.05f;
            float frameWidth = 0.1f;

            // Top frame
            Vector3 topFrameSize = isVertical
                ? new Vector3(_wallThickness + frameDepth, frameWidth, windowWidth)
                : new Vector3(windowWidth, frameWidth, _wallThickness + frameDepth);
            PrimitiveBuilder.CreateBox("FrameTop", wallCenter + new Vector3(0f, windowCenterY + windowHeight / 2f - frameWidth / 2f, 0f), topFrameSize, frameColor, parent);

            // Bottom frame
            PrimitiveBuilder.CreateBox("FrameBottom", wallCenter + new Vector3(0f, windowCenterY - windowHeight / 2f + frameWidth / 2f, 0f), topFrameSize, frameColor, parent);

            // Side frames
            Vector3 sideFrameSize = isVertical
                ? new Vector3(_wallThickness + frameDepth, windowHeight - 2 * frameWidth, frameWidth)
                : new Vector3(frameWidth, windowHeight - 2 * frameWidth, _wallThickness + frameDepth);

            float sideOffset = windowWidth / 2f - frameWidth / 2f;
            Vector3 leftFrameOffset = isVertical ? new Vector3(0f, windowCenterY, sideOffset) : new Vector3(-sideOffset, windowCenterY, 0f);
            Vector3 rightFrameOffset = isVertical ? new Vector3(0f, windowCenterY, -sideOffset) : new Vector3(sideOffset, windowCenterY, 0f);

            PrimitiveBuilder.CreateBox("FrameLeft", wallCenter + leftFrameOffset, sideFrameSize, frameColor, parent);
            PrimitiveBuilder.CreateBox("FrameRight", wallCenter + rightFrameOffset, sideFrameSize, frameColor, parent);
        }

        private void ApplyWallMaterial(GameObject wall)
        {
            if (_palette.WallMaterial != null)
            {
                Renderer r = wall.GetComponent<Renderer>();
                if (r != null) r.material = _palette.WallMaterial;
            }
        }

        #endregion
    }
}
