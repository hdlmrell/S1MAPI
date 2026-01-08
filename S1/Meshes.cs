using MAPI.Core;

namespace MAPI.S1
{
    /// <summary>
    /// Registry of known Schedule 1 static mesh assets.
    /// Use these for decorative elements that don't need game logic.
    /// For interactive items, use Prefabs instead.
    /// </summary>
    public static class Meshes
    {
        #region Doors

        /// <summary>Single door mesh.</summary>
        public static readonly MeshRef Door = new("SM_Door");

        /// <summary>Double door mesh.</summary>
        public static readonly MeshRef DoorDouble = new("SM_DoorDouble");

        /// <summary>Single door frame mesh.</summary>
        public static readonly MeshRef DoorFrame = new("SM_Door_Frame");

        /// <summary>Double door frame mesh.</summary>
        public static readonly MeshRef DoorDoubleFrame = new("SM_DoubleDoor_Frame");

        /// <summary>Garage door mesh.</summary>
        public static readonly MeshRef GarageDoor = new("SM_GarageDoor");

        /// <summary>Garage door frame mesh.</summary>
        public static readonly MeshRef GarageDoorFrame = new("SM_GarageDoor_Frame");

        /// <summary>Garage door button mesh.</summary>
        public static readonly MeshRef GarageDoorButton = new("SM_GarageDoor_Button");

        #endregion

        #region Furniture

        /// <summary>Basic chair mesh.</summary>
        public static readonly MeshRef Chair = new("Chair");

        /// <summary>Armchair mesh.</summary>
        public static readonly MeshRef Armchair = new("Armchair");

        /// <summary>Bench mesh.</summary>
        public static readonly MeshRef Bench = new("Bench");

        /// <summary>Coffee table mesh.</summary>
        public static readonly MeshRef CoffeeTable = new("CoffeeTable");

        /// <summary>Desk mesh.</summary>
        public static readonly MeshRef Desk = new("Desk");

        /// <summary>Bed mesh.</summary>
        public static readonly MeshRef Bed = new("Bed");

        /// <summary>Bedside drawer mesh.</summary>
        public static readonly MeshRef BedsideDrawer = new("Bedside_Drawer");

        /// <summary>Fridge/refrigerator mesh.</summary>
        public static readonly MeshRef Fridge = new("Fridge");

        /// <summary>Office table mesh.</summary>
        public static readonly MeshRef OfficeTable = new("Office_table");

        /// <summary>Safe mesh.</summary>
        public static readonly MeshRef Safe = new("Safe");

        /// <summary>Front desk mesh.</summary>
        public static readonly MeshRef Frontdesk = new("Frontdesk");

        /// <summary>Desk pedestal/cabinet component.</summary>
        public static readonly MeshRef DeskPedestal = new("Desk");

        #endregion

        #region Containers

        /// <summary>Basic box mesh.</summary>
        public static readonly MeshRef Box = new("Box");

        /// <summary>Barrel mesh.</summary>
        public static readonly MeshRef Barrel = new("Barrel");

        /// <summary>Plant box/planter mesh.</summary>
        public static readonly MeshRef PlantBox = new("PlantBox");

        /// <summary>Planter mesh.</summary>
        public static readonly MeshRef Planter = new("Planter");

        /// <summary>Bin/trash can mesh.</summary>
        public static readonly MeshRef Bin = new("Bin");

        /// <summary>Dumpster mesh.</summary>
        public static readonly MeshRef Dumpster = new("Dumpster");

        /// <summary>Dumpster cover/lid mesh.</summary>
        public static readonly MeshRef DumpsterCover = new("Dumpster_Cover");

        /// <summary>Cabinet mesh.</summary>
        public static readonly MeshRef Cabinet = new("Cabinet1");

        /// <summary>Drawer mesh.</summary>
        public static readonly MeshRef Drawer = new("Drawer");

        /// <summary>Locker shelf mesh.</summary>
        public static readonly MeshRef LockerShelf = new("Locker_Shelf");

        #endregion

        #region Props - Decorative

        /// <summary>
        /// Vase/bong mesh.
        /// The Bong trash item uses this mesh internally.
        /// </summary>
        public static readonly MeshRef Vase = new("SM_Prop_Vase_02");

        /// <summary>Plant prop mesh.</summary>
        public static readonly MeshRef Plant = new("SM_Prop_Plant_02");

        /// <summary>No smoking sign mesh.</summary>
        public static readonly MeshRef NoSmokingSign = new("Nonsmoking sign");

        #endregion

        #region Props - Items

        /// <summary>Cigarette mesh.</summary>
        public static readonly MeshRef Cigarette = new("SM_Prop_Cigarette_Long_01");

