using UnityEngine;
using MAPI.Core;
using MAPI.Utils;
using Object = UnityEngine.Object;

#if !MONO
using Il2CppFishNet;
using Il2CppFishNet.Managing;
using Il2CppFishNet.Managing.Object;
using Il2CppFishNet.Object;
#else
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Object;
using FishNet.Object;
#endif

namespace MAPI.Building
{
    /// <summary>
    /// High-level semantic API for LLM-friendly building construction
    /// Uses descriptive terms instead of precise coordinates
    /// </summary>
    public class SemanticBuildingBuilder
    {
        #region Fields
        
        private readonly GameObject _root;
        private Vector3 _roomSize;
        private Vector3 _roomCenter;
        private readonly string _buildingName;
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Create a new semantic building builder
        /// </summary>
        /// <param name="buildingName">Name for the building</param>
        public SemanticBuildingBuilder(string buildingName)
        {
            _buildingName = buildingName;
            _root = new GameObject(buildingName);
            _roomSize = new Vector3(8f, 3f, 6f); // Default room size
            _roomCenter = Vector3.zero;
        }
        
        #endregion

        #region Public API - Room Definition
        
        /// <summary>
        /// Define a room with semantic size descriptors
        /// </summary>
        /// <param name="sizeCategory">small, medium, large, huge</param>
        /// <param name="heightCategory">low, normal, tall, cathedral</param>
        /// <returns>This builder for chaining</returns>
        public SemanticBuildingBuilder DefineRoom(string sizeCategory, string heightCategory = "normal")
        {
            // Parse size category
            float width, depth;
            switch (sizeCategory.ToLower())
            {
                case "tiny":
                    width = 3f; depth = 3f;
                    break;
                case "small":
                    width = 5f; depth = 4f;
                    break;
                case "medium":
                    width = 8f; depth = 6f;
                    break;
                case "large":
                    width = 12f; depth = 10f;
                    break;
                case "huge":
                    width = 20f; depth = 15f;
                    break;
                default:
                    DebugLog.Warning($"Unknown size category: {sizeCategory}, using medium");
                    width = 8f; depth = 6f;
                    break;
            }

            // Parse height category
            float height;
            switch (heightCategory.ToLower())
            {
                case "low":
                    height = 2.4f;
                    break;
                case "normal":
                    height = 3f;
                    break;
                case "tall":
                    height = 4f;
                    break;
                case "cathedral":
                    height = 6f;
                    break;
                default:
                    height = 3f;
                    break;
            }

            _roomSize = new Vector3(width, height, depth);
            _roomCenter = new Vector3(width / 2f, height / 2f, depth / 2f);
            
            return this;
        }

        /// <summary>
        /// Define a room with exact dimensions (for precision when needed)
        /// </summary>
        public SemanticBuildingBuilder DefineRoom(float width, float height, float depth)
        {
            _roomSize = new Vector3(width, height, depth);
            _roomCenter = new Vector3(width / 2f, height / 2f, depth / 2f);
            return this;
        }
        
        #endregion

        #region Public API - Walls and Structure
        
        /// <summary>
        /// Add walls to the room with optional openings
        /// </summary>
        /// <param name="hasNorthDoor">Add door on north wall</param>
        /// <param name="hasSouthDoor">Add door on south wall</param>
        /// <param name="hasEastWindow">Add window on east wall</param>
        /// <param name="hasWestWindow">Add window on west wall</param>
        /// <param name="wallColor">Wall color (defaults to beige)</param>
        /// <param name="wallMaterial">Optional material override for walls</param>
        /// <returns>This builder for chaining</returns>
        public SemanticBuildingBuilder AddWalls(
            bool hasNorthDoor = false,
            bool hasSouthDoor = false,
            bool hasEastWindow = false,
            bool hasWestWindow = false,
            Color? wallColor = null,
            Material? wallMaterial = null)
        {
            Color color = wallColor ?? new Color(0.9f, 0.85f, 0.7f); // Beige
            float wallThickness = 0.2f;
            GameObject wallsContainer = BuildingUtilities.CreateFolder("Walls", _root.transform);

            // North wall (along X axis, at Z = depth)
            if (!hasNorthDoor)
            {
                CreateWall("NorthWall", wallsContainer.transform,
                    new Vector3(_roomSize.x / 2f, _roomSize.y / 2f, _roomSize.z),
                    new Vector3(_roomSize.x, _roomSize.y, wallThickness),
                    color, wallMaterial);
            }
            else
            {
                CreateWallWithDoor("NorthWall", wallsContainer.transform,
                    new Vector3(_roomSize.x / 2f, _roomSize.y / 2f, _roomSize.z),
                    _roomSize.x, _roomSize.y, wallThickness, color, Quaternion.identity, wallMaterial);
            }

            // South wall (along X axis, at Z = 0)
            if (!hasSouthDoor)
            {
                CreateWall("SouthWall", wallsContainer.transform,
                    new Vector3(_roomSize.x / 2f, _roomSize.y / 2f, 0f),
                    new Vector3(_roomSize.x, _roomSize.y, wallThickness),
                    color, wallMaterial);
            }
            else
            {
                CreateWallWithDoor("SouthWall", wallsContainer.transform,
                    new Vector3(_roomSize.x / 2f, _roomSize.y / 2f, 0f),
                    _roomSize.x, _roomSize.y, wallThickness, color, Quaternion.identity, wallMaterial);
            }

            // East wall (along Z axis, at X = width)
            if (!hasEastWindow)
            {
                CreateWall("EastWall", wallsContainer.transform,
                    new Vector3(_roomSize.x, _roomSize.y / 2f, _roomSize.z / 2f),
                    new Vector3(wallThickness, _roomSize.y, _roomSize.z),
                    color, wallMaterial);
            }
            else
            {
                CreateWallWithWindow("EastWall", wallsContainer.transform,
                    new Vector3(_roomSize.x, _roomSize.y / 2f, _roomSize.z / 2f),
                    wallThickness, _roomSize.y, _roomSize.z, color, wallMaterial);
            }

            // West wall (along Z axis, at X = 0)
            if (!hasWestWindow)
            {
                CreateWall("WestWall", wallsContainer.transform,
                    new Vector3(0f, _roomSize.y / 2f, _roomSize.z / 2f),
                    new Vector3(wallThickness, _roomSize.y, _roomSize.z),
                    color, wallMaterial);
            }
            else
            {
                CreateWallWithWindow("WestWall", wallsContainer.transform,
                    new Vector3(0f, _roomSize.y / 2f, _roomSize.z / 2f),
                    wallThickness, _roomSize.y, _roomSize.z, color, wallMaterial);
            }

            return this;
        }

