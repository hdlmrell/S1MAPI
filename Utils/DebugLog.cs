using UnityEngine;

namespace MAPI.Utils
{
    /// <summary>
    /// Debug logging utility for MAPI library
    /// Provides consistent logging interface with conditional compilation
    /// </summary>
    public static class DebugLog
    {
        private static bool _debugEnabled = true;

        /// <summary>
        /// Enable or disable debug logging
        /// </summary>
        public static bool DebugEnabled
        {
            get => _debugEnabled;
            set => _debugEnabled = value;
        }

        /// <summary>
        /// Log an informational message
        /// </summary>
        public static void Info(string message)
        {
            if (!_debugEnabled) return;
            Debug.Log($"[{Constants.LIBRARY_NAME}] {message}");
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        public static void Warning(string message)
        {
            if (!_debugEnabled) return;
            Debug.LogWarning($"[{Constants.LIBRARY_NAME}] {message}");
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        public static void Error(string message)
        {
            Debug.LogError($"[{Constants.LIBRARY_NAME}] {message}");
        }

        /// <summary>
        /// Log an exception
        /// </summary>
        public static void Exception(Exception ex)
        {
            CrossException.LogException(ex);
        }

        /// <summary>
        /// Log a formatted informational message
        /// </summary>
        public static void InfoFormat(string format, params object[] args)
        {
            if (!_debugEnabled) return;
            Info(string.Format(format, args));
        }

        /// <summary>
        /// Log a formatted warning message
        /// </summary>
        public static void WarningFormat(string format, params object[] args)
        {
            if (!_debugEnabled) return;
            Warning(string.Format(format, args));
        }

        /// <summary>
        /// Log a formatted error message
        /// </summary>
        public static void ErrorFormat(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }
    }
}
