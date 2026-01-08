using UnityEngine;
using S1MAPI.Core;
using S1MAPI.Extensions;
using S1MAPI.S1;

#if IL2CPP
using Il2CppFishNet;
using Il2CppFishNet.Managing;
using Il2CppFishNet.Managing.Object;
using Il2CppFishNet.Object;
using Il2CppTMPro;
#else
using TMPro;
#endif

namespace S1MAPI.Building.Components
{
    /// <summary>
    /// Places network-spawnable prefabs from the game.
    /// Handles FishNet network spawning for multiplayer compatibility.
    /// Extracted from SemanticBuildingBuilder for SRP compliance.
    /// </summary>
    public sealed class PrefabPlacer
    {
        #region Fields

        private readonly Transform _parent;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new prefab placer.
        /// </summary>
        /// <param name="parent">Parent transform for placed prefabs</param>
        public PrefabPlacer(Transform parent)
        {
            _parent = parent;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Place a prefab at the specified position.
        /// </summary>
        /// <param name="prefab">Prefab reference from GamePrefabs</param>
        /// <param name="localPosition">Local position relative to parent</param>
        /// <param name="localRotation">Local rotation</param>
        /// <param name="networked">Whether to spawn on network (server only)</param>
        /// <param name="enableComponents">Whether to enable MonoBehaviour components (default: false)</param>
        /// <returns>The instantiated prefab or null if not found</returns>
        public GameObject? Place(PrefabRef prefab, Vector3 localPosition, Quaternion localRotation, bool networked = true, bool enableComponents = false)
        {
            GameObject? instance = networked ? prefab.InstantiateNetworked() : prefab.Instantiate();
            if (instance == null) return null;

            instance.transform.SetParent(_parent);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = localRotation;

            // Optionally enable game logic components
            if (enableComponents)
            {
                instance.EnableAllComponents(recursive: true);
            }

            return instance;
        }

        /// <summary>
        /// Place a prefab with specific components enabled by name.
        /// Useful for enabling only certain game logic (e.g., ATM, VendingMachine).
        /// </summary>
        /// <param name="prefab">Prefab reference from GamePrefabs</param>
        /// <param name="localPosition">Local position relative to parent</param>
        /// <param name="localRotation">Local rotation</param>
        /// <param name="componentNames">Array of component type names to enable</param>
        /// <param name="networked">Whether to spawn on network (server only)</param>
        /// <returns>The instantiated prefab or null if not found</returns>
        public GameObject? PlaceWithComponents(PrefabRef prefab, Vector3 localPosition, Quaternion localRotation, string[] componentNames, bool networked = true)
        {
            GameObject? instance = networked ? prefab.InstantiateNetworked() : prefab.Instantiate();
            if (instance == null) return null;

            instance.transform.SetParent(_parent);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = localRotation;

            // Enable specific components by name
            instance.EnableComponentsByName(componentNames, recursive: true);

            return instance;
        }

        /// <summary>
        /// Place sliding double doors at a wall opening.
        /// </summary>
        /// <param name="localPosition">Local position for the doors</param>
        /// <param name="localRotation">Local rotation</param>
        /// <param name="openingHoursText">Text to display for opening hours</param>
        /// <param name="doorMaterial">Optional material for door panels</param>
        /// <returns>The door instance or null if prefab not found</returns>
        public GameObject? PlaceSlidingDoors(Vector3 localPosition, Quaternion localRotation, string openingHoursText = "6AM-6PM", Material? doorMaterial = null)
        {
            GameObject? doors = Place(Prefabs.SlidingDoors, localPosition, localRotation, networked: true);
            if (doors == null) return null;

            doors.name = "SlidingDoors";

            // Apply material to door panels
            if (doorMaterial != null)
            {
                ApplyMaterialToPath(doors, "Door/Door", doorMaterial);
                ApplyMaterialToPath(doors, "Door/Door (1)", doorMaterial);
            }

            // Set opening hours text
            if (!string.IsNullOrEmpty(openingHoursText))
            {
                SetOpeningHoursText(doors, openingHoursText);
            }

            return doors;
        }

        /// <summary>
        /// Place an item prefab (like bongs on shelves).
        /// </summary>
        /// <param name="prefab">Prefab reference</param>
        /// <param name="localPosition">Local position</param>
        /// <param name="localRotation">Local rotation</param>
        /// <returns>The item instance or null if prefab not found</returns>
        public GameObject? PlaceItem(PrefabRef prefab, Vector3 localPosition, Quaternion localRotation)
        {
            return Place(prefab, localPosition, localRotation, networked: false);
        }

        /// <summary>
        /// Place multiple items in a row (e.g., items on a shelf).
        /// </summary>
        /// <param name="prefab">Prefab reference for items</param>
        /// <param name="startPosition">Starting position</param>
        /// <param name="spacing">Spacing between items</param>
        /// <param name="count">Number of items to place</param>
        /// <param name="baseRotation">Base rotation for items</param>
        /// <param name="rotationVariance">Random rotation variance in degrees</param>
        /// <returns>Array of placed items</returns>
        public GameObject?[] PlaceItemRow(PrefabRef prefab, Vector3 startPosition, float spacing, int count, Quaternion baseRotation, float rotationVariance = 15f)
        {
            GameObject?[] items = new GameObject?[count];
            
            for (int i = 0; i < count; i++)
            {
                Vector3 offset = new Vector3(spacing * (i - (count - 1) / 2f), 0f, 0f);
                float yRotation = UnityEngine.Random.Range(-rotationVariance, rotationVariance);
                Quaternion rotation = baseRotation * Quaternion.Euler(0f, yRotation, 0f);
                
                items[i] = PlaceItem(prefab, startPosition + offset, rotation);
            }

            return items;
        }

        #endregion

        #region Private Methods

        private static void ApplyMaterialToPath(GameObject root, string path, Material material)
        {
            Transform t = root.transform.Find(path);
            if (t != null)
            {
                Renderer r = t.GetComponent<Renderer>();
                if (r != null) r.material = material;
            }
        }

        private static void SetOpeningHoursText(GameObject doorInstance, string text)
        {
            Transform signTransform = doorInstance.transform.Find("Door/Door/OpeningHoursSign");
            if (signTransform != null)
            {
                TextMeshPro? tmp = signTransform.GetComponent<TextMeshPro>();
                if (tmp != null)
                {
                    tmp.text = text;
                    return;
                }
            }

            // Fallback: search recursively
            TextMeshPro[] tmps = doorInstance.GetComponentsInChildren<TextMeshPro>(true);
            foreach (var t in tmps)
            {
                if (t.name == "OpeningHoursSign" || t.transform.parent?.name == "OpeningHoursSign")
                {
                    t.text = text;
                    return;
                }
            }
        }

        #endregion
    }
}
