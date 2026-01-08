# Procedural Mesh Guide

Learn how to generate meshes at runtime using MAPI's fluent API.

## Creating Basic Shapes

The `ProceduralMeshBuilder` class provides methods for creating common geometric shapes:

### Box

```csharp
// Simple box at origin, 1 meter cube
GameObject cube = new ProceduralMeshBuilder("Cube")
    .AddBox(Vector3.zero, Vector3.one)
    .Build();

// Box at custom position and size
GameObject box = new ProceduralMeshBuilder("MyBox")
    .AddBox(new Vector3(5, 2, 3), new Vector3(2, 1, 3))
    .Build();
```

### Sphere

```csharp
// Low-poly sphere (few subdivisions = blocky)
GameObject lowPolySphere = new ProceduralMeshBuilder("LowPoly")
    .AddSphere(Vector3.zero, 0.5f, subdivisions: 4)
    .Build();

// Smooth sphere (default subdivisions = 12)
GameObject smoothSphere = new ProceduralMeshBuilder("Smooth")
    .AddSphere(new Vector3(0, 1, 0), 1.0f)
    .Build();
```

### Cylinder

```csharp
// Vertical cylinder from point A to B
Vector3 bottom = new Vector3(0, 0, 0);
Vector3 top = new Vector3(0, 3, 0);

GameObject cylinder = new ProceduralMeshBuilder("Pillar")
    .AddCylinder(bottom, top, radius: 0.2f, segments: 8)
    .Build();
```

### Capsule

```csharp
// Capsule = cylinder + two hemispheres
Vector3 bottom = new Vector3(0, 0, 0);
Vector3 top = new Vector3(0, 2, 0);

GameObject capsule = new ProceduralMeshBuilder("Capsule")
    .AddCapsule(bottom, top, radius: 0.3f)
    .Build();
```

## Combining Shapes

Chain multiple shape methods to create complex geometries:

```csharp
GameObject arrow = new ProceduralMeshBuilder("Arrow")
    .AddCylinder(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0.1f, 8)     // Shaft
    .AddCone(new Vector3(0, 1, 0), new Vector3(0, 2, 0), 0.3f, 8)        // Tip
    .SetColor(Color.red)
    .Build();
```

## Applying Materials and Colors

### Set a Specific Material

```csharp
Material metalMaterial = MaterialPresets.Metal(Color.gray, metallic: 0.9f);

GameObject metalBox = new ProceduralMeshBuilder("MetalBox")
    .AddBox(Vector3.zero, Vector3.one)
    .SetMaterial(metalMaterial)
    .Build();
```

### Set a Solid Color

```csharp
GameObject redBox = new ProceduralMeshBuilder("RedBox")
    .AddBox(Vector3.zero, Vector3.one)
    .SetColor(Color.red)
    .Build();

// Custom color
Color purple = new Color(0.5f, 0, 0.5f);
GameObject purpleSphere = new ProceduralMeshBuilder("PurpleSphere")
    .AddSphere(Vector3.zero, 0.5f)
    .SetColor(purple)
    .Build();
```

### Flat Shading

Apply flat shading for a low-poly aesthetic:

```csharp
GameObject lowPolyTree = new ProceduralMeshBuilder("LowPolyTree")
    .AddCylinder(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0.2f, 6)    // Trunk
    .AddSphere(new Vector3(0, 1.5f, 0), 1.0f, subdivisions: 4)           // Foliage
    .SetColor(new Color(0.3f, 0.5f, 0.2f))
    .ApplyFlatShading()
    .Build();
```

## Building Without a GameObject

If you only need the `Mesh` object (not a GameObject):

```csharp
Mesh mesh = new ProceduralMeshBuilder("MyMesh")
    .AddBox(Vector3.zero, Vector3.one)
    .AddSphere(new Vector3(2, 0, 0), 0.5f)
    .BuildMesh();

// Use the mesh manually
MeshFilter filter = gameObject.AddComponent<MeshFilter>();
filter.mesh = mesh;

MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
renderer.material = MaterialPresets.Opaque(Color.blue);
```

## Example: Complete Object

```csharp
using MAPI.ProceduralMesh;
using MAPI.Utils;
using UnityEngine;

public static class ProceduralObjects
{
    public static GameObject CreateCrate(Vector3 position, Color woodColor)
    {
        return new ProceduralMeshBuilder("WoodenCrate")
            .AddBox(Vector3.zero, new Vector3(1, 1, 1))              // Main body
            .AddBox(new Vector3(0, 0.51f, 0), new Vector3(1.05f, 0.05f, 1.05f))  // Lid rim
            .SetColor(woodColor)
            .ApplyFlatShading()
            .Build();
    }

    public static GameObject CreateBeacon(Vector3 position)
    {
        // Metal pole
        GameObject beacon = new ProceduralMeshBuilder("Beacon")
            .AddCylinder(position, position + Vector3.up * 3, 0.1f, 8)
            .SetMaterial(MaterialPresets.Metal(Color.gray))
            .Build();

        // Glowing light at top
        GameObject light = new ProceduralMeshBuilder("BeaconLight")
            .AddSphere(position + Vector3.up * 3.2f, 0.2f, 6)
            .SetMaterial(MaterialPresets.Emissive(Color.yellow, intensity: 3f))
            .Build();

        light.transform.SetParent(beacon.transform);

        return beacon;
    }
}
```

## Performance Tips

1. **Combine shapes**: Adding multiple shapes to one builder is more efficient than creating separate meshes.

2. **Use flat shading wisely**: Flat shading requires recalculating normals. Apply it only when needed.

3. **Consider vertex limits**: Unity's Mesh uses 16-bit indices by default (65,535 vertices). For larger meshes, set `mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32`.

4. **Register resources**: MAPI automatically tracks created meshes, but for very large numbers, consider manual cleanup.

## Next Steps

- [Building Guide](building.html) - Create structured buildings
- [GLTF Loading](gltf-loading.html) - Import external models
- [API Reference](api/MAPI.ProceduralMesh.ProceduralMeshBuilder.html) - Full API docs
