using UnityEngine;
using MAPI.Utils;
using MAPI.S1;
using System;
using System.Collections.Generic;
using MAPI.Building.Components;

namespace MAPI.Building.Interior
{
    /// <summary>
    /// Fluent builder for creating interior furnishings and decorations using Schedule 1 assets
    /// Based on patterns from PetShopSpawner interior system
    /// </summary>
    public class InteriorBuilder
    {
        #region Fields
        
        private readonly string _name;
        private readonly List<GameObject> _furniture = new List<GameObject>();
        private Transform? _parent;
        private GameObject? _interiorContainer;
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Create a new interior builder
        /// </summary>
        /// <param name="name">Name for the interior container</param>
        public InteriorBuilder(string name = "Interior")
        {
            _name = name;
        }

        /// <summary>
        /// Create a new interior builder with a parent transform
        /// </summary>
        /// <param name="parent">Parent transform to attach furniture to</param>
        /// <param name="name">Name for the interior container</param>
        public InteriorBuilder(Transform parent, string name = "Interior")
        {
            _name = name;
            _parent = parent;
        }
        
        #endregion

        #region Private Methods

        private GameObject GetOrCreateContainer()
        {
            if (_interiorContainer != null)
            {
                return _interiorContainer;
            }

            if (_parent == null)
            {
                DebugLog.Warning("InteriorBuilder: No parent set. Furniture will not be organized in a container.");
                return null!;
            }

            _interiorContainer = BuildingUtilities.CreateFolder(_name, _parent);
            return _interiorContainer;
        }

        private void ParentToContainer(GameObject obj, Vector3 localPosition, Quaternion? rotation = null)
        {
            GameObject? container = GetOrCreateContainer();
            if (container == null)
            {
                return;
            }

            obj.transform.SetParent(container.transform);
            obj.transform.localPosition = localPosition;
            obj.transform.localRotation = rotation ?? Quaternion.identity;
        }

        #endregion

        #region Public API - Furniture

        /// <summary>
        /// Add a desk/counter to the interior using S1 desk mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddDesk(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? desk = Meshes.Desk.Instantiate("Desk", position, rotation ?? Quaternion.identity);
            
