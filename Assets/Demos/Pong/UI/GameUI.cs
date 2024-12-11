using UnityEngine;
using UnityEngine.UI;
using Pong.Core;
using Pong.Network.Server;
using Pong.Network.Client;
using Pong.Utils;

namespace Pong.UI
{
    /// <summary>
    /// Manages the game UI, including disconnect and stop server buttons.
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        public Button disconnectButton;
        public Button stopServerButton;

        void Start()
        {
            PongLogger.Configure(true, false);
            PongLogger.Info("GameUI", "Game UI initialized.");

            if (Globals.IsServer)
            {
                SetupServerUI();
            }
            else
            {
                SetupClientUI();
            }
        }

        private void SetupServerUI()
        {
            disconnectButton.gameObject.SetActive(false);
            stopServerButton.gameObject.SetActive(true);
            stopServerButton.onClick.AddListener(StopServer);
        }

        private void SetupClientUI()
        {
            disconnectButton.gameObject.SetActive(true);
            stopServerButton.gameObject.SetActive(false);
            disconnectButton.onClick.AddListener(DisconnectClient);
        }

        private void StopServer()
        {
            var serverManager = FindFirstObjectByType<ServerManager>();
            if (serverManager != null)
            {
                PongLogger.Info("GameUI", "Stop server button clicked.");
                serverManager.StopServer();
            }
            else
            {
                PongLogger.Error("GameUI", "ServerManager not found.");
            }
        }

        private void DisconnectClient()
        {
            var clientManager = FindFirstObjectByType<ClientManager>();
            if (clientManager != null)
            {
                PongLogger.Info("GameUI", "Disconnect button clicked.");
                clientManager.Disconnect();
            }
            else
            {
                PongLogger.Error("GameUI", "ClientManager not found.");
            }
        }
    }
}
