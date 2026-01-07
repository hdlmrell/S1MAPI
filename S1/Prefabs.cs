using MAPI.Core;

namespace MAPI.S1
{
    /// <summary>
    /// Registry of known Schedule 1 network-spawnable prefabs.
    /// These have full game logic: NetworkObject, pickup/drop, interaction triggers, etc.
    /// Use these for functional items that need multiplayer sync or player interaction.
    /// For static decoration meshes, use Meshes instead.
    /// </summary>
    public static class Prefabs
    {
        #region Doors

        /// <summary>
        /// Automatic sliding double doors (as seen in car dealership).
        /// </summary>
        public static readonly PrefabRef SlidingDoors = new("Dealership Sliding Doors");

        /// <summary>
        /// Sliding glass door.
        /// </summary>
        public static readonly PrefabRef SlidingGlassDoor = new("Sliding Glass Door");

        /// <summary>
        /// Basic metal and glass door.
        /// </summary>
        public static readonly PrefabRef MetalGlassDoor = new("Basic Metal Glass Door");

        /// <summary>
        /// Classical wooden door.
        /// </summary>
        public static readonly PrefabRef ClassicalWoodenDoor = new("Classical Wooden door");

        /// <summary>
        /// Industrial metal door with peephole.
        /// </summary>
        public static readonly PrefabRef IndustrialMetalDoorPeephole = new("Industrial Metal Door Peephole");

        /// <summary>
        /// Industrial metal door.
        /// </summary>
        public static readonly PrefabRef IndustrialMetalDoor = new("Industrial Metal Door");

        /// <summary>
        /// Base door (simple door template).
        /// </summary>
        public static readonly PrefabRef BaseDoor = new("BaseDoor");

        #endregion

        #region Items

        /// <summary>
        /// Glass bong item.
        /// </summary>
        public static readonly PrefabRef Bong = new("Bong_Trash");

        /// <summary>
        /// Cash register.
        /// </summary>
        public static readonly PrefabRef CashRegister = new("CashRegister");

        /// <summary>
        /// ATM machine.
        /// </summary>
        public static readonly PrefabRef ATM = new("ATM");

        /// <summary>
        /// Cigarette box (trash/empty).
        /// </summary>
        public static readonly PrefabRef CigaretteBox = new("CigaretteBox_Trash");

        /// <summary>
        /// Lit cigarette item.
        /// </summary>
        public static readonly PrefabRef CigaretteLit = new("Cigarette_Lit");

        /// <summary>
        /// Cigarette (trash).
        /// </summary>
        public static readonly PrefabRef Cigarette = new("Cigarette_Trash");

        /// <summary>
        /// Used cigarette (trash).
        /// </summary>
        public static readonly PrefabRef CigaretteUsed = new("Cigarette_Used_Trash");

        /// <summary>
        /// Beaker item for chemistry.
        /// </summary>
        public static readonly PrefabRef Beaker = new("Beaker");

        /// <summary>
        /// Stirring rod item.
        /// </summary>
        public static readonly PrefabRef StirringRod = new("StirringRod");

        /// <summary>
        /// Measuring jug item.
        /// </summary>
        public static readonly PrefabRef MeasuringJug = new("MeasuringJug");

        /// <summary>
        /// Hammer tool.
        /// </summary>
        public static readonly PrefabRef Hammer = new("Hammer");

        #endregion

        #region Weapons

        /// <summary>Baseball bat weapon.</summary>
        public static readonly PrefabRef BaseballBat = new("BaseballBat_");

        /// <summary>Frying pan weapon.</summary>
        public static readonly PrefabRef FryingPan = new("FryingPan_");

        /// <summary>M1911 pistol weapon.</summary>
        public static readonly PrefabRef M1911 = new("M1911_");

        /// <summary>Machete weapon.</summary>
        public static readonly PrefabRef Machete = new("Machete_");

        /// <summary>Revolver weapon.</summary>
        public static readonly PrefabRef Revolver = new("Revolver_");

        /// <summary>M1911 Magazine.</summary>
        public static readonly PrefabRef M1911Magazine = new("Magazine_");

        /// <summary>Revolver Cylinder.</summary>
        public static readonly PrefabRef RevolverCylinder = new("RevolverCylinder_");

        #endregion

        #region Skateboards

        /// <summary>Standard skateboard.</summary>
        public static readonly PrefabRef Skateboard = new("Skateboard");

        /// <summary>Cheap skateboard.</summary>
        public static readonly PrefabRef CheapSkateboard = new("CheapSkateboard");

        /// <summary>Cruiser skateboard.</summary>
        public static readonly PrefabRef CruiserSkateboard = new("Cruiser");

        /// <summary>Golden skateboard.</summary>
        public static readonly PrefabRef GoldenSkateboard = new("GoldSkateboard");

        /// <summary>Lightweight skateboard.</summary>
        public static readonly PrefabRef LightweightSkateboard = new("LightweightSkateboard");

        #endregion

        #region Lights

        /// <summary>Antique wall lamp (Built).</summary>
        public static readonly PrefabRef AntiqueWallLamp = new("AntiqueWallLamp_Built");

        /// <summary>Floor lamp.</summary>
        public static readonly PrefabRef FloorLamp = new("FloorLamp");

        /// <summary>Full spectrum light (Built).</summary>
        public static readonly PrefabRef FullSpectrumLight = new("FullSpectrumLight_Built");

        /// <summary>Halogen light.</summary>
        public static readonly PrefabRef HalogenLight = new("HalogenLight");

        /// <summary>LED light.</summary>
        public static readonly PrefabRef LEDLight = new("LEDLight");