        /// <summary>
        /// Add floor to the room
        /// </summary>
        /// <param name="floorColor">Floor color (defaults to light gray)</param>
        /// <param name="material">Optional material override</param>
        public SemanticBuildingBuilder AddFloor(Color? floorColor = null, Material? material = null)
        {
            Color color = floorColor ?? new Color(0.7f, 0.7f, 0.7f);
            float floorThickness = 0.1f;

            GameObject floor = PrimitiveBuilder.CreateBox("Floor",
                new Vector3(_roomSize.x / 2f, -floorThickness / 2f, _roomSize.z / 2f),
                new Vector3(_roomSize.x, floorThickness, _roomSize.z),
                color,
                _root.transform);

            if (material != null)
            {
                Renderer r = floor.GetComponent<Renderer>();
                if (r != null) r.material = material;
            }

            return this;
        }

        /// <summary>
        /// Add ceiling to the room
        /// </summary>
        /// <param name="ceilingColor">Ceiling color (defaults to white)</param>
        /// <param name="material">Optional material override</param>
        public SemanticBuildingBuilder AddCeiling(Color? ceilingColor = null, Material? material = null)
        {
            Color color = ceilingColor ?? Color.white;
            float ceilingThickness = 0.1f;

            GameObject ceiling = PrimitiveBuilder.CreateBox("Ceiling",
                new Vector3(_roomSize.x / 2f, _roomSize.y + ceilingThickness / 2f, _roomSize.z / 2f),
                new Vector3(_roomSize.x, ceilingThickness, _roomSize.z),
                color,
                _root.transform);

            if (material != null)
            {
                Renderer r = ceiling.GetComponent<Renderer>();
                if (r != null) r.material = material;
            }

            return this;
        }
        
        /// <summary>
        /// Add a solid foundation block beneath the building
        /// Useful for buildings placed on slopes or over water
        /// </summary>
        /// <param name="height">Depth of the foundation (default 2.0m)</param>
        /// <param name="expandX">Extra width expansion beyond room bounds (default 0)</param>
        /// <param name="expandZ">Extra depth expansion beyond room bounds (default 0)</param>
        /// <param name="foundationColor">Color of the foundation</param>
        /// <param name="foundationMaterial">Material for the foundation</param>
        /// <returns>This builder for chaining</returns>
        public SemanticBuildingBuilder AddFoundation(float height = 2.0f, float expandX = 0f, float expandZ = 0f, Color? foundationColor = null, Material? foundationMaterial = null)
        {
            Color color = foundationColor ?? new Color(0.4f, 0.4f, 0.4f); // Concrete gray
            GameObject foundationContainer = BuildingUtilities.CreateFolder("Foundation", _root.transform);

            // Extend foundation slightly beyond walls (like trim)
            float padding = 0.1f;
            
            // Create the main foundation block
            // Positioned downwards from y=0
            // Shift Y slightly down to avoid z-fighting with floors at Y=0
            float yOffset = -0.001f;
            
            // Calculate size with expansion
            float width = _roomSize.x + padding * 2 + expandX * 2;
            float depth = _roomSize.z + padding * 2 + expandZ * 2;
            
            GameObject foundation = PrimitiveBuilder.CreateBox("FoundationBlock",
                new Vector3(_roomSize.x / 2f, -height / 2f + yOffset, _roomSize.z / 2f),
                new Vector3(width, height, depth),
                color,
                foundationContainer.transform);

            if (foundationMaterial != null)
            {
                Renderer r = foundation.GetComponent<Renderer>();
                if (r != null) r.material = foundationMaterial;
            }

            return this;
        }
        
        /// <summary>
        /// Add a base game prefab double door to a wall opening
        /// </summary>
        public SemanticBuildingBuilder AddRealDoubleDoors(string openingHoursText = "6AM-6PM")
        {
            // Finds the last wall with door added and attaches the prefab
            // This requires tracking the last created door position
            // For now, we'll scan for the "SouthWall_Top" or "NorthWall_Top" to find the door gap
            
            Transform southTop = _root.transform.Find("Walls/SouthWall_Top");
            if (southTop != null)
            {
                // Door is below SouthWall_Top
                // SouthWall_Top position is at y = doorHeight + topHeight/2
                // Door center is at y = doorHeight/2
                
                // Hardcoded door dimensions from CreateWallWithDoor:
                float doorHeight = 2.2f;
                // SouthWall_Top pos x/z matches the wall center
                Vector3 doorPos = southTop.position;
                doorPos.y = doorHeight / 2f; // Center of door vertically
                // Z might need adjustment if wall thickness logic changes, but usually center aligns
                
                // Spawn the door
                SpawnRealDoubleDoor(doorPos, Quaternion.Euler(0f, 180f, 0f), openingHoursText);
                return this;
            }
            
            Transform northTop = _root.transform.Find("Walls/NorthWall_Top");
            if (northTop != null)
            {
                float doorHeight = 2.2f;
                Vector3 doorPos = northTop.position;
                doorPos.y = doorHeight / 2f;
                
                SpawnRealDoubleDoor(doorPos, Quaternion.identity, openingHoursText);
            }

            return this;
        }

