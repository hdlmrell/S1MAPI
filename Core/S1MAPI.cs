using S1MAPI.Utils;

namespace S1MAPI.Core
{
    /// <summary>
    /// Main entry point for S1MAPI library.
    /// Provides library metadata and optional configuration.
    /// </summary>
    /// <remarks>
    /// S1MAPI requires no initialization - all APIs work immediately.
    /// Unity automatically handles resource cleanup on scene unload and application quit.
    /// Use MaterialPresets.DefaultShader to override the default shader if needed.
    /// </remarks>
    public static class S1MAPI
    {
        #region Public Members

        /// <summary>
        /// Gets the S1MAPI library version.
        /// </summary>
        public static string Version =>
            Constants.LIBRARY_VERSION;

        /// <summary>
        /// Gets the S1MAPI library name.
        /// </summary>
        public static string Name =>
            Constants.LIBRARY_NAME;

        #endregion
    }
}
