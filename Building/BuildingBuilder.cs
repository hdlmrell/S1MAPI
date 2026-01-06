using System.Collections.Generic;
using UnityEngine;
using MAPI.Utils;

namespace MAPI.Building
{
    /// <summary>
    /// Fluent builder for constructing buildings from Unity primitives
    /// Based on patterns from PetShopSpawner
    /// </summary>
    public class BuildingBuilder
    {
        #region Fields
        
        private readonly string _name;
        private readonly GameObject _root;
        private readonly GameObject _buildingFolder;
        
        private Vector3 _footprint = new Vector3(6f, 0f, 9f);
        private float _height = 6f;
        private float _scale = 1f;
        private readonly List<GameObject> _components = new List<GameObject>();
        
        // Default colors
        private Color _floorColor = new Color(0.5f, 0.5f, 0.5f);
        private Color _wallColor = new Color(0.85f, 0.85f, 0.82f);
        private Color _trimColor = new Color(0.3f, 0.25f, 0.2f);
        private Color _roofColor = new Color(0.4f, 0.35f, 0.3f);
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Create a new building builder
        /// </summary>
        /// <param name="name">Name for the building</param>
        public BuildingBuilder(string name)
        {
            _name = name;
            _root = BuildingUtilities.CreateFolder(name);
            _buildingFolder = BuildingUtilities.CreateFolder("Building", _root.transform);
        }
        
        #endregion

        #region Public API - Configuration
        
        /// <summary>
        /// Set the building footprint (width and depth)
        /// </summary>
        public BuildingBuilder SetFootprint(float width, float depth)
        {
            _footprint = new Vector3(width, 0f, depth);
            return this;
        }

        /// <summary>
        /// Set the building height
        /// </summary>
        public BuildingBuilder SetHeight(float height)
        {
            _height = height;
            return this;
        }

        /// <summary>
        /// Set the global scale multiplier
        /// </summary>
        public BuildingBuilder SetScale(float scale)
        {
            _scale = scale;
            return this;
        }

        /// <summary>
        /// Set the floor color
        /// </summary>
        public BuildingBuilder SetFloorColor(Color color)
        {
            _floorColor = color;
            return this;
        }

        /// <summary>
        /// Set the wall color
        /// </summary>
        public BuildingBuilder SetWallColor(Color color)
        {
            _wallColor = color;
            return this;
        }

        /// <summary>
        /// Set the trim/molding color
        /// </summary>
        public BuildingBuilder SetTrimColor(Color color)
        {
            _trimColor = color;
            return this;
        }

        /// <summary>
        /// Set the roof color
        /// </summary>
        public BuildingBuilder SetRoofColor(Color color)
        {
            _roofColor = color;
            return this;
        }
        
        #endregion

        #region Public API - Structure Components
        
        /// <summary>
        /// Add a floor to the building
        /// </summary>
        public BuildingBuilder AddFloor(float thickness = 0.1f)
        {
            GameObject floor = PrimitiveBuilder.CreateBox(
                "Floor",
                new Vector3(0f, -thickness / 2f, 0f),
                new Vector3(_footprint.x, thickness, _footprint.z),
                _floorColor,
                _buildingFolder.transform
            );

            _components.Add(floor);
            return this;
        }

        /// <summary>
        /// Add walls to the building (back, left, right)
        /// </summary>
        /// <param name="thickness">Wall thickness</param>
        public BuildingBuilder AddWalls(float thickness = 0.2f)
        {
            float halfWidth = _footprint.x / 2f;
            float halfDepth = _footprint.z / 2f;
            float halfHeight = _height / 2f;

            // Back wall
            GameObject backWall = PrimitiveBuilder.CreateBox(
                "Wall_Back",
                new Vector3(0f, halfHeight, -halfDepth + thickness / 2f),
                new Vector3(_footprint.x, _height, thickness),
                _wallColor,
                _buildingFolder.transform
            );
            _components.Add(backWall);
            BuildingUtilities.AddNavMeshObstacle(backWall);

            // Left wall
            GameObject leftWall = PrimitiveBuilder.CreateBox(
                "Wall_Left",
                new Vector3(-halfWidth + thickness / 2f, halfHeight, 0f),
                new Vector3(thickness, _height, _footprint.z),
                _wallColor,
                _buildingFolder.transform
            );
            _components.Add(leftWall);
            BuildingUtilities.AddNavMeshObstacle(leftWall);

            // Right wall
            GameObject rightWall = PrimitiveBuilder.CreateBox(
                "Wall_Right",
                new Vector3(halfWidth - thickness / 2f, halfHeight, 0f),
                new Vector3(thickness, _height, _footprint.z),
                _wallColor,
                _buildingFolder.transform
            );
            _components.Add(rightWall);
            BuildingUtilities.AddNavMeshObstacle(rightWall);

            return this;
        }

        /// <summary>
        /// Add a flat roof to the building
        /// </summary>
        public BuildingBuilder AddRoof(float thickness = 0.3f)
        {
            GameObject roof = PrimitiveBuilder.CreateBox(
                "Roof",
                new Vector3(0f, _height + thickness / 2f, 0f),
                new Vector3(_footprint.x * 1.1f, thickness, _footprint.z * 1.1f),
                _roofColor,
                _buildingFolder.transform
            );

            _components.Add(roof);
            BuildingUtilities.AddNavMeshObstacle(roof);
            return this;
        }

