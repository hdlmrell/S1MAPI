using UnityEngine;
using MAPI.Utils;

namespace MAPI.S1
{
    /// <summary>
    /// Registry of known Schedule 1 game materials.
    /// These are base game materials that can be reused for custom content.
    /// </summary>
    public static class Materials
    {
        #region Fields

        private static Material? _woodMediumBrown;
        private static Material? _graniteDullSalmonLighter;
        private static Material? _brickBrickColored;
        private static Material? _metalDarkGrey;
        private static Material? _concreteLightGrey;
        private static Material? _laundromatGlass;

        #endregion

        #region Wood

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
                        DebugLog.Warning("[S1.Materials] Could not find 'wood_mediumbrown'. Using fallback.");
                        _woodMediumBrown = MaterialPresets.Opaque(new Color(0.4f, 0.25f, 0.1f));
                    }
                }
                return _woodMediumBrown;
            }
        }

        #endregion

        #region Stone

        /// <summary>
        /// "granite dull salmon lighter" - A specific granite material found in the game.
        /// </summary>
        public static Material GraniteDullSalmonLighter
        {
            get
            {
                if (_graniteDullSalmonLighter == null)
                {
                    _graniteDullSalmonLighter = MaterialPresets.FindExistingMaterial("granite dull salmon lighter");
                    if (_graniteDullSalmonLighter == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'granite dull salmon lighter'. Using fallback.");
                        _graniteDullSalmonLighter = MaterialPresets.Opaque(new Color(0.8f, 0.55f, 0.5f));
                    }
                }
                return _graniteDullSalmonLighter;
            }
        }

        /// <summary>
        /// "concrete light grey" - Light grey concrete material.
        /// </summary>
        public static Material ConcreteLightGrey
        {
            get
            {
                if (_concreteLightGrey == null)
                {
                    _concreteLightGrey = MaterialPresets.FindExistingMaterial("concrete light grey");
                    if (_concreteLightGrey == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'concrete light grey'. Using fallback.");
                        _concreteLightGrey = MaterialPresets.Opaque(new Color(0.75f, 0.75f, 0.75f));
                    }
                }
                return _concreteLightGrey;
            }
        }

        #endregion

        #region Brick

        /// <summary>
        /// "brick brick colored" - Reddish brick material.
        /// </summary>
        public static Material BrickBrickColored
        {
            get
            {
                if (_brickBrickColored == null)
                {
                    _brickBrickColored = MaterialPresets.FindExistingMaterial("brick brick colored");
                    if (_brickBrickColored == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'brick brick colored'. Using fallback.");
                        _brickBrickColored = MaterialPresets.Opaque(new Color(0.6f, 0.3f, 0.2f));
                    }
                }
                return _brickBrickColored;
            }
        }

        #endregion

        #region Metal

        /// <summary>
        /// "metal_darkgrey_mat" - Dark grey metal material.
        /// </summary>
        public static Material MetalDarkGrey
        {
            get
            {
                if (_metalDarkGrey == null)
                {
                    _metalDarkGrey = MaterialPresets.FindExistingMaterial("metal_darkgrey_mat");
                    if (_metalDarkGrey == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'metal_darkgrey_mat'. Using fallback.");
                        _metalDarkGrey = MaterialPresets.Opaque(new Color(0.2f, 0.2f, 0.2f));
                    }
                }
                return _metalDarkGrey;
            }
        }

        #endregion

        #region Glass

        /// <summary>
        /// "laundromat glass mat" - Specific glass material used in the laundromat.
        /// </summary>
        public static Material LaundromatGlass
        {
            get
            {
                if (_laundromatGlass == null)
                {
                    _laundromatGlass = MaterialPresets.FindExistingMaterial("laundromat glass mat");
                    if (_laundromatGlass == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'laundromat glass mat'. Using fallback.");
                        _laundromatGlass = MaterialPresets.ClearGlass();
                    }
                }
                return _laundromatGlass;
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Find a material by name from the game's loaded materials.
        /// </summary>
        /// <param name="name">Partial or full material name</param>
        /// <returns>The material or null if not found</returns>
        public static Material? Find(string name) => MaterialPresets.FindExistingMaterial(name);

        #endregion
    }
}
