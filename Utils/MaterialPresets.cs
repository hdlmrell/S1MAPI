using UnityEngine;

namespace S1MAPI.Utils
{
    /// <summary>
    /// Standardized material creation for common use cases.
    /// Provides consistent visual styling across S1MAPI-generated objects.
    /// </summary>
    public static class MaterialPresets
    {
        #region Internal Members

        /// <summary>
        /// INTERNAL: Cached default shader reference.
        /// </summary>
        internal static Shader? _defaultShader;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets the default shader used for material creation.
        /// Falls back to URP Lit, then Standard, then Internal-Colored.
        /// </summary>
        public static Shader DefaultShader
        {
            get
            {
                if (_defaultShader == null)
                {
                    // Try Universal Render Pipeline Lit shader (for URP projects like Schedule I)
                    _defaultShader = Shader.Find("Universal Render Pipeline/Lit");
                    
                    if (_defaultShader == null)
                    {
                        DebugLog.Warning("URP Lit shader not found, trying Standard shader");
                        _defaultShader = Shader.Find("Standard");
                    }
                    
                    if (_defaultShader == null)
                    {
                        DebugLog.Warning("Standard shader not found, falling back to Internal-Colored");
                        _defaultShader = Shader.Find("Hidden/Internal-Colored");
                    }
                }
                return _defaultShader;
            }
            set =>
                _defaultShader = value;
        }

        /// <summary>
        /// Creates a material using a texture found by name (Resources or Memory).
        /// </summary>
        /// <param name="textureName">Name or path of the texture</param>
        /// <param name="color">Optional tint color</param>
        /// <returns>A textured material</returns>
        public static Material FromTextureName(string textureName, Color? color = null)
        {
            // 1. Try direct Resource load (Fastest)
            Texture texture = Resources.Load<Texture>(textureName);

            // 2. If not found, search all loaded textures in memory (Robust)
            if (texture == null)
            {
                Texture2D[] allTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
                foreach (Texture2D t in allTextures)
                {
                    if (t.name.Equals(textureName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        texture = t;
                        break;
                    }
                }
            }

            // 3. Fallback to opaque color if still missing
            if (texture == null)
            {
                DebugLog.Warning($"Could not find texture '{textureName}' in Resources or Memory.");
                return Opaque(color ?? Color.white);
            }

            Material material = new Material(DefaultShader);
            material.name = $"MAPI_Texture_{texture.name}";
            
            // Set main texture
            if (material.HasProperty("_MainTex"))
            {
                material.SetTexture("_MainTex", texture);
            }
            if (material.HasProperty("_BaseMap")) // URP support
            {
                material.SetTexture("_BaseMap", texture);
            }

            // Apply tint if provided
            if (color.HasValue)
            {
                material.color = color.Value;
                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", color.Value);
                }
            }

            ApplyStandardProperties(material);
            return material;
        }

        /// <summary>
        /// Finds an existing material in Resources or memory by name.
        /// </summary>
        /// <param name="materialName">Name of the material to find (partial match supported)</param>
        /// <returns>The found material or null</returns>
        public static Material? FindExistingMaterial(string materialName)
        {
            // 1. Try direct Resource load
            Material mat = Resources.Load<Material>(materialName);
            if (mat != null) return mat;

            // 2. Search all loaded materials
            Material[] allMats = Resources.FindObjectsOfTypeAll<Material>();
            foreach (Material m in allMats)
            {
                // Robust matching: exact, contains, or starts with
                if (m.name.Equals(materialName, System.StringComparison.OrdinalIgnoreCase) ||
                    m.name.Contains(materialName))
                {
                    return m;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Creates an opaque material with the specified color.
        /// </summary>
        /// <param name="color">The base color of the material</param>
        /// <param name="shader">Optional shader to use (defaults to Standard shader)</param>
        /// <returns>A new opaque material</returns>
        public static Material Opaque(Color color, Shader? shader = null)
        {
            Shader materialShader = shader ?? DefaultShader;
            Material material = new Material(materialShader)
            {
                name = $"MAPI_Opaque_{color.ToString()}",
                color = color
            };

            ApplyStandardProperties(material);
            
            // Ensure opaque rendering mode
            if (material.HasProperty("_Mode"))
            {
                material.SetFloat("_Mode", 0); // Opaque
            }
            if (material.HasProperty("_SrcBlend"))
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            }
            if (material.HasProperty("_DstBlend"))
            {
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            }
            if (material.HasProperty("_ZWrite"))
            {
                material.SetInt("_ZWrite", 1);
            }

            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;

            return material;
        }

        /// <summary>
        /// Creates a transparent material with the specified color and alpha.
        /// </summary>
        /// <param name="color">The base color of the material</param>
        /// <param name="alpha">The alpha transparency (0-1)</param>
        /// <returns>A new transparent material</returns>
        public static Material Transparent(Color color, float alpha = Constants.Materials.DefaultTransparencyAlpha)
        {
            Material material = new Material(DefaultShader)
            {
                name = $"MAPI_Transparent_{color.ToString()}_{alpha}",
                color = new Color(color.r, color.g, color.b, alpha)
            };

            ApplyStandardProperties(material);
            
            // Set to transparent rendering mode
            if (material.HasProperty("_Mode"))
            {
                material.SetFloat("_Mode", 3); // Transparent
            }
            if (material.HasProperty("_SrcBlend"))
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            }
            if (material.HasProperty("_DstBlend"))
            {
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            }
            if (material.HasProperty("_ZWrite"))
            {
                material.SetInt("_ZWrite", 0);
            }

            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;

            return material;
        }

        /// <summary>
        /// Creates a glass-like material with the specified color and transparency.
        /// </summary>
        /// <param name="color">The base color tint of the glass</param>
        /// <param name="alpha">The alpha transparency (0-1)</param>
        /// <returns>A new glass material</returns>
        public static Material Glass(Color color, float alpha = Constants.Materials.GlassAlpha)
        {
            Material material = Transparent(color, alpha);
            material.name = $"MAPI_Glass_{color.ToString()}_{alpha}";

            // Glass-like properties
            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", 0.95f);
            }
            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", 0.0f);
            }

