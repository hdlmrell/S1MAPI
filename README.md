# S1MAPI - Schedule 1 Mapping API

**S1MAPI** is a mapping and construction library for Schedule 1 mods. Create procedural meshes, build structures, and load GLTF assets without asset bundles.

[![GitHub release](https://img.shields.io/github/v/release/ifBars/S1MAPI?include_prereleases&sort=semver)](https://github.com/ifBars/S1MAPI/releases)
[![GitHub stars](https://img.shields.io/github/stars/ifBars/S1MAPI)](https://github.com/ifBars/S1MAPI/stargazers)
[![GitHub license](https://img.shields.io/github/license/ifBars/S1MAPI)](https://github.com/ifBars/S1MAPI/blob/main/LICENSE)

## What S1MAPI Does

- **Procedural Meshes**: Generate 3D shapes at runtime (boxes, spheres, cylinders, capsules)
- **Building Construction**: Create buildings with walls, floors, roofs, windows, and furniture
- **GLTF Loading**: Import external 3D models without external dependencies
- **Update Resilience**: Works across game updates by avoiding Assembly-CSharp types

## Installation

### For Users
1. Download the latest release from [GitHub Releases](https://github.com/ifBars/S1MAPI/releases)
2. Extract the ZIP and copy the contents to your Schedule 1 game directory
3. The `UserLibs` folder from the release merges with your existing `UserLibs`
4. S1MAPI loads automatically with MelonLoader

### For Developers
Clone the repository and build:

```bash
git clone https://github.com/ifBars/S1MAPI.git
cd S1MAPI
dotnet build -c Mono   # For Mono builds
dotnet build -c Il2cpp # For IL2CPP builds
```

## Requirements

- **Game**: Schedule 1
- **Mod Loader**: MelonLoader 0.7.0+

## Learn More

- [Getting Started](docs/getting-started.md) - Installation and your first project
- [Examples](docs/examples.md) - Code examples and patterns
- [MAPITesting Repository](https://github.com/ifBars/MAPITesting) - Full working mod example

## Relationship to S1API

S1MAPI and S1API are complementary:
- **S1MAPI**: Mesh construction, building generation, GLTF loading (no game dependencies)
- **S1API**: Game component wrappers, entity management, quests (wraps game types)

Most mods use both libraries together.

## Contributing

Contributions are welcome! See [Contributing Guide](docs/contributing.md) for guidelines.

## License

GNU GPL v3 License - see [LICENSE](LICENSE) file.
