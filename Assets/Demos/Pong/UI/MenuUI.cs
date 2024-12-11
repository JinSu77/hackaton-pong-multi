using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Pong.Core;
using Pong.Utils;

namespace Pong.UI
{
    /// <summary>
    /// Manages the main menu UI, including server/client setup and scene transitions.
    /// </summary>
    public class MenuUI : MonoBehaviour
    {
        public TMP_InputField InpIP;

        void Start()
        {
            InpIP.text = Globals.ServerIP;
            PongLogger.Configure(true, true);
            PongLogger.Info("MenuUI", "Menu initialized.");
        }

        public void SetRole(bool isServer)
        {
            Globals.IsServer = isServer;
            PongLogger.Info("MenuUI", $"Role set to {(isServer ? "Server" : "Client")}.");

            if (!isServer && !string.IsNullOrEmpty(InpIP.text))
            {
                Globals.ServerIP = InpIP.text;
                PongLogger.Info("MenuUI", $"Server IP set to: {Globals.ServerIP}");
            }
        }

        public void StartGame()
        {
            if (!Globals.IsServer && string.IsNullOrEmpty(InpIP.text))
            {
                PongLogger.Warning("MenuUI", "Please enter a valid IP.");
                return;
            }

            PongLogger.Info("MenuUI", "Loading Pong scene.");
            SceneManager.LoadScene("Pong");
        }
    }
}
