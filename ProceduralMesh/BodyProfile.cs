namespace MAPI.ProceduralMesh
{
    /// <summary>
    /// Defines the shape profile for a segmented organic body.
    /// Reference implementation for creating organic shapes like animal bodies.
    /// </summary>
    /// <remarks>
    /// This is a reference implementation provided as an example.
    /// For custom organic shapes, inherit from <see cref="OrganicShapeGenerator"/> and implement your own generation logic.
    /// </remarks>
    public class BodyProfile
    {
        #region Properties
        
        /// <summary>
        /// Array of ring profiles defining the body shape
        /// Each Vector4 contains: (z-position, widthScale, heightScaleTop, heightScaleBottom, asymmetry)
        /// </summary>
        public RingProfile[] Rings { get; set; }

        /// <summary>
        /// Number of vertices per ring (default 8)
        /// </summary>
        public int RingSegments { get; set; } = 8;
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Create a custom body profile
        /// </summary>
        /// <param name="rings">Array of ring profiles</param>
        /// <param name="ringSegments">Number of segments per ring</param>
        public BodyProfile(RingProfile[] rings, int ringSegments = 8)
        {
            Rings = rings;
            RingSegments = ringSegments;
        }
        
        #endregion

        #region Presets
        
        /// <summary>
        /// Default dog body profile (front to tail)
        /// </summary>
        public static BodyProfile DefaultDogBody =>
            new BodyProfile(new RingProfile[]
            {
                new RingProfile(0.5f, 0.8f, 0.9f, 0.9f, 0f),      // Front (chest)
                new RingProfile(0.25f, 1f, 1f, 1f, 0f),           // Chest widest
                new RingProfile(0f, 0.9f, 0.9f, 0.9f, -0.05f),    // Mid-back
                new RingProfile(-0.25f, 0.7f, 0.7f, 0.7f, -0.1f), // Taper toward tail
                new RingProfile(-0.5f, 0.8f, 0.8f, 0.8f, 0f)      // Tail base
            }, 8);

        /// <summary>
        /// Simple cylindrical body profile
        /// </summary>
        public static BodyProfile Cylindrical =>
            new BodyProfile(new RingProfile[]
            {
                new RingProfile(0.5f, 1f, 1f, 1f, 0f),
                new RingProfile(-0.5f, 1f, 1f, 1f, 0f)
            }, 12);

        /// <summary>
        /// Tapered body profile (wider at front, narrow at back)
        /// </summary>
        public static BodyProfile Tapered =>
            new BodyProfile(new RingProfile[]
            {
                new RingProfile(0.5f, 1f, 1f, 1f, 0f),
                new RingProfile(0f, 0.75f, 0.75f, 0.75f, 0f),
                new RingProfile(-0.5f, 0.5f, 0.5f, 0.5f, 0f)
            }, 8);
        
        #endregion
    }

    /// <summary>
    /// Defines a single ring in a body profile
    /// </summary>
    public struct RingProfile
    {
        /// <summary>
        /// Position along the body (typically -0.5 to 0.5 normalized)
        /// </summary>
        public float ZPosition;

        /// <summary>
        /// Width scale factor for this ring
        /// </summary>
        public float WidthScale;

        /// <summary>
        /// Height scale factor for upper half of ring
        /// </summary>
        public float HeightScaleTop;

        /// <summary>
        /// Height scale factor for lower half of ring
        /// </summary>
        public float HeightScaleBottom;

        /// <summary>
        /// Asymmetry factor (0 = symmetrical, positive/negative = asymmetric deformation)
        /// </summary>
        public float Asymmetry;

        /// <summary>
        /// Create a new ring profile
        /// </summary>
        public RingProfile(float zPosition, float widthScale, float heightScaleTop, float heightScaleBottom, float asymmetry)
        {
            ZPosition = zPosition;
            WidthScale = widthScale;
            HeightScaleTop = heightScaleTop;
            HeightScaleBottom = heightScaleBottom;
            Asymmetry = asymmetry;
        }
    }
}
