# MAPI - Schedule 1 Mapping API

**MAPI** is a mapping and construction library specifically designed for Schedule 1 mods. It provides powerful tools for creating procedural meshes, building structures, and handling GLTF assets while remaining update-resilient by avoiding direct dependencies on Schedule 1's Assembly-CSharp types.

## Installation

### Manual Installation (Recommended for Schedule 1)

1. Download the latest release from the [releases page](https://github.com/ifBars/MAPI/releases)
2. Add the appropriate `MAPI_Mono.dll` or `MAPI_Il2cpp.dll` as a reference in your mod project
3. Ensure FishNet.Runtime is available in your mod environment
4. Initialize MAPI in your mod's initialization code

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
        MAPI.Initialize();

        LoggerInstance.Msg("MAPI initialized successfully!");
    }

    public override void OnApplicationQuit()
    {
        // Cleanup MAPI
        MAPI.Shutdown();
    }
}
```

## Requirements

- **Game**: Schedule 1
- **Unity Version**: Schedule 1's Unity version
- **Target Framework**: .NET Standard 2.1
- **Scripting Backend**: Mono or IL2CPP (matching your Schedule 1 installation)
- **Networking**: FishNet.Runtime (included with Schedule 1)
- **Mod Loader**: MelonLoader or compatible Schedule 1 mod loader

## Building from Source

### Build Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/ifBars/MAPI.git
   cd MAPI
   ```

2. Copy `local.build.props.example` to `local.build.props` and configure Schedule 1 paths:
   ```bash
   cp local.build.props.example local.build.props
   ```

3. Edit `local.build.props` to point to your Schedule 1 installation:
   ```xml
   <MonoAssembliesPath>D:\SteamLibrary\steamapps\common\Schedule 1\Schedule 1_Data\Managed</MonoAssembliesPath>
   <Il2CppAssembliesPath>D:\SteamLibrary\steamapps\common\Schedule 1\Schedule 1_Data\Managed</Il2CppAssembliesPath>
   ```

4. Build the project for your target configuration:
   ```bash
   # For Mono Schedule 1 builds
   dotnet build -c Mono
   
   # For IL2CPP Schedule 1 builds  
   dotnet build -c Il2cpp
   ```

## Design Philosophy

MAPI is designed with the following principles:

1. **Update Resilience**: Avoids Schedule 1 Assembly-CSharp types to survive game updates
2. **Schedule 1 Integration**: Leverages FishNet networking and Unity primitives
3. **Performance**: Optimized for runtime mesh generation and building in multiplayer environments
4. **Maintainability**: Clean architecture with clear separation of concerns
5. **Developer Experience**: Well-documented APIs with sensible defaults

## Contributing

Contributions are welcome! Please read our [contributing guidelines](CONTRIBUTING.md) before submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Roadmap

### Phase 1: Core Infrastructure ✅
- [x] Schedule 1 specific project setup
- [x] Core initialization system
- [x] Logging utilities
- [x] Resource management (ResourceTracker)
- [x] Material presets system
- [x] Embedded resource loader
- [x] FishNet.Runtime integration for Mono and IL2CPP

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
