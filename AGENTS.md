# Repository Guidelines

## Project Structure & Module Organization
MAPI is a mesh and building construction library for Schedule 1 mods. It avoids ScheduleOne types to remain update-resilient—using only Unity primitives and FishNet.

- `Core/` — Initialization, resource tracking, embedded resource loading
- `ProceduralMesh/` — Fluent mesh builders, organic shapes, mesh utilities
- `Building/` — Building construction, primitives, interior generation
- `Gltf/` — GLTF file loading and processing
- `Utils/` — Constants, logging, material presets, game object utilities

Namespaces mirror folder structure (e.g., `MAPI.ProceduralMesh`, `MAPI.Building`).

## Build, Test, and Development Commands
- `dotnet build MAPI.csproj -c Mono` — Mono-specific build
- `dotnet build MAPI.csproj -c Il2cpp` — IL2CPP-specific build
- `dotnet clean` — Clean build artifacts

## Coding Style & Naming Conventions
Follow `CODING_STANDARDS.md`. Key points:
- PascalCase for types/methods/properties; camelCase with `_` prefix for private/internal fields
- Fluent builder pattern: configuration methods return `this`, terminate with `Build()`
- Use nested static classes for constants (`Constants.Mesh.MaxVerticesPerMesh`)
- Register created meshes with `ResourceTracker`; use `DebugLog` for logging
- XML documentation required for all public APIs

## Relationship to S1API
MAPI and S1API serve complementary roles:
- **MAPI**: Mesh construction, building generation, GLTF loading (no ScheduleOne types)
- **S1API**: Game component wrappers, entity management, quest systems (wraps ScheduleOne types)

Mods typically use both: MAPI for construction, S1API for game integration. MAPI should never depend on S1API or ScheduleOne types.

## Testing Guidelines
- No automated test suite yet; treat multiplatform builds as acceptance gate
- Build both `Mono` and `Il2cpp` configurations before submitting PRs
- Test mesh generation and building construction in-game across both runtimes
- Use `MAPI.Utils.DebugLog` with clear module context for all logging

## Commit & Pull Request Guidelines
- Conventional Commits with module scope: `feat(ProceduralMesh): add sphere generator`
- Keep commits single-purpose and imperative
- PRs: Include description, testing steps, and screenshots for visual changes
- Never commit `local.build.props` or secrets
