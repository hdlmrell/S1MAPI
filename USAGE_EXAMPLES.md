# MAPI Usage Examples

This document provides practical examples of using MAPI in your Schedule 1 mods. All examples assume you're working with FishNet networking and the Schedule 1 environment.

## Table of Contents
- [Creating Simple Meshes](#creating-simple-meshes)
- [Working with Materials](#working-with-materials)
- [Loading Embedded Resources](#loading-embedded-resources)
- [Resource Management](#resource-management)

---

## No Initialization Required!

MAPI requires **no initialization** - all APIs work immediately. Unity automatically handles resource cleanup on scene unload and application quit. Just start creating!

```csharp
using MAPI.ProceduralMesh;
using UnityEngine;

public class YourSchedule1Mod : MelonMod
{
    public override void OnInitializeMelon()
    {
        // No initialization needed - just use MAPI!
        GameObject cube = new ProceduralMeshBuilder("MyCube")
            .AddBox(Vector3.zero, Vector3.one)
            .SetColor(Color.green)
            .Build();
        
        LoggerInstance.Msg("Created a green cube!");
    }
}
```

---

## Creating Simple Meshes

### Create a Colored Box

```csharp
using MAPI.ProceduralMesh;
using UnityEngine;

// Create a red box at the origin
GameObject box = new ProceduralMeshBuilder("MyBox")
    .AddBox(Vector3.zero, new Vector3(1f, 1f, 1f))
    .SetColor(Color.red)
    .Build();
```

### Create a Low-Poly Sphere

```csharp
// Create a blue low-poly sphere with flat shading
GameObject sphere = new ProceduralMeshBuilder("MySphere")
    .AddSphere(Vector3.zero, 0.5f, subdivisions: 4)
    .SetColor(Color.blue)
    .ApplyFlatShading()
    .Build();
```

### Create a Cylinder

```csharp
// Create a green cylinder from point A to point B
Vector3 start = new Vector3(0, 0, 0);
Vector3 end = new Vector3(0, 2, 0);

GameObject cylinder = new ProceduralMeshBuilder("MyCylinder")
    .AddCylinder(start, end, radius: 0.3f, segments: 12)
    .SetColor(Color.green)
    .Build();
```

### Combine Multiple Shapes

```csharp
// Create a complex object from multiple primitives
GameObject complex = new ProceduralMeshBuilder("ComplexObject")
    .AddBox(Vector3.zero, new Vector3(1f, 0.2f, 1f))  // Base
    .AddCylinder(
        new Vector3(0, 0.1f, 0), 
        new Vector3(0, 2f, 0), 
        0.1f, 
        12
    )  // Pole
    .AddSphere(new Vector3(0, 2f, 0), 0.3f, 6)  // Top
    .SetColor(new Color(0.8f, 0.6f, 0.4f))  // Brown
    .ApplyFlatShading()
    .Build();
```

---

## Working with Materials

### Create Materials with Presets

```csharp
using MAPI.Utils;
using UnityEngine;

// Opaque material
Material opaque = MaterialPresets.Opaque(Color.red);

// Transparent material
Material transparent = MaterialPresets.Transparent(Color.blue, alpha: 0.5f);

// Glass material
Material glass = MaterialPresets.Glass(Color.cyan, alpha: 0.3f);

// Metallic material
Material metal = MaterialPresets.Metal(Color.gray, metallic: 0.9f, smoothness: 0.95f);

// Emissive (glowing) material
Material emissive = MaterialPresets.Emissive(Color.yellow, intensity: 2.0f);
```

### Apply Material to Mesh

```csharp
// Create mesh with custom material
Material customMaterial = MaterialPresets.Metal(Color.silver);

GameObject metalBox = MAPI.CreateMesh("MetalBox")
    .AddBox(Vector3.zero, Vector3.one)
    .SetMaterial(customMaterial)
    .Build();
```

---

## Loading Embedded Resources

### Load Texture from Assembly

```csharp
using MAPI.Core;
using UnityEngine;

// Load an embedded texture
Texture2D? texture = MAPI.LoadEmbeddedTexture("YourNamespace.Resources.myTexture.png");

if (texture != null)
{
    // Use the texture
    Material mat = new Material(Shader.Find("Standard"));
    mat.mainTexture = texture;
}
```

### Load Sprite for UI

```csharp
// Load a sprite for UI
Sprite? sprite = MAPI.LoadEmbeddedSprite(
    "YourNamespace.Resources.myIcon.png",
    pixelsPerUnit: 100f
);

if (sprite != null)
{
    // Use in UI Image component
    imageComponent.sprite = sprite;
}
```

### Load Raw Bytes

```csharp
// Load raw binary data
byte[]? data = MAPI.LoadEmbeddedBytes("YourNamespace.Resources.data.bin");

if (data != null)
{
    // Process the data
    Debug.Log($"Loaded {data.Length} bytes");
}
```

---

## Resource Management

### Automatic Resource Tracking

MAPI automatically tracks resources created through its APIs:

```csharp
// These are automatically registered for cleanup
GameObject mesh1 = MAPI.CreateMesh("Mesh1")
    .AddBox(Vector3.zero, Vector3.one)
    .Build();

Texture2D? texture = MAPI.LoadEmbeddedTexture("YourNamespace.texture.png");
```

### Manual Resource Registration

For resources created outside MAPI:

```csharp
using MAPI.Core;
using UnityEngine;

// Create a material manually
Material myMaterial = new Material(Shader.Find("Standard"));
myMaterial.color = Color.red;

// Register it for automatic cleanup
MAPI.RegisterResource(myMaterial);
```

### Manual Cleanup

```csharp
using MAPI.Core;

// Clean up all tracked resources immediately
ResourceTracker.CleanupAll();

// Or cleanup specific scene resources
ResourceTracker.CleanupScene(SceneManager.GetActiveScene());
```

---

## Utility Functions

### Snap to Grid

```csharp
Vector3 position = new Vector3(1.23f, 4.56f, 7.89f);
Vector3 snapped = MAPI.SnapToGrid(position, gridSize: 0.5f);
// Result: (1.0, 4.5, 8.0)
```

### Create Quick Material

```csharp
// Quick opaque material
Material mat1 = MAPI.CreateMaterial(Color.red);

// Quick transparent material
Material mat2 = MAPI.CreateMaterial(Color.blue, transparent: true, alpha: 0.7f);
```

---

## Complete Example: Creating a Crate

```csharp
using MAPI.Core;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    private void Start()
    {
        // Ensure MAPI is initialized
        if (!MAPI.IsInitialized)
        {
            MAPI.Initialize();
        }

        // Create a wooden crate
        Color woodColor = new Color(0.6f, 0.4f, 0.2f);
        
        GameObject crate = MAPI.CreateMesh("WoodenCrate")
            .AddBox(Vector3.zero, new Vector3(1f, 1f, 1f))
            .SetColor(woodColor)
            .ApplyFlatShading()
            .Build();

        // Position it
        crate.transform.position = MAPI.SnapToGrid(
            new Vector3(5.2f, 0.5f, 3.7f),
            gridSize: 0.5f
        );

        // Add physics
        crate.AddComponent<Rigidbody>();
        crate.AddComponent<BoxCollider>();

        Debug.Log($"Created crate: {crate.name}");
    }
}
```

---

## Complete Example: Glowing Beacon

```csharp
using MAPI.Core;
using MAPI.Utils;
using UnityEngine;

public class BeaconCreator
{
    public static GameObject CreateBeacon(Vector3 position)
    {
        // Create the base
        GameObject beacon = MAPI.CreateMesh("Beacon")
            .AddCylinder(
                position,
                position + Vector3.up * 0.5f,
                0.3f,
                8
            )
            .SetMaterial(MaterialPresets.Metal(Color.gray))
            .Build();

        // Create glowing top
        GameObject light = MAPI.CreateMesh("BeaconLight")
            .AddSphere(position + Vector3.up * 0.75f, 0.2f, 6)
            .SetMaterial(MaterialPresets.Emissive(Color.yellow, intensity: 3f))
            .Build();

        // Parent the light to the beacon
        light.transform.SetParent(beacon.transform);

        // Add actual light component
        Light lightComponent = light.AddComponent<Light>();
        lightComponent.color = Color.yellow;
        lightComponent.intensity = 5f;
        lightComponent.range = 10f;

        return beacon;
    }
}
```

---

---

## Complete Example: Organic Dog Character

```csharp
using MAPI.Core;
using MAPI.ProceduralMesh;
using UnityEngine;

public class DogCreator
{
    public static GameObject CreateProceduralDog(Vector3 position)
    {
        Color dogColor = new Color(0.8f, 0.6f, 0.4f); // Brown
        
        // Create the dog body
        GameObject dog = MAPI.CreateMesh("ProceduralDog")
            .AddSegmentedBody(
                new Vector3(0.4f, 0.5f, 1f),
                BodyProfile.DefaultDogBody
            )
            // Front legs
            .AddArticulatedLimb(
                LimbProfile.DefaultDogLeg(0.6f, 0.15f),
                new Vector3(-0.15f, -0.25f, 0.3f)
            )
            .AddPaw(0.1f, 0.15f, 0.12f, new Vector3(-0.15f, -0.55f, 0.3f))
            .AddArticulatedLimb(
                LimbProfile.DefaultDogLeg(0.6f, 0.15f),
                new Vector3(0.15f, -0.25f, 0.3f)
            )
            .AddPaw(0.1f, 0.15f, 0.12f, new Vector3(0.15f, -0.55f, 0.3f))
            // Back legs
            .AddArticulatedLimb(
                LimbProfile.DefaultDogLeg(0.6f, 0.15f),
                new Vector3(-0.15f, -0.25f, -0.3f)
            )
            .AddPaw(0.1f, 0.15f, 0.12f, new Vector3(-0.15f, -0.55f, -0.3f))
            .AddArticulatedLimb(
                LimbProfile.DefaultDogLeg(0.6f, 0.15f),
                new Vector3(0.15f, -0.25f, -0.3f)
            )
            .AddPaw(0.1f, 0.15f, 0.12f, new Vector3(0.15f, -0.55f, -0.3f))
            // Ears
            .AddEar(0.1f, new Vector3(-0.12f, 0.35f, 0.45f))
            .AddEar(0.1f, new Vector3(0.12f, 0.35f, 0.45f))
            // Head
            .AddSphere(new Vector3(0f, 0.2f, 0.55f), 0.2f, 8)
            .SetColor(dogColor)
            .ApplyFlatShading()
            .Build();
        
        dog.transform.position = position;
        return dog;
    }
}
```

---

## Complete Example: Pet Shop Building

```csharp
using MAPI.Core;
using MAPI.Building;
using UnityEngine;

public class PetShopBuilder
{
    public static GameObject CreatePetShop(Vector3 position)
    {
        // Define colors
        Color woodColor = new Color(0.5f, 0.35f, 0.25f);
        Color glassColor = new Color(0.7f, 0.85f, 0.9f);
        
        // Create the interior first
        InteriorBuilder interior = MAPI.CreateInterior("ShopInterior")
            // Front desk
            .AddDesk(
                new Vector3(0f, 0.45f, -3f),
                new Vector3(2f, 0.9f, 0.6f)
            )
            // Display shelves on left wall
            .AddWallShelves(
                new Vector3(-2.75f, 0.65f, 0f),
                tiers: 3,
                shelfSize: new Vector3(0.45f, 0.08f, 0.8f),
                spacing: 0.35f
            )
            // Display shelves on right wall
            .AddWallShelves(
                new Vector3(2.75f, 0.65f, 0f),
                tiers: 3,
                shelfSize: new Vector3(0.45f, 0.08f, 0.8f),
                spacing: 0.35f
            )
            // Seating area
            .AddTable(
                new Vector3(2f, 0f, 2f),
                new Vector3(0.8f, 0.7f, 0.8f)
            )
            .AddChair(new Vector3(2f, 0f, 2.6f))
            .AddChair(new Vector3(2f, 0f, 1.4f))
            // Floor rug
            .AddRug(
                new Vector3(0f, 0.01f, 0f),
                new Vector2(4f, 6f),
                new Color(0.6f, 0.3f, 0.2f)
            );
        
        // Build the main structure
        GameObject shop = MAPI.CreateBuilding("PetShop")
            .SetFootprint(6f, 9f)
            .SetHeight(6f)
            .SetWallColor(new Color(0.85f, 0.85f, 0.82f))
            .SetTrimColor(new Color(0.3f, 0.25f, 0.2f))
            .SetRoofColor(new Color(0.4f, 0.35f, 0.3f))
            .AddFloor()
            .AddWalls()
            .AddRoof()
            .AddBaseMolding()
            .AddWindows(count: 4, windowHeight: 5f, glassColor: glassColor)
            .Build(position, Quaternion.identity);
        
        // Add the interior
        interior.Build(shop);
        
        return shop;
    }
}
```

---

## Next Steps

- Explore the [API Documentation](API.md) for complete reference
- Check out the [Roadmap](README.md#roadmap) for upcoming features
- Join the community to share your creations!
