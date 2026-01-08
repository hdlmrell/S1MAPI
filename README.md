# MAPI - Schedule 1 Mapping API

**MAPI** is a mapping and construction library for Schedule 1 mods. Create procedural meshes, build structures, and load GLTF assets without depending on game assemblies.

[![GitHub release](https://img.shields.io/github/v/release/ifBars/MAPI?include_prereleases&sort=semver)](https://github.com/ifBars/MAPI/releases)
[![GitHub stars](https://img.shields.io/github/stars/ifBars/MAPI)](https://github.com/ifBars/MAPI/stargazers)
[![GitHub license](https://img.shields.io/github/license/ifBars/MAPI)](https://github.com/ifBars/MAPI/blob/main/LICENSE)

## What MAPI Does

- **Procedural Meshes**: Generate 3D shapes at runtime (boxes, spheres, cylinders, capsules)
- **Building Construction**: Create buildings with walls, floors, roofs, windows, and furniture
- **GLTF Loading**: Import external 3D models without external dependencies
- **Update Resilience**: Works across game updates by avoiding Assembly-CSharp types

## Quick Start

```csharp
using MAPI.ProceduralMesh;
using UnityEngine;

public class YourMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        // No initialization needed
        GameObject cube = new ProceduralMeshBuilder("MyCube")
            .AddBox(Vector3.zero, Vector3.one)
            .SetColor(Color.blue)
            .Build();

        LoggerInstance.Msg("Created a blue cube!");
    }
}
```

## Installation

### For Users
1. Download the latest release from [GitHub Releases](https://github.com/ifBars/MAPI/releases)
2. Copy `MAPI_Mono.dll` or `MAPI_Il2cpp.dll` to your mod's `Plugins` folder
3. MAPI loads automatically with MelonLoader

### For Developers
Clone the repository and build:

```bash
git clone https://github.com/ifBars/MAPI.git
cd MAPI
dotnet build -c Mono   # For Mono builds
dotnet build -c Il2cpp # For IL2CPP builds
```

## Requirements

- **Game**: Schedule 1
- **Scripting Backend**: Mono or IL2CPP
- **Networking**: FishNet.Runtime (included with Schedule 1)
- **Mod Loader**: MelonLoader 0.7.0+

## Learn More

- [Getting Started](docs/getting-started.html) - Installation and your first project
- [API Reference](api/) - Complete API documentation
- [Examples](docs/examples.html) - Code examples and patterns
- [MAPITesting Repository](https://github.com/ifBars/MAPITesting) - Full working mod example

## Relationship to S1API

MAPI and S1API are complementary:
- **MAPI**: Mesh construction, building generation, GLTF loading (no game dependencies)
- **S1API**: Game component wrappers, entity management, quests (wraps game types)

Most mods use both libraries together.

## Contributing

Contributions are welcome! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

MIT License - see [LICENSE](LICENSE) file.
