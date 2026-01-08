# Introduction

MAPI (Mapping API) is a library for creating procedural 3D content in Schedule 1 mods. It focuses on mesh generation, building construction, and GLTF model loading while maintaining update resilience by avoiding direct dependencies on game assemblies.

## Core Design Principles

### Zero Initialization
MAPI requires no initialization. All APIs work immediately without calling any setup methods. Unity automatically handles resource cleanup when scenes unload or the application quits.

### Update Resilience
MAPI never references `Assembly-CSharp.dll` types. This means your mods continue working across game updates, as long as Unity and FishNet remain stable (they typically do).

### Fluent API
MAPI uses a fluent builder pattern for intuitive method chaining:

```csharp
GameObject building = new BuildingBuilder("MyShop")
    .WithConfig(BuildingConfig.Medium)
    .AddFloor()
    .AddWalls(southDoor: true)
    .AddLights()
    .Build();
```

## Key Features

### Procedural Meshes
Generate basic shapes and combine them into complex geometries:
- Boxes, spheres, cylinders, capsules
- Flat shading for low-poly aesthetics
- Custom materials and colors

### Building Construction
Create complete building structures with:
- Walls, floors, ceilings, roofs
- Windows and doors
- Interior furniture and decorations
- Lighting systems

### GLTF Import
Load external 3D models at runtime:
- Supports GLB and GLTF formats
- No external dependencies
- Automatic coordinate system conversion

### Resource Management
Optional tracking and cleanup of created resources:
- Automatic registration with `ResourceTracker`
- Manual cleanup via `ResourceTracker.CleanupAll()`
- Unity handles scene-based cleanup automatically

## When to Use MAPI

Use MAPI when you need to:
- Create custom 3D objects programmatically
- Build structures that don't depend on game entities
- Import external 3D models
- Avoid game assembly dependencies

Use [S1API](https://github.com/ifBars/S1API) when you need to:
- Interact with game entities (NPCs, items, quests)
- Access game systems (storage, sales, cooking)
- Create custom NPC behaviors

## Next Steps

- [Getting Started](getting-started.html) - Install MAPI and create your first project
- [Procedural Mesh Guide](procedural-mesh.html) - Learn mesh generation
- [Building Guide](building.html) - Create structured buildings
- [GLTF Loading](gltf-loading.html) - Import external models
