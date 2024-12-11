using UnityEngine;

namespace Pong.Utils
{
    /// <summary>
    /// Provides a centralized logging system for debug messages.
    /// </summary>
    public static class PongLogger
    {
        private static bool enableLogging = true;
        private static bool enableVerboseLogs = false;

        /// <summary>
        /// Logs an info message.
        /// </summary>
        public static void Info(string context, string message)
        {
            if (enableLogging)
            {
                Debug.Log($"[INFO] [{context}] {message}");
            }
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public static void Warning(string context, string message)
        {
            if (enableLogging)
            {
                Debug.LogWarning($"[WARNING] [{context}] {message}");
            }
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        public static void Error(string context, string message)
        {
            if (enableLogging)
            {
                Debug.LogError($"[ERROR] [{context}] {message}");
            }
        }

        /// <summary>
        /// Logs a verbose message.
        /// </summary>
        public static void Verbose(string context, string message)
        {
            if (enableLogging && enableVerboseLogs)
            {
                Debug.Log($"[VERBOSE] [{context}] {message}");
            }
        }

        /// <summary>
        /// Configures the logger settings.
        /// </summary>
        /// <param name="enable">Enable or disable logging.</param>
        /// <param name="verbose">Enable or disable verbose logging.</param>
        public static void Configure(bool enable, bool verbose = false)
        {
            enableLogging = enable;
            enableVerboseLogs = verbose;
        }
    }
}