        /// <summary>Modern wall lamp (Built).</summary>
        public static readonly PrefabRef ModernWallLamp = new("ModernWallLamp_Built");

        #endregion

        #region Furniture

        /// <summary>
        /// Metal shelf unit.
        /// </summary>
        public static readonly PrefabRef MetalShelf = new("Metal Shelf");

        /// <summary>
        /// Wooden crate.
        /// </summary>
        public static readonly PrefabRef WoodenCrate = new("Wooden Crate");

        /// <summary>
        /// Coffee table with item storage functionality.
        /// </summary>
        public static readonly PrefabRef CoffeeTable = new("CoffeeTable_Built");

        /// <summary>
        /// Metal square table.
        /// </summary>
        public static readonly PrefabRef MetalSquareTable = new("MetalSquareTable");

        /// <summary>
        /// Wood square table.
        /// </summary>
        public static readonly PrefabRef WoodSquareTable = new("WoodSquareTable");

        /// <summary>
        /// Single bed.
        /// </summary>
        public static readonly PrefabRef SingleBed = new("SingleBed");

        /// <summary>
        /// Floor rack for storage.
        /// </summary>
        public static readonly PrefabRef FloorRack = new("FloorRack");

        /// <summary>
        /// Grandfather clock.
        /// </summary>
        public static readonly PrefabRef GrandfatherClock = new("GrandfatherClock_Built");

        /// <summary>
        /// TV with interactive functionality.
        /// </summary>
        public static readonly PrefabRef TV = new("TV_Built");

        /// <summary>
        /// Standard TV (furniture/tv).
        /// </summary>
        public static readonly PrefabRef Television = new("tv");

        /// <summary>
        /// Safe for secure storage.
        /// </summary>
        public static readonly PrefabRef Safe = new("Safe");

        /// <summary>
        /// Small safe for secure storage.
        /// </summary>
        public static readonly PrefabRef SmallSafe = new("Small Safe");

        /// <summary>
        /// Dumpster with storage capacity.
        /// </summary>
        public static readonly PrefabRef Dumpster = new("Dumpster_Built");

        /// <summary>
        /// Metal trash can.
        /// </summary>
        public static readonly PrefabRef TrashCan = new("TrashCan_Built");

        /// <summary>
        /// Small trash can.
        /// </summary>
        public static readonly PrefabRef SmallTrashCan = new("SmallTrashCan_Built");

        /// <summary>
        /// Wall mounted shelf for storage.
        /// </summary>
        public static readonly PrefabRef WallMountedShelf = new("WallMountedShelf_Built");

        /// <summary>
        /// Display cabinet for items.
        /// </summary>
        public static readonly PrefabRef DisplayCabinet = new("DisplayCabinet_Built");

        /// <summary>
        /// Filing cabinet for documents and storage.
        /// </summary>
        public static readonly PrefabRef FilingCabinet = new("FilingCabinet_Built");

        /// <summary>
        /// Safe (Built version).
        /// </summary>
        public static readonly PrefabRef SafeBuilt = new("Safe_Built");

        #endregion

        #region Security

        /// <summary>
        /// Functional passcode panel for secure access control.
        /// </summary>
        public static readonly PrefabRef PasscodePanel = new("Passcode Panel (Functional)");

        /// <summary>
        /// Modular switch for electrical control.
        /// </summary>
        public static readonly PrefabRef ModularSwitch = new("ModularSwitch");

        #endregion

        #region Production Equipment

        /// <summary>
        /// Brick press for compressing materials.
        /// </summary>
        public static readonly PrefabRef BrickPress = new("BrickPress");

        /// <summary>
        /// Cauldron for cooking/brewing.
        /// </summary>
        public static readonly PrefabRef Cauldron = new("Cauldron_Built");

        /// <summary>
        /// Chemistry station for chemical processing.
        /// </summary>
        public static readonly PrefabRef ChemistryStation = new("ChemistryStation_Built");

        /// <summary>
        /// Drying rack for drying materials.
        /// </summary>
        public static readonly PrefabRef DryingRack = new("DryingRack_Built");

        /// <summary>
        /// Lab oven for heating/processing.
        /// </summary>
        public static readonly PrefabRef LabOven = new("LabOven_Built");

        /// <summary>
        /// Laundering station for money laundering.
        /// </summary>
        public static readonly PrefabRef LaunderingStation = new("LaunderingStation_Built");

        /// <summary>
        /// Mixing station for combining ingredients.
        /// </summary>
        public static readonly PrefabRef MixingStation = new("MixingStation_Built");

        /// <summary>
        /// Mixing station Mk2 (upgraded version).
        /// </summary>
        public static readonly PrefabRef MixingStationMk2 = new("MixingStationMk2_Built");

        /// <summary>
        /// Packaging station for packaging products.
        /// </summary>
        public static readonly PrefabRef PackagingStation = new("PackagingStation");

        /// <summary>
        /// Packaging station Mk2 (upgraded version).
        /// </summary>
        public static readonly PrefabRef PackagingStationMk2 = new("PackagingStation_Mk2");

        #endregion

        #region Lighting

        /// <summary>
        /// Ceiling fluorescent light.
        /// </summary>
        public static readonly PrefabRef FluorescentLight = new("Fluorescent Light");

        #endregion

        #region Utilities

        /// <summary>
        /// Create a custom PrefabRef for a prefab not in the registry.
        /// </summary>
        /// <param name="prefabName">Exact prefab name in FishNet registry</param>
        /// <returns>New PrefabRef instance</returns>
        public static PrefabRef Custom(string prefabName) => new(prefabName);

        #endregion
    }
}
