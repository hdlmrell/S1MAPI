using System.Collections.Generic;
using S1MAPI.Building.Config;
using S1MAPI.Building.Structural;
using S1MAPI.Building.Interior;
using S1MAPI.Building.Components;
using UnityEngine;
using S1MAPI.Core;
using S1MAPI.S1;
using S1MAPI.Utils;

namespace S1MAPI.Building
{
    /// <summary>
    /// Fluent builder for constructing buildings with a clean, chainable API.
    /// Delegates to specialized builders (WallBuilder, FurnitureBuilder, etc.).
    /// </summary>
    /// <example>
    /// var building = new BuildingBuilder("MyShop")
    ///     .WithConfig(BuildingConfig.Dispensary)
    ///     .AddFloor()
    ///     .AddCeiling()
    ///     .AddWalls(southDoor: true, eastDoor: true, westWindow: true)
    ///     .AddLights()
    ///     .AddFurniture(FurnitureType.Counter, "north")
    ///     .Build();
    /// </example>
    public sealed class BuildingBuilder
    {
        #region Fields

        private readonly string _name;
        private readonly GameObject _root;
        private BuildingConfig _config;
        private Vector3 _roomSize;

        // Lazy-initialized builders
        private WallBuilder? _wallBuilder;
        private FurnitureBuilder? _furnitureBuilder;
        private LightingBuilder? _lightingBuilder;
        private DecorBuilder? _decorBuilder;
        private RoofBuilder? _roofBuilder;
        private PrefabPlacer? _prefabPlacer;
        private InteriorWallBuilder? _interiorWallBuilder;

        // Interior wall physics layer (-1 = default layer, no change)
        private int _interiorWallLayer = -1;