        /// <summary>Smoke pipe mesh.</summary>
        public static readonly MeshRef SmokePipe = new("SM_Prop_SmokePipe_01");

        /// <summary>Syringe mesh.</summary>
        public static readonly MeshRef Syringe = new("SM_Prop_Syringe_02");

        /// <summary>Beaker (lab glassware) mesh.</summary>
        public static readonly MeshRef Beaker = new("Beaker");

        /// <summary>Bottle mesh.</summary>
        public static readonly MeshRef Bottle = new("Bottle");

        /// <summary>Coffee cup mesh.</summary>
        public static readonly MeshRef Cup = new("Cup");

        /// <summary>Mug mesh.</summary>
        public static readonly MeshRef Mug = new("Mug");

        /// <summary>Basketball mesh.</summary>
        public static readonly MeshRef Basketball = new("Basketball");

        /// <summary>Computer mesh.</summary>
        public static readonly MeshRef Computer = new("Computer");

        /// <summary>Keyboard mesh.</summary>
        public static readonly MeshRef Keyboard = new("Keyboard");

        /// <summary>Mouse mesh.</summary>
        public static readonly MeshRef Mouse = new("Mouse");

        /// <summary>Monitor/screen mesh.</summary>
        public static readonly MeshRef Screen = new("Screen");

        /// <summary>Clock mesh.</summary>
        public static readonly MeshRef Clock = new("Clock");

        /// <summary>Hammer mesh.</summary>
        public static readonly MeshRef Hammer = new("Hammer");

        /// <summary>Magazine mesh.</summary>
        public static readonly MeshRef Magazine = new("Magazine");

        /// <summary>Notepad mesh.</summary>
        public static readonly MeshRef Notepad = new("Notepad");

        #endregion

        #region Props - Industrial

        /// <summary>Lab tank (small) mesh.</summary>
        public static readonly MeshRef LabTankSmall = new("SM_Prop_Lab_Tank_01");

        /// <summary>Lab tank (large) mesh.</summary>
        public static readonly MeshRef LabTankLarge = new("SM_Prop_Lab_Tank_03");

        /// <summary>Plastic barrier mesh.</summary>
        public static readonly MeshRef BarrierPlastic = new("SM_Prop_Barrier_Plastic_01");

        /// <summary>Security camera mesh.</summary>
        public static readonly MeshRef SecurityCamera = new("SM_Prop_Security_Camera_01");

        /// <summary>Power boxes mesh.</summary>
        public static readonly MeshRef PowerBoxes = new("SM_Prop_PowerBoxes_01");

        /// <summary>Rubber mat mesh.</summary>
        public static readonly MeshRef RubberMat = new("SM_Prop_RubberMat_01");

        /// <summary>Barricade mesh.</summary>
        public static readonly MeshRef Barricade = new("Barricade");

        /// <summary>Barrier mesh.</summary>
        public static readonly MeshRef Barrier = new("Barrier");

        /// <summary>Bollard mesh.</summary>
        public static readonly MeshRef Bollard = new("Bollard");

        /// <summary>Generator mesh.</summary>
        public static readonly MeshRef Generator = new("SM_Prop_Generator_01_Convex");

        /// <summary>Cauldron mesh.</summary>
        public static readonly MeshRef Cauldron = new("Cauldron");

        /// <summary>Conveyor mesh.</summary>
        public static readonly MeshRef Conveyor = new("Conveyor");

        /// <summary>Extractor mesh.</summary>
        public static readonly MeshRef Extractor = new("Extractor");

        /// <summary>Mixer mesh.</summary>
        public static readonly MeshRef Mixer = new("Mixer");

        /// <summary>Fusebox mesh.</summary>
        public static readonly MeshRef Fusebox = new("Fusebox2");

        /// <summary>Alarm mesh.</summary>
        public static readonly MeshRef Alarm = new("Alarm");

        /// <summary>Intercom mesh.</summary>
        public static readonly MeshRef Intercom = new("Intercom");

        /// <summary>Light switch mesh.</summary>
        public static readonly MeshRef Lightswitch = new("Lightswitch");

        #endregion

        #region Outdoor & Street

        /// <summary>Mailbox mesh.</summary>
        public static readonly MeshRef Mailbox = new("Mailbox1");

        /// <summary>Garbage bag mesh.</summary>
        public static readonly MeshRef GarbageBag = new("Garbage bag");

        /// <summary>Arcade machine mesh.</summary>
        public static readonly MeshRef Arcade = new("Arcade");

        /// <summary>Billboard mesh.</summary>
        public static readonly MeshRef Billboard = new("Billboard");

        /// <summary>Bus stop mesh.</summary>
        public static readonly MeshRef BusStop = new("Bus stop (2024112815191238)");

        /// <summary>Pay phone mesh.</summary>
        public static readonly MeshRef PayPhone = new("PayPhone_low");

