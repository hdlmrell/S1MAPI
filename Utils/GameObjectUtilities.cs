using UnityEngine;

namespace MAPI.Utils
{
    /// <summary>
    /// Utility functions for GameObject manipulation and configuration
    /// </summary>
    public static class GameObjectUtilities
    {
        #region Public API - Layer Management
        
        /// <summary>
        /// Set the layer for a GameObject and optionally all its children recursively
        /// </summary>
        /// <param name="gameObject">The GameObject to modify</param>
        /// <param name="layer">The layer index to set</param>
        /// <param name="includeChildren">Whether to apply recursively to children</param>
        public static void SetLayerRecursively(GameObject gameObject, int layer, bool includeChildren = true)
        {
            if (gameObject == null)
            {
                DebugLog.Warning("Cannot set layer on null GameObject");
                return;
            }

            gameObject.layer = layer;

            if (includeChildren)
            {
                foreach (Transform child in gameObject.transform)
                {
                    SetLayerRecursively(child.gameObject, layer, true);
                }
            }
        }

        /// <summary>
        /// Set the layer by name for a GameObject and optionally all its children
        /// </summary>
        /// <param name="gameObject">The GameObject to modify</param>
        /// <param name="layerName">The layer name</param>
        /// <param name="includeChildren">Whether to apply recursively to children</param>
        public static void SetLayerRecursively(GameObject gameObject, string layerName, bool includeChildren = true)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer == -1)
            {
                DebugLog.Warning($"Layer not found: {layerName}");
                return;
            }

            SetLayerRecursively(gameObject, layer, includeChildren);
        }
        
        #endregion
    }
}