        private void SpawnRealDoubleDoor(Vector3 position, Quaternion rotation, string openingHoursText)
        {
            // 1. Find the prefab
            GameObject doorPrefab = GetDealershipDoorPrefab();
            if (doorPrefab == null)
            {
                DebugLog.Error("[MAPI] Failed to find 'Dealership Sliding Doors' prefab");
                return;
            }

            // 2. Instantiate
            GameObject doorInstance = Object.Instantiate(doorPrefab);
            doorInstance.name = "RealDoubleDoors"; 

            // 3. Spawn on network (Server only)
            if (InstanceFinder.NetworkManager != null && InstanceFinder.NetworkManager.IsServer)
            {
                var netObj = doorInstance.GetComponent<NetworkObject>();
                if (netObj != null)
                {
                    InstanceFinder.NetworkManager.ServerManager.Spawn(netObj);
                }
            }

            // 4. Parent and Position
            doorInstance.transform.SetParent(_root.transform);
            // Adjust local position relative to root since we used global-ish logic above?
            // Actually _root.transform might be at 0,0,0 initially during build.
            // If the user moves _root later, the door moves with it.
            // If position passed in was global (from southTop.position), we need to convert to local if parenting.
            // BUT southTop.position is likely local if _root hasn't moved.
            // Safest to use local coordinates from the start.
            
            // Re-calculate local pos based on wall logic
            // South wall is at Z=0. Door center X=roomWidth/2. Y=1.1 (2.2/2)
            // But we need exact offset for the Dealership prefab pivot.
            // Prefab specific offset:
            Vector3 finalPos = position + new Vector3(0f, -1.158f, 0.0655f); // approximate adjustment based on previous values
            // Previous code used: (6f, -0.058f, 0.0655f) for a door at X=6.
            // -0.058 is very close to floor (0). 
            // My position.y was 1.1. So 1.1 - 1.158 = -0.058.
            
            doorInstance.transform.localPosition = finalPos;
            doorInstance.transform.localRotation = rotation;

            // 5. Apply Material (Metal Dark Grey)
            Material metalMat = GameAssets.Materials.MetalDarkGrey;
            if (metalMat != null)
            {
                ApplyMaterialToPath(doorInstance, "Door/Door", metalMat);
                ApplyMaterialToPath(doorInstance, "Door/Door (1)", metalMat);
            }

            // 6. Customize Opening Hours Text
            if (!string.IsNullOrEmpty(openingHoursText))
            {
                SetOpeningHoursText(doorInstance, openingHoursText);
            }
        }

        private GameObject GetDealershipDoorPrefab()
        {
            try
            {
                var networkManager = InstanceFinder.NetworkManager;
                if (networkManager == null) return null;

                var spawnablePrefabs = networkManager.GetPrefabObjects<PrefabObjects>(0, false);
                if (spawnablePrefabs == null) return null;

                int count = spawnablePrefabs.GetObjectCount();
                for (int i = 0; i < count; i++)
                {
                    NetworkObject obj = spawnablePrefabs.GetObject(true, i);
                    if (obj != null && obj.gameObject != null && obj.gameObject.name == "Dealership Sliding Doors")
                    {
                        return obj.gameObject;
                    }
                }
            }
            catch (System.Exception e)
            {
                DebugLog.Error($"[MAPI] Error getting door prefab: {e.Message}");
            }
            return null;
        }

        private void ApplyMaterialToPath(GameObject root, string path, Material mat)
        {
            Transform t = root.transform.Find(path);
            if (t != null)
            {
                Renderer r = t.GetComponent<Renderer>();
                if (r != null) r.material = mat;
            }
        }

        private void SetOpeningHoursText(GameObject doorInstance, string text)
        {
            Transform signTransform = doorInstance.transform.Find("Door/Door/OpeningHoursSign");
            if (signTransform != null)
            {
                TMPro.TextMeshPro tmp = signTransform.GetComponent<TMPro.TextMeshPro>();
                if (tmp != null) tmp.text = text;
            }
            else
            {
                TMPro.TextMeshPro[] tmps = doorInstance.GetComponentsInChildren<TMPro.TextMeshPro>(true);
                foreach (var t in tmps)
                {
                    if (t.name == "OpeningHoursSign" || t.transform.parent.name == "OpeningHoursSign")
                    {
                        t.text = text;
                        return;
                    }
                }
            }
        }

        #endregion

        #region Public API - Furniture Placement
        
        /// <summary>
        /// Add furniture using semantic positioning
        /// </summary>
        /// <param name="furnitureType">table, chair, bed, desk, bookshelf, etc.</param>
        /// <param name="position">center, north, south, east, west, northeast, northwest, southeast, southwest</param>
        /// <param name="color">Optional color override</param>
        /// <returns>This builder for chaining</returns>
        public SemanticBuildingBuilder AddFurniture(string furnitureType, string position, Color? color = null)
        {
            // Determine optimal margin based on furniture type
            float margin = GetOptimalMargin(furnitureType);
            (Vector3 placementPosition, Quaternion rotation) = ParseSemanticPosition(position, margin);
            
            Color furnitureColor = color ?? GetDefaultFurnitureColor(furnitureType);

            GameObject furnitureContainer = GetOrCreateContainer("Furniture");

            switch (furnitureType.ToLower())
            {
                case "table":
                    CreateTable(placementPosition, rotation, furnitureColor, furnitureContainer.transform);
                    break;
                case "chair":
                    CreateChair(placementPosition, rotation, furnitureColor, furnitureContainer.transform);
                    break;
                case "desk":
                    CreateDesk(placementPosition, rotation, furnitureColor, furnitureContainer.transform);
                    break;
                case "bed":
                    CreateBed(placementPosition, rotation, furnitureColor, furnitureContainer.transform);
                    break;
                case "bookshelf":
                    CreateBookshelf(placementPosition, rotation, furnitureColor, furnitureContainer.transform);
                    break;
                case "counter":
                    CreateCounter(placementPosition, rotation, furnitureColor, furnitureContainer.transform);
                    break;
                default:
                    DebugLog.Warning($"Unknown furniture type: {furnitureType}");
                    break;
            }

            return this;
        }

        /// <summary>
        /// Add a furniture group (e.g., "dining set" = table + 4 chairs)
        /// </summary>
        /// <param name="groupType">dining_set, bedroom_set, office_set, living_room_set</param>
        /// <param name="position">Semantic position for the group center</param>
        /// <returns>This builder for chaining</returns>
        public SemanticBuildingBuilder AddFurnitureGroup(string groupType, string position)
        {
            // Groups need more space, use larger default margin
            (Vector3 centerPos, Quaternion rotation) = ParseSemanticPosition(position, 2.0f);

            switch (groupType.ToLower())
            {
                case "dining_set":
                    CreateDiningSet(centerPos, rotation);
                    break;
                case "bedroom_set":
                    CreateBedroomSet(centerPos, rotation);
                    break;
                case "office_set":
                    CreateOfficeSet(centerPos, rotation);
                    break;
                case "living_room_set":
                    CreateLivingRoomSet(centerPos, rotation);
                    break;
                default:
                    DebugLog.Warning($"Unknown furniture group: {groupType}");
                    break;
            }

            return this;
        }
        
