using UnityEngine;
using S1MAPI.Utils;

namespace S1MAPI.S1
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
        private static Material? _woodCrate;
        private static Material? _docksGround;
        private static Material? _concreteDocksRamps;
        private static Material? _concreteClothingStoreGreen;
        private static Material? _concreteClothingStoreDarkGreen;
        private static Material? _straightRoad10m;
        private static Material? _straightRoad10mNoLines;
        private static Material? _straightRoad10mDesert;
        private static Material? _fabricWhite;
        private static Material? _fabricRed;
        private static Material? _fabricGreen;
        private static Material? _fabricDenim;
        private static Material? _fabricBrown;
        private static Material? _fabricBlue;
        private static Material? _fabricBlack;
        private static Material? _industrialBuildingAUpperWalls;
        private static Material? _industrialBuildingATopWalls;
        private static Material? _industrialBuildingARoof;
        private static Material? _industrialBuildingALowerWalls;
        private static Material? _industrialBuildingAFoundation;
        private static Material? _industrialBuildingRollerdoor;
        private static Material? _metalWarehouseGreen;
        private static Material? _metalWarehouseTrimGreen;
        private static Material? _metalMediumGrey;
        private static Material? _metalVeryDarkGrey;
        private static Material? _metalLightGrey;
        private static Material? _metalWhite;
        private static Material? _woodPlanksMediumBrown;
        private static Material? _woodPlanksLightBrown;
        private static Material? _woodPlanksWhite;
        private static Material? _tilesDarkGrey;
        private static Material? _smallTileDirtyWhite;
        private static Material? _concreteParkingLot;
        private static Material? _windowGlass;
        private static Material? _chainlinkFenceMetal;
        private static Material? _industrialMetalDoor;
        private static Material? _industrialMetalDoorRed;
        private static Material? _industrialMetalDoorGreen;
        private static Material? _industrialMetalDoorBlue;
        private static Material? _industrialMetalDoorYellow;
        private static Material? _industrialMetalDoorGrey;
        private static Material? _docksWarehouseMezzanine;
        private static Material? _docksWarehouseFence;
        private static Material? _dockswallMain;
        private static Material? _straightRoad10x4;
        private static Material? _straightRoad10x4NoLines;
        private static Material? _straightRoad10x4Sidewalk;
        private static Material? _industrialBuildingAPillar;
        private static Material? _industrialBuildingAMiddleTrim;
        private static Material? _industrialBuildingASmallRoof;

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
        public static Material BrickWallRed
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

        #region Wood

        /// <summary>
        /// "wood crate mat" - Wood crate material used for storage containers.
        /// </summary>
        public static Material WoodCrate
        {
            get
            {
                if (_woodCrate == null)
                {
                    _woodCrate = MaterialPresets.FindExistingMaterial("wood crate mat");
                    if (_woodCrate == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'wood crate mat'. Using fallback.");
                        _woodCrate = MaterialPresets.Opaque(new Color(0.5f, 0.35f, 0.2f));
                    }
                }
                return _woodCrate;
            }
        }

        /// <summary>
        /// "wood planks medium brown mat" - Medium brown wood planks.
        /// </summary>
        public static Material WoodPlanksMediumBrown
        {
            get
            {
                if (_woodPlanksMediumBrown == null)
                {
                    _woodPlanksMediumBrown = MaterialPresets.FindExistingMaterial("wood planks medium brown mat");
                    if (_woodPlanksMediumBrown == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'wood planks medium brown mat'. Using fallback.");
                        _woodPlanksMediumBrown = MaterialPresets.Opaque(new Color(0.45f, 0.3f, 0.15f));
                    }
                }
                return _woodPlanksMediumBrown;
            }
        }

        /// <summary>
        /// "wood planks light brown mat" - Light brown wood planks.
        /// </summary>
        public static Material WoodPlanksLightBrown
        {
            get
            {
                if (_woodPlanksLightBrown == null)
                {
                    _woodPlanksLightBrown = MaterialPresets.FindExistingMaterial("wood planks light brown mat");
                    if (_woodPlanksLightBrown == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'wood planks light brown mat'. Using fallback.");
                        _woodPlanksLightBrown = MaterialPresets.Opaque(new Color(0.55f, 0.4f, 0.25f));
                    }
                }
                return _woodPlanksLightBrown;
            }
        }

        /// <summary>
        /// "wood planks white" - White painted wood planks.
        /// </summary>
        public static Material WoodPlanksWhite
        {
            get
            {
                if (_woodPlanksWhite == null)
                {
                    _woodPlanksWhite = MaterialPresets.FindExistingMaterial("wood planks white");
                    if (_woodPlanksWhite == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'wood planks white'. Using fallback.");
                        _woodPlanksWhite = MaterialPresets.Opaque(new Color(0.9f, 0.9f, 0.9f));
                    }
                }
                return _woodPlanksWhite;
            }
        }

        #endregion

        #region Docks

        /// <summary>
        /// "docks ground" - Ground material for dock areas.
        /// </summary>
        public static Material DocksGround
        {
            get
            {
                if (_docksGround == null)
                {
                    _docksGround = MaterialPresets.FindExistingMaterial("docks ground");
                    if (_docksGround == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'docks ground'. Using fallback.");
                        _docksGround = MaterialPresets.Opaque(new Color(0.35f, 0.35f, 0.3f));
                    }
                }
                return _docksGround;
            }
        }

        /// <summary>
        /// "concrete_docks_ramps" - Concrete material for dock ramps.
        /// </summary>
        public static Material ConcreteDocksRamps
        {
            get
            {
                if (_concreteDocksRamps == null)
                {
                    _concreteDocksRamps = MaterialPresets.FindExistingMaterial("concrete_docks_ramps");
                    if (_concreteDocksRamps == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'concrete_docks_ramps'. Using fallback.");
                        _concreteDocksRamps = MaterialPresets.Opaque(new Color(0.5f, 0.5f, 0.5f));
                    }
                }
                return _concreteDocksRamps;
            }
        }

        /// <summary>
        /// "docks warehouse mezzanine mat" - Material for warehouse mezzanine structures.
        /// </summary>
        public static Material DocksWarehouseMezzanine
        {
            get
            {
                if (_docksWarehouseMezzanine == null)
                {
                    _docksWarehouseMezzanine = MaterialPresets.FindExistingMaterial("docks warehouse mezzanine mat");
                    if (_docksWarehouseMezzanine == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'docks warehouse mezzanine mat'. Using fallback.");
                        _docksWarehouseMezzanine = MaterialPresets.Opaque(new Color(0.4f, 0.4f, 0.35f));
                    }
                }
                return _docksWarehouseMezzanine;
            }
        }

        /// <summary>
        /// "docks warehouse fence mat" - Material for warehouse fencing.
        /// </summary>
        public static Material DocksWarehouseFence
        {
            get
            {
                if (_docksWarehouseFence == null)
                {
                    _docksWarehouseFence = MaterialPresets.FindExistingMaterial("docks warehouse fence mat");
                    if (_docksWarehouseFence == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'docks warehouse fence mat'. Using fallback.");
                        _docksWarehouseFence = MaterialPresets.Opaque(new Color(0.3f, 0.3f, 0.25f));
                    }
                }
                return _docksWarehouseFence;
            }
        }

        /// <summary>
        /// "dockswall_Main mat" - Main dock wall material.
        /// </summary>
        public static Material DockswallMain
        {
            get
            {
                if (_dockswallMain == null)
                {
                    _dockswallMain = MaterialPresets.FindExistingMaterial("dockswall_Main mat");
                    if (_dockswallMain == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'dockswall_Main mat'. Using fallback.");
                        _dockswallMain = MaterialPresets.Opaque(new Color(0.45f, 0.45f, 0.4f));
                    }
                }
                return _dockswallMain;
            }
        }

        #endregion

        #region Fabrics

        /// <summary>
        /// "fabric white" - White fabric material.
        /// </summary>
        public static Material FabricWhite
        {
            get
            {
                if (_fabricWhite == null)
                {
                    _fabricWhite = MaterialPresets.FindExistingMaterial("fabric white");
                    if (_fabricWhite == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'fabric white'. Using fallback.");
                        _fabricWhite = MaterialPresets.Opaque(new Color(0.95f, 0.95f, 0.95f));
                    }
                }
                return _fabricWhite;
            }
        }

        /// <summary>
        /// "fabric red" - Red fabric material.
        /// </summary>
        public static Material FabricRed
        {
            get
            {
                if (_fabricRed == null)
                {
                    _fabricRed = MaterialPresets.FindExistingMaterial("fabric red");
                    if (_fabricRed == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'fabric red'. Using fallback.");
                        _fabricRed = MaterialPresets.Opaque(new Color(0.7f, 0.2f, 0.2f));
                    }
                }
                return _fabricRed;
            }
        }

        /// <summary>
        /// "fabric green" - Green fabric material.
        /// </summary>
        public static Material FabricGreen
        {
            get
            {
                if (_fabricGreen == null)
                {
                    _fabricGreen = MaterialPresets.FindExistingMaterial("fabric green");
                    if (_fabricGreen == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'fabric green'. Using fallback.");
                        _fabricGreen = MaterialPresets.Opaque(new Color(0.2f, 0.5f, 0.3f));
                    }
                }
                return _fabricGreen;
            }
        }

        /// <summary>
        /// "fabric denim" - Denim fabric material.
        /// </summary>
        public static Material FabricDenim
        {
            get
            {
                if (_fabricDenim == null)
                {
                    _fabricDenim = MaterialPresets.FindExistingMaterial("fabric denim");
                    if (_fabricDenim == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'fabric denim'. Using fallback.");
                        _fabricDenim = MaterialPresets.Opaque(new Color(0.25f, 0.35f, 0.55f));
                    }
                }
                return _fabricDenim;
            }
        }

        /// <summary>
        /// "fabric brown" - Brown fabric material.
        /// </summary>
        public static Material FabricBrown
        {
            get
            {
                if (_fabricBrown == null)
                {
                    _fabricBrown = MaterialPresets.FindExistingMaterial("fabric brown");
                    if (_fabricBrown == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'fabric brown'. Using fallback.");
                        _fabricBrown = MaterialPresets.Opaque(new Color(0.4f, 0.25f, 0.15f));
                    }
                }
                return _fabricBrown;
            }
        }

        /// <summary>
        /// "fabric blue" - Blue fabric material.
        /// </summary>
        public static Material FabricBlue
        {
            get
            {
                if (_fabricBlue == null)
                {
                    _fabricBlue = MaterialPresets.FindExistingMaterial("fabric blue");
                    if (_fabricBlue == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'fabric blue'. Using fallback.");
                        _fabricBlue = MaterialPresets.Opaque(new Color(0.2f, 0.3f, 0.6f));
                    }
                }
                return _fabricBlue;
            }
        }

        /// <summary>
        /// "fabric black" - Black fabric material.
        /// </summary>
        public static Material FabricBlack
        {
            get
            {
                if (_fabricBlack == null)
                {
                    _fabricBlack = MaterialPresets.FindExistingMaterial("fabric black");
                    if (_fabricBlack == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'fabric black'. Using fallback.");
                        _fabricBlack = MaterialPresets.Opaque(new Color(0.15f, 0.15f, 0.15f));
                    }
                }
                return _fabricBlack;
            }
        }

        #endregion

        #region Industrial Building

        /// <summary>
        /// "industrialbuildingA_UpperWalls mat" - Upper walls of industrial building A.
        /// </summary>
        public static Material IndustrialBuildingAUpperWalls
        {
            get
            {
                if (_industrialBuildingAUpperWalls == null)
                {
                    _industrialBuildingAUpperWalls = MaterialPresets.FindExistingMaterial("industrialbuildingA_UpperWalls mat");
                    if (_industrialBuildingAUpperWalls == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrialbuildingA_UpperWalls mat'. Using fallback.");
                        _industrialBuildingAUpperWalls = MaterialPresets.Opaque(new Color(0.6f, 0.6f, 0.55f));
                    }
                }
                return _industrialBuildingAUpperWalls;
            }
        }

        /// <summary>
        /// "industrialbuildingA_TopWalls mat" - Top walls of industrial building A.
        /// </summary>
        public static Material IndustrialBuildingATopWalls
        {
            get
            {
                if (_industrialBuildingATopWalls == null)
                {
                    _industrialBuildingATopWalls = MaterialPresets.FindExistingMaterial("industrialbuildingA_TopWalls mat");
                    if (_industrialBuildingATopWalls == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrialbuildingA_TopWalls mat'. Using fallback.");
                        _industrialBuildingATopWalls = MaterialPresets.Opaque(new Color(0.55f, 0.55f, 0.5f));
                    }
                }
                return _industrialBuildingATopWalls;
            }
        }

        /// <summary>
        /// "industrialbuildingA_Roof mat" - Roof material for industrial building A.
        /// </summary>
        public static Material IndustrialBuildingARoof
        {
            get
            {
                if (_industrialBuildingARoof == null)
                {
                    _industrialBuildingARoof = MaterialPresets.FindExistingMaterial("industrialbuildingA_Roof mat");
                    if (_industrialBuildingARoof == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrialbuildingA_Roof mat'. Using fallback.");
                        _industrialBuildingARoof = MaterialPresets.Opaque(new Color(0.25f, 0.25f, 0.25f));
                    }
                }
                return _industrialBuildingARoof;
            }
        }

        /// <summary>
        /// "industrialbuildingA_SmallRoof mat" - Small roof material for industrial building A.
        /// </summary>
        public static Material IndustrialBuildingASmallRoof
        {
            get
            {
                if (_industrialBuildingASmallRoof == null)
                {
                    _industrialBuildingASmallRoof = MaterialPresets.FindExistingMaterial("industrialbuildingA_SmallRoof mat");
                    if (_industrialBuildingASmallRoof == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrialbuildingA_SmallRoof mat'. Using fallback.");
                        _industrialBuildingASmallRoof = MaterialPresets.Opaque(new Color(0.3f, 0.3f, 0.3f));
                    }
                }
                return _industrialBuildingASmallRoof;
            }
        }

        /// <summary>
        /// "industrialbuildingA_LowerWalls mat" - Lower walls of industrial building A.
        /// </summary>
        public static Material IndustrialBuildingALowerWalls
        {
            get
            {
                if (_industrialBuildingALowerWalls == null)
                {
                    _industrialBuildingALowerWalls = MaterialPresets.FindExistingMaterial("industrialbuildingA_LowerWalls mat");
                    if (_industrialBuildingALowerWalls == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrialbuildingA_LowerWalls mat'. Using fallback.");
                        _industrialBuildingALowerWalls = MaterialPresets.Opaque(new Color(0.5f, 0.5f, 0.45f));
                    }
                }
                return _industrialBuildingALowerWalls;
            }
        }

        /// <summary>
        /// "industrialbuildingA_Foundation mat" - Foundation material for industrial building A.
        /// </summary>
        public static Material IndustrialBuildingAFoundation
        {
            get
            {
                if (_industrialBuildingAFoundation == null)
                {
                    _industrialBuildingAFoundation = MaterialPresets.FindExistingMaterial("industrialbuildingA_Foundation mat");
                    if (_industrialBuildingAFoundation == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrialbuildingA_Foundation mat'. Using fallback.");
                        _industrialBuildingAFoundation = MaterialPresets.Opaque(new Color(0.35f, 0.35f, 0.35f));
                    }
                }
                return _industrialBuildingAFoundation;
            }
        }

        /// <summary>
        /// "industrialbuildingA_Pillar mat" - Pillar material for industrial building A.
        /// </summary>
        public static Material IndustrialBuildingAPillar
        {
            get
            {
                if (_industrialBuildingAPillar == null)
                {
                    _industrialBuildingAPillar = MaterialPresets.FindExistingMaterial("industrialbuildingA_Pillar mat");
                    if (_industrialBuildingAPillar == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrialbuildingA_Pillar mat'. Using fallback.");
                        _industrialBuildingAPillar = MaterialPresets.Opaque(new Color(0.45f, 0.45f, 0.4f));
                    }
                }
                return _industrialBuildingAPillar;
            }
        }

        /// <summary>
        /// "industrialbuildingA_MiddleTrim mat" - Middle trim material for industrial building A.
        /// </summary>
        public static Material IndustrialBuildingAMiddleTrim
        {
            get
            {
                if (_industrialBuildingAMiddleTrim == null)
                {
                    _industrialBuildingAMiddleTrim = MaterialPresets.FindExistingMaterial("industrialbuildingA_MiddleTrim mat");
                    if (_industrialBuildingAMiddleTrim == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrialbuildingA_MiddleTrim mat'. Using fallback.");
                        _industrialBuildingAMiddleTrim = MaterialPresets.Opaque(new Color(0.4f, 0.4f, 0.35f));
                    }
                }
                return _industrialBuildingAMiddleTrim;
            }
        }

        /// <summary>
        /// "industrialbuilding rollerdoor mat" - Roller door material for industrial buildings.
        /// </summary>
        public static Material IndustrialBuildingRollerdoor
        {
            get
            {
                if (_industrialBuildingRollerdoor == null)
                {
                    _industrialBuildingRollerdoor = MaterialPresets.FindExistingMaterial("industrialbuilding rollerdoor mat");
                    if (_industrialBuildingRollerdoor == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrialbuilding rollerdoor mat'. Using fallback.");
                        _industrialBuildingRollerdoor = MaterialPresets.Opaque(new Color(0.3f, 0.3f, 0.35f));
                    }
                }
                return _industrialBuildingRollerdoor;
            }
        }

        #endregion

        #region Clothing Store Concrete

        /// <summary>
        /// "concrete clothing store green" - Green tinted concrete from clothing store.
        /// </summary>
        public static Material ConcreteClothingStoreGreen
        {
            get
            {
                if (_concreteClothingStoreGreen == null)
                {
                    _concreteClothingStoreGreen = MaterialPresets.FindExistingMaterial("concrete clothing store green");
                    if (_concreteClothingStoreGreen == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'concrete clothing store green'. Using fallback.");
                        _concreteClothingStoreGreen = MaterialPresets.Opaque(new Color(0.6f, 0.65f, 0.6f));
                    }
                }
                return _concreteClothingStoreGreen;
            }
        }

        /// <summary>
        /// "concrete clothing store dark green" - Dark green tinted concrete from clothing store.
        /// </summary>
        public static Material ConcreteClothingStoreDarkGreen
        {
            get
            {
                if (_concreteClothingStoreDarkGreen == null)
                {
                    _concreteClothingStoreDarkGreen = MaterialPresets.FindExistingMaterial("concrete clothing store dark green");
                    if (_concreteClothingStoreDarkGreen == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'concrete clothing store dark green'. Using fallback.");
                        _concreteClothingStoreDarkGreen = MaterialPresets.Opaque(new Color(0.4f, 0.45f, 0.4f));
                    }
                }
                return _concreteClothingStoreDarkGreen;
            }
        }

        #endregion

        #region Roads

        /// <summary>
        /// "straight road 10m mat" - Standard straight road material (10m width).
        /// </summary>
        public static Material StraightRoad10m
        {
            get
            {
                if (_straightRoad10m == null)
                {
                    _straightRoad10m = MaterialPresets.FindExistingMaterial("straight road 10m mat");
                    if (_straightRoad10m == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'straight road 10m mat'. Using fallback.");
                        _straightRoad10m = MaterialPresets.Opaque(new Color(0.25f, 0.25f, 0.25f));
                    }
                }
                return _straightRoad10m;
            }
        }

        /// <summary>
        /// "straight road 10m mat nolines" - Straight road material without lane markings.
        /// </summary>
        public static Material StraightRoad10mNoLines
        {
            get
            {
                if (_straightRoad10mNoLines == null)
                {
                    _straightRoad10mNoLines = MaterialPresets.FindExistingMaterial("straight road 10m mat nolines");
                    if (_straightRoad10mNoLines == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'straight road 10m mat nolines'. Using fallback.");
                        _straightRoad10mNoLines = MaterialPresets.Opaque(new Color(0.25f, 0.25f, 0.25f));
                    }
                }
                return _straightRoad10mNoLines;
            }
        }

        /// <summary>
        /// "straight road 10m mat desert" - Desert variant of straight road material.
        /// </summary>
        public static Material StraightRoad10mDesert
        {
            get
            {
                if (_straightRoad10mDesert == null)
                {
                    _straightRoad10mDesert = MaterialPresets.FindExistingMaterial("straight road 10m mat desert");
                    if (_straightRoad10mDesert == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'straight road 10m mat desert'. Using fallback.");
                        _straightRoad10mDesert = MaterialPresets.Opaque(new Color(0.55f, 0.5f, 0.4f));
                    }
                }
                return _straightRoad10mDesert;
            }
        }

        /// <summary>
        /// "straight road 10x4 mat" - Wide straight road material (10x4m).
        /// </summary>
        public static Material StraightRoad10x4
        {
            get
            {
                if (_straightRoad10x4 == null)
                {
                    _straightRoad10x4 = MaterialPresets.FindExistingMaterial("straight road 10x4 mat");
                    if (_straightRoad10x4 == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'straight road 10x4 mat'. Using fallback.");
                        _straightRoad10x4 = MaterialPresets.Opaque(new Color(0.25f, 0.25f, 0.25f));
                    }
                }
                return _straightRoad10x4;
            }
        }

        /// <summary>
        /// "straight road 10x4 mat nolines" - Wide straight road material without lane markings.
        /// </summary>
        public static Material StraightRoad10x4NoLines
        {
            get
            {
                if (_straightRoad10x4NoLines == null)
                {
                    _straightRoad10x4NoLines = MaterialPresets.FindExistingMaterial("straight road 10x4 mat nolines");
                    if (_straightRoad10x4NoLines == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'straight road 10x4 mat nolines'. Using fallback.");
                        _straightRoad10x4NoLines = MaterialPresets.Opaque(new Color(0.25f, 0.25f, 0.25f));
                    }
                }
                return _straightRoad10x4NoLines;
            }
        }

        /// <summary>
        /// "straight road 10x4 sidewalk mat" - Sidewalk material for 10x4 road.
        /// </summary>
        public static Material StraightRoad10x4Sidewalk
        {
            get
            {
                if (_straightRoad10x4Sidewalk == null)
                {
                    _straightRoad10x4Sidewalk = MaterialPresets.FindExistingMaterial("straight road 10x4 sidewalk mat");
                    if (_straightRoad10x4Sidewalk == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'straight road 10x4 sidewalk mat'. Using fallback.");
                        _straightRoad10x4Sidewalk = MaterialPresets.Opaque(new Color(0.7f, 0.7f, 0.7f));
                    }
                }
                return _straightRoad10x4Sidewalk;
            }
        }

        #endregion

        #region Extended Metal

        /// <summary>
        /// "metal warehouse green mat" - Green painted metal for warehouses.
        /// </summary>
        public static Material MetalWarehouseGreen
        {
            get
            {
                if (_metalWarehouseGreen == null)
                {
                    _metalWarehouseGreen = MaterialPresets.FindExistingMaterial("metal warehouse green mat");
                    if (_metalWarehouseGreen == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'metal warehouse green mat'. Using fallback.");
                        _metalWarehouseGreen = MaterialPresets.Opaque(new Color(0.25f, 0.4f, 0.3f));
                    }
                }
                return _metalWarehouseGreen;
            }
        }

        /// <summary>
        /// "metal warehouse trim green mat" - Green metal trim for warehouses.
        /// </summary>
        public static Material MetalWarehouseTrimGreen
        {
            get
            {
                if (_metalWarehouseTrimGreen == null)
                {
                    _metalWarehouseTrimGreen = MaterialPresets.FindExistingMaterial("metal warehouse trim green mat");
                    if (_metalWarehouseTrimGreen == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'metal warehouse trim green mat'. Using fallback.");
                        _metalWarehouseTrimGreen = MaterialPresets.Opaque(new Color(0.2f, 0.35f, 0.25f));
                    }
                }
                return _metalWarehouseTrimGreen;
            }
        }

        /// <summary>
        /// "metal_mediumgrey_mat" - Medium grey metal material.
        /// </summary>
        public static Material MetalMediumGrey
        {
            get
            {
                if (_metalMediumGrey == null)
                {
                    _metalMediumGrey = MaterialPresets.FindExistingMaterial("metal_mediumgrey_mat");
                    if (_metalMediumGrey == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'metal_mediumgrey_mat'. Using fallback.");
                        _metalMediumGrey = MaterialPresets.Opaque(new Color(0.45f, 0.45f, 0.45f));
                    }
                }
                return _metalMediumGrey;
            }
        }

        /// <summary>
        /// "metal_verydarkgrey_mat" - Very dark grey metal material.
        /// </summary>
        public static Material MetalVeryDarkGrey
        {
            get
            {
                if (_metalVeryDarkGrey == null)
                {
                    _metalVeryDarkGrey = MaterialPresets.FindExistingMaterial("metal_verydarkgrey_mat");
                    if (_metalVeryDarkGrey == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'metal_verydarkgrey_mat'. Using fallback.");
                        _metalVeryDarkGrey = MaterialPresets.Opaque(new Color(0.15f, 0.15f, 0.15f));
                    }
                }
                return _metalVeryDarkGrey;
            }
        }

        /// <summary>
        /// "metal_lightgrey_mat" - Light grey metal material.
        /// </summary>
        public static Material MetalLightGrey
        {
            get
            {
                if (_metalLightGrey == null)
                {
                    _metalLightGrey = MaterialPresets.FindExistingMaterial("metal_lightgrey_mat");
                    if (_metalLightGrey == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'metal_lightgrey_mat'. Using fallback.");
                        _metalLightGrey = MaterialPresets.Opaque(new Color(0.65f, 0.65f, 0.65f));
                    }
                }
                return _metalLightGrey;
            }
        }

        /// <summary>
        /// "metal white" - White painted metal.
        /// </summary>
        public static Material MetalWhite
        {
            get
            {
                if (_metalWhite == null)
                {
                    _metalWhite = MaterialPresets.FindExistingMaterial("metal white");
                    if (_metalWhite == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'metal white'. Using fallback.");
                        _metalWhite = MaterialPresets.Opaque(new Color(0.9f, 0.9f, 0.9f));
                    }
                }
                return _metalWhite;
            }
        }

        /// <summary>
        /// "industrial metal door mat" - Standard industrial metal door.
        /// </summary>
        public static Material IndustrialMetalDoor
        {
            get
            {
                if (_industrialMetalDoor == null)
                {
                    _industrialMetalDoor = MaterialPresets.FindExistingMaterial("industrial metal door mat");
                    if (_industrialMetalDoor == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrial metal door mat'. Using fallback.");
                        _industrialMetalDoor = MaterialPresets.Opaque(new Color(0.35f, 0.35f, 0.4f));
                    }
                }
                return _industrialMetalDoor;
            }
        }

        /// <summary>
        /// "industrial metal door red mat" - Red painted industrial metal door.
        /// </summary>
        public static Material IndustrialMetalDoorRed
        {
            get
            {
                if (_industrialMetalDoorRed == null)
                {
                    _industrialMetalDoorRed = MaterialPresets.FindExistingMaterial("industrial metal door red mat");
                    if (_industrialMetalDoorRed == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrial metal door red mat'. Using fallback.");
                        _industrialMetalDoorRed = MaterialPresets.Opaque(new Color(0.5f, 0.2f, 0.2f));
                    }
                }
                return _industrialMetalDoorRed;
            }
        }

        /// <summary>
        /// "industrial metal door green mat" - Green painted industrial metal door.
        /// </summary>
        public static Material IndustrialMetalDoorGreen
        {
            get
            {
                if (_industrialMetalDoorGreen == null)
                {
                    _industrialMetalDoorGreen = MaterialPresets.FindExistingMaterial("industrial metal door green mat");
                    if (_industrialMetalDoorGreen == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrial metal door green mat'. Using fallback.");
                        _industrialMetalDoorGreen = MaterialPresets.Opaque(new Color(0.2f, 0.4f, 0.3f));
                    }
                }
                return _industrialMetalDoorGreen;
            }
        }

        /// <summary>
        /// "industrial metal door blue mat" - Blue painted industrial metal door.
        /// </summary>
        public static Material IndustrialMetalDoorBlue
        {
            get
            {
                if (_industrialMetalDoorBlue == null)
                {
                    _industrialMetalDoorBlue = MaterialPresets.FindExistingMaterial("industrial metal door blue mat");
                    if (_industrialMetalDoorBlue == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrial metal door blue mat'. Using fallback.");
                        _industrialMetalDoorBlue = MaterialPresets.Opaque(new Color(0.2f, 0.25f, 0.5f));
                    }
                }
                return _industrialMetalDoorBlue;
            }
        }

        /// <summary>
        /// "industrial metal door yellow mat" - Yellow painted industrial metal door.
        /// </summary>
        public static Material IndustrialMetalDoorYellow
        {
            get
            {
                if (_industrialMetalDoorYellow == null)
                {
                    _industrialMetalDoorYellow = MaterialPresets.FindExistingMaterial("industrial metal door yellow mat");
                    if (_industrialMetalDoorYellow == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrial metal door yellow mat'. Using fallback.");
                        _industrialMetalDoorYellow = MaterialPresets.Opaque(new Color(0.7f, 0.7f, 0.2f));
                    }
                }
                return _industrialMetalDoorYellow;
            }
        }

        /// <summary>
        /// "industrial metal door grey mat" - Grey industrial metal door.
        /// </summary>
        public static Material IndustrialMetalDoorGrey
        {
            get
            {
                if (_industrialMetalDoorGrey == null)
                {
                    _industrialMetalDoorGrey = MaterialPresets.FindExistingMaterial("industrial metal door grey mat");
                    if (_industrialMetalDoorGrey == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'industrial metal door grey mat'. Using fallback.");
                        _industrialMetalDoorGrey = MaterialPresets.Opaque(new Color(0.4f, 0.4f, 0.4f));
                    }
                }
                return _industrialMetalDoorGrey;
            }
        }

        /// <summary>
        /// "chainlinkfence metal mat" - Chain-link fence metal material.
        /// </summary>
        public static Material ChainlinkFenceMetal
        {
            get
            {
                if (_chainlinkFenceMetal == null)
                {
                    _chainlinkFenceMetal = MaterialPresets.FindExistingMaterial("chainlinkfence metal mat");
                    if (_chainlinkFenceMetal == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'chainlinkfence metal mat'. Using fallback.");
                        _chainlinkFenceMetal = MaterialPresets.Opaque(new Color(0.5f, 0.5f, 0.5f));
                    }
                }
                return _chainlinkFenceMetal;
            }
        }

        #endregion

        #region Tiles

        /// <summary>
        /// "tiles_darkgrey" - Dark grey tiles.
        /// </summary>
        public static Material TilesDarkGrey
        {
            get
            {
                if (_tilesDarkGrey == null)
                {
                    _tilesDarkGrey = MaterialPresets.FindExistingMaterial("tiles_darkgrey");
                    if (_tilesDarkGrey == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'tiles_darkgrey'. Using fallback.");
                        _tilesDarkGrey = MaterialPresets.Opaque(new Color(0.3f, 0.3f, 0.3f));
                    }
                }
                return _tilesDarkGrey;
            }
        }

        /// <summary>
        /// "small tile dirty white" - Dirty white small tiles.
        /// </summary>
        public static Material SmallTileDirtyWhite
        {
            get
            {
                if (_smallTileDirtyWhite == null)
                {
                    _smallTileDirtyWhite = MaterialPresets.FindExistingMaterial("small tile dirty white");
                    if (_smallTileDirtyWhite == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'small tile dirty white'. Using fallback.");
                        _smallTileDirtyWhite = MaterialPresets.Opaque(new Color(0.8f, 0.8f, 0.75f));
                    }
                }
                return _smallTileDirtyWhite;
            }
        }

        #endregion

        #region Extended Concrete

        /// <summary>
        /// "concrete_parking_lot" - Concrete material for parking lots.
        /// </summary>
        public static Material ConcreteParkingLot
        {
            get
            {
                if (_concreteParkingLot == null)
                {
                    _concreteParkingLot = MaterialPresets.FindExistingMaterial("concrete_parking_lot");
                    if (_concreteParkingLot == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'concrete_parking_lot'. Using fallback.");
                        _concreteParkingLot = MaterialPresets.Opaque(new Color(0.45f, 0.45f, 0.4f));
                    }
                }
                return _concreteParkingLot;
            }
        }

        #endregion

        #region Glass

        /// <summary>
        /// "window_glass_material" - Standard window glass material.
        /// </summary>
        public static Material WindowGlass
        {
            get
            {
                if (_windowGlass == null)
                {
                    _windowGlass = MaterialPresets.FindExistingMaterial("window_glass_material");
                    if (_windowGlass == null)
                    {
                        DebugLog.Warning("[S1.Materials] Could not find 'window_glass_material'. Using fallback.");
                        _windowGlass = MaterialPresets.ClearGlass();
                    }
                }
                return _windowGlass;
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
