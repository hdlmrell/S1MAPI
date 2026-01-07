using System;
using System.Collections.Generic;
using UnityEngine;

#if IL2CPP
using Il2CppInterop.Runtime;
using S1Type = Il2CppSystem.Type;
#else
using S1Type = System.Type;
#endif

namespace MAPI.Utils
{
    /// <summary>
    /// Provides extension methods for converting between C# and Il2Cpp lists.
    /// Ensures compatibility across both Mono and IL2CPP runtimes.
    /// </summary>
    public static class Il2CppListExtensions
    {
        /// <summary>
        /// Converts a C# List to an IEnumerable.
        /// No-op for C# lists but provides consistency with Il2Cpp lists.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the collection</typeparam>
        /// <param name="list">The source list to convert</param>
        /// <returns>An IEnumerable containing the elements of the source</returns>
        public static IEnumerable<T> AsEnumerable<T>(this List<T> list)
        {
            return list ?? new List<T>();
        }

#if IL2CPP
        /// <summary>
        /// Converts an IEnumerable to an Il2Cpp List.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the collection</typeparam>
        /// <param name="source">The source enumerable to convert</param>
        /// <returns>An Il2Cpp List containing the elements of the source</returns>
        public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this IEnumerable<T> source)
        {
            var il2CppList = new Il2CppSystem.Collections.Generic.List<T>();
            foreach (var item in source)
            {
                il2CppList.Add(item);
            }
            return il2CppList;
        }

        /// <summary>
        /// Converts an Il2Cpp List to a C# List.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the list</typeparam>
        /// <param name="il2CppList">The Il2Cpp list to convert</param>
        /// <returns>A C# List containing the elements from the Il2Cpp list</returns>
        public static List<T> ToCSharpList<T>(this Il2CppSystem.Collections.Generic.List<T> il2CppList)
        {
            if (il2CppList == null)
            {
                return new List<T>();
            }
            
            List<T> csharpList = new List<T>();
            T[] array = il2CppList.ToArray();
            csharpList.AddRange(array);
            return csharpList;
        }

        /// <summary>
        /// Converts an Il2Cpp List to an IEnumerable.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the list</typeparam>
        /// <param name="list">The Il2Cpp list to convert</param>
        /// <returns>An IEnumerable containing the elements from the Il2Cpp list</returns>
        public static IEnumerable<T> AsEnumerable<T>(this Il2CppSystem.Collections.Generic.List<T> list)
        {
            if (list == null)
            {
                return new List<T>();
            }
            return list._items.Take(list._size);
        }
#endif
    }

    /// <summary>
    /// Cross-platform type utilities for Mono and IL2CPP compatibility.
    /// </summary>
    public static class CrossType
    {
        /// <summary>
        /// Gets the proper type of a class for the current runtime.
        /// Returns Il2CppSystem.Type under IL2CPP, System.Type under Mono.
        /// </summary>
        /// <typeparam name="T">The type to get.</typeparam>
        /// <returns>The runtime-appropriate type.</returns>
        public static S1Type Of<T>()
        {
#if IL2CPP
            return Il2CppType.Of<T>();
#else
            return typeof(T);
#endif
        }

        /// <summary>
        /// Converts a System.Type to the appropriate runtime type.
        /// Under IL2CPP, this uses Il2CppInterop to get the Il2CppSystem.Type.
        /// </summary>
        /// <param name="systemType">The System.Type to convert.</param>
        /// <returns>The runtime-appropriate type.</returns>
        public static S1Type ToRuntimeType(System.Type systemType)
        {
#if IL2CPP
            return Il2CppType.From(systemType);
#else
            return systemType;
#endif
        }

#if IL2CPP
        /// <summary>
        /// Checks if the given object is of type T and casts it.
        /// Uses Il2Cpp type system for proper type checking.
        /// </summary>
        /// <typeparam name="T">The type to check against</typeparam>
        /// <param name="obj">The object to check</param>
        /// <param name="result">The cast object if successful</param>
        /// <returns>True if the object is of type T</returns>
        public static bool Is<T>(object obj, out T result) where T : Il2CppSystem.Object
        {
            if (obj is Il2CppSystem.Object il2CppObj)
            {
                var targetType = Il2CppType.Of<T>();
                var objType = il2CppObj.GetIl2CppType();

                if (targetType.IsAssignableFrom(objType))
                {
                    result = il2CppObj.TryCast<T>()!;
                    return result != null;
                }
            }

            result = default!;
            return false;
        }
#else
        /// <summary>
        /// Checks if the given object is of type T and casts it.
        /// Simple type check for Mono runtime.
        /// </summary>
        /// <typeparam name="T">The type to check against</typeparam>
        /// <param name="obj">The object to check</param>
        /// <param name="result">The cast object if successful</param>
        /// <returns>True if the object is of type T</returns>
        public static bool Is<T>(object obj, out T result) where T : class
        {
            if (obj is T t)
            {
                result = t;
                return true;
            }

            result = default!;
            return false;
        }
#endif
    }

    /// <summary>
    /// Cross-platform exception handling utilities for Mono and IL2CPP compatibility.
    /// </summary>
    public static class CrossException
    {
        /// <summary>
        /// Logs an exception to the Unity console in a cross-platform manner.
        /// Under IL2CPP, Debug.LogException expects Il2CppSystem.Exception, so we log as error string instead.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        public static void LogException(System.Exception ex)
        {
#if IL2CPP
            // IL2CPP's Debug.LogException expects Il2CppSystem.Exception, not System.Exception.
            // Log as error with full details instead.
            Debug.LogError($"[Exception] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
#else
            Debug.LogException(ex);
#endif
        }

        /// <summary>
        /// Logs an exception with a custom prefix message.
        /// </summary>
        /// <param name="message">Prefix message to include.</param>
        /// <param name="ex">The exception to log.</param>
        public static void LogException(string message, System.Exception ex)
        {
#if IL2CPP
            Debug.LogError($"[Exception] {message}: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
#else
            Debug.LogError(message);
            Debug.LogException(ex);
#endif
        }
    }

    /// <summary>
    /// Extension methods for GameObjects providing cross-platform utility functions.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Gets a component or adds it if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type of component</typeparam>
        /// <param name="gameObject">The GameObject to get or add the component to</param>
        /// <returns>The existing component or a newly added one</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            
            return gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Gets the full hierarchy path of a Transform.
        /// </summary>
        /// <param name="transform">The Transform to get the path for</param>
        /// <returns>The full hierarchy path</returns>
        public static string GetHierarchyPath(this Transform transform)
        {
            if (transform == null)
            {
                return "null";
            }

            string path = transform.name;
            Transform current = transform.parent;

            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }

        /// <summary>
        /// Gets all components of type T in the given GameObject and its children recursively.
        /// </summary>
        /// <typeparam name="T">The type of component to search for</typeparam>
        /// <param name="obj">The GameObject to search in</param>
        /// <returns>A list of all components of type T found</returns>
        public static List<T> GetAllComponentsInChildrenRecursive<T>(this GameObject obj) where T : Component
        {
            List<T> results = new List<T>();
            
            if (obj == null)
            {
                return results;
            }

            T[] components = obj.GetComponents<T>();
            if (components.Length > 0)
            {
                results.AddRange(components);
            }

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                results.AddRange(GetAllComponentsInChildrenRecursive<T>(child.gameObject));
            }

            return results;
        }
    }
}
