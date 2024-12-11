using UnityEngine;

namespace Pong.Core
{
    /// <summary>
    /// Stores global variables accessible across the game.
    /// </summary>
    public static class Globals
    {
        public static bool IsServer { get; set; } = true;
        public static string ServerIP { get; set; } = "127.0.0.1";

        public static void ResetGlobals()
        {
            IsServer = true;
            ServerIP = "127.0.0.1";
        }
    }
}
