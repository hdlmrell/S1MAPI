using UnityEngine;

namespace S1MAPI.Extensions
{
    /// <summary>
    /// Extension methods for Vector3 operations.
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Create a new Vector3 with modified x component.
        /// </summary>
        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        /// <summary>
        /// Create a new Vector3 with modified y component.
        /// </summary>
        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        /// <summary>
        /// Create a new Vector3 with modified z component.
        /// </summary>
        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        /// <summary>
        /// Flatten vector to ground plane (set y to 0).
        /// </summary>
        public static Vector3 Flattened(this Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }

        /// <summary>
        /// Distance from this vector to another (shorthand).
        /// </summary>
        public static float DistanceTo(this Vector3 from, Vector3 to)
        {
            return Vector3.Distance(from, to);
        }

        /// <summary>
        /// Angle in degrees from this vector to another.
        /// </summary>
        public static float AngleTo(this Vector3 from, Vector3 to)
        {
            return Vector3.Angle(from, to);
        }

        /// <summary>
        /// Convert to Vector2 (drops y).
        /// </summary>
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }
    }
}