        /// <summary>
        /// Add ceiling lights to the room
        /// </summary>
        /// <param name="intensity">Light intensity (default 1.0)</param>
        /// <param name="color">Light color (default warm white)</param>
        /// <returns>This builder for chaining</returns>
        public SemanticBuildingBuilder AddLights(float intensity = 1.0f, Color? color = null)
        {
            Color lightColor = color ?? new Color(1f, 0.95f, 0.8f); // Warm white
            GameObject lightsContainer = BuildingUtilities.CreateFolder("Lights", _root.transform);
            
            // Calculate grid based on room size
            // Place a light every ~4 meters
            int xCount = Mathf.Max(1, Mathf.RoundToInt(_roomSize.x / 4f));
            int zCount = Mathf.Max(1, Mathf.RoundToInt(_roomSize.z / 4f));
            
            float xStep = _roomSize.x / (xCount + 1);
            float zStep = _roomSize.z / (zCount + 1);
            float yPos = _roomSize.y - 0.2f; // Just below ceiling

            for (int x = 1; x <= xCount; x++)
            {
                for (int z = 1; z <= zCount; z++)
                {
                    Vector3 pos = new Vector3(x * xStep, yPos, z * zStep);
                    
                    // Create the light source
                    PrimitiveBuilder.CreatePointLight(
                        $"CeilingLight_{x}_{z}",
                        pos,
                        lightColor,
                        range: 8f,
                        intensity: intensity,
                        parent: lightsContainer.transform);

                    // Create a visual fixture (small emissive cylinder)
                    PrimitiveBuilder.CreateCylinder(
                        $"LightFixture_{x}_{z}",
                        pos + Vector3.up * 0.1f, // Embedded in ceiling
                        new Vector3(0.3f, 0.1f, 0.3f),
                        lightColor, // This will be opaque, technically should be emissive but this works for visual
                        lightsContainer.transform);
                }
            }

            return this;
        }

        /// <summary>
        /// Add a decorative trim around the top of the building
        /// </summary>
        /// <param name="trimHeight">Height of the trim (default 0.3m)</param>
        /// <param name="trimMaterial">Material for the trim</param>
        /// <returns>This builder for chaining</returns>
        public SemanticBuildingBuilder AddRoofTrim(float trimHeight = 0.3f, Material? trimMaterial = null)
        {
            float wallThickness = 0.2f;
            float trimDepth = wallThickness + 0.1f; // Slightly thicker than walls to stick out
            GameObject trimContainer = BuildingUtilities.CreateFolder("RoofTrim", _root.transform);
            
            // Trim color fallback
            Color color = new Color(0.6f, 0.3f, 0.2f); 

            float yPos = _roomSize.y - trimHeight / 2f;

            // North Trim
            GameObject north = PrimitiveBuilder.CreateBox("NorthTrim",
                new Vector3(_roomSize.x / 2f, yPos, _roomSize.z),
                new Vector3(_roomSize.x + trimDepth, trimHeight, trimDepth), // Extend slightly past corners
                color, trimContainer.transform);

            // South Trim
            GameObject south = PrimitiveBuilder.CreateBox("SouthTrim",
                new Vector3(_roomSize.x / 2f, yPos, 0f),
                new Vector3(_roomSize.x + trimDepth, trimHeight, trimDepth),
                color, trimContainer.transform);

            // East Trim
            GameObject east = PrimitiveBuilder.CreateBox("EastTrim",
                new Vector3(_roomSize.x, yPos, _roomSize.z / 2f),
                new Vector3(trimDepth, trimHeight, _roomSize.z - trimDepth), // Shorten slightly to avoid z-fighting at corners if overlap
                color, trimContainer.transform);

            // West Trim
            GameObject west = PrimitiveBuilder.CreateBox("WestTrim",
                new Vector3(0f, yPos, _roomSize.z / 2f),
                new Vector3(trimDepth, trimHeight, _roomSize.z - trimDepth),
                color, trimContainer.transform);

            // Apply Material
            if (trimMaterial != null)
            {
                if (north.GetComponent<Renderer>()) north.GetComponent<Renderer>().material = trimMaterial;
                if (south.GetComponent<Renderer>()) south.GetComponent<Renderer>().material = trimMaterial;
                if (east.GetComponent<Renderer>()) east.GetComponent<Renderer>().material = trimMaterial;
                if (west.GetComponent<Renderer>()) west.GetComponent<Renderer>().material = trimMaterial;
            }

            return this;
        }

        /// <summary>
        /// Add structural pillars to the 4 corners of the building
        /// </summary>
        /// <param name="width">Width of the pillar (default 0.4m)</param>
        /// <param name="pillarMaterial">Material for the pillars</param>
        /// <returns>This builder for chaining</returns>
        public SemanticBuildingBuilder AddCornerPillars(float width = 0.4f, Material? pillarMaterial = null)
        {
            GameObject pillarContainer = BuildingUtilities.CreateFolder("CornerPillars", _root.transform);
            
            // Pillar color fallback
            Color color = new Color(0.6f, 0.3f, 0.2f); 

            float height = _roomSize.y; // Full wall height (plus a bit if needed)
            float offset = width / 2f;  // Center offset to align with corner

            // Northeast
            GameObject ne = PrimitiveBuilder.CreateBox("Pillar_NE",
                new Vector3(_roomSize.x + offset - 0.1f, height / 2f, _roomSize.z + offset - 0.1f), // Slight overlap 0.1f
                new Vector3(width, height, width),
                color, pillarContainer.transform);

            // Northwest
            GameObject nw = PrimitiveBuilder.CreateBox("Pillar_NW",
                new Vector3(0f - offset + 0.1f, height / 2f, _roomSize.z + offset - 0.1f),
                new Vector3(width, height, width),
                color, pillarContainer.transform);

            // Southeast
            GameObject se = PrimitiveBuilder.CreateBox("Pillar_SE",
                new Vector3(_roomSize.x + offset - 0.1f, height / 2f, 0f - offset + 0.1f),
                new Vector3(width, height, width),
                color, pillarContainer.transform);

            // Southwest
            GameObject sw = PrimitiveBuilder.CreateBox("Pillar_SW",
                new Vector3(0f - offset + 0.1f, height / 2f, 0f - offset + 0.1f),
                new Vector3(width, height, width),
                color, pillarContainer.transform);

            // Apply Material
            if (pillarMaterial != null)
            {
                if (ne.GetComponent<Renderer>()) ne.GetComponent<Renderer>().material = pillarMaterial;
                if (nw.GetComponent<Renderer>()) nw.GetComponent<Renderer>().material = pillarMaterial;
                if (se.GetComponent<Renderer>()) se.GetComponent<Renderer>().material = pillarMaterial;
                if (sw.GetComponent<Renderer>()) sw.GetComponent<Renderer>().material = pillarMaterial;
            }

            return this;
        }