            return material;
        }

        /// <summary>
        /// Creates a clear, transparent glass material with minimal tint.
        /// Perfect for windows, display cases, and architectural glass elements.
        /// </summary>
        /// <param name="alpha">The alpha transparency (0-1, default 0.3 for visible but clear glass)</param>
        /// <param name="tint">Optional subtle color tint (defaults to very light blue)</param>
        /// <returns>A clear glass material optimized for transparency</returns>
        public static Material ClearGlass(float alpha = 0.3f, Color? tint = null)
        {
            Color glassTint = tint ?? new Color(0.95f, 0.97f, 1f); // Very subtle cool tint
            Material material = Transparent(glassTint, alpha);
            material.name = $"MAPI_ClearGlass_a{alpha}";

            // Maximally smooth and non-metallic for realistic glass
            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", 1.0f); // Maximum smoothness
            }
            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", 0.0f); // No metallic properties
            }

            return material;
        }

        /// <summary>
        /// Creates a metallic material with the specified color.
        /// </summary>
        /// <param name="color">The base color of the metal</param>
        /// <param name="metallic">The metallic value (0-1, default 0.8)</param>
        /// <param name="smoothness">The smoothness value (0-1, default 0.9)</param>
        /// <returns>A new metallic material</returns>
        public static Material Metal(Color color, float metallic = 0.8f, float smoothness = 0.9f)
        {
            Material material = Opaque(color);
            material.name = $"MAPI_Metal_{color.ToString()}";

            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", metallic);
            }
            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", smoothness);
            }

            return material;
        }

        /// <summary>
        /// Creates an emissive (glowing) material with the specified color.
        /// </summary>
        /// <param name="color">The emission color</param>
        /// <param name="intensity">The emission intensity (default 1.0)</param>
        /// <returns>A new emissive material</returns>
        public static Material Emissive(Color color, float intensity = 1.0f)
        {
            Material material = Opaque(color);
            material.name = $"MAPI_Emissive_{color.ToString()}";

            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", color * intensity);
                material.EnableKeyword("_EMISSION");
            }

            return material;
        }

        /// <summary>
        /// Creates a material from a shader with a base color.
        /// </summary>
        /// <param name="shader">The shader to use</param>
        /// <param name="color">The base color</param>
        /// <returns>A new material with the specified shader and color</returns>
        public static Material CreateFromShader(Shader shader, Color color)
        {
            if (shader == null)
            {
                DebugLog.Error("Cannot create material from null shader");
                return Opaque(color);
            }

            Material material = new Material(shader)
            {
                name = $"MAPI_{shader.name}_{color.ToString()}",
                color = color
            };

            ApplyStandardProperties(material);
            return material;
        }

        /// <summary>
        /// Apply standard S1MAPI properties to a material.
        /// </summary>
        /// <param name="material">The material to configure</param>
        public static void ApplyStandardProperties(Material material)
        {
            if (material == null)
            {
                DebugLog.Warning("Cannot apply properties to null material");
                return;
            }

            // Enable GPU instancing if supported
            material.enableInstancing = true;

            // Set standard rendering properties if available
            if (material.HasProperty("_Glossiness"))
            {
                material.SetFloat("_Glossiness", 0.5f);
            }
        }

        #endregion
    }
}
