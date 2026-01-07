using UnityEngine;
using MAPI.Utils;

namespace MAPI.Building
{
    /// <summary>
    /// Fluent builder for creating interior furnishings and decorations
    /// Based on patterns from PetShopSpawner interior system
    /// </summary>
    public class InteriorBuilder
    {
        #region Fields
        
        private readonly string _name;
        private readonly List<GameObject> _furniture = new List<GameObject>();
        private readonly Color _woodColor = new Color(0.5f, 0.35f, 0.25f);
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Create a new interior builder
        /// </summary>
        public InteriorBuilder(string name = "Interior")
        {
            _name = name;
        }
        
        #endregion

        #region Public API - Furniture
        
        /// <summary>
        /// Add a desk/counter to the interior
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="size">Size of the desk</param>
        /// <param name="color">Color of the desk (defaults to wood color)</param>
        public InteriorBuilder AddDesk(Vector3 position, Vector3 size, Color? color = null)
        {
            Color deskColor = color ?? _woodColor;

            GameObject desk = PrimitiveBuilder.CreateBox(
                "Desk",
                position,
                size,
                deskColor
            );

            _furniture.Add(desk);
            BuildingUtilities.AddNavMeshObstacle(desk);
            return this;
        }

        /// <summary>
        /// Add a shelf to the interior
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="size">Size of the shelf</param>
        /// <param name="color">Color of the shelf (defaults to wood color)</param>
        public InteriorBuilder AddShelf(Vector3 position, Vector3 size, Color? color = null)
        {
            Color shelfColor = color ?? _woodColor;

            GameObject shelf = PrimitiveBuilder.CreateBox(
                "Shelf",
                position,
                size,
                shelfColor
            );

            _furniture.Add(shelf);
            BuildingUtilities.AddNavMeshObstacle(shelf);
            return this;
        }

        /// <summary>
        /// Add a chair to the interior
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="color">Color of the chair</param>
        public InteriorBuilder AddChair(Vector3 position, Color? color = null)
        {
            Color chairColor = color ?? _woodColor;

            // Seat
            GameObject seat = PrimitiveBuilder.CreateBox(
                "Chair_Seat",
                position + new Vector3(0f, 0.25f, 0f),
                new Vector3(0.4f, 0.05f, 0.4f),
                chairColor
            );
            _furniture.Add(seat);

            // Backrest
            GameObject backrest = PrimitiveBuilder.CreateBox(
                "Chair_Backrest",
                position + new Vector3(0f, 0.5f, -0.175f),
                new Vector3(0.4f, 0.5f, 0.05f),
                chairColor
            );
            backrest.transform.SetParent(seat.transform);
            _furniture.Add(backrest);

            // Legs (4 corners)
            float legRadius = 0.025f;
            float legHeight = 0.25f;
            Vector3[] legPositions = new Vector3[]
            {
                new Vector3(-0.175f, 0f, -0.175f),
                new Vector3(0.175f, 0f, -0.175f),
                new Vector3(-0.175f, 0f, 0.175f),
                new Vector3(0.175f, 0f, 0.175f)
            };

            for (int i = 0; i < 4; i++)
            {
                GameObject leg = PrimitiveBuilder.CreateCylinder(
                    $"Chair_Leg_{i}",
                    position + legPositions[i],
                    new Vector3(legRadius * 2f, legHeight, legRadius * 2f),
                    chairColor
                );
                leg.transform.SetParent(seat.transform);
                _furniture.Add(leg);
            }

            return this;
        }

        /// <summary>
        /// Add a table to the interior
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="size">Size of the table</param>
        /// <param name="color">Color of the table</param>
        public InteriorBuilder AddTable(Vector3 position, Vector3 size, Color? color = null)
        {
            Color tableColor = color ?? _woodColor;

            // Table top
            GameObject tabletop = PrimitiveBuilder.CreateBox(
                "Table_Top",
                position + new Vector3(0f, size.y * 0.9f, 0f),
                new Vector3(size.x, size.y * 0.1f, size.z),
                tableColor
            );
            _furniture.Add(tabletop);
            BuildingUtilities.AddNavMeshObstacle(tabletop);

            // Legs
            float legRadius = 0.05f;
            float legHeight = size.y * 0.85f;
            Vector3[] legOffsets = new Vector3[]
            {
                new Vector3(-size.x * 0.4f, legHeight / 2f, -size.z * 0.4f),
                new Vector3(size.x * 0.4f, legHeight / 2f, -size.z * 0.4f),
                new Vector3(-size.x * 0.4f, legHeight / 2f, size.z * 0.4f),
                new Vector3(size.x * 0.4f, legHeight / 2f, size.z * 0.4f)
            };

            for (int i = 0; i < 4; i++)
            {
                GameObject leg = PrimitiveBuilder.CreateCylinder(
                    $"Table_Leg_{i}",
                    position + legOffsets[i],
                    new Vector3(legRadius * 2f, legHeight, legRadius * 2f),
                    tableColor
                );
                leg.transform.SetParent(tabletop.transform);
                _furniture.Add(leg);
            }

            return this;
        }
        
