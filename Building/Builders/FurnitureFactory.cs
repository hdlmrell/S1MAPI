using MAPI.Building.Config;
using UnityEngine;
using MAPI.S1;

namespace MAPI.Building.Builders
{
    /// <summary>
    /// Available furniture types.
    /// </summary>
    public enum FurnitureType
    {
        Table,
        Chair,
        Desk,
        Bookshelf,
        Counter,
        CoffeeTable
    }

    /// <summary>
    /// Creates procedural furniture pieces.
    /// Extracted from SemanticBuildingBuilder for SRP compliance.
    /// </summary>
    public sealed class FurnitureFactory
    {
        #region Fields

        private readonly Transform _parent;
        private readonly BuildingPalette _palette;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new furniture factory.
        /// </summary>
        /// <param name="parent">Parent transform for furniture</param>
        /// <param name="palette">Material and color palette</param>
        public FurnitureFactory(Transform parent, BuildingPalette palette)
        {
            _parent = parent;
            _palette = palette;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Create a furniture piece at the specified position.
        /// </summary>
        /// <param name="type">Type of furniture to create</param>
        /// <param name="position">Local position</param>
        /// <param name="rotation">Local rotation</param>
        /// <param name="color">Optional color override (uses palette accent if null)</param>
        /// <returns>The created furniture GameObject</returns>
        public GameObject Create(FurnitureType type, Vector3 position, Quaternion rotation, Color? color = null)
        {
            Color furnitureColor = color ?? GetDefaultColor(type);

            return type switch
            {
                FurnitureType.Table => CreateTable(position, rotation, furnitureColor),
                FurnitureType.Chair => CreateChair(position, rotation, furnitureColor),
                FurnitureType.Desk => CreateDesk(position, rotation, furnitureColor),
                FurnitureType.Bookshelf => CreateBookshelf(position, rotation, furnitureColor),
                FurnitureType.Counter => CreateCounter(position, rotation, furnitureColor),
                FurnitureType.CoffeeTable => CreateCoffeeTable(position, rotation, furnitureColor),
                _ => throw new System.ArgumentException($"Unknown furniture type: {type}")
            };
        }

        /// <summary>
        /// Get the approximate footprint of a furniture piece for layout calculations.
        /// </summary>
        /// <param name="type">Type of furniture</param>
        /// <returns>Approximate size (width, height, depth)</returns>
        public static Vector3 GetFootprint(FurnitureType type)
        {
            return type switch
            {
                FurnitureType.Table => new Vector3(1.4f, 0.75f, 0.9f),
                FurnitureType.Chair => new Vector3(0.5f, 0.95f, 0.5f),
                FurnitureType.Desk => new Vector3(1.5f, 0.75f, 0.7f),
                FurnitureType.Bookshelf => new Vector3(1.2f, 2.0f, 0.3f),
                FurnitureType.Counter => new Vector3(2.0f, 0.95f, 0.6f),
                FurnitureType.CoffeeTable => new Vector3(1.1f, 0.45f, 0.6f),
                _ => Vector3.one
            };
        }

        #endregion

        #region Private Methods - Individual Furniture

        private Color GetDefaultColor(FurnitureType type)
        {
            return type switch
            {
                FurnitureType.Table or FurnitureType.Desk or FurnitureType.Chair or FurnitureType.Bookshelf => 
                    new Color(0.55f, 0.35f, 0.2f), // Wood brown
                FurnitureType.Counter => _palette.AccentColor,
                FurnitureType.CoffeeTable => new Color(0.4f, 0.25f, 0.1f),
                _ => new Color(0.5f, 0.5f, 0.5f)
            };
        }

        private GameObject CreateTable(Vector3 position, Quaternion rotation, Color color)
        {
            GameObject table = BuildingUtilities.CreateFolder("Table", _parent);
            table.transform.localPosition = position;
            table.transform.localRotation = rotation;

            Material tableMat = Materials.WoodMediumBrown;
            Color legColor = new Color(0.2f, 0.2f, 0.2f);

            // Table top
            GameObject top = PrimitiveBuilder.CreateBox("TableTop",
                new Vector3(0f, 0.75f, 0f),
                new Vector3(1.4f, 0.08f, 0.9f),
                color, table.transform);
            
            if (tableMat != null)
            {
                Renderer r = top.GetComponent<Renderer>();
                if (r != null) r.material = tableMat;
            }

            // Legs
            float legWidth = 0.08f;
            Vector3[] legPositions = {
                new(0.6f, 0.375f, 0.35f),
                new(-0.6f, 0.375f, 0.35f),
                new(0.6f, 0.375f, -0.35f),
                new(-0.6f, 0.375f, -0.35f)
            };

            for (int i = 0; i < 4; i++)
            {
                PrimitiveBuilder.CreateBox($"Leg{i + 1}", legPositions[i], new Vector3(legWidth, 0.75f, legWidth), legColor, table.transform);
            }

            // Cross braces
            PrimitiveBuilder.CreateBox("CrossBrace1", new Vector3(0.6f, 0.2f, 0f), new Vector3(legWidth, 0.05f, 0.7f), legColor, table.transform);
            PrimitiveBuilder.CreateBox("CrossBrace2", new Vector3(-0.6f, 0.2f, 0f), new Vector3(legWidth, 0.05f, 0.7f), legColor, table.transform);
            PrimitiveBuilder.CreateBox("CrossBrace3", new Vector3(0f, 0.2f, 0f), new Vector3(1.2f, 0.05f, legWidth), legColor, table.transform);

            return table;
        }

        private GameObject CreateChair(Vector3 position, Quaternion rotation, Color color)
        {
            GameObject chair = BuildingUtilities.CreateFolder("Chair", _parent);
            chair.transform.localPosition = position;
            chair.transform.localRotation = rotation;

            // Seat
            PrimitiveBuilder.CreateBox("Seat", new Vector3(0f, 0.45f, 0f), new Vector3(0.5f, 0.05f, 0.5f), color, chair.transform);

            // Backrest
            PrimitiveBuilder.CreateBox("Backrest", new Vector3(0f, 0.7f, -0.225f), new Vector3(0.5f, 0.5f, 0.05f), color, chair.transform);

            // Legs
            float legRadius = 0.03f;
            PrimitiveBuilder.CreateCylinder("Leg1", new Vector3(0.2f, 0.225f, 0.2f), new Vector3(legRadius, 0.45f, legRadius), color, chair.transform);
            PrimitiveBuilder.CreateCylinder("Leg2", new Vector3(-0.2f, 0.225f, 0.2f), new Vector3(legRadius, 0.45f, legRadius), color, chair.transform);
            PrimitiveBuilder.CreateCylinder("Leg3", new Vector3(0.2f, 0.225f, -0.2f), new Vector3(legRadius, 0.45f, legRadius), color, chair.transform);
            PrimitiveBuilder.CreateCylinder("Leg4", new Vector3(-0.2f, 0.225f, -0.2f), new Vector3(legRadius, 0.45f, legRadius), color, chair.transform);

            return chair;
        }

        private GameObject CreateDesk(Vector3 position, Quaternion rotation, Color color)
        {
            GameObject desk = BuildingUtilities.CreateFolder("Desk", _parent);
            desk.transform.localPosition = position;
            desk.transform.localRotation = rotation;

            // Desk top
            PrimitiveBuilder.CreateBox("DeskTop", new Vector3(0f, 0.75f, 0f), new Vector3(1.5f, 0.05f, 0.7f), color, desk.transform);

            // Side panels
            PrimitiveBuilder.CreateBox("LeftPanel", new Vector3(-0.6f, 0.375f, 0f), new Vector3(0.05f, 0.75f, 0.7f), color, desk.transform);
            PrimitiveBuilder.CreateBox("RightPanel", new Vector3(0.6f, 0.375f, 0f), new Vector3(0.05f, 0.75f, 0.7f), color, desk.transform);

            return desk;
        }

        private GameObject CreateBookshelf(Vector3 position, Quaternion rotation, Color color)
        {
            GameObject shelf = BuildingUtilities.CreateFolder("Bookshelf", _parent);
            shelf.transform.localPosition = position;
            shelf.transform.localRotation = rotation;

            // Back panel
            PrimitiveBuilder.CreateBox("BackPanel", new Vector3(0f, 1f, 0f), new Vector3(1.2f, 2f, 0.05f), color, shelf.transform);

            // Shelves (3 shelves with good spacing for items)
            for (int i = 0; i < 3; i++)
            {
                float y = i * 0.6f + 0.35f;
                PrimitiveBuilder.CreateBox($"Shelf{i}", new Vector3(0f, y, 0.15f), new Vector3(1.2f, 0.03f, 0.3f), color, shelf.transform);
            }

            return shelf;
        }

        private GameObject CreateCounter(Vector3 position, Quaternion rotation, Color color)
        {
            GameObject counter = BuildingUtilities.CreateFolder("Counter", _parent);
            counter.transform.localPosition = position;
            counter.transform.localRotation = rotation;

            Material metalMat = Materials.MetalDarkGrey;

            // Counter top
            PrimitiveBuilder.CreateBox("CounterTop", new Vector3(0f, 0.9f, 0f), new Vector3(2f, 0.05f, 0.6f), color, counter.transform);

            // Cabinet base
            GameObject cabinet = PrimitiveBuilder.CreateBox("Cabinet", new Vector3(0f, 0.45f, 0f), new Vector3(2f, 0.9f, 0.6f), color, counter.transform);
            
            if (metalMat != null)
            {
                Renderer r = cabinet.GetComponent<Renderer>();
                if (r != null) r.material = metalMat;
            }

            return counter;
        }

        private GameObject CreateCoffeeTable(Vector3 position, Quaternion rotation, Color color)
        {
            GameObject table = BuildingUtilities.CreateFolder("CoffeeTable", _parent);
            table.transform.localPosition = position;
            table.transform.localRotation = rotation;

            Material tableMat = Materials.WoodMediumBrown;
            Color legColor = new Color(0.2f, 0.2f, 0.2f);

            // Low table top
            GameObject top = PrimitiveBuilder.CreateBox("TableTop", new Vector3(0f, 0.45f, 0f), new Vector3(1.1f, 0.06f, 0.6f), color, table.transform);
            
            if (tableMat != null)
            {
                Renderer r = top.GetComponent<Renderer>();
                if (r != null) r.material = tableMat;
            }

            // Legs
            float legWidth = 0.06f;
            float xOff = 0.27f;
            float zOff = 0.22f;

            Vector3[] legPositions = {
                new(xOff, 0.225f, zOff),
                new(-xOff, 0.225f, zOff),
                new(xOff, 0.225f, -zOff),
                new(-xOff, 0.225f, -zOff)
            };

            for (int i = 0; i < 4; i++)
            {
                PrimitiveBuilder.CreateBox($"Leg{i + 1}", legPositions[i], new Vector3(legWidth, 0.45f, legWidth), legColor, table.transform);
            }

            return table;
        }

        #endregion
    }
}
