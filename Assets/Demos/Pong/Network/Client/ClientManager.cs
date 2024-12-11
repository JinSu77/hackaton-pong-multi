using System.Net;
using UnityEngine;
using Pong.Core;
using Pong.Core.Data;
using Pong.Constants;
using Pong.Utils;

/// <summary>
/// Manages the client-side logic, including message sending and receiving.
/// </summary>
public class ClientManager : MonoBehaviour
{
    public UDPService UDP;
    public int ServerPort = 25000;

    private float NextCoucouTimeout = -1;
    public IPEndPoint ServerEndpoint { get; private set; }

    void Awake()
    {
        if (Globals.IsServer)
        {
            PongLogger.Info("Client", "ClientManager disabled as this is the server instance.");
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        UDP.InitClient();
        ServerEndpoint = new IPEndPoint(IPAddress.Parse(Globals.ServerIP), ServerPort);

        PongLogger.Info("Client", $"Client initialized. Connecting to server at {Globals.ServerIP}:{ServerPort}");

        // Register message handlers
        MessageHandler.RegisterHandler(MessageType.Welcome, HandleWelcomeMessage);
        MessageHandler.RegisterHandler(MessageType.GameStart, HandleGameStartMessage);
        MessageHandler.RegisterHandler(MessageType.ServerStopping, HandleServerStoppingMessage);
        MessageHandler.RegisterHandler(MessageType.BallUpdate, HandleBallUpdate);
    }

    void Update()
    {
        if (Time.time > NextCoucouTimeout)
        {
            UDP.SendUDPMessage(MessageType.Coucou, ServerEndpoint);
            PongLogger.Verbose("Client", $"Sent 'coucou' to server at {ServerEndpoint.Address}:{ServerEndpoint.Port}");
            NextCoucouTimeout = Time.time + 1f; // Send "coucou" every 1 seconds
        }
    }

    /// <summary>
    /// Disconnects the client from the server and returns to the menu.
    /// </summary>
    public void Disconnect()
    {
        PongLogger.Info("Client", "Disconnecting from server...");
        UDP.SendUDPMessage(MessageType.Disconnect, ServerEndpoint);

        PongLogger.Info("Client", "Returning to menu.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("PongMenu");
    }

    /// <summary>
    /// Handles the "welcome" message from the server.
    /// </summary>
    /// <param name="data">The data received from the server.</param>
    /// <param name="sender">The server's endpoint.</param>
    private void HandleWelcomeMessage(string data, IPEndPoint sender)
    {
        PongLogger.Info("Client", $"Received 'welcome' from server at {sender.Address}:{sender.Port}");
        // Implement logic for welcome if needed
    }

    /// <summary>
    /// Handles the "game start" message from the server.
    /// </summary>
    /// <param name="data">The data received from the server.</param>
    /// <param name="sender">The server's endpoint.</param>
    private void HandleGameStartMessage(string data, IPEndPoint sender)
    {
        PongLogger.Info("Client", "Received 'GameStart' from server. Starting the game.");
        // Implement game start logic if needed
    }

    /// <summary>
    /// Handles the server stopping message.
    /// </summary>
    /// <param name="data">The data received from the server.</param>
    /// <param name="sender">The server's endpoint.</param>
    private void HandleServerStoppingMessage(string data, IPEndPoint sender)
    {
        PongLogger.Warning("Client", "Received 'ServerStopping' from server. Returning to menu.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("PongMenu");
    }

    private void HandleBallUpdate(string data, IPEndPoint sender)
    {
        BallState state = JsonUtility.FromJson<BallState>(data);
        PongLogger.Verbose("Client", $"Received ball position update: {state.Position}");
    }
}
