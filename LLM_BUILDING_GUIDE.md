# LLM-Friendly Building Generation Guide

This guide demonstrates how to use MAPI's semantic building API to construct complete buildings and interiors using natural language concepts instead of precise coordinates.

## Key Advantages for LLMs

1. **No coordinate math required** - Use semantic terms like "center", "north", "large"
2. **Automatic furniture sizing** - "table" knows how big a table should be
3. **Furniture groups** - "dining_set" creates table + chairs automatically
4. **Sensible defaults** - Materials, colors, and sizes are pre-configured

---

## Quick Start: Complete Pet Shop Example

```csharp
using MAPI.Core;

// Initialize MAPI
MAPI.Initialize();

// Create a pet shop with interior in 15 lines of code
GameObject petShop = MAPI.CreateSemanticBuilding("PetShop")
    .DefineRoom("large", "normal")           // 12m x 10m x 3m room
    .AddFloor()                              // Gray floor
    .AddCeiling()                            // White ceiling
    .AddWalls(                               // Walls with door and window
        hasNorthDoor: true,                  
        hasEastWindow: true)
    .AddFurnitureGroup("office_set", "northwest")  // Desk, chair, bookshelf
    .AddFurniture("counter", "east")         // Sales counter on east wall
    .AddFurniture("table", "center")         // Display table in center
    .AddFurniture("bookshelf", "south")      // Product shelves on south wall
    .Build();

// Position in world
petShop.transform.position = new Vector3(0, 0, 0);
```

**That's it!** The LLM doesn't need to calculate any positions, sizes, or rotations.

---

## Semantic Room Sizes

Use descriptive size categories instead of numbers:

```csharp
// Size categories (width x depth)
.DefineRoom("tiny", "normal")       // 3m x 3m x 3m - Closet/bathroom
.DefineRoom("small", "normal")      // 5m x 4m x 3m - Small bedroom
.DefineRoom("medium", "normal")     // 8m x 6m x 3m - Living room
.DefineRoom("large", "normal")      // 12m x 10m x 3m - Shop/office
.DefineRoom("huge", "cathedral")    // 20m x 15m x 6m - Warehouse/hall

// Height categories
.DefineRoom("medium", "low")        // 2.4m ceiling - Low ceiling
.DefineRoom("medium", "normal")     // 3m ceiling - Standard
.DefineRoom("medium", "tall")       // 4m ceiling - High ceiling
.DefineRoom("medium", "cathedral")  // 6m ceiling - Very tall
```

Or use exact dimensions when needed:
```csharp
.DefineRoom(width: 10f, height: 3.5f, depth: 8f)
```

---

## Semantic Positioning

Use compass directions and named positions:

```csharp
// Cardinal directions
.AddFurniture("desk", "north")      // Against north wall
.AddFurniture("bed", "south")       // Against south wall
.AddFurniture("bookshelf", "east")  // Against east wall
.AddFurniture("chair", "west")      // Against west wall

// Corners
.AddFurniture("table", "northeast") // Northeast corner
.AddFurniture("lamp", "southwest")  // Southwest corner

// Center
.AddFurniture("table", "center")    // Room center
```

**Automatic spacing:** Furniture automatically placed 1m from walls (configurable).

---

## Furniture Types

Individual furniture pieces with automatic sizing:

```csharp
.AddFurniture("table", "center")       // Dining table (1.2m x 0.8m)
.AddFurniture("chair", "north")        // Chair (0.5m x 0.5m)
.AddFurniture("desk", "east")          // Desk (1.5m x 0.7m)
.AddFurniture("bed", "south")          // Bed (1.5m x 2m)
.AddFurniture("bookshelf", "west")     // Bookshelf (1.2m wide x 2m tall)
.AddFurniture("counter", "northeast")  // Counter (2m x 0.6m)
```

Optional color override:
```csharp
.AddFurniture("table", "center", Color.red)
```

---

## Furniture Groups

Pre-configured furniture sets that work together:

```csharp
// Dining room setup (table + 4 chairs)
.AddFurnitureGroup("dining_set", "center")

// Bedroom setup (bed + desk)
.AddFurnitureGroup("bedroom_set", "south")

// Office setup (desk + chair + bookshelf)
.AddFurnitureGroup("office_set", "north")

// Living room setup (couch + coffee table)
.AddFurnitureGroup("living_room_set", "center")
```

---

## Wall Configuration

Automatically create walls with openings:

```csharp
// Simple enclosed room
.AddWalls()

// Room with door on north wall
.AddWalls(hasNorthDoor: true)

// Room with door and windows
.AddWalls(
    hasNorthDoor: true,     // Door on north wall
    hasSouthDoor: false,    // No door on south wall
    hasEastWindow: true,    // Window on east wall
    hasWestWindow: true)    // Window on west wall

// Custom wall color
.AddWalls(wallColor: new Color(0.8f, 0.9f, 1f)) // Light blue walls
```

**Automatic features:**
- Doors: 1m wide, 2.1m tall, positioned centrally
- Windows: 1.2m x 1.2m, centered vertically, includes glass pane

---

## Complete LLM Prompt Examples

### Example 1: Cozy Bedroom

**Prompt to LLM:**
> "Create a small cozy bedroom with a bed against the south wall, a desk by the window on the east wall, and a bookshelf in the corner."

