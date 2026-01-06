using UnityEngine;

namespace MAPI.Utils
{
    /// <summary>
    /// Centralized access to base Schedule 1 game assets.
    /// Use this instead of hardcoded string paths to ensure reliability and ease of use.
    /// </summary>
    public static class GameAssets
    {
        /// <summary>
        /// Common materials found in the base game
        /// </summary>
        public static class Materials
        {
            private static Material _woodMediumBrown;
            
            /// <summary>
            /// "wood_mediumbrown" - The standard wood material used for tables and furniture.
            /// </summary>
            public static Material WoodMediumBrown
            {
                get
                {
                    if (_woodMediumBrown == null)
                    {
                        _woodMediumBrown = MaterialPresets.FindExistingMaterial("wood_mediumbrown");
                        if (_woodMediumBrown == null)
                        {
                            DebugLog.Warning("Could not find base game material 'wood_mediumbrown'. Using fallback.");
                            // Fallback to a procedural wood-like material if the asset is missing
                            _woodMediumBrown = MaterialPresets.Opaque(new Color(0.4f, 0.25f, 0.1f)); 
                        }
                    }
                    return _woodMediumBrown;
                }
            }

            /// <summary>
            /// "granite dull salmon lighter" - A specific granite material found in the game.
            /// </summary>
            public static Material GraniteDullSalmonLighter
            {
                get
                {
                    // Search for partial name to catch "(Instance)" or cleaned names
                    Material mat = MaterialPresets.FindExistingMaterial("granite dull salmon lighter");
                    if (mat == null)
                    {
                        DebugLog.Warning("Could not find base game material 'granite dull salmon lighter'. Using fallback.");
                        // Fallback to a dull salmon color
                        mat = MaterialPresets.Opaque(new Color(0.8f, 0.55f, 0.5f)); 
                    }
                    return mat;
                }
            }

            /// <summary>
            /// "brick brick colored" - Reddish brick material.
            /// </summary>
            public static Material BrickBrickColored
            {
                get
                {
                    Material mat = MaterialPresets.FindExistingMaterial("brick brick colored");
                    if (mat == null)
                    {
                        DebugLog.Warning("Could not find base game material 'brick brick colored'. Using fallback.");
                        mat = MaterialPresets.Opaque(new Color(0.6f, 0.3f, 0.2f)); // Brick red
                    }
                    return mat;
                }
            }

            /// <summary>
            /// "metal_darkgrey_mat" - Dark grey metal material.
            /// </summary>
            public static Material MetalDarkGrey
            {
                get
                {
                    Material mat = MaterialPresets.FindExistingMaterial("metal_darkgrey_mat");
                    if (mat == null)
                    {
                        DebugLog.Warning("Could not find base game material 'metal_darkgrey_mat'. Using fallback.");
                        mat = MaterialPresets.Opaque(new Color(0.2f, 0.2f, 0.2f)); // Dark grey
                    }
                    return mat;
                }
            }

            /// <summary>
            /// "concrete light grey" - Light grey concrete material.
            /// </summary>
            public static Material ConcreteLightGrey
            {
                get
                {
                    Material mat = MaterialPresets.FindExistingMaterial("concrete light grey");
                    if (mat == null)
                    {
                        DebugLog.Warning("Could not find base game material 'concrete light grey'. Using fallback.");
                        mat = MaterialPresets.Opaque(new Color(0.75f, 0.75f, 0.75f)); // Light concrete
                    }
                    return mat;
                }
            }

            /// <summary>
            /// "laundromat glass mat" - Specific glass material used in the laundromat.
            /// </summary>
            public static Material LaundromatGlass
            {
                get
                {
                    Material mat = MaterialPresets.FindExistingMaterial("laundromat glass mat");
                    if (mat == null)
                    {
                        DebugLog.Warning("Could not find base game material 'laundromat glass mat'. Using fallback.");
                        mat = MaterialPresets.ClearGlass(); 
                    }
                    return mat;
                }
            }
        }

        /// <summary>
        /// Names of common prefabs found in the base game.
        /// Pass these to PrimitiveBuilder.CreatePrefab().
        /// </summary>
        public static class Prefabs
        {
            /// <summary>
            /// "Bong_Trash" - A standard glass bong item.
            /// </summary>
            public const string Bong = "Bong_Trash";

            // Add other known prefabs here
            // public const string TrashBag = "TrashBag";
        }
    }
}
