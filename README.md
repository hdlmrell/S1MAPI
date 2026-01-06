# MAPI - Modding API for Unity Games

**MAPI** is a universal, game-agnostic Unity modding library that provides powerful tools for mod developers working with Unity-based games. It offers procedural mesh generation, building construction systems, and advanced modding utilities without requiring game-specific dependencies.

## Features

- **Game-Agnostic Design**: Works across different Unity games and modding frameworks
- **Procedural Mesh Generation**: Create complex 3D meshes at runtime
- **Building Construction System**: Build and manage structures dynamically
- **Mono & IL2CPP Support**: Compatible with both Unity scripting backends
- **MelonLoader Integration**: Designed to work seamlessly with MelonLoader
- **Zero Game Dependencies**: Uses only standard Unity and .NET APIs

## Installation

### As a NuGet Package (Recommended)

```bash
dotnet add package MAPI
```

### Manual Installation

1. Download the latest release from the [releases page](https://github.com/yourusername/MAPI/releases)
2. Add `MAPI.dll` as a reference in your mod project
3. Initialize MAPI in your mod's initialization code

## Quick Start

```csharp
using MAPI.Core;
using MAPI.ProceduralMesh;
using MAPI.Building;

public class YourMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        // Initialize MAPI
        MAPICore.Initialize();
        
        LoggerInstance.Msg("MAPI initialized successfully!");
    }

    public override void OnApplicationQuit()
    {
        // Cleanup MAPI
        MAPICore.Shutdown();
    }
}
```

## Requirements

- **Unity Version**: 2019.4+ (2020.1+ recommended)
- **Target Framework**: .NET Standard 2.1
- **Scripting Backend**: Mono or IL2CPP
- **Mod Loader**: MelonLoader (optional, but recommended)

## Building from Source

### Prerequisites

1. .NET 6.0 SDK
2. Unity assemblies from your target game
3. (Optional) MelonLoader assemblies

### Build Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/MAPI.git
   cd MAPI
   ```

2. Copy `local.build.props.example` to `local.build.props` and configure paths:
   ```bash
   cp local.build.props.example local.build.props
   ```

3. Edit `local.build.props` to point to your Unity assemblies:
   ```xml
   <MonoAssembliesPath>D:\SteamLibrary\steamapps\common\YourGame\YourGame_Data\Managed</MonoAssembliesPath>
   ```

4. Build the project:
   ```bash
   dotnet build -c Universal
   ```

### Build Configurations

- **Universal**: Default configuration for game-agnostic use (recommended)
- **Mono**: For Mono Unity builds
- **Il2cpp**: For IL2CPP Unity builds

## Project Structure

```
MAPI/
├── Core/              # Core initialization and library management
├── ProceduralMesh/    # Procedural mesh generation system
├── Building/          # Building construction system
├── Utils/             # Utility classes and helpers
└── Resources/         # Embedded resources
```

## Documentation

Comprehensive documentation is coming soon. For now, refer to:

- XML documentation in the source code
- Example mods in the `examples/` directory (coming soon)
- API reference (coming soon)

## Design Philosophy

MAPI is designed with the following principles:

1. **Universal Compatibility**: No game-specific dependencies
2. **Extensibility**: Easy to extend with game-specific adapters
3. **Performance**: Optimized for runtime mesh generation and building
4. **Maintainability**: Clean architecture with clear separation of concerns
5. **Developer Experience**: Well-documented APIs with sensible defaults

## Contributing

Contributions are welcome! Please read our [contributing guidelines](CONTRIBUTING.md) before submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Unity Technologies for the Unity Engine
- The MelonLoader team for their excellent mod loader
- The modding community for inspiration and feedback

## Support

- **Issues**: [GitHub Issues](https://github.com/yourusername/MAPI/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/MAPI/discussions)
- **Discord**: Coming soon

## Roadmap

### Phase 1: Core Infrastructure ✅
- [x] Project setup
- [x] Core initialization system
- [x] Logging utilities
- [x] Resource management (ResourceTracker)
- [x] Material presets system
- [x] Embedded resource loader

### Phase 2: Procedural Mesh Generation ✅
- [x] Basic mesh primitives (Box, Sphere, Cylinder, Capsule)
- [x] ProceduralMeshBuilder fluent API
- [x] Mesh utilities (flat shading, normal calculation, merging)
- [x] Organic shape builders (segmented bodies, articulated limbs)
- [x] Body and limb profile system
- [x] Specialized organic shapes (paws, ears)
- [ ] Advanced mesh operations (extrusion, beveling)
- [ ] GLTF import support

### Phase 3: Building Construction ✅
- [x] BuildingBuilder system
- [x] Wall/floor/roof generators
- [x] Window and molding systems
- [x] InteriorBuilder for furnishings
- [x] Furniture (desks, chairs, tables, shelves)
- [x] Decorations (rugs, boxes, spheres, cylinders)
- [x] Collision and physics integration
- [x] NavMesh integration helpers
- [x] PrimitiveBuilder for Unity primitive manipulation
- [x] BuildingUtilities (layers, occlusion, grid snapping)

### Phase 4: Advanced Features ✅
- [x] GLTF/GLB Loading System
  - [x] Binary GLTF parsing (no external dependencies required if Newtonsoft present)
  - [x] Mesh processing (Positions, Normals, UVs, Tangents)
  - [x] Node hierarchy reconstruction
  - [x] Coordinate system conversion (GLTF Right-handed -> Unity Left-handed)
- [x] Object Cloning Utility
  - [x] Deep cloning with component filtering
  - [x] Material replacement
  - [x] Collider stripping
- [x] Advanced Mesh Operations
  - [x] Vertex welding (optimization)
  - [x] Surface area and volume calculation
- [ ] Animation support (Skinned Mesh basics implemented)
- [ ] Texture loading utilities (partially via GLTF)

### Phase 5: Integration & Optimization
- [ ] Performance profiling tools
- [ ] Debug visualization
- [ ] Comprehensive examples
- [ ] API documentation

### Phase 6: Documentation & Distribution
- [ ] Full API documentation
- [ ] Tutorial series
- [ ] NuGet package publishing
- [ ] Example mods showcase

---

**Note**: MAPI is currently in early development. APIs may change between versions until v1.0.0 is released.