        #endregion

        #region Public API - Decorations
        
        /// <summary>
        /// Add a rug to the interior
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="size">Size of the rug</param>
        /// <param name="color">Color of the rug</param>
        public InteriorBuilder AddRug(Vector3 position, Vector2 size, Color color)
        {
            GameObject rug = PrimitiveBuilder.CreateBox(
                "Rug",
                position,
                new Vector3(size.x, 0.01f, size.y),
                color
            );

            _furniture.Add(rug);
            BuildingUtilities.RemoveColliders(rug); // Rugs shouldn't block movement
            return this;
        }

        /// <summary>
        /// Add a decorative box/crate
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="size">Size of the box</param>
        /// <param name="color">Color of the box</param>
        public InteriorBuilder AddBox(Vector3 position, Vector3 size, Color color)
        {
            GameObject box = PrimitiveBuilder.CreateBox(
                "DecorativeBox",
                position,
                size,
                color
            );

            _furniture.Add(box);
            return this;
        }

        /// <summary>
        /// Add a cylindrical decoration (barrel, pot, etc.)
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="radius">Radius of the cylinder</param>
        /// <param name="height">Height of the cylinder</param>
        /// <param name="color">Color</param>
        public InteriorBuilder AddCylinder(Vector3 position, float radius, float height, Color color)
        {
            GameObject cylinder = PrimitiveBuilder.CreateCylinder(
                "DecorativeCylinder",
                position,
                new Vector3(radius * 2f, height, radius * 2f),
                color
            );

            _furniture.Add(cylinder);
            return this;
        }

        /// <summary>
        /// Add a spherical decoration
        /// </summary>
        /// <param name="position">Local position</param>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="color">Color</param>
        public InteriorBuilder AddSphere(Vector3 position, float radius, Color color)
        {
            GameObject sphere = PrimitiveBuilder.CreateSphere(
                "DecorativeSphere",
                position,
                radius,
                color
            );

            _furniture.Add(sphere);
            return this;
        }
        
        #endregion

        #region Public API - Shelving System
        
        /// <summary>
        /// Add a wall-mounted shelf system
        /// </summary>
        /// <param name="wallPosition">Position along the wall</param>
        /// <param name="tiers">Number of vertical tiers</param>
        /// <param name="shelfSize">Size of each shelf</param>
        /// <param name="spacing">Vertical spacing between shelves</param>
        /// <param name="color">Color of shelves</param>
        public InteriorBuilder AddWallShelves(
            Vector3 wallPosition,
            int tiers,
            Vector3 shelfSize,
            float spacing,
            Color? color = null)
        {
            Color shelfColor = color ?? _woodColor;

            for (int tier = 0; tier < tiers; tier++)
            {
                float shelfY = wallPosition.y + tier * spacing;

                GameObject shelf = PrimitiveBuilder.CreateBox(
                    $"WallShelf_Tier{tier}",
                    new Vector3(wallPosition.x, shelfY, wallPosition.z),
                    shelfSize,
                    shelfColor
                );

                _furniture.Add(shelf);
                BuildingUtilities.AddNavMeshObstacle(shelf);
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
                gameObject.transform.localPosition = position;
                _furniture.Add(gameObject);
            }

            return this;
        }
        
        #endregion

        #region Public API - Build
        
        /// <summary>
        /// Build the interior and attach all furniture to the parent
        /// </summary>
        /// <param name="parent">Parent GameObject to attach furniture to</param>
        /// <returns>Array of created furniture GameObjects</returns>
        public GameObject[] Build(GameObject parent)
        {
            if (parent == null)
            {
                DebugLog.Warning("Interior parent is null, furniture will not be parented");
                return _furniture.ToArray();
            }

            GameObject interiorFolder = BuildingUtilities.CreateFolder(_name, parent.transform);

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
        /// Get all furniture objects without building
        /// </summary>
        public GameObject[] GetFurniture() => _furniture.ToArray();
        
        #endregion
    }
}