            if (desk != null)
            {
                ParentToContainer(desk, position, rotation);
                _furniture.Add(desk);
                BuildingUtilities.AddNavMeshObstacle(desk);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Desk mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a locker to the interior using S1 locker shelf mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddLocker(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? locker = Meshes.LockerShelf.Instantiate("Locker", position, rotation ?? Quaternion.identity);
            
            if (locker != null)
            {
                ParentToContainer(locker, position, rotation);
                _furniture.Add(locker);
                BuildingUtilities.AddNavMeshObstacle(locker);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Locker mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a chair to the interior using S1 chair mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddChair(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? chair = Meshes.Chair.Instantiate("Chair", position, rotation ?? Quaternion.identity);
            
            if (chair != null)
            {
                ParentToContainer(chair, position, rotation);
                _furniture.Add(chair);
                BuildingUtilities.AddNavMeshObstacle(chair);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Chair mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add an armchair to the interior using S1 armchair mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddArmchair(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? armchair = Meshes.Armchair.Instantiate("Armchair", position, rotation ?? Quaternion.identity);
            
            if (armchair != null)
            {
                ParentToContainer(armchair, position, rotation);
                _furniture.Add(armchair);
                BuildingUtilities.AddNavMeshObstacle(armchair);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Armchair mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a table to the interior using S1 coffee table mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddTable(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? table = Meshes.CoffeeTable.Instantiate("Table", position, rotation ?? Quaternion.identity);
            
            if (table != null)
            {
                ParentToContainer(table, position, rotation);
                _furniture.Add(table);
                BuildingUtilities.AddNavMeshObstacle(table);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Table mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add an office table to the interior using S1 office table mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddOfficeTable(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? table = Meshes.OfficeTable.Instantiate("OfficeTable", position, rotation ?? Quaternion.identity);
            
            if (table != null)
            {
                ParentToContainer(table, position, rotation);
                _furniture.Add(table);
                BuildingUtilities.AddNavMeshObstacle(table);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Office Table mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a bench to the interior using S1 bench mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddBench(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? bench = Meshes.Bench.Instantiate("Bench", position, rotation ?? Quaternion.identity);
            
            if (bench != null)
            {
                ParentToContainer(bench, position, rotation);
                _furniture.Add(bench);
                BuildingUtilities.AddNavMeshObstacle(bench);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Bench mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a bed to the interior using S1 bed mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddBed(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? bed = Meshes.Bed.Instantiate("Bed", position, rotation ?? Quaternion.identity);
            
            if (bed != null)
            {
                ParentToContainer(bed, position, rotation);
                _furniture.Add(bed);
                BuildingUtilities.AddNavMeshObstacle(bed);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Bed mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a fridge to the interior using S1 fridge mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddFridge(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? fridge = Meshes.Fridge.Instantiate("Fridge", position, rotation ?? Quaternion.identity);
            
            if (fridge != null)
            {
                ParentToContainer(fridge, position, rotation);
                _furniture.Add(fridge);
                BuildingUtilities.AddNavMeshObstacle(fridge);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Fridge mesh");
            }
            
            return this;
        }
        
        #endregion

        #region Public API - Containers

        /// <summary>
        /// Add a decorative box/crate using S1 box mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddBox(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? box = Meshes.Box.Instantiate("Box", position, rotation ?? Quaternion.identity);
            
            if (box != null)
            {
                ParentToContainer(box, position, rotation);
                _furniture.Add(box);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Box mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a barrel using S1 barrel mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddBarrel(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? barrel = Meshes.Barrel.Instantiate("Barrel", position, rotation ?? Quaternion.identity);
            
            if (barrel != null)
            {
                ParentToContainer(barrel, position, rotation);
                _furniture.Add(barrel);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Barrel mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a cabinet using S1 cabinet mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddCabinet(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? cabinet = Meshes.Cabinet.Instantiate("Cabinet", position, rotation ?? Quaternion.identity);
            
            if (cabinet != null)
            {
                ParentToContainer(cabinet, position, rotation);
                _furniture.Add(cabinet);
                BuildingUtilities.AddNavMeshObstacle(cabinet);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Cabinet mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a drawer using S1 drawer mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddDrawer(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? drawer = Meshes.Drawer.Instantiate("Drawer", position, rotation ?? Quaternion.identity);
            
            if (drawer != null)
            {
                ParentToContainer(drawer, position, rotation);
                _furniture.Add(drawer);
                BuildingUtilities.AddNavMeshObstacle(drawer);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Drawer mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a safe using S1 safe mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddSafe(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? safe = Meshes.Safe.Instantiate("Safe", position, rotation ?? Quaternion.identity);
            
            if (safe != null)
            {
                ParentToContainer(safe, position, rotation);
                _furniture.Add(safe);
                BuildingUtilities.AddNavMeshObstacle(safe);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Safe mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a bin/trash can using S1 bin mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddBin(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? bin = Meshes.Bin.Instantiate("Bin", position, rotation ?? Quaternion.identity);
            
            if (bin != null)
            {
                ParentToContainer(bin, position, rotation);
                _furniture.Add(bin);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Bin mesh");
            }
            
            return this;
        }
        
        #endregion

        #region Public API - Decorations

        /// <summary>
        /// Add a plant decoration using S1 plant mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddPlant(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? plant = Meshes.Plant.Instantiate("Plant", position, rotation ?? Quaternion.identity);
            
            if (plant != null)
            {
                ParentToContainer(plant, position, rotation);
                _furniture.Add(plant);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Plant mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a planter using S1 planter mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddPlanter(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? planter = Meshes.Planter.Instantiate("Planter", position, rotation ?? Quaternion.identity);
            
            if (planter != null)
            {
                ParentToContainer(planter, position, rotation);
                _furniture.Add(planter);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Planter mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a vase decoration using S1 vase mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddVase(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? vase = Meshes.Vase.Instantiate("Vase", position, rotation ?? Quaternion.identity);
            
            if (vase != null)
            {
                ParentToContainer(vase, position, rotation);
                _furniture.Add(vase);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Vase mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a painting decoration using S1 paintings mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddPainting(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? painting = Meshes.Paintings.Instantiate("Painting", position, rotation ?? Quaternion.identity);
            
            if (painting != null)
            {
                ParentToContainer(painting, position, rotation);
                _furniture.Add(painting);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Painting mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a clock using S1 clock mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddClock(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? clock = Meshes.Clock.Instantiate("Clock", position, rotation ?? Quaternion.identity);
            
            if (clock != null)
            {
                ParentToContainer(clock, position, rotation);
                _furniture.Add(clock);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Clock mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a wall clock using S1 wall clock mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddWallClock(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? wallClock = Meshes.WallClock.Instantiate("WallClock", position, rotation ?? Quaternion.identity);
            
            if (wallClock != null)
            {
                ParentToContainer(wallClock, position, rotation);
                _furniture.Add(wallClock);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Wall Clock mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a toilet using S1 toilet mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddToilet(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? toilet = Meshes.Toilet.Instantiate("Toilet", position, rotation ?? Quaternion.identity);
            
            if (toilet != null)
            {
                ParentToContainer(toilet, position, rotation);
                _furniture.Add(toilet);
                BuildingUtilities.AddNavMeshObstacle(toilet);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Toilet mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a computer using S1 computer mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddComputer(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? computer = Meshes.Computer.Instantiate("Computer", position, rotation ?? Quaternion.identity);
            
            if (computer != null)
            {
                ParentToContainer(computer, position, rotation);
                _furniture.Add(computer);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Computer mesh");
            }
            
            return this;
        }

        /// <summary>
        /// Add a screen/monitor using S1 screen mesh
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddScreen(Vector3 position, Quaternion? rotation = null)
        {
            GameObject? screen = Meshes.Screen.Instantiate("Screen", position, rotation ?? Quaternion.identity);
            
            if (screen != null)
            {
                ParentToContainer(screen, position, rotation);
                _furniture.Add(screen);
            }
            else
            {
                DebugLog.Warning("Failed to instantiate Screen mesh");
            }
            
            return this;
        }
        
        #endregion

        #region Public API - Custom Objects
        
        /// <summary>
        /// Add a custom GameObject to the interior
        /// </summary>
        /// <param name="gameObject">The GameObject to add</param>
        /// <param name="position">Local position</param>
        public InteriorBuilder AddCustomObject(GameObject gameObject, Vector3 position)
        {
            if (gameObject != null)
            {
                ParentToContainer(gameObject, position);
                _furniture.Add(gameObject);
            }

            return this;
        }

        /// <summary>
        /// Add a custom S1 mesh to the interior
        /// </summary>
        /// <param name="meshRef">The S1 MeshRef to instantiate</param>
        /// <param name="name">Name for the GameObject</param>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        public InteriorBuilder AddCustomMesh(Core.MeshRef meshRef, string name, Vector3 position, Quaternion? rotation = null)
        {
            GameObject? obj = meshRef.Instantiate(name, position, rotation ?? Quaternion.identity, _parent);
            
            if (obj != null)
            {
                ParentToContainer(obj, position, rotation);
                _furniture.Add(obj);
            }
            else
            {
                DebugLog.Warning($"Failed to instantiate custom mesh: {name}");
            }

            return this;
        }

        /// <summary>
        /// Add a prefab using PrefabPlacer pattern for networked/functional objects
        /// </summary>
        /// <param name="prefab">The prefab reference to instantiate</param>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Rotation (defaults to identity)</param>
        /// <param name="networked">Whether this should be a networked object</param>
        /// <param name="enableComponents">Whether to enable components on the prefab</param>
        /// <param name="onCreated">Optional callback after creation</param>
        public InteriorBuilder AddPrefab(
            Core.PrefabRef prefab, 
            Vector3 position, 
            Quaternion? rotation = null, 
            bool networked = false,
            bool enableComponents = true,
            Action<GameObject>? onCreated = null)
        {
            if (_parent == null)
            {
                DebugLog.Warning("Cannot add prefab without parent transform. Use constructor with parent or call SetParent first.");
                return this;
            }

            PrefabPlacer placer = new PrefabPlacer(_parent);
            GameObject? obj = placer.Place(
                prefab, 
                position, 
                rotation ?? Quaternion.identity, 
                networked, 
                enableComponents);

            if (obj != null)
            {
                ParentToContainer(obj, position, rotation);
                _furniture.Add(obj);
                onCreated?.Invoke(obj);
            }
            else
            {
                DebugLog.Warning($"Failed to place prefab");
            }

            return this;
        }
        
        #endregion

        #region Public API - Configuration
        
        /// <summary>
        /// Set the parent transform for all future furniture placements
        /// </summary>
        /// <param name="parent">Parent transform</param>
        public InteriorBuilder SetParent(Transform parent)
        {
            _parent = parent;
            return this;
        }
        
        #endregion

        #region Public API - Build
        
        /// <summary>
        /// Build the interior and attach all furniture to the parent
        /// </summary>
        /// <param name="parent">Parent GameObject to attach furniture to (overrides constructor parent)</param>
        /// <returns>Array of created furniture GameObjects</returns>
        public GameObject[] Build(GameObject parent)
        {
            if (parent == null)
            {
                DebugLog.Warning("Interior parent is null, furniture will not be parented");
                return _furniture.ToArray();
            }

            if (_parent == null || parent != _parent.gameObject)
            {
                _parent = parent.transform;
                _interiorContainer = null;
            }

            GameObject interiorFolder = GetOrCreateContainer();

            foreach (GameObject furniture in _furniture)
            {
                if (furniture != null && furniture.transform.parent == null)
                {
                    furniture.transform.SetParent(interiorFolder.transform);
                }
            }

            DebugLog.Info($"Built interior: {_name} with {_furniture.Count} pieces");
            return _furniture.ToArray();
        }

        /// <summary>
        /// Build the interior using the parent transform set in constructor
        /// </summary>
        /// <returns>Array of created furniture GameObjects</returns>
        public GameObject[] Build()
        {
            if (_parent == null)
            {
                DebugLog.Warning("No parent set for interior builder. Use Build(GameObject) or set parent in constructor.");
                return _furniture.ToArray();
            }

            return Build(_parent.gameObject);
        }

        /// <summary>
        /// Get all furniture objects without building
        /// </summary>
        public GameObject[] GetFurniture() => _furniture.ToArray();
        
        #endregion
    }
}
