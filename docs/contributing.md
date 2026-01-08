# Contributing to S1MAPI

This guide covers coding standards, development practices, and contribution guidelines for S1MAPI.

I won't hold you accountable for every little detail in this guide, especially if I already fail to follow the standard with my code. Nonetheless, make the effort to at least adhere to most of it for faster PR merge.

## Core Principle

**Avoid ScheduleOne types** — S1MAPI remains update-resilient by using only Unity primitives and FishNet. Never reference `Assembly-CSharp.dll`.

## Getting Started

### Prerequisites
- .NET SDK matching the project's target framework
- Access to Schedule 1 game assemblies (configured in `local.build.props`)
- MelonLoader for testing

### Building

```bash
# Build for Mono (Schedule 1 Windows)
dotnet build -c Mono

# Build for IL2CPP (Schedule 1 Linux server)
dotnet build -c Il2cpp

# Clean build artifacts
dotnet clean
```

## Project Structure

```
S1MAPI/
├── Core/           # Initialization, resource tracking
├── ProceduralMesh/ # Mesh generation builders
├── Building/       # Building construction
├── Gltf/           # GLTF loading
├── Extensions/     # Unity extension methods
├── Utils/          # Constants, logging, presets
└── S1/             # Schedule 1 specific materials/meshes
```

Namespaces mirror folder structure: `namespace S1MAPI.Building { ... }`

## Coding Standards

### Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Classes, methods, properties | PascalCase | `ProceduralMeshBuilder` |
| Local variables, parameters | camelCase | `meshBuilder` |
| Private/internal fields | `_camelCase` | `_vertices` |
| Static readonly fields | PascalCase | `DefaultMeshName` |
| Constants | Nested classes with PascalCase | `Constants.Mesh.MaxVertices` |

```csharp
namespace S1MAPI.ProceduralMesh
{
    public sealed class ProceduralMeshBuilder
    {
        private readonly List<Vector3> _vertices;
        private Material? _material;

        public ProceduralMeshBuilder(string name) { ... }

        public ProceduralMeshBuilder AddBox(Vector3 center, Vector3 size) => this;
    }
}
```

### Access Modifiers

- **Public API**: Use `public` with complete XML documentation
- **Internal details**: Use `internal` 
- **Inheritance not intended**: Use `sealed`

```csharp
public sealed class ProceduralMeshBuilder { ... }
```

### Nullable Types

Declare nullable variables with `?`:

```csharp
private Material? _material;
public Material? DefaultMaterial { get; set; }
```

## Code Organization

### Member Order

1. Fields (private, internal, public)
2. Constructors
3. Properties
4. Public API methods
5. Private/internal helper methods
6. Event handlers

### Regions

Use `#region` to group related members:

```csharp
#region Fields
private readonly string _name;
private readonly List<Vector3> _vertices;
#endregion

#region Public API
public ProceduralMeshBuilder AddBox(...) { ... }
public GameObject Build() { ... }
#endregion
```

## Fluent Builder Pattern

S1MAPI's primary API style uses fluent builders:

```csharp
GameObject building = new BuildingBuilder("MyShop")
    .WithConfig(BuildingConfig.Medium)
    .AddFloor()
    .AddWalls(southDoor: true)
    .AddRoofTrim()
    .AddLights()
    .Build();
```

**Builder rules:**
- Configuration methods return `this` for chaining
- Place configuration methods before terminal `Build()` methods
- Provide sensible defaults

### Logging

Use `DebugLog` instead of `Debug.Log`:

```csharp
DebugLog.Info($"Built mesh: {_name} ({_vertices.Count} vertices)");
DebugLog.Warning("Attempted to register null resource");
DebugLog.Error($"Failed to load GLTF: {path}");
```

## Documentation

### XML Documentation

All public APIs require XML documentation:

```csharp
/// <summary>
/// Add a box (cube) to the mesh.
/// </summary>
/// <param name="center">Center position of the box</param>
/// <param name="size">Size of the box</param>
/// <returns>This builder for method chaining</returns>
public ProceduralMeshBuilder AddBox(Vector3 center, Vector3 size) { ... }
```

Internal members should have brief documentation:

```csharp
/// <summary>INTERNAL: Accumulated vertex data.</summary>
internal readonly List<Vector3> _vertices = new();
```

## Platform-Specific Code

Use conditional compilation for Mono/IL2CPP differences:

```csharp
#if IL2CPP
using Il2CppUnityEngine;
using Il2CppFishNet.Object;
#elif MONO
using UnityEngine;
using FishNet.Object;
#endif
```

## Networking

S1MAPI uses FishNet for multiplayer compatibility:

- Networked prefabs sync across clients
- Keep networking logic separate from mesh generation
- Use `PrefabPlacer` for networked prefab placement

```csharp
var placer = new PrefabPlacer(building.transform);
GameObject? networkedDoor = placer.Place(
    Prefabs.SlidingDoors,
    position,
    rotation,
    networked: true
);
```

## What NOT to Do

- ❌ Reference ScheduleOne types (`Assembly-CSharp.dll`)
- ❌ Use magic strings — use enums or constants
- ❌ Ignore compiler warnings
- ❌ Leave commented-out code in commits
- ❌ Use `var` when type is not obvious

```csharp
// Bad - type not obvious
var x = GetSomething();

// Good - type is clear
MeshData x = GetSomething();

// Good - type is obvious from construction
var builder = new ProceduralMeshBuilder("Test");
```

## Git Commit Standards

Follow [Conventional Commits](https://www.conventionalcommits.org/) with module scope:

```
feat(ProceduralMesh): add cylinder mesh generator
fix(Building): null reference in BuildingBuilder.Build
docs(Core): update core documentation
refactor(Gltf): extract node processing logic
chore: update dependencies
```

## Relationship to S1API

| Library | Purpose |
|---------|---------|
| **S1MAPI** | Mesh construction, building generation, GLTF loading (no game dependencies) |
| **S1API** | Game component wrappers, entity management, quests (wraps ScheduleOne types) |

Mods typically use both libraries together.

## Pull Request Guidelines

1. **Review**: Thoroughly review the codebase before submitting
2. **Test**: Build both Mono and Il2cpp configurations
3. **Document**: Update XML docs and guides as needed
4. **Scope**: Keep commits single-purpose
5. **Format**: Follow coding standards above

## Questions?

Open an issue on GitHub or contact IfBars for clarification on whether specific changes align with these standards.
