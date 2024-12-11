using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using Pong.Core;
using Pong.Constants;
using Pong.Utils;

namespace Pong.Network.Server
{
    /// <summary>
    /// Manages server-side logic, including player connections and state broadcasting.
    /// </summary>
    public class ServerManager : MonoBehaviour
    {
        public UDPService UDP;
        public int ListenPort = 25000;

        private const int REQUIRED_PLAYERS = 1;
        private Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>();

        void Awake()
        {
            if (!Globals.IsServer)
            {
                PongLogger.Info("Server", "ServerManager disabled as this is not the server instance.");
                gameObject.SetActive(false);
            }
        }

        void Start()
        {
            UDP.Listen(ListenPort);

            PongLogger.Info("Server", $"Server started on port {ListenPort}");

            // Register message handlers
            MessageHandler.RegisterHandler(MessageType.Coucou, HandleCoucouMessage);

            PongLogger.Info("Server", "Message handlers registered.");
        }

        void Update()
        {
            if (Clients.Count >= REQUIRED_PLAYERS)
            {
                BroadcastGameStart();
            }
        }

        public void StopServer()
        {
            PongLogger.Info("Server", "Stopping server...");
            BroadcastUDPMessage(MessageType.ServerStopping);

            StartCoroutine(StopServerCoroutine());
        }

        private IEnumerator StopServerCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            UDP.Close();
            Clients.Clear();

            PongLogger.Info("Server", "Server stopped. Returning to menu.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("PongMenu");
        }

        private void HandleCoucouMessage(string data, IPEndPoint sender)
        {
            string addr = sender.ToString();
            if (!Clients.ContainsKey(addr))
            {
                Clients.Add(addr, sender);
                PongLogger.Info("Server", $"New client connected: {addr}. Total clients: {Clients.Count}");

                if (Clients.Count == REQUIRED_PLAYERS)
                {
                    BroadcastGameStart();
                }
            }
            else
            {
                PongLogger.Warning("Server", $"Duplicate 'coucou' from client: {addr}");
            }

            UDP.SendUDPMessage(MessageType.Welcome, sender);
        }

        private void BroadcastGameStart()
        {
            PongLogger.Info("Server", "Broadcasting game start...");
            BroadcastUDPMessage(MessageType.GameStart);
        }

        public void BroadcastUDPMessage(string message)
        {
            foreach (var client in Clients.Values)
            {
                UDP.SendUDPMessage(message, client);
                PongLogger.Verbose("Server", $"Broadcasted message to {client.Address}:{client.Port} => {message}");
            }
        }
    }

}