        /// <summary>Pallet rack mesh.</summary>
        public static readonly MeshRef PalletRack = new("Pallet rack (20241127234446952)");

        /// <summary>Outdoor fence mesh.</summary>
        public static readonly MeshRef OutdoorFence = new("2m outdoor fence (202532147291)");

        /// <summary>Fence mesh.</summary>
        public static readonly MeshRef Fence = new("Fence");

        /// <summary>Basketball hoop mesh.</summary>
        public static readonly MeshRef Hoop = new("Hoop");

        #endregion

        #region Building Components

        /// <summary>Pillar mesh.</summary>
        public static readonly MeshRef Pillar = new("SM_Pillar");

        /// <summary>Ladder mesh.</summary>
        public static readonly MeshRef Ladder = new("SM_Ladder");

        /// <summary>Small wall segment mesh.</summary>
        public static readonly MeshRef WallSmall = new("SM_SmallWall");

        /// <summary>Medium wall segment mesh.</summary>
        public static readonly MeshRef WallMedium = new("SM_MediumWall");

        /// <summary>Large wall segment mesh.</summary>
        public static readonly MeshRef WallLarge = new("SM_LargelWall");

        /// <summary>Side wall segment mesh.</summary>
        public static readonly MeshRef WallSide = new("SM_SideWall");

        /// <summary>Small window mesh.</summary>
        public static readonly MeshRef WindowSmall = new("SM_SmallWindow");

        #endregion

        #region HVAC

        /// <summary>AC unit mesh.</summary>
        public static readonly MeshRef ACUnit = new("SM_AC_Unit");

        /// <summary>AC exterior unit mesh.</summary>
        public static readonly MeshRef AC = new("SM_AC");

        /// <summary>AC interior unit mesh.</summary>
        public static readonly MeshRef ACInteriorUnit = new("AC_Interior_Unit");

        /// <summary>AC pipe (long) mesh.</summary>
        public static readonly MeshRef ACPipeLong = new("AC_Pipe_Long");

        /// <summary>AC pipe (medium) mesh.</summary>
        public static readonly MeshRef ACPipeMedium = new("AC_Pipe_Medium");

        /// <summary>AC pipe side (right) mesh.</summary>
        public static readonly MeshRef ACPipeSideRight = new("AC_Pipe_Side_Right");

        /// <summary>AC pipe side (left) mesh.</summary>
        public static readonly MeshRef ACPipeSideLeft = new("AC_Pipe_Side_left");

        #endregion

        #region Weapons

        /// <summary>Revolver mesh.</summary>
        public static readonly MeshRef Revolver = new("Revolver");

        /// <summary>Machete mesh.</summary>
        public static readonly MeshRef Machete = new("Machete");

        /// <summary>Frying Pan mesh.</summary>
        public static readonly MeshRef FryingPan = new("FryingPan");

        /// <summary>M1911 mesh.</summary>
        public static readonly MeshRef M1911 = new("M1911");

        /// <summary>Baseball Bat mesh.</summary>
        public static readonly MeshRef BaseballBat = new("BaseballBat");

        #endregion

        #region Additional Decorations

        /// <summary>Brut Dug Loop mesh.</summary>
        public static readonly MeshRef BrutDugLoop = new("brutdugloop");

        /// <summary>Chateau La Pee Pee mesh.</summary>
        public static readonly MeshRef ChateauLaPeePee = new("chateaulapeepee");

        /// <summary>Gold bar mesh.</summary>
        public static readonly MeshRef GoldBar = new("goldbar");

        /// <summary>Jukebox mesh.</summary>
        public static readonly MeshRef Jukebox = new("jukebox");

        /// <summary>Metal sign mesh.</summary>
        public static readonly MeshRef MetalSign = new("metalsign");

        /// <summary>Old Man Jimmys mesh.</summary>
        public static readonly MeshRef OldManJimmys = new("oldmanjimmys");

        /// <summary>Paintings mesh.</summary>
        public static readonly MeshRef Paintings = new("paintings");

        /// <summary>Toilet mesh.</summary>
        public static readonly MeshRef Toilet = new("toilet");

        /// <summary>Wall clock mesh.</summary>
        public static readonly MeshRef WallClock = new("wallclock");

        /// <summary>Wooden sign mesh.</summary>
        public static readonly MeshRef WoodenSign = new("woodensign");

        #endregion

        #region Utilities

        /// <summary>
        /// Create a custom MeshRef for a mesh not in the registry.
        /// </summary>
        /// <param name="meshName">Exact mesh name in game assets</param>
        /// <returns>New MeshRef instance</returns>
        public static MeshRef Custom(string meshName) => new(meshName);

        #endregion
    }
}
