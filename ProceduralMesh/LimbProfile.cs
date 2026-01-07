using UnityEngine;

namespace MAPI.ProceduralMesh
{
    /// <summary>
    /// Defines the shape profile for an articulated limb (leg, arm, tail, etc.).
    /// Reference implementation for organic shapes with joints.
    /// </summary>
    /// <remarks>
    /// This is a reference implementation provided as an example.
    /// For custom organic shapes, inherit from <see cref="OrganicShapeGenerator"/> and implement your own generation logic.
    /// </remarks>
    public class LimbProfile
    {
        #region Properties
        
        /// <summary>
        /// Joint positions along the limb (from top to bottom)
        /// </summary>
        public Vector3[] JointPositions { get; set; }

        /// <summary>
        /// Radius at each joint
        /// </summary>
        public float[] JointRadii { get; set; }

        /// <summary>
        /// Forward push amount at each joint (for knee/elbow bending)
        /// </summary>
        public float[] ForwardPush { get; set; }

        /// <summary>
        /// Number of radial segments per ring (default 12)
        /// </summary>
        public int Segments { get; set; } = 12;
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Create a custom limb profile
        /// </summary>
        public LimbProfile(Vector3[] jointPositions, float[] jointRadii, float[]? forwardPush = null, int segments = 12)
        {
            JointPositions = jointPositions;
            JointRadii = jointRadii;
            ForwardPush = forwardPush ?? new float[jointPositions.Length];
            Segments = segments;
        }
        
        #endregion

        #region Presets
        
        /// <summary>
        /// Default dog leg profile with knee and ankle articulation
        /// </summary>
        public static LimbProfile DefaultDogLeg(float height, float topRadius)
        {
            float bottomRadius = topRadius * 0.7f;
            
            return new LimbProfile(
                jointPositions: new Vector3[]
                {
                    new Vector3(0, height * 0.5f, 0),          // Top
                    new Vector3(0, 0f, 0.1f * height),          // Knee
                    new Vector3(0, -height * 0.4f, -0.05f * height), // Ankle
                    new Vector3(0, -height * 0.5f, 0)           // Bottom
                },
                jointRadii: new float[]
                {
                    topRadius,
                    topRadius * 0.9f,
                    bottomRadius * 1.1f,
                    bottomRadius
                },
                forwardPush: new float[]
                {
                    0.15f,  // Top
                    0.25f,  // Knee
                    0f,     // Ankle
                    0f      // Bottom
                },
                segments: 12
            );
        }

        /// <summary>
        /// Simple cylindrical limb (no articulation)
        /// </summary>
        public static LimbProfile SimpleCylinder(float height, float radius)
        {
            return new LimbProfile(
                jointPositions: new Vector3[]
                {
                    new Vector3(0, height * 0.5f, 0),
                    new Vector3(0, -height * 0.5f, 0)
                },
                jointRadii: new float[] { radius, radius },
                forwardPush: new float[] { 0f, 0f },
                segments: 12
            );
        }

        /// <summary>
        /// Tapered limb (wider at top, narrower at bottom)
        /// </summary>
        public static LimbProfile Tapered(float height, float topRadius, float bottomRadius)
        {
            return new LimbProfile(
                jointPositions: new Vector3[]
                {
                    new Vector3(0, height * 0.5f, 0),
                    new Vector3(0, -height * 0.5f, 0)
                },
                jointRadii: new float[] { topRadius, bottomRadius },
                forwardPush: new float[] { 0f, 0f },
                segments: 12
            );
        }
        
        #endregion
    }
}