        #endregion

        #region Public API - Build
        
        /// <summary>
        /// Finalize and return the building GameObject
        /// </summary>
        /// <returns>The completed building GameObject</returns>
        public GameObject Build()
        {
            ResourceTracker.Register(_root);
            DebugLog.Info($"Built semantic building: {_buildingName}");
            return _root;
        }
        
        #endregion

        #region Private Helper Methods - Positioning
        
        private float GetOptimalMargin(string furnitureType)
        {
            // Returns the distance from wall center to furniture center
            // Wall thickness is usually 0.2f (inner face at bound - 0.1f)
            // We want a small gap (e.g. 0.05f) between furniture back and wall face
            float baseOffset = 0.1f + 0.05f; // Wall half-thickness + gap

            switch (furnitureType.ToLower())
            {
                case "bookshelf":
                    return 0.25f + baseOffset; // Increased from 0.15 to 0.25 to prevent wall clipping
                case "counter":
                    return 0.3f + baseOffset;  // Depth 0.6 -> Half 0.3
                case "desk":
                    return 0.35f + baseOffset; // Depth 0.7 -> Half 0.35
                case "bed":
                    return 1.0f + baseOffset;  // Depth 2.0 -> Half 1.0
                case "chair":
                    return 0.3f + baseOffset;  // Depth ~0.6 -> Half 0.3
                case "table":
                    return 0.4f + baseOffset;  // Depth 0.8 -> Half 0.4
                default:
                    return 1.0f; // Default generic margin
            }
        }
        
        private (Vector3 position, Quaternion rotation) ParseSemanticPosition(string position, float margin = -1f)
        {
            float x = _roomSize.x / 2f;
            float z = _roomSize.z / 2f;
            
            // Use provided margin or default to 1.5f if not specified
            float usedMargin = margin > 0f ? margin : 1.5f; 
            
            // Increased corner margin multiplier from 1.5 to 2.2
            // Diagonal objects need significantly more clearance to avoid corner clipping
            float cornerMargin = usedMargin * 2.2f; 

            Vector3 pos;
            Quaternion rot = Quaternion.identity;

            switch (position.ToLower())
            {
                case "center":
                    pos = new Vector3(x, 0f, z);
                    rot = Quaternion.identity;
                    break;
                case "north":
                    pos = new Vector3(x, 0f, _roomSize.z - usedMargin);
                    rot = Quaternion.Euler(0f, 180f, 0f); // Face south
                    break;
                case "south":
                    pos = new Vector3(x, 0f, usedMargin);
                    rot = Quaternion.identity; // Face north
                    break;
                case "east":
                    pos = new Vector3(_roomSize.x - usedMargin, 0f, z);
                    rot = Quaternion.Euler(0f, -90f, 0f); // Face west
                    break;
                case "west":
                    pos = new Vector3(usedMargin, 0f, z);
                    rot = Quaternion.Euler(0f, 90f, 0f); // Face east
                    break;
                case "northeast":
                    pos = new Vector3(_roomSize.x - cornerMargin, 0f, _roomSize.z - cornerMargin);
                    rot = Quaternion.Euler(0f, -135f, 0f); // Face toward center
                    break;
                case "northwest":
                    pos = new Vector3(cornerMargin, 0f, _roomSize.z - cornerMargin);
                    rot = Quaternion.Euler(0f, 135f, 0f); // Face toward center
                    break;
                case "southeast":
                    pos = new Vector3(_roomSize.x - cornerMargin, 0f, cornerMargin);
                    rot = Quaternion.Euler(0f, -45f, 0f); // Face toward center
                    break;
                case "southwest":
                    pos = new Vector3(cornerMargin, 0f, cornerMargin);
                    rot = Quaternion.Euler(0f, 45f, 0f); // Face toward center
                    break;
                default:
                    DebugLog.Warning($"Unknown position: {position}, using center");
                    pos = new Vector3(x, 0f, z);
                    rot = Quaternion.identity;
                    break;
            }

            return (pos, rot);
        }

        private Color GetDefaultFurnitureColor(string furnitureType)
        {
            switch (furnitureType.ToLower())
            {
                case "table":
                case "desk":
                case "chair":
                case "bookshelf":
                    return new Color(0.55f, 0.35f, 0.2f); // Lighter brown wood
                case "bed":
                    return new Color(0.9f, 0.9f, 0.95f); // White/cream
                case "counter":
                    return new Color(0.65f, 0.45f, 0.25f); // Wood-toned counter
                default:
                    return new Color(0.5f, 0.5f, 0.5f); // Gray
            }
        }

        private GameObject GetOrCreateContainer(string name)
        {
            Transform existing = _root.transform.Find(name);
            if (existing != null) return existing.gameObject;
            return BuildingUtilities.CreateFolder(name, _root.transform);
        }
        
        #endregion

        #region Private Helper Methods - Wall Creation
        
        private void CreateWall(string name, Transform parent, Vector3 position, Vector3 size, Color color, Material? material = null)
        {
            GameObject wall = PrimitiveBuilder.CreateBox(name, position, size, color, parent);
            if (material != null)
            {
                Renderer r = wall.GetComponent<Renderer>();
                if (r != null) r.material = material;
            }
        }

        private void CreateWallWithDoor(string name, Transform parent, Vector3 wallCenter, 
            float wallWidth, float wallHeight, float wallThickness, Color wallColor, Quaternion rotation, Material? material = null)
        {
            float doorWidth = 2.0f;
            float doorHeight = 2.2f;
            float sideWallWidth = (wallWidth - doorWidth) / 2f;

            // Left wall segment
            CreateWall($"{name}_Left", parent,
                wallCenter + Vector3.left * (doorWidth / 2f + sideWallWidth / 2f),
                new Vector3(sideWallWidth, wallHeight, wallThickness),
                wallColor, material);

            // Right wall segment
            CreateWall($"{name}_Right", parent,
                wallCenter + Vector3.right * (doorWidth / 2f + sideWallWidth / 2f),
                new Vector3(sideWallWidth, wallHeight, wallThickness),
                wallColor, material);

            // Top wall segment (above door)
            float topWallHeight = wallHeight - doorHeight;
            CreateWall($"{name}_Top", parent,
                wallCenter + Vector3.up * (doorHeight / 2f + topWallHeight / 2f),
                new Vector3(doorWidth, topWallHeight, wallThickness),
                wallColor, material);
        }

