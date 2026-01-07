---
_layout: landing
---

<link rel="stylesheet" href="styles/landing.css">

<div class="mapi-hero-wrapper">
<div class="mapi-grid-bg"></div>
<div class="mapi-container">
<div class="mapi-hero-content">
<div class="mapi-brand">
<h1 class="mapi-title">MAPI<span class="mapi-dot">.</span></h1>
<div class="mapi-tagline">The Schedule 1 Mapping & Construction Framework</div>
</div>
<div class="mapi-badges">
<span class="mapi-badge"><i class="icon">📐</i> Procedural Geometry</span>
<span class="mapi-badge"><i class="icon">🏗️</i> Building System</span>
<span class="mapi-badge"><i class="icon">📦</i> GLTF Loader</span>
</div>
<div class="mapi-hero-split">
<div class="mapi-hero-text">
<p class="mapi-description">
Construct complex structures, generate procedural meshes, and import 3D models at runtime. 
Designed for stability, performance, and seamless integration.
</p>
<div class="mapi-cta-group">
<a class="mapi-btn mapi-btn-primary" href="docs/getting-started.html">
<span class="btn-text">Start Building</span>
<span class="btn-icon">→</span>
</a>
<a class="mapi-btn mapi-btn-outline" href="api/MAPI.Core.MAPI.html">API Reference</a>
</div>
</div>
<div class="mapi-code-card">
<div class="mapi-card-header">
<span class="dot red"></span>
<span class="dot yellow"></span>
<span class="dot green"></span>
<span class="filename">Construction.cs</span>
</div>
<pre><code class="lang-csharp">MAPI.Initialize();

// Fluent Construction API
var office = new BuildingBuilder()
    .WithWalls(walls => walls
        .AddRoom(width: 10f, length: 8f, height: 3f)
        .WithWindow(WallSide.Front, offset: 2f))
    .WithInterior(interior => interior
        .AddDesk(style: FurnitureStyle.Modern)
        .AddChair(position: new Vector3(2, 0, 2)))
    .Build();</code></pre>
</div>
</div>
</div>
</div>
</div>

<div class="mapi-section mapi-section-dark">
<div class="mapi-container">
<div class="mapi-section-header">
<h2>Core Capabilities</h2>
<div class="mapi-divider"></div>
</div>
<div class="mapi-grid">
<div class="mapi-tech-card">
<h3>🏗️ Building Construction</h3>
<p>A fluent, semantic API for generating buildings with walls, floors, roofs, and windows. Includes high-level builders for interiors like desks and shelves that automatically align with room geometry.</p>
</div>
<div class="mapi-tech-card">
<h3>📦 GLTF Import</h3>
<p>Native runtime GLTF/GLB loading support. Automatically handles coordinate system conversion, mesh processing, and texture mapping without external dependencies.</p>
</div>
<div class="mapi-tech-card">
<h3>🛡️ Update Resilient</h3>
<p>Engineered to survive game updates. MAPI abstracts direct Assembly-CSharp dependencies, keeping your mod functional across versions.</p>
</div>
<div class="mapi-tech-card">
<h3>⚡ Optimized Core</h3>
<p>Designed for performance with object pooling, deep cloning utilities, and efficient resource management for heavy construction tasks.</p>
</div>
</div>
</div>
</div>

<div class="mapi-footer-cta">
<div class="mapi-container">
<h2>Ready to construct?</h2>
<div class="mapi-links">
<a href="docs/introduction.html">Read the Docs</a>
<span class="separator">/</span>
<a href="docs/getting-started.html">Get Started</a>
<span class="separator">/</span>
<a href="api/MAPI.Core.MAPI.html">API Reference</a>
</div>
</div>
</div>