        /// <summary>
        /// Add windows to the front of the building
        /// </summary>
        /// <param name="count">Number of windows</param>
        /// <param name="windowHeight">Height of each window</param>
        /// <param name="glassColor">Color of the glass</param>
        public BuildingBuilder AddWindows(int count = 4, float windowHeight = 5f, Color? glassColor = null)
        {
            Color glass = glassColor ?? new Color(0.7f, 0.85f, 0.9f);
            float windowWidth = _footprint.x / count;
            float halfDepth = _footprint.z / 2f;

            for (int i = 0; i < count; i++)
            {
                // Only add actual windows at positions 0 and 3 (like the pet shop)
                if (i != 0 && i != count - 1)
                {
                    continue;
                }

                float xPos = -_footprint.x / 2f + windowWidth * (i + 0.5f);

                // Glass panel
                GameObject windowPanel = PrimitiveBuilder.CreateBox(
                    $"Window_Panel_{i}",
                    new Vector3(xPos, _height * 0.37f, halfDepth + 0.03f),
                    new Vector3(windowWidth * 0.92f, windowHeight, 0.06f),
                    glass,
                    _buildingFolder.transform
                );
                _components.Add(windowPanel);

                // Window frames
                float frameThickness = 0.04f;
                float frameDepth = 0.12f;

                // Left frame
                GameObject leftFrame = PrimitiveBuilder.CreateBox(
                    $"Window_Frame_Left_{i}",
                    new Vector3(xPos - windowWidth / 2f + frameThickness / 2f, _height * 0.37f, halfDepth + 0.06f),
                    new Vector3(frameThickness, windowHeight + frameThickness, frameDepth),
                    _trimColor,
                    _buildingFolder.transform
                );
                _components.Add(leftFrame);

                // Right frame
                GameObject rightFrame = PrimitiveBuilder.CreateBox(
                    $"Window_Frame_Right_{i}",
                    new Vector3(xPos + windowWidth / 2f - frameThickness / 2f, _height * 0.37f, halfDepth + 0.06f),
                    new Vector3(frameThickness, windowHeight + frameThickness, frameDepth),
                    _trimColor,
                    _buildingFolder.transform
                );
                _components.Add(rightFrame);

                // Top frame
                GameObject topFrame = PrimitiveBuilder.CreateBox(
                    $"Window_Frame_Top_{i}",
                    new Vector3(xPos, _height * 0.37f + windowHeight / 2f, halfDepth + 0.06f),
                    new Vector3(windowWidth, frameThickness, frameDepth),
                    _trimColor,
                    _buildingFolder.transform
                );
                _components.Add(topFrame);

                // Bottom frame
                GameObject bottomFrame = PrimitiveBuilder.CreateBox(
                    $"Window_Frame_Bottom_{i}",
                    new Vector3(xPos, _height * 0.37f - windowHeight / 2f, halfDepth + 0.06f),
                    new Vector3(windowWidth, frameThickness, frameDepth),
                    _trimColor,
                    _buildingFolder.transform
                );
                _components.Add(bottomFrame);
            }

            return this;
        }

        /// <summary>
        /// Add base molding around the building
        /// </summary>
        public BuildingBuilder AddBaseMolding(float height = 0.3f, float depth = 0.1f)
        {
            float halfWidth = _footprint.x / 2f;
            float halfDepth = _footprint.z / 2f;

            // Back molding
            GameObject backMolding = PrimitiveBuilder.CreateBox(
                "BaseMolding_Back",
                new Vector3(0f, height / 2f, -halfDepth - depth / 2f),
                new Vector3(_footprint.x + depth * 2f, height, depth),
                _trimColor,
                _buildingFolder.transform
            );
            _components.Add(backMolding);

            // Left molding
            GameObject leftMolding = PrimitiveBuilder.CreateBox(
                "BaseMolding_Left",
                new Vector3(-halfWidth - depth / 2f, height / 2f, 0f),
                new Vector3(depth, height, _footprint.z),
                _trimColor,
                _buildingFolder.transform
            );
            _components.Add(leftMolding);

            // Right molding
            GameObject rightMolding = PrimitiveBuilder.CreateBox(
                "BaseMolding_Right",
                new Vector3(halfWidth + depth / 2f, height / 2f, 0f),
                new Vector3(depth, height, _footprint.z),
                _trimColor,
                _buildingFolder.transform
            );
            _components.Add(rightMolding);

            return this;
        }
        
        #endregion

        #region Public API - Build
        
        /// <summary>
        /// Build the complete building GameObject
        /// </summary>
        /// <param name="position">World position for the building</param>
        /// <param name="rotation">World rotation for the building</param>
        /// <returns>The root GameObject of the building</returns>
        public GameObject Build(Vector3 position, Quaternion rotation)
        {
            _root.transform.position = position;
            _root.transform.rotation = rotation;
            _root.transform.localScale = Vector3.one * _scale;

            // Apply occlusion settings to all renderers
            BuildingUtilities.ApplyOcclusionSettings(_root, false);

            DebugLog.Info($"Built building: {_name} with {_components.Count} components");
            return _root;
        }

        /// <summary>
        /// Build the building at the origin
        /// </summary>
        public GameObject Build()
        {
            return Build(Vector3.zero, Quaternion.identity);
        }
        
        #endregion

        #region Public API - Access
        
        /// <summary>
        /// Get the root GameObject (can be used before Build() is called)
        /// </summary>
        public GameObject GetRoot() => _root;

        /// <summary>
        /// Get the building folder GameObject
        /// </summary>
        public GameObject GetBuildingFolder() => _buildingFolder;
        
        #endregion
    }
}