**Generated Code:**
```csharp
GameObject bedroom = MAPI.CreateSemanticBuilding("CozyBedroom")
    .DefineRoom("small", "normal")
    .AddFloor(new Color(0.7f, 0.6f, 0.5f))  // Warm wooden floor
    .AddCeiling()
    .AddWalls(hasEastWindow: true)
    .AddFurniture("bed", "south", new Color(0.9f, 0.9f, 1f))
    .AddFurniture("desk", "east", new Color(0.4f, 0.25f, 0.1f))
    .AddFurniture("bookshelf", "northeast", new Color(0.4f, 0.25f, 0.1f))
    .Build();
```

### Example 2: Coffee Shop

**Prompt to LLM:**
> "Build a medium-sized coffee shop with a counter on the north wall, three dining sets spread around, and a bookshelf for ambiance."

**Generated Code:**
```csharp
GameObject coffeeShop = MAPI.CreateSemanticBuilding("CoffeeShop")
    .DefineRoom("medium", "normal")
    .AddFloor()
    .AddCeiling()
    .AddWalls(
        hasSouthDoor: true,
        hasEastWindow: true,
        hasWestWindow: true,
        wallColor: new Color(0.85f, 0.8f, 0.7f))  // Warm beige
    .AddFurniture("counter", "north", new Color(0.3f, 0.3f, 0.35f))
    .AddFurnitureGroup("dining_set", "southwest")
    .AddFurnitureGroup("dining_set", "southeast")
    .AddFurnitureGroup("dining_set", "center")
    .AddFurniture("bookshelf", "west", new Color(0.4f, 0.25f, 0.1f))
    .Build();
```

### Example 3: Home Office

**Prompt to LLM:**
> "Design a professional home office with tall ceilings, an office set in the corner, and good natural light."

**Generated Code:**
```csharp
GameObject homeOffice = MAPI.CreateSemanticBuilding("HomeOffice")
    .DefineRoom("medium", "tall")  // Tall ceiling (4m)
    .AddFloor(new Color(0.6f, 0.6f, 0.6f))  // Gray floor
    .AddCeiling()
    .AddWalls(
        hasEastWindow: true,
        hasWestWindow: true,
        wallColor: Color.white)
    .AddFurnitureGroup("office_set", "northwest")
    .AddFurniture("bookshelf", "south", new Color(0.4f, 0.25f, 0.1f))
    .Build();
```

---

## LLM Decision Flow

When an LLM receives a building request, it should follow this pattern:

1. **Choose room size:** tiny/small/medium/large/huge
2. **Choose ceiling height:** low/normal/tall/cathedral
3. **Add structure:** floor, ceiling, walls (with door/window placement)
4. **Add furniture groups:** dining_set, bedroom_set, office_set, living_room_set
5. **Add individual furniture:** Fill gaps with specific pieces
6. **Use semantic positions:** center, north, south, east, west, corners

---

## API Comparison: Old vs New

### Old API (Coordinate-Based)
```csharp
// LLM must calculate exact positions
GameObject wall = PrimitiveBuilder.CreateBox("Wall",
    new Vector3(5f, 1.5f, 0f),      // Where is this exactly?
    new Vector3(10f, 3f, 0.2f),     // What size makes sense?
    Color.gray);

GameObject table = PrimitiveBuilder.CreateBox("Table",
    new Vector3(2.5f, 0.4f, 1.5f),  // How to center this?
    new Vector3(1.2f, 0.8f, 0.6f),  // Is this table-sized?
    Color.brown);
```
❌ **Problems:** LLMs struggle with spatial reasoning, prone to errors

### New API (Semantic)
```csharp
// LLM uses natural language concepts
GameObject room = MAPI.CreateSemanticBuilding("Dining Room")
    .DefineRoom("medium", "normal")         // Clear size concept
    .AddWalls()                             // Automatic positioning
    .AddFurnitureGroup("dining_set", "center")  // Semantic placement
    .Build();
```
✅ **Benefits:** Natural language, no math, semantically correct

---

## Extension Points for Future

The semantic API can be easily extended:

```csharp
// Add more furniture types
case "lamp":
    CreateLamp(placementPosition, furnitureColor, furnitureContainer.transform);
    break;

// Add more groups
case "kitchen_set":
    CreateKitchenSet(centerPos);
    break;

// Add room styles
.ApplyStyle("modern")    // Changes colors/materials
.ApplyStyle("rustic")
.ApplyStyle("minimalist")
```

---

## Error Handling

The API is forgiving with LLM mistakes:

```csharp
// Unknown size? Falls back to "medium"
.DefineRoom("gigantic", "normal")  // → Uses "medium" + warning log

// Unknown position? Falls back to "center"
.AddFurniture("table", "above")    // → Places at center + warning log

// Unknown furniture? Logs warning, continues
.AddFurniture("spaceship", "north") // → Logs warning, skips
```

---

## Best Practices for LLM Integration

1. **Start with room definition** - Always call `DefineRoom()` first
2. **Add structure before furniture** - Floor, ceiling, walls, then furniture
3. **Use furniture groups** - More coherent than individual pieces
4. **Semantic positions** - Easier for LLMs than coordinates
5. **Let defaults work** - Colors and sizes are pre-configured

---

## Performance Notes

- Each furniture piece is 1-4 GameObjects (low overhead)
- Walls are split into segments (doors/windows) for visual accuracy
- All objects registered with ResourceTracker for automatic cleanup
- No runtime mesh generation (uses Unity primitives for speed)

---

## Summary

The `SemanticBuildingBuilder` makes it possible for LLMs to:

✅ Build complete rooms without coordinate math  
✅ Place furniture using natural language  
✅ Create coherent interiors with furniture groups  
✅ Handle doors, windows, and walls automatically  
✅ Generate game-ready buildings in 10-20 lines of code  

Perfect for procedural generation, AI-assisted level design, and natural language building tools.
