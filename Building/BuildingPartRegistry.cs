using System.Collections.Generic;
using S1MAPI.Building.Config;
using S1MAPI.Building.Structural;
using UnityEngine;

namespace S1MAPI.Building
{
    /// <summary>
    /// Post-build registry that tracks building parts by category.
    /// Attached to the building root during construction by <see cref="BuildingBuilder"/>.
    /// Consumers use this to target specific parts (all exterior walls, just the north wall,
    /// the floor, trim, etc.) and get their renderers for material swaps.
    /// </summary>
    public sealed class BuildingPartRegistry : MonoBehaviour
    {
        private readonly Dictionary<BuildingPart, List<GameObject>> _parts = new Dictionary<BuildingPart, List<GameObject>>();
        private readonly Dictionary<WallSide, List<GameObject>> _wallsByDirection = new Dictionary<WallSide, List<GameObject>>();

        #region Registration (called by BuildingBuilder during construction)

        internal void Register(BuildingPart part, GameObject go)
        {
            if (!_parts.TryGetValue(part, out var list))
            {
                list = new List<GameObject>();
                _parts[part] = list;
            }
            list.Add(go);
        }

        internal void Register(WallSide side, GameObject go)
        {
            if (!_wallsByDirection.TryGetValue(side, out var list))
            {
                list = new List<GameObject>();
                _wallsByDirection[side] = list;
            }
            list.Add(go);

            Register(BuildingPart.ExteriorWalls, go);
        }

        #endregion

        #region Consumer API — Renderers

        /// <summary>
        /// Get all renderers for a building part category.
        /// </summary>
        public Renderer[] GetRenderers(BuildingPart part)
        {
            if (!_parts.TryGetValue(part, out var list))
                return System.Array.Empty<Renderer>();

            var renderers = new List<Renderer>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                    renderers.AddRange(list[i].GetComponentsInChildren<Renderer>());
            }
            return renderers.ToArray();
        }

        /// <summary>
        /// Get all renderers for a specific wall direction.
        /// </summary>
        public Renderer[] GetRenderers(WallSide side)
        {
            if (!_wallsByDirection.TryGetValue(side, out var list))
                return System.Array.Empty<Renderer>();

            var renderers = new List<Renderer>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                    renderers.AddRange(list[i].GetComponentsInChildren<Renderer>());
            }
            return renderers.ToArray();
        }

        #endregion

        #region Consumer API — GameObjects

        /// <summary>
        /// Get all GameObjects for a building part category.
        /// </summary>
        public GameObject[] GetParts(BuildingPart part)
        {
            if (!_parts.TryGetValue(part, out var list))
                return System.Array.Empty<GameObject>();

            var result = new List<GameObject>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                    result.Add(list[i]);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get all GameObjects for a specific wall direction.
        /// </summary>
        public GameObject[] GetParts(WallSide side)
        {
            if (!_wallsByDirection.TryGetValue(side, out var list))
                return System.Array.Empty<GameObject>();

            var result = new List<GameObject>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                    result.Add(list[i]);
            }
            return result.ToArray();
        }

        #endregion

        #region Consumer API — Material swap

        /// <summary>
        /// Set the material on all renderers for a building part category.
        /// </summary>
        public void SetMaterial(BuildingPart part, Material material)
        {
            var renderers = GetRenderers(part);
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material = material;
        }

        /// <summary>
        /// Set the material on all renderers for a specific wall direction.
        /// </summary>
        public void SetMaterial(WallSide side, Material material)
        {
            var renderers = GetRenderers(side);
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material = material;
        }

        #endregion
    }
}