        private void CreateWallWithWindow(string name, Transform parent, Vector3 wallCenter,
            float wallThickness, float wallHeight, float wallDepth, Color wallColor, Material? material = null)
        {
            // Retail-style large windows
            float windowWidth = 2.5f; // Much wider for shop feel
            float windowHeight = 2.0f; // Taller
            float windowBottom = 0.8f; // Standard sill height (0.8m)
            
            // Adjust if wall is too small
            if (wallDepth < windowWidth + 0.5f) windowWidth = wallDepth - 0.5f;
            if (wallHeight < windowHeight + 1.0f) windowHeight = wallHeight - 1.2f;

            // Recalculate segments
            float topHeight = wallHeight - (windowBottom + windowHeight);
            
            // 1. Bottom Segment (Sill down to floor)
            CreateWall($"{name}_Bottom", parent,
                wallCenter + Vector3.down * ((wallHeight / 2f) - (windowBottom / 2f)),
                new Vector3(wallThickness, windowBottom, wallDepth),
                wallColor, material);

            // 2. Top Segment (Header up to ceiling)
            float topSegmentCenterY = (wallHeight / 2f) - (topHeight / 2f);
            CreateWall($"{name}_Top", parent,
                wallCenter + Vector3.up * topSegmentCenterY,
                new Vector3(wallThickness, topHeight, wallDepth),
                wallColor, material);

            // 3. Side Segments
            float sideWidth = (wallDepth - windowWidth) / 2f;
            float windowCenterY = (windowBottom + windowHeight / 2f) - (wallHeight / 2f); // Relative to wall center
            
            // Left Side
            CreateWall($"{name}_Left", parent,
                wallCenter + new Vector3(0f, windowCenterY, (windowWidth / 2f + sideWidth / 2f)),
                new Vector3(wallThickness, windowHeight, sideWidth),
                wallColor, material);

            // Right Side
            CreateWall($"{name}_Right", parent,
                wallCenter + new Vector3(0f, windowCenterY, -(windowWidth / 2f + sideWidth / 2f)),
                new Vector3(wallThickness, windowHeight, sideWidth),
                wallColor, material);

            // 4. Window Frame (Matte Black)
            Color frameColor = new Color(0.1f, 0.1f, 0.1f);
            float frameDepth = 0.05f; // Frame thickness sticking out
            float frameWidth = 0.1f; // Width of the frame face
            
            // Top Frame
            PrimitiveBuilder.CreateBox($"{name}_FrameTop",
                wallCenter + new Vector3(0f, windowCenterY + windowHeight/2f - frameWidth/2f, 0f),
                new Vector3(wallThickness + frameDepth, frameWidth, windowWidth),
                frameColor, parent);
                
            // Bottom Frame
            PrimitiveBuilder.CreateBox($"{name}_FrameBottom",
                wallCenter + new Vector3(0f, windowCenterY - windowHeight/2f + frameWidth/2f, 0f),
                new Vector3(wallThickness + frameDepth, frameWidth, windowWidth),
                frameColor, parent);
                
            // Left Frame
            PrimitiveBuilder.CreateBox($"{name}_FrameLeft",
                wallCenter + new Vector3(0f, windowCenterY, windowWidth/2f - frameWidth/2f),
                new Vector3(wallThickness + frameDepth, windowHeight - 2*frameWidth, frameWidth),
                frameColor, parent);
                
            // Right Frame
            PrimitiveBuilder.CreateBox($"{name}_FrameRight",
                wallCenter + new Vector3(0f, windowCenterY, -(windowWidth/2f - frameWidth/2f)),
                new Vector3(wallThickness + frameDepth, windowHeight - 2*frameWidth, frameWidth),
                frameColor, parent);

            // 5. Window Glass
            PrimitiveBuilder.CreateBox($"{name}_WindowGlass",
                wallCenter + new Vector3(0f, windowCenterY, 0f),
                new Vector3(wallThickness * 0.2f, windowHeight - 0.1f, windowWidth - 0.1f), // Slightly smaller to fit in frame
                new Color(0.7f, 0.9f, 1f),
                parent);
                
            // Apply laundromat glass material if available
            Transform glass = parent.Find($"{name}_WindowGlass");
            if (glass != null)
            {
                Material glassMat = GameAssets.Materials.LaundromatGlass;
                if (glassMat != null && glass.GetComponent<Renderer>())
                {
                    glass.GetComponent<Renderer>().material = glassMat;
                }
            }
        }
        
        #endregion

        #region Private Helper Methods - Furniture Creation
        
        private void CreateTable(Vector3 position, Quaternion rotation, Color color, Transform parent)
        {
            GameObject table = BuildingUtilities.CreateFolder("Table", parent);
            table.transform.localPosition = position;
            table.transform.localRotation = rotation;

            // Use the centralized GameAssets helper to get the proper wood material
            Material tableMat = GameAssets.Materials.WoodMediumBrown;
            
            // Fallback logic handled inside GameAssets, but if we wanted color tinting we could do it here.
            // For now, we trust the material.

            // Table top (Thicker, more substantial)
            GameObject top = PrimitiveBuilder.CreateBox("TableTop",
                new Vector3(0f, 0.75f, 0f),
                new Vector3(1.4f, 0.08f, 0.9f), // Slightly larger
                color, table.transform);
            
            // Apply material
            if (tableMat != null)
            {
                Renderer r = top.GetComponent<Renderer>();
                if (r != null) r.material = tableMat;
            }

            // Legs - Change to a modern industrial "H" frame or sturdy legs
            // Using 4 thicker legs for stability visual
            float legWidth = 0.08f;
            Color legColor = new Color(0.2f, 0.2f, 0.2f); // Dark metal legs usually look better with wood tops

            Vector3[] legPositions = new Vector3[]
            {
                new Vector3(0.6f, 0.375f, 0.35f),
                new Vector3(-0.6f, 0.375f, 0.35f),
                new Vector3(0.6f, 0.375f, -0.35f),
                new Vector3(-0.6f, 0.375f, -0.35f)
            };

            for (int i = 0; i < 4; i++)
            {
                PrimitiveBuilder.CreateBox($"Leg{i+1}",
                    legPositions[i],
                    new Vector3(legWidth, 0.75f, legWidth), // Square metal legs
                    legColor, table.transform);
            }
            
            // Cross braces for industrial look
            PrimitiveBuilder.CreateBox("CrossBraceShort1",
                new Vector3(0.6f, 0.2f, 0f),
                new Vector3(legWidth, 0.05f, 0.7f),
                legColor, table.transform);
                
            PrimitiveBuilder.CreateBox("CrossBraceShort2",
                new Vector3(-0.6f, 0.2f, 0f),
                new Vector3(legWidth, 0.05f, 0.7f),
                legColor, table.transform);
                
            PrimitiveBuilder.CreateBox("CrossBraceLong",
                new Vector3(0f, 0.2f, 0f),
                new Vector3(1.2f, 0.05f, legWidth),
                legColor, table.transform);
        }

