using MAPI.Utils;

namespace MAPI.Core
{
    /// <summary>
    /// Main entry point for MAPI library.
    /// Provides library metadata and optional configuration.
    /// </summary>
    /// <remarks>
    /// MAPI requires no initialization - all APIs work immediately.
    /// Unity automatically handles resource cleanup on scene unload and application quit.
    /// Use MaterialPresets.DefaultShader to override the default shader if needed.
    /// </remarks>
    public static class MAPI
    {
        #region Public Members

        /// <summary>
        /// Gets the MAPI library version.
        /// </summary>
        public static string Version =>
            Constants.LIBRARY_VERSION;

        /// <summary>
        /// Gets the MAPI library name.
        /// </summary>
        public static string Name =>
            Constants.LIBRARY_NAME;

        #endregion
    }
}
