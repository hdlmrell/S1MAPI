# 🏗️ MAPI - Schedule One Modding API

**A comprehensive building framework and modding API for Schedule One - create custom buildings, interiors, and game modifications**

[Documentation](https://ifbars.github.io/MAPI/) • [GitHub Repository](https://github.com/ifBars/MAPI)

---

## ✨ What is MAPI?

MAPI is a comprehensive building framework and modding API designed specifically for Schedule One. It provides a robust foundation for creating custom buildings, interiors, and game modifications with an easy-to-use, component-based architecture.

Perfect for modders who want to add custom structures and gameplay features without the hassle of asset bundles!

---

## 🎯 Who Needs This?

- **Players**: Only if you're using mods that declare MAPI as a requirement
- **Mod Developers**: If you want a robust, component-based building framework and modding API for Schedule One

---

## 🚀 Key Features

- **🏠 Building System**: Complete building framework with support for walls, furniture, lighting, and structural elements
- **📦 Component Architecture**: Modular component system for reusable building pieces and custom functionality
- **🎨 Procedural Meshes**: Generate custom meshes programmatically with support for primitives and advanced mesh operations
- **📁 GLTF Importing**: Built-in GLTF model loading and processing for importing custom 3D assets
- **🔧 Extension Methods**: Comprehensive extension methods for Unity components, transforms, materials, and more
- **📋 Configuration System**: Flexible configuration system with palette support for building presets
- **🎯 Game Integration**: Seamless integration with Schedule One's materials, meshes, and prefabs

---

## 💡 Why Use MAPI?

- **Structured Development**: Organized architecture following modern C# and Unity patterns
- **Component-Based Design**: Reusable, composable components for building and game features
- **Comprehensive Utilities**: Extensive helper methods and utilities for common modding tasks
- **Documentation**: Full API documentation and getting started guides available
- **Cross-Platform**: Supports both Mono and Il2Cpp build configurations

---

## 📦 Installation

### For Players

If a mod you want to use requires MAPI:

1. Download the MAPI zip from Thunderstore
2. Extract the zip - it contains both `MAPI-Mono.dll` and `MAPI-Il2Cpp.dll`
3. Copy the correct DLL for your Steam branch to your game's `UserLibs` folder:
   - **Regular Steam branch (default)**: Use `MAPI-Il2Cpp.dll`
   - **Alternate Steam branch**: Use `MAPI-Mono.dll`
4. Launch the game - MAPI will load automatically with MelonLoader

**To check your Steam branch:** Right-click Schedule One in Steam → Properties → Betas. If it shows "None" or no selection, you're on the regular branch (use Il2Cpp).

### For Mod Developers

To use MAPI in your mod project:

1. Add the **Mono** MAPI DLL as a reference to your mod project (via NuGet or direct DLL reference)
2. Build your mod against the Mono DLL - it will work on both runtimes
3. Users will need to install MAPI separately as a dependency with the correct DLL for their Steam branch

---

## 🔨 Building from Source

For developers who want to build from source:

1. Clone the repository
2. Copy `local.build.props.example` to `local.build.props`
3. Edit `local.build.props` and set your game installation path
4. Build with `dotnet build -c Mono` or `dotnet build -c Il2cpp`

---

## ⚙️ Requirements

- .NET Framework 4.7.2+ or .NET 6.0+
- MelonLoader (for mod development)
- Schedule One game installation

---

## 📚 Documentation

Complete documentation, API reference, and getting started guides are available at:

**[📖 MAPI Documentation](https://ifbars.github.io/MAPI/)**

The documentation includes:

- **Getting Started**: Setup guide and your first MAPI project
- **API Reference**: Complete reference for all classes, methods, and components
- **Building Guide**: Creating custom buildings, interiors, and structures
- **Code Examples**: Real-world usage patterns and samples

---

## 📄 License

MIT License - [View License](https://github.com/ifBars/MAPI/blob/master/LICENSE)

---

## 🤝 Contributing

We welcome contributions! See [CONTRIBUTING.md](https://github.com/ifBars/MAPI/blob/master/CONTRIBUTING.md) for guidelines.

---

## 🔒 Security

If you discover a security vulnerability, please see [SECURITY.md](https://github.com/ifBars/MAPI/blob/master/SECURITY.md) for reporting guidelines.

---

## ⚠️ Disclaimer

This modding API is provided "as-is" under the MIT License. The author is not responsible for any misuse, damages, or issues arising from its use. Users are responsible for complying with all applicable terms of service and licenses.

---

[Documentation](https://ifbars.github.io/MAPI/) • [GitHub Repository](https://github.com/ifBars/MAPI)
