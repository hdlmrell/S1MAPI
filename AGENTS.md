# Repository Guidelines

## Project Structure & Module Organization
S1MAPI is a mesh and building construction library for Schedule 1 mods. It avoids ScheduleOne types to remain update-resilient—using only Unity primitives and FishNet.

- `Core/` — Initialization, resource tracking, embedded resource loading
- `ProceduralMesh/` — Fluent mesh builders, organic shapes, mesh utilities
- `Building/` — Building construction, primitives, interior generation
- `Gltf/` — GLTF file loading and processing
- `Extensions/` — Extension methods for Unity types (Transform, GameObject, Component, Mesh, etc.)
- `Utils/` — Constants, logging, material presets, game object utilities

Namespaces mirror folder structure (e.g., `S1MAPI.ProceduralMesh`, `S1MAPI.Building`).

## Build, Test, and Development Commands
- `dotnet build S1MAPI.csproj -c Mono` — Mono-specific build
- `dotnet build S1MAPI.csproj -c Il2cpp` — IL2CPP-specific build
- `dotnet clean` — Clean build artifacts

## Prefab Placement Guidelines

### Networked vs Non-Networked Prefabs
Prefabs should **usually be networked** to ensure consistency across all clients. Use networked prefabs when the object needs to be visible to and interactable by other players, or when its state must be synchronized across the network.

**Networked Prefab Placement:**
```csharp
// Use PrefabPlacer for networked prefab placement
var placer = new PrefabPlacer(parentTransform);
GameObject? networkedPrefab = placer.Place(prefab, position, rotation, networked: true);
// Automatically handles NetworkObject spawning and ownership
```

**Non-Networked Prefab Placement:**
```csharp
// Only use for client-only visual elements that don't need synchronization
GameObject nonNetworked = UnityEngine.Object.Instantiate(prefab, position, rotation);
// No network synchronization - use sparingly
```

**Decision Guidelines:**
- Default to networked prefabs (NetworkObject, NetworkTransform)
- Use non-networked only for: UI elements, client-only effects, debug visualizations
- Never use non-networked for: interactive objects, player-synced state, important world objects
- When in doubt, use networked - it's easier to add than remove network behavior

### Mesh Instantiation
S1MAPI provides two primary pathways for mesh creation:

**PrimitiveBuilder (Simple meshes):**
```csharp
var builder = new PrimitiveBuilder()
    .WithBox(position, size)
    .WithMaterial(material);
GameObject mesh = builder.Build();
```

**ProceduralMeshBuilder (Complex/Custom meshes):**
```csharp
var meshBuilder = new ProceduralMeshBuilder()
    .WithVertices(customVertices)
    .WithTriangles(triangles)
    .WithUVs(uvs)
    .WithNormals(normals);
GameObject mesh = meshBuilder.Build();
// For complex organic shapes, use OrganicShapeGenerator
```

### Building Namespace Usage

The Building namespace provides a structured approach to construction:

**BuildingBuilder (Main entry point):**
```csharp
var building = new BuildingBuilder()
    .WithConfig(buildingConfig)
    .WithPalette(buildingPalette)
    .WithPosition(basePosition)
    .Build();
// Orchestrates entire building construction
```

**Component Builders:**
- `WallBuilder` - Structural walls, windows, doors
- `InteriorBuilder` - Room generation, floor/ceiling placement
- `DecorBuilder` - Interior decoration, fixtures
- `FurnitureBuilder` - Furniture placement
- `LightingBuilder` - Light fixtures and illumination
- `PrefabPlacer` - Strategic prefab placement within buildings

**Configuration:**
- `BuildingConfig` defines structural parameters (dimensions, materials, style)
- `BuildingPalette` manages material and texture selection
- Both support serialization for saved building configurations

**Best Practices:**
1. Start with BuildingBuilder for complete structures
2. Use component builders (WallBuilder, InteriorBuilder) for specific systems
3. Configure via BuildingConfig/BuildingPalette before building
4. PrefabPlacer integrates external prefabs into building constructions

## Coding Style & Naming Conventions
Follow `CODING_STANDARDS.md`. Key points:
- PascalCase for types/methods/properties; camelCase with `_` prefix for private/internal fields
- Fluent builder pattern: configuration methods return `this`, terminate with `Build()`
- Use nested static classes for constants (`Constants.Mesh.MaxVerticesPerMesh`)
- Use `DebugLog` for logging; XML documentation required for all public APIs
- XML documentation required for all public APIs
- Avoid implementing unnecessary features, prefer less code in favor of cleaner code
- Separate concerns of Schedule 1 specific Meshes/Prefabs and primitive game objects/meshes, for example S1 namespace has Schedule 1 assets, PrimitiveBuilder and things built with it should be separated by concern as of C# standards
- Always use the appropriate builder for the task: `PrefabPlacer` for prefabs, `PrimitiveBuilder` for simple meshes, `ProceduralMeshBuilder` for complex meshes, and component builders (`WallBuilder`, `InteriorBuilder`, etc.) for building construction

## Relationship to S1API
S1MAPI and S1API serve complementary roles:
- **S1MAPI**: Mesh construction, building generation, GLTF loading (no ScheduleOne types)
- **S1API**: Game component wrappers, entity management, quest systems (wraps ScheduleOne types)

Mods typically use both: S1MAPI for construction, S1API for game integration. S1MAPI should never depend on S1API or ScheduleOne types.

## Testing Guidelines
- No automated test suite yet; treat multiplatform builds as acceptance gate
- Build both `Mono` and `Il2cpp` configurations before submitting PRs
- Test mesh generation and building construction in-game across both runtimes
- Use `S1MAPI.Utils.DebugLog` with clear module context for all logging

## Commit & Pull Request Guidelines
- Conventional Commits with module scope: `feat(ProceduralMesh): add sphere generator`
- Keep commits single-purpose and imperative
- PRs: Include description, testing steps, and screenshots for visual changes
- Never commit `local.build.props` or secrets
