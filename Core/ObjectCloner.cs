using UnityEngine;
using MAPI.Utils;

namespace MAPI.Core
{
    /// <summary>
    /// Utility for cloning and modifying GameObjects
    /// Useful for creating variations of existing prefabs or scene objects
    /// </summary>
    public class ObjectCloner
    {
        #region Fields
        
        private readonly GameObject _original;
        private readonly List<Type> _componentsToDisable = new List<Type>();
        private readonly List<Type> _componentsToRemove = new List<Type>();
        private Material _replacementMaterial;
        private bool _stripColliders;
        private int? _targetLayer;
        private Action<GameObject> _postProcessAction;
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Create a new ObjectCloner for a specific object
        /// </summary>
        /// <param name="original">The object to clone</param>
        public ObjectCloner(GameObject original)
        {
            if (original == null)
            {
                throw new ArgumentNullException(nameof(original));
            }
            _original = original;
        }
        
        #endregion

        #region Public API - Configuration
        
        /// <summary>
        /// Disable components of a specific type on the clone
        /// </summary>
        public ObjectCloner DisableComponents<T>() where T : Component
        {
            _componentsToDisable.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Remove components of a specific type from the clone
        /// </summary>
        public ObjectCloner RemoveComponents<T>() where T : Component
        {
            _componentsToRemove.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Replace all materials on the clone with a specific material
        /// </summary>
        public ObjectCloner ReplaceMaterials(Material material)
        {
            _replacementMaterial = material;
            return this;
        }

        /// <summary>
        /// Remove all colliders from the clone
        /// </summary>
        public ObjectCloner StripColliders()
        {
            _stripColliders = true;
            return this;
        }

        /// <summary>
        /// Set the layer for the clone and its children
        /// </summary>
        public ObjectCloner SetLayer(int layer)
        {
            _targetLayer = layer;
            return this;
        }

        /// <summary>
        /// Add a custom action to run after cloning
        /// </summary>
        public ObjectCloner AddPostProcess(Action<GameObject> action)
        {
            _postProcessAction += action;
            return this;
        }
        
        #endregion

        #region Public API - Cloning
        
        /// <summary>
        /// Create a clone of the object
        /// </summary>
        /// <param name="position">World position for the clone</param>
        /// <param name="rotation">World rotation for the clone</param>
        /// <param name="parent">Parent transform</param>
        /// <returns>The cloned GameObject</returns>
        public GameObject Clone(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (_original == null)
            {
                DebugLog.Error("Cannot clone null object");
                return null;
            }

            GameObject clone = UnityEngine.Object.Instantiate(_original, position, rotation, parent);
            clone.name = $"{_original.name}_Clone";

            // Process modifications
            ProcessClone(clone);

            _postProcessAction?.Invoke(clone);

            // Register with resource tracker if it's a root object created by MAPI
            ResourceTracker.Register(clone);

            return clone;
        }

        /// <summary>
        /// Create a clone at the origin
        /// </summary>
        public GameObject Clone()
        {
            return Clone(Vector3.zero, Quaternion.identity);
        }
        
        #endregion

        #region Private Methods
        
        private void ProcessClone(GameObject clone)
        {
            // 1. Disable components
            foreach (Type type in _componentsToDisable)
            {
                foreach (Component comp in clone.GetComponentsInChildren(type, true))
                {
                    if (comp is MonoBehaviour mb) mb.enabled = false;
                    else if (comp is Renderer r) r.enabled = false;
                    else if (comp is Collider c) c.enabled = false;
                }
            }

            // 2. Remove components
            foreach (Type type in _componentsToRemove)
            {
                foreach (Component comp in clone.GetComponentsInChildren(type, true))
                {
                    UnityEngine.Object.Destroy(comp);
                }
            }

            // 3. Strip colliders
            if (_stripColliders)
            {
                foreach (Collider c in clone.GetComponentsInChildren<Collider>(true))
                {
                    UnityEngine.Object.Destroy(c);
                }
            }

            // 4. Replace materials
            if (_replacementMaterial != null)
            {
                foreach (Renderer r in clone.GetComponentsInChildren<Renderer>(true))
                {
                    r.material = _replacementMaterial;
                }
            }

            // 5. Set Layer
            if (_targetLayer.HasValue)
            {
                GameObjectUtilities.SetLayerRecursively(clone, _targetLayer.Value);
            }
        }
        
        #endregion
        
        #region Static Helpers
        
        /// <summary>
        /// Quick static helper to clone an object
        /// </summary>
        public static GameObject Clone(GameObject original, Vector3 position, Quaternion rotation)
        {
            return new ObjectCloner(original).Clone(position, rotation);
        }
        
        #endregion
    }
}