        private void CreateChair(Vector3 position, Quaternion rotation, Color color, Transform parent)
        {
            GameObject chair = BuildingUtilities.CreateFolder("Chair", parent);
            chair.transform.localPosition = position;
            chair.transform.localRotation = rotation;

            // Seat
            PrimitiveBuilder.CreateBox("Seat",
                new Vector3(0f, 0.45f, 0f),
                new Vector3(0.5f, 0.05f, 0.5f),
                color, chair.transform);

            // Backrest
            PrimitiveBuilder.CreateBox("Backrest",
                new Vector3(0f, 0.7f, -0.225f),
                new Vector3(0.5f, 0.5f, 0.05f),
                color, chair.transform);

            // Legs
            float legRadius = 0.03f;
            PrimitiveBuilder.CreateCylinder("Leg1",
                new Vector3(0.2f, 0.225f, 0.2f),
                new Vector3(legRadius, 0.45f, legRadius),
                color, chair.transform);
            PrimitiveBuilder.CreateCylinder("Leg2",
                new Vector3(-0.2f, 0.225f, 0.2f),
                new Vector3(legRadius, 0.45f, legRadius),
                color, chair.transform);
            PrimitiveBuilder.CreateCylinder("Leg3",
                new Vector3(0.2f, 0.225f, -0.2f),
                new Vector3(legRadius, 0.45f, legRadius),
                color, chair.transform);
            PrimitiveBuilder.CreateCylinder("Leg4",
                new Vector3(-0.2f, 0.225f, -0.2f),
                new Vector3(legRadius, 0.45f, legRadius),
                color, chair.transform);
        }

        private void CreateDesk(Vector3 position, Quaternion rotation, Color color, Transform parent)
        {
            GameObject desk = BuildingUtilities.CreateFolder("Desk", parent);
            desk.transform.localPosition = position;
            desk.transform.localRotation = rotation;

            // Desk top
            PrimitiveBuilder.CreateBox("DeskTop",
                new Vector3(0f, 0.75f, 0f),
                new Vector3(1.5f, 0.05f, 0.7f),
                color, desk.transform);

            // Side panels
            PrimitiveBuilder.CreateBox("LeftPanel",
                new Vector3(-0.6f, 0.375f, 0f),
                new Vector3(0.05f, 0.75f, 0.7f),
                color, desk.transform);
            PrimitiveBuilder.CreateBox("RightPanel",
                new Vector3(0.6f, 0.375f, 0f),
                new Vector3(0.05f, 0.75f, 0.7f),
                color, desk.transform);
        }

        private void CreateBed(Vector3 position, Quaternion rotation, Color color, Transform parent)
        {
            GameObject bed = BuildingUtilities.CreateFolder("Bed", parent);
            bed.transform.localPosition = position;
            bed.transform.localRotation = rotation;

            // Mattress
            PrimitiveBuilder.CreateBox("Mattress",
                new Vector3(0f, 0.4f, 0f),
                new Vector3(1.5f, 0.3f, 2f),
                color, bed.transform);

            // Headboard
            PrimitiveBuilder.CreateBox("Headboard",
                new Vector3(0f, 0.75f, -1f),
                new Vector3(1.5f, 1.2f, 0.1f),
                new Color(0.3f, 0.2f, 0.1f), bed.transform);
        }

        private void CreateBookshelf(Vector3 position, Quaternion rotation, Color color, Transform parent)
        {
            GameObject shelf = BuildingUtilities.CreateFolder("Bookshelf", parent);
            shelf.transform.localPosition = position;
            shelf.transform.localRotation = rotation;

            // Back panel
            PrimitiveBuilder.CreateBox("BackPanel",
                new Vector3(0f, 1f, 0f),
                new Vector3(1.2f, 2f, 0.05f),
                color, shelf.transform);

            // Shelves - Reduced to 3 shelves with more spacing for taller items (bongs)
            for (int i = 0; i < 3; i++)
            {
                // Start higher (0.35) and use larger spacing (0.6)
                // y = 0.35, 0.95, 1.55
                float y = i * 0.6f + 0.35f;
                PrimitiveBuilder.CreateBox($"Shelf{i}",
                    new Vector3(0f, y, 0.15f),
                    new Vector3(1.2f, 0.03f, 0.3f),
                    color, shelf.transform);
            }
        }

        private void CreateCounter(Vector3 position, Quaternion rotation, Color color, Transform parent)
        {
            GameObject counter = BuildingUtilities.CreateFolder("Counter", parent);
            counter.transform.localPosition = position;
            counter.transform.localRotation = rotation;

            // Use Dark Grey Metal for the counter body as requested
            Material metalMat = GameAssets.Materials.MetalDarkGrey;

            // Counter top (Glass or Metal? Let's use Metal for a solid look, or keep it clean)
            // If we want a "modern" dispensary, maybe a glass top on metal base?
            // The request said "use metal... for the front counter". I'll apply it to the main cabinet.
            // I'll keep the top as is (maybe glass tint from color) or just make it all metal.
            // Let's make the base metal and the top glass-like or clean white.
            // Actually, "front counter" usually refers to the whole unit. I'll make the cabinet metal.
            
            // Counter top
            PrimitiveBuilder.CreateBox("CounterTop",
                new Vector3(0f, 0.9f, 0f),
                new Vector3(2f, 0.05f, 0.6f),
                color, counter.transform); // Keep top customizable or use a specific material?

            // Cabinet base
            GameObject cabinet = PrimitiveBuilder.CreateBox("Cabinet",
                new Vector3(0f, 0.45f, 0f),
                new Vector3(2f, 0.9f, 0.6f),
                color, counter.transform);
            
            if (metalMat != null)
            {
                if (cabinet.GetComponent<Renderer>()) cabinet.GetComponent<Renderer>().material = metalMat;
            }
        }
        
