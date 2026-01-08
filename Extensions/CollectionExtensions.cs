using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace S1MAPI.Extensions
{
    /// <summary>
    /// Extension methods for collection operations.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Perform action on each item in collection.
        /// </summary>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, System.Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
            return collection;
        }

        /// <summary>
        /// Shuffle collection in place using Fisher-Yates algorithm.
        /// </summary>
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list;
        }

        /// <summary>
        /// Get distinct items by key selector.
        /// </summary>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> collection, System.Func<T, TKey> keySelector)
        {
            return collection.GroupBy(keySelector).Select(g => g.First());
        }

        /// <summary>
        /// Safe index access that returns default if out of range.
        /// </summary>
        public static T? SafeGet<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count ? list[index] : default;
        }

        /// <summary>
        /// Convert Vector3 array to List.
        /// </summary>
        public static List<Vector3> ToVector3List(this IEnumerable<Vector3> array)
        {
            return new List<Vector3>(array);
        }

        /// <summary>
        /// Convert int array to List.
        /// </summary>
        public static List<int> ToIntList(this IEnumerable<int> array)
        {
            return new List<int>(array);
        }
    }
}