        // Stored wall openings for cross-builder communication (e.g., base molding gap)
        private WallOpening? _northOpening;
        private WallOpening? _southOpening;
        private WallOpening? _eastOpening;
        private WallOpening? _westOpening;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new building builder.
        /// </summary>
        /// <param name="name">Name for the building GameObject</param>
        public BuildingBuilder(string name)
        {
            _name = name;
            _root = new GameObject(name);
            _config = BuildingConfig.Default;
            _roomSize = _config.Size;
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Apply a building configuration preset.
        /// </summary>
        /// <param name="config">Building configuration</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder WithConfig(BuildingConfig config)
        {
            _config = config;
            _roomSize = config.Size;
            InvalidateBuilders();
            return this;
        }

        /// <summary>
        /// Apply a palette to the current config.
        /// </summary>
        /// <param name="palette">Building palette</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder WithPalette(BuildingPalette palette)
        {
            _config.Palette = palette;
            InvalidateBuilders();
            return this;
        }

        /// <summary>
        /// Define room dimensions directly.
        /// </summary>
        /// <param name="width">Room width (X axis) in meters</param>
        /// <param name="height">Room height (Y axis) in meters</param>
        /// <param name="depth">Room depth (Z axis) in meters</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder DefineRoom(float width, float height, float depth)
        {
            _config.Width = width;
            _config.Height = height;
            _config.Depth = depth;
            _roomSize = new Vector3(width, height, depth);
            InvalidateBuilders();
            return this;
        }

        #endregion

        #region Structure

        /// <summary>
        /// Add floor to the room.
        /// </summary>
        /// <param name="color">Optional color override</param>
        /// <param name="material">Optional material override</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddFloor(Color? color = null, Material? material = null)
        {
            var palette = color.HasValue || material != null 
                ? _config.Palette.Clone().WithFloor(material!) 
                : _config.Palette;
            
            if (color.HasValue)
            {
                palette.FloorColor = color.Value;
            }
            if (material != null)
            {
                palette.FloorMaterial = material;
            }

            GetDecorBuilder(palette).AddFloor(_config.FloorThickness);
            return this;
        }

        /// <summary>
        /// Add ceiling to the room.
        /// </summary>
        /// <param name="color">Optional color override</param>
        /// <param name="material">Optional material override</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddCeiling(Color? color = null, Material? material = null)
        {
            var palette = _config.Palette;
            if (color.HasValue || material != null)
            {
                palette = palette.Clone();
                if (color.HasValue) palette.CeilingColor = color.Value;
                if (material != null) palette.CeilingMaterial = material;
            }

            GetDecorBuilder(palette).AddCeiling(_config.CeilingThickness);
            return this;
        }



        /// <summary>
        /// Add walls with specified openings.
        /// </summary>
        /// <param name="northDoor">Add door on north wall</param>
        /// <param name="southDoor">Add door on south wall</param>
        /// <param name="eastDoor">Add door on east wall</param>
        /// <param name="westDoor">Add door on west wall</param>
        /// <param name="northWindow">Add window on north wall</param>
        /// <param name="southWindow">Add window on south wall</param>
        /// <param name="eastWindow">Add window on east wall</param>
        /// <param name="westWindow">Add window on west wall</param>
        /// <param name="northDoorWindows">Add windows alongside north door (requires northDoor)</param>
        /// <param name="southDoorWindows">Add windows alongside south door (requires southDoor)</param>
        /// <param name="eastDoorWindows">Add windows alongside east door (requires eastDoor)</param>
        /// <param name="westDoorWindows">Add windows alongside west door (requires westDoor)</param>
        /// <param name="color">Optional wall color override</param>
        /// <param name="material">Optional wall material override</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddWalls(
            bool northDoor = false, bool southDoor = false,
            bool eastDoor = false, bool westDoor = false,
            bool northWindow = false, bool southWindow = false,
            bool eastWindow = false, bool westWindow = false,
            bool northDoorWindows = false, bool southDoorWindows = false,
            bool eastDoorWindows = false, bool westDoorWindows = false,
            Color? color = null,
            Material? material = null)
        {
            var palette = _config.Palette;
            if (color.HasValue || material != null)
            {
                palette = palette.Clone();
                if (color.HasValue) palette.WallColor = color.Value;
                if (material != null) palette.WallMaterial = material;
            }

            _northOpening = northDoor
                ? (northDoorWindows ? WallOpening.DoorWithWindows() : WallOpening.Door())
                : (northWindow ? WallOpening.Window() : null);
            _southOpening = southDoor
                ? (southDoorWindows ? WallOpening.DoorWithWindows() : WallOpening.Door())
                : (southWindow ? WallOpening.Window() : null);
            _eastOpening = eastDoor
                ? (eastDoorWindows ? WallOpening.DoorWithWindows() : WallOpening.Door())
                : (eastWindow ? WallOpening.Window() : null);
            _westOpening = westDoor
                ? (westDoorWindows ? WallOpening.DoorWithWindows() : WallOpening.Door())
                : (westWindow ? WallOpening.Window() : null);

            var builder = GetWallBuilder(palette);
            builder.BuildWalls(
                northOpening: _northOpening,
                southOpening: _southOpening,
                eastOpening: _eastOpening,
                westOpening: _westOpening);

            return this;
        }

        /// <summary>
        /// Add walls with fine-grained control over openings.
        /// </summary>
        /// <param name="north">North wall opening configuration</param>
        /// <param name="south">South wall opening configuration</param>
        /// <param name="east">East wall opening configuration</param>
        /// <param name="west">West wall opening configuration</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddWalls(
            WallOpening? north = null,
            WallOpening? south = null,
            WallOpening? east = null,
            WallOpening? west = null)
        {
            _northOpening = north;
            _southOpening = south;
            _eastOpening = east;
            _westOpening = west;

            GetWallBuilder().BuildWalls(north, south, east, west);
            return this;
        }

        #endregion

        #region Interior Walls

