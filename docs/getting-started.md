# Getting Started

This guide walks you through installing MAPI and creating your first procedural mesh.

## Installation

### For Users (Installing Mods)

1. **Install MelonLoader**
   - Download from [melonwiki.xyz](https://melonwiki.xyz/#/README)
   - Install for Schedule 1
   - Verify installation by launching the game

2. **Install MAPI**
   - Download the latest release from [GitHub Releases](https://github.com/ifBars/MAPI/releases)
   - Extract the ZIP file
   - Copy the contents to your Schedule 1 game directory
   - The `Plugins` folder from the release merges with your existing `Plugins`

3. **Install Mods Requiring MAPI**
   - Place mod DLLs in the `Plugins` folder as usual
   - MAPI loads automatically before your mods

### For Developers

#### Option 1: Template Project (Recommended)

Use the [MAPITemplate repository](https://github.com/ifBars/MAPITemplate) for a ready-to-go project structure.

#### Option 2: Manual Setup

1. **Create a new MelonLoader mod project**

2. **Add MAPI reference**
   - Download the appropriate MAPI DLL from [releases](https://github.com/ifBars/MAPI/releases)
   - Add `MAPI_Mono.dll` or `MAPI_Il2cpp.dll` as a project reference
   - Set "Copy Local" to `false` (MAPI loads separately)

3. **Configure local.build.props**
   Copy `local.build.props.example` to `local.build.props`:
   ```xml
   <MonoAssembliesPath>D:\SteamLibrary\steamapps\common\Schedule 1\Schedule 1_Data\Managed</MonoAssembliesPath>
   <Il2CppAssembliesPath>D:\SteamLibrary\steamapps\common\Schedule 1\Schedule 1_Data\Managed</Il2CppAssembliesPath>
   ```

4. **Build your mod**
   ```bash
   dotnet build -c Mono   # For Mono builds
   dotnet build -c Il2cpp # For IL2CPP builds
   ```

## Your First Mesh

Create a simple colored cube in your mod's `OnInitializeMelon`:

```csharp
using MAPI.ProceduralMesh;
using UnityEngine;

public class YourMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        // Create a red box at position (0, 1, 0)
        GameObject cube = new ProceduralMeshBuilder("MyRedCube")
            .AddBox(new Vector3(0, 1, 0), Vector3.one)
            .SetColor(Color.red)
            .Build();

        LoggerInstance.Msg("Created a red cube!");
    }
}
```

## Creating More Shapes

Combine multiple shapes into a single mesh:

```csharp
GameObject snowman = new ProceduralMeshBuilder("Snowman")
    .AddSphere(new Vector3(0, 0.5f, 0), 0.5f)   // Body
    .AddSphere(new Vector3(0, 1.25f, 0), 0.35f) // Head
    .AddSphere(new Vector3(0, 1.8f, 0), 0.15f)  // Hat top
    .SetColor(Color.white)
    .ApplyFlatShading()
    .Build();
```

## Creating a Building

Use `BuildingBuilder` for structured constructions:

```csharp
using MAPI.Building;
using MAPI.Building.Config;

GameObject shop = new BuildingBuilder("MyDispensary")
    .WithConfig(BuildingConfig.Medium)
    .AddFloor()
    .AddCeiling()
    .AddWalls(southDoor: true, eastWindow: true)
    .AddRoofTrim()
    .AddLights()
    .Build();

// Position the building
shop.transform.position = new Vector3(100, 0, 50);
shop.transform.rotation = Quaternion.Euler(0, 90, 0);
```

## Loading GLTF Models

Load external 3D models embedded in your mod:

```csharp
using MAPI.Gltf;

// Load from embedded resource
byte[]? glbData = File.ReadAllBytes("path/to/model.glb");
GameObject? model = GltfLoader.LoadGlb(glbData);

if (model != null)
{
    model.transform.position = new Vector3(0, 0, 0);
    model.transform.localScale = Vector3.one * 0.5f;
}
```

## Using Materials

Create materials with different visual properties:

```csharp
using MAPI.Utils;

// Opaque red material
Material redOpaque = MaterialPresets.Opaque(Color.red);

// Transparent blue glass
Material blueGlass = MaterialPresets.Glass(Color.blue, alpha: 0.3f);

// Metallic silver
Material silverMetal = MaterialPresets.Metal(Color.gray, metallic: 0.9f);

// Glowing green
Material glowingGreen = MaterialPresets.Emissive(Color.green, intensity: 2.0f);

// Apply to mesh
GameObject cube = new ProceduralMeshBuilder("GlowingCube")
    .AddBox(Vector3.zero, Vector3.one)
    .SetMaterial(glowingGreen)
    .Build();
```

## Next Steps

- [Procedural Mesh Guide](procedural-mesh.html) - Deep dive into mesh generation
- [Building Guide](building.html) - Create complete buildings
- [GLTF Loading](gltf-loading.html) - Import external models
- [Examples](examples.html) - Complete code examples
- Explore the [API Reference](api/) for detailed documentation

## Complete Example: dispensary Building

See the [MAPITesting repository](https://github.com/ifBars/MAPITesting) for a full working mod that demonstrates:
- Building construction with `BuildingBuilder`
- Interior decoration with `InteriorBuilder`
- GLTF model loading
- Integration with S1API for game entity placement