        #endregion

        #region Private Helper Methods - Furniture Groups
        
        private void CreateDiningSet(Vector3 centerPos, Quaternion rotation)
        {
            GameObject group = GetOrCreateContainer("FurnitureGroups");
            Color woodColor = new Color(0.4f, 0.25f, 0.1f);

            CreateTable(centerPos, rotation, woodColor, group.transform);
            CreateChair(centerPos + new Vector3(0.8f, 0f, 0f), rotation, woodColor, group.transform);
            CreateChair(centerPos + new Vector3(-0.8f, 0f, 0f), rotation, woodColor, group.transform);
            CreateChair(centerPos + new Vector3(0f, 0f, 0.6f), rotation, woodColor, group.transform);
            CreateChair(centerPos + new Vector3(0f, 0f, -0.6f), rotation, woodColor, group.transform);
        }

        private void CreateBedroomSet(Vector3 centerPos, Quaternion rotation)
        {
            GameObject group = GetOrCreateContainer("FurnitureGroups");
            Color bedColor = new Color(0.9f, 0.9f, 0.95f);
            Color woodColor = new Color(0.4f, 0.25f, 0.1f);

            CreateBed(centerPos, rotation, bedColor, group.transform);
            CreateDesk(centerPos + new Vector3(2f, 0f, -1f), rotation, woodColor, group.transform);
        }

        private void CreateOfficeSet(Vector3 centerPos, Quaternion rotation)
        {
            GameObject group = GetOrCreateContainer("FurnitureGroups");
            Color woodColor = new Color(0.4f, 0.25f, 0.1f);

            CreateDesk(centerPos, rotation, woodColor, group.transform);
            CreateChair(centerPos + new Vector3(0f, 0f, 0.6f), rotation, woodColor, group.transform);
            CreateBookshelf(centerPos + new Vector3(-1.5f, 0f, 0f), rotation, woodColor, group.transform);
        }

        private void CreateLivingRoomSet(Vector3 centerPos, Quaternion rotation)
        {
            GameObject group = GetOrCreateContainer("FurnitureGroups");
            Color couchColor = new Color(0.2f, 0.3f, 0.5f); // Blue
            Color tableColor = new Color(0.4f, 0.25f, 0.1f);

            // Simple couch (just a box for now)
            PrimitiveBuilder.CreateBox("Couch",
                centerPos,
                new Vector3(2f, 0.8f, 0.9f),
                couchColor, group.transform);

            CreateCoffeeTable(centerPos + new Vector3(0f, 0f, 1.2f), rotation, tableColor, group.transform);
        }

        private void CreateCoffeeTable(Vector3 position, Quaternion rotation, Color color, Transform parent)
        {
            GameObject table = BuildingUtilities.CreateFolder("CoffeeTable", parent);
            table.transform.localPosition = position;
            table.transform.localRotation = rotation;

            // Use the centralized GameAssets helper
            Material tableMat = GameAssets.Materials.WoodMediumBrown;

            // Low Table top (Refined size: 1.1m x 0.6m)
            GameObject top = PrimitiveBuilder.CreateBox("TableTop",
                new Vector3(0f, 0.45f, 0f), // 45cm height
                new Vector3(1.1f, 0.06f, 0.6f), // Thinner top for smaller table
                color, table.transform);
            
            if (tableMat != null)
            {
                Renderer r = top.GetComponent<Renderer>();
                if (r != null) r.material = tableMat;
            }

            // Legs - Industrial style
            // Legs - Industrial style
            float legWidth = 0.06f; // Slightly thinner legs
            Color legColor = new Color(0.2f, 0.2f, 0.2f); // Dark metal

            // Leg positions adjusted for new size 
            // User requested local x to be 0.27
            float xOff = 0.27f;
            float zOff = 0.6f / 2f - 0.05f - legWidth/2f; // ~0.22f

            Vector3[] legPositions = new Vector3[]
            {
                new Vector3(xOff, 0.225f, zOff),
                new Vector3(-xOff, 0.225f, zOff),
                new Vector3(xOff, 0.225f, -zOff),
                new Vector3(-xOff, 0.225f, -zOff)
            };

            for (int i = 0; i < 4; i++)
            {
                PrimitiveBuilder.CreateBox($"Leg{i+1}",
                    legPositions[i],
                    new Vector3(legWidth, 0.45f, legWidth),
                    legColor, table.transform);
            }
            
            // Industrial H-Frame Bracing
            float braceHeight = 0.15f; // Height from floor
            
            // Short Braces (Now connecting legs along X axis at Z offsets)
            // Position on Z axis (0.22 and -0.22)
            // Rotated 90 degrees so Scale Z (0.6) runs along World X
            
            // Bar 1 (Positive Z side)
            GameObject brace1 = PrimitiveBuilder.CreateBox("CrossBraceShort1",
                new Vector3(0f, braceHeight, zOff),
                new Vector3(legWidth, 0.04f, 0.6f), // Scale Z is length
                legColor, table.transform);
            brace1.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                
            // Bar 2 (Negative Z side)
            GameObject brace2 = PrimitiveBuilder.CreateBox("CrossBraceShort2",
                new Vector3(0f, braceHeight, -zOff),
                new Vector3(legWidth, 0.04f, 0.6f), 
                legColor, table.transform);
            brace2.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                
            // Connector (Center)
            // Rotated 90 degrees so Scale X (0.5) runs along World Z
            GameObject braceLong = PrimitiveBuilder.CreateBox("CrossBraceLong",
                new Vector3(0f, braceHeight, 0f),
                new Vector3(0.5f, 0.04f, legWidth), // Scale X is length
                legColor, table.transform);
            braceLong.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }
        
        #endregion
    }
}