        /// <summary>
        /// Set the physics layer for interior wall GameObjects.
        /// Use this to place interior walls on a layer outside the placement raycast mask
        /// so the ghost model passes through them while players still physically collide.
        /// </summary>
        /// <param name="layer">Unity layer index (0–31). -1 leaves walls on the default layer.</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder WithInteriorWallLayer(int layer)
        {
            _interiorWallLayer = layer;
            return this;
        }

        /// <summary>
        /// Add an interior wall spanning a sub-region of the room.
        /// </summary>
        /// <param name="axis">Axis the wall runs along (X or Z)</param>
        /// <param name="position">Position on the perpendicular axis (Z for X-axis walls, X for Z-axis walls)</param>
        /// <param name="from">Start coordinate along the wall's axis</param>
        /// <param name="to">End coordinate along the wall's axis</param>
        /// <param name="opening">Optional opening (door or window) centered in the wall</param>
        /// <param name="color">Optional wall color override (defaults to palette wall color)</param>
        /// <param name="material">Optional wall material override (defaults to palette wall material)</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddInteriorWall(
            InteriorWallAxis axis, float position, float from, float to,
            WallOpening? opening = null,
            Color? color = null, Material? material = null)
        {
            var def = new InteriorWallDefinition(axis, position, from, to, opening, color, material);
            GetInteriorWallBuilder().BuildInteriorWall(def);
            return this;
        }

        /// <summary>
        /// Add an interior wall spanning the full room width along the specified axis.
        /// </summary>
        /// <param name="axis">Axis the wall runs along (X or Z)</param>
        /// <param name="position">Position on the perpendicular axis (Z for X-axis walls, X for Z-axis walls)</param>
        /// <param name="opening">Optional opening (door or window) centered in the wall</param>
        /// <param name="color">Optional wall color override (defaults to palette wall color)</param>
        /// <param name="material">Optional wall material override (defaults to palette wall material)</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddInteriorWall(
            InteriorWallAxis axis, float position,
            WallOpening? opening = null,
            Color? color = null, Material? material = null)
        {
            float axisMax = axis == InteriorWallAxis.X ? _roomSize.x : _roomSize.z;
            return AddInteriorWall(axis, position, 0f, axisMax, opening, color, material);
        }

        /// <summary>
        /// Doorway positions recorded from all interior walls.
        /// Each entry provides center, dimensions, and orientation for future NavMesh link generation.
        /// </summary>
        public IReadOnlyList<DoorwayInfo> InteriorDoorways =>
            _interiorWallBuilder?.Doorways ?? (IReadOnlyList<DoorwayInfo>)System.Array.Empty<DoorwayInfo>();

        #endregion

        #region Decoration

        /// <summary>
        /// Add decorative trim around the roofline.
        /// Not needed when using <see cref="AddParapetRoof"/> or <see cref="AddHipRoof"/>,
        /// which include their own roof structure. Useful for custom roof designs with <see cref="AddCeiling"/>.
        /// </summary>
        /// <param name="height">Trim height in meters</param>
        /// <param name="material">Optional material override</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddRoofTrim(float height = 0.3f, Material? material = null)
        {
            GetDecorBuilder().AddRoofTrim(height, material);
            return this;
        }

        /// <summary>
        /// Add a secondary decorative trim above the roofline.
        /// Not needed when using <see cref="AddParapetRoof"/> or <see cref="AddHipRoof"/>,
        /// which include their own roof structure. Useful for custom roof designs with <see cref="AddCeiling"/>.
        /// </summary>
        /// <param name="height">Trim height in meters</param>
        /// <param name="material">Optional material override</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddSecondaryRoofTrim(float height = 0.15f, Material? material = null)
        {
            GetDecorBuilder().AddSecondaryRoofTrim(height, material);
            return this;
        }

        /// <summary>
        /// Add a parapet roof (raised wall and cap above the roofline).
        /// The cap extends past the parapet wall by the overhang amount, creating a ledge.
        /// Includes a thin roof slab at ceiling height. For interior ceilings, use
        /// <see cref="AddCeiling"/> separately — the ceiling sits just below the roof slab with no overlap.
        /// Use <see cref="ParapetPreset.Deep"/> for a prominent commercial look or
        /// <see cref="ParapetPreset.Shallow"/> for a subtler profile.
        /// </summary>
        /// <param name="preset">Sizing preset (Deep or Shallow). Overridden by explicit dimensions.</param>
        /// <param name="parapetHeight">Height of the parapet wall in meters. Null uses preset default.</param>
        /// <param name="parapetDepth">Depth of the parapet wall. Null uses wall thickness + padding.</param>
        /// <param name="capHeight">Height of the cap. Null uses preset default.</param>
        /// <param name="capOverhang">How far the cap extends past the parapet wall on each side. Null uses preset default.</param>
        /// <param name="parapetColor">Color override for the parapet wall.</param>
        /// <param name="parapetMaterial">Material override for the parapet wall.</param>
        /// <param name="capColor">Color override for the cap.</param>
        /// <param name="capMaterial">Material override for the cap.</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddParapetRoof(
            ParapetPreset preset = ParapetPreset.Deep,
            float? parapetHeight = null,
            float? parapetDepth = null,
            float? capHeight = null,
            float? capOverhang = null,
            Color? parapetColor = null,
            Material? parapetMaterial = null,
            Color? capColor = null,
            Material? capMaterial = null)
        {
            GetRoofBuilder().AddParapetRoof(preset, parapetHeight, parapetDepth,
                capHeight, capOverhang, parapetColor, parapetMaterial, capColor, capMaterial);
            return this;
        }

        /// <summary>
        /// Add a hip (four-slope) roof using custom mesh geometry.
        /// All four sides slope inward to a central ridge that is shorter than the building length.
        /// For square buildings, the ridge collapses to a point (pyramid roof).
        /// A base slab sits at ceiling height giving the roof visible thickness from below.
        /// For interior ceilings, use <see cref="AddCeiling"/> separately — the ceiling sits
        /// just below the roof slab with no overlap.
        /// </summary>
        /// <param name="ridgeHeight">Height of the ridge peak above the ceiling in meters.</param>
        /// <param name="overhang">How far the roof eaves extend past the walls in meters.</param>
        /// <param name="ridgeAlongX">If true, ridge runs along X axis. If false, along Z. Null auto-selects the longer axis.</param>
        /// <param name="roofColor">Color for the sloped roof planes.</param>
        /// <param name="roofMaterial">Material for the sloped roof planes. Null uses fallback color.</param>
        /// <param name="baseSlabHeight">Height of the 3D base slab beneath the slopes. 0 disables the slab.</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddHipRoof(
            float ridgeHeight = Constants.Roof.DefaultRidgeHeight,
            float overhang = Constants.Roof.DefaultOverhang,
            bool? ridgeAlongX = null,
            Color? roofColor = null,
            Material? roofMaterial = null,
            float baseSlabHeight = Constants.Roof.DefaultBaseSlabHeight)
        {
            GetRoofBuilder().AddHipRoof(ridgeHeight, overhang, ridgeAlongX,
                roofColor, roofMaterial, baseSlabHeight);
            return this;
        }

        /// <summary>
        /// Add structural pillars at corners.
        /// </summary>
        /// <param name="width">Pillar width in meters</param>
        /// <param name="material">Optional material override</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddCornerPillars(float width = 0.4f, Material? material = null)
        {
            GetDecorBuilder().AddCornerPillars(width, material);
            return this;
        }

        /// <summary>
        /// Add thin vertical trim strips at the four corners of the building.
        /// Each corner gets two perpendicular strips forming a right angle that seamlessly
        /// connects with horizontal trims (<see cref="AddRoofTrim"/>, <see cref="AddBaseMolding"/>).
        /// </summary>
        /// <param name="width">Visible width of each trim strip on the wall face in meters</param>
        /// <param name="depth">How far the trim protrudes past the wall surface in meters</param>
        /// <param name="material">Optional material override</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddCornerTrim(float width = 0.3f, float depth = 0.1f, Material? material = null)
        {
            GetDecorBuilder().AddCornerTrim(width, depth, material);
            return this;
        }

        /// <summary>
        /// Add foundation beneath the building.
        /// </summary>
        /// <param name="height">Foundation depth in meters</param>
        /// <param name="expandX">Extra expansion on X axis</param>
        /// <param name="expandZ">Extra expansion on Z axis</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddFoundation(float height = 2.0f, float expandX = 0f, float expandZ = 0f)
        {
            GetDecorBuilder().AddFoundation(height, expandX, expandZ);
            return this;
        }

        /// <summary>
        /// Add stairs from ground level up to the building floor on the specified wall.
        /// Automatically aligns with the door opening offset on the specified wall.
        /// Supports multiple visual styles: Solid (default concrete box steps), ClosedRiser (two-tone wood with risers),
        /// or OpenStringer (plank treads on diagonal stringer beams).
        /// </summary>
        /// <param name="wall">Which wall the stairs attach to</param>
        /// <param name="foundationHeight">Foundation height in meters (must match AddFoundation height)</param>
        /// <param name="maxStepHeight">Maximum height per step. Lower values create more, shallower steps. (Solid only)</param>
        /// <param name="width">Step width in meters (Solid only)</param>
        /// <param name="stepDepth">Step depth (tread) in meters. Controls how far stairs extend outward. (Solid only)</param>
        /// <param name="color">Optional color override — defaults to palette floor color (Solid only)</param>
        /// <param name="material">Optional material override — defaults to palette floor material (Solid only)</param>
        /// <param name="style">Visual style of stairs to generate</param>
        /// <param name="flushWithFloor">If true, topmost step is flush with floor level. If false (default), topmost step is one step below floor. (Solid only)</param>
        /// <param name="gap">Vertical gap between foundation edge and top step. ClosedRiser/OpenStringer default to 0 (flush).</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddStairs(
            WallSide wall,
            float foundationHeight = 2.0f,
            float maxStepHeight = Constants.Spatial.DefaultMaxStepHeight,
            float width = 2.5f,
            float stepDepth = Constants.Spatial.DefaultStepDepth,
            Color? color = null,
            Material? material = null,
            StairStyle style = StairStyle.Solid,
            bool flushWithFloor = false,
            float gap = 0f)
        {
            float lateralOffset = GetDoorOffset(wall);
            GetDecorBuilder().AddStairs(wall, foundationHeight, maxStepHeight, width, stepDepth, color, material, style, flushWithFloor, gap, lateralOffset);
            return this;
        }

        /// <summary>
        /// Add trim-style door frames around door openings.
        /// Must be called after AddWalls.
        /// </summary>
        /// <param name="material">Optional material override</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddDoorFrames(Material? material = null)
        {
            GetDecorBuilder().AddDoorFrames(
                _northOpening, _southOpening, _eastOpening, _westOpening, material);
            return this;
        }

        /// <summary>
        /// Add base molding around the bottom of the building.
        /// </summary>
        /// <param name="height">Molding height in meters</param>
        /// <param name="depth">Molding depth in meters</param>
        /// <param name="material">Optional material override</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddBaseMolding(float height = 0.3f, float depth = 0.1f, Material? material = null)
        {
            GetDecorBuilder().AddBaseMolding(height, depth, material,
                _northOpening, _southOpening, _eastOpening, _westOpening);
            return this;
        }

        #endregion

        #region Lighting

        /// <summary>
        /// Add ceiling lights distributed across the room.
        /// </summary>
        /// <param name="intensity">Light intensity</param>
        /// <param name="color">Light color</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddLights(float? intensity = null, Color? color = null)
        {
            GetLightingBuilder().AddCeilingLights(intensity, color);
            return this;
        }

        /// <summary>
        /// Add ambient fill lighting.
        /// </summary>
        /// <param name="intensity">Ambient light intensity</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddAmbientLighting(float intensity = 0.3f)
        {
            GetLightingBuilder().AddAmbientLighting(intensity);
            return this;
        }

        #endregion

        #region Furniture

        /// <summary>
        /// Add furniture at a semantic position.
        /// </summary>
        /// <param name="type">Type of furniture</param>
        /// <param name="position">Semantic position (center, north, south, east, west, northeast, etc.)</param>
        /// <param name="color">Optional color override</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddFurniture(FurnitureType type, string position, Color? color = null)
        {
            (Vector3 pos, Quaternion rot) = ParseSemanticPosition(position, GetOptimalMargin(type));
            GetFurnitureBuilder().Create(type, pos, rot, color);
            return this;
        }

        /// <summary>
        /// Add furniture at exact coordinates.
        /// </summary>
        /// <param name="type">Type of furniture</param>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Local rotation</param>
        /// <param name="color">Optional color override</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddFurniture(FurnitureType type, Vector3 position, Quaternion rotation, Color? color = null)
        {
            GetFurnitureBuilder().Create(type, position, rotation, color);
            return this;
        }

        #endregion

        #region Prefabs

        /// <summary>
        /// Place a game prefab at the specified position.
        /// </summary>
        /// <param name="prefab">Prefab reference from GamePrefabs</param>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Local rotation</param>
        /// <param name="onCreated">Optional callback invoked with the instantiated GameObject</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddPrefab(PrefabRef prefab, Vector3 position, Quaternion rotation, Action<GameObject>? onCreated = null)
        {
            GameObject? instance = GetPrefabPlacer().Place(prefab, position, rotation);
            if (instance != null)
            {
                onCreated?.Invoke(instance);
            }
            return this;
        }

        /// <summary>
        /// Add sliding double doors at a door opening.
        /// </summary>
        /// <param name="position">Local position for doors</param>
        /// <param name="rotation">Local rotation</param>
        /// <param name="openingHours">Text for opening hours sign</param>
        /// <param name="onCreated">Optional callback invoked with the instantiated door GameObject</param>
        /// <returns>This builder for chaining</returns>
        public BuildingBuilder AddSlidingDoors(Vector3 position, Quaternion rotation, string openingHours = "6AM-6PM", Action<GameObject>? onCreated = null)
        {
            GameObject? instance = GetPrefabPlacer().PlaceSlidingDoors(position, rotation, openingHours, Materials.MetalDarkGrey);
            if (instance != null)
            {
                onCreated?.Invoke(instance);
            }
            return this;
        }

        #endregion

        #region Build

        /// <summary>
        /// Finalize and return the building GameObject.
        /// </summary>
        /// <returns>The completed building</returns>
        public GameObject Build()
        {
            DebugLog.Info($"[BuildingBuilder] Built '{_name}' ({_roomSize.x}x{_roomSize.y}x{_roomSize.z}m)");
            return _root;
        }

        /// <summary>
        /// Finalize and return the building, then execute a post-build action.
        /// </summary>
        /// <param name="postBuild">Action to execute with the completed building</param>
        /// <returns>The completed building</returns>
        public GameObject Build(Action<GameObject> postBuild)
        {
            var building = Build();
            postBuild?.Invoke(building);
            return building;
        }

        /// <summary>
        /// Get the root GameObject before Build() is called.
        /// </summary>
        public GameObject Root => _root;

        /// <summary>
        /// Get the current room size.
        /// </summary>
        public Vector3 RoomSize => _roomSize;

        /// <summary>
        /// Get the current configuration.
        /// </summary>
        public BuildingConfig Config => _config;

        #endregion

        #region Private Methods - Builder Access

        private void InvalidateBuilders()
        {
            _wallBuilder = null;
            _furnitureBuilder = null;
            _lightingBuilder = null;
            _decorBuilder = null;
            _roofBuilder = null;
            _interiorWallBuilder = null;
            // PrefabPlacer doesn't depend on room size
        }

        private WallBuilder GetWallBuilder(BuildingPalette? palette = null)
        {
            return _wallBuilder ??= new WallBuilder(_root.transform, _roomSize, _config.WallThickness, palette ?? _config.Palette);
        }

        private FurnitureBuilder GetFurnitureBuilder()
        {
            if (_furnitureBuilder == null)
            {
                var container = BuildingUtilities.CreateFolder("Furniture", _root.transform);
                _furnitureBuilder = new FurnitureBuilder(container.transform, _config.Palette);
            }
            return _furnitureBuilder;
        }

        private LightingBuilder GetLightingBuilder()
        {
            return _lightingBuilder ??= new LightingBuilder(_root.transform, _roomSize, _config.Palette);
        }

        private DecorBuilder GetDecorBuilder(BuildingPalette? palette = null)
        {
            return _decorBuilder ??= new DecorBuilder(_root.transform, _roomSize, palette ?? _config.Palette);
        }

        private RoofBuilder GetRoofBuilder()
        {
            return _roofBuilder ??= new RoofBuilder(_root.transform, _roomSize, _config.WallThickness, _config.Palette);
        }

        private InteriorWallBuilder GetInteriorWallBuilder()
        {
            return _interiorWallBuilder ??= new InteriorWallBuilder(
                _root.transform, _roomSize, _config.WallThickness, _config.Palette, _interiorWallLayer);
        }

        private PrefabPlacer GetPrefabPlacer()
        {
            return _prefabPlacer ??= new PrefabPlacer(_root.transform);
        }

        private float GetDoorOffset(WallSide wall)
        {
            WallOpening? opening = wall switch
            {
                WallSide.North => _northOpening,
                WallSide.South => _southOpening,
                WallSide.East => _eastOpening,
                WallSide.West => _westOpening,
                _ => null
            };
            return opening?.Offset ?? 0f;
        }

        #endregion

        #region Private Methods - Positioning

        private float GetOptimalMargin(FurnitureType type)
        {
            float baseOffset = 0.15f; // Wall half-thickness + gap
            var footprint = FurnitureBuilder.GetFootprint(type);
            return Mathf.Max(footprint.x, footprint.z) / 2f + baseOffset;
        }

        private (Vector3 position, Quaternion rotation) ParseSemanticPosition(string position, float margin)
        {
            float x = _roomSize.x / 2f;
            float z = _roomSize.z / 2f;
            float cornerMargin = margin * 2.2f;

            return position.ToLowerInvariant() switch
            {
                "center" => (new Vector3(x, 0f, z), Quaternion.identity),
                "north" => (new Vector3(x, 0f, _roomSize.z - margin), Quaternion.Euler(0f, 180f, 0f)),
                "south" => (new Vector3(x, 0f, margin), Quaternion.identity),
                "east" => (new Vector3(_roomSize.x - margin, 0f, z), Quaternion.Euler(0f, -90f, 0f)),
                "west" => (new Vector3(margin, 0f, z), Quaternion.Euler(0f, 90f, 0f)),
                "northeast" => (new Vector3(_roomSize.x - cornerMargin, 0f, _roomSize.z - cornerMargin), Quaternion.Euler(0f, -135f, 0f)),
                "northwest" => (new Vector3(cornerMargin, 0f, _roomSize.z - cornerMargin), Quaternion.Euler(0f, 135f, 0f)),
                "southeast" => (new Vector3(_roomSize.x - cornerMargin, 0f, cornerMargin), Quaternion.Euler(0f, -45f, 0f)),
                "southwest" => (new Vector3(cornerMargin, 0f, cornerMargin), Quaternion.Euler(0f, 45f, 0f)),
                _ => (new Vector3(x, 0f, z), Quaternion.identity)
            };
        }

        #endregion
    }
}
