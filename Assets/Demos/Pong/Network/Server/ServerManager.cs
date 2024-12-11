using System.Collections.Generic;
using System.Collections;
using System.Net;
using UnityEngine;
using Pong.Core;
using Pong.Core.Objects;
using Pong.Core.Data;
using Pong.Constants;
using Pong.Utils;

/// <summary>
/// Manages server-side logic, including player connections and state broadcasting.
/// </summary>
public class ServerManager : MonoBehaviour
{
    public UDPService UDP;
    public int ListenPort = 25000;

    private PongBall ball;
    private const int REQUIRED_PLAYERS = 1;

    private Vector3 leftPaddlePosition;
    private Vector3 rightPaddlePosition;
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
        ball = FindFirstObjectByType<PongBall>();

        PongLogger.Info("Server", $"Server started on port {ListenPort}");

        // Register message handlers
        PongLogger.Info("Server", "Registering message handlers...");
        MessageHandler.RegisterHandler(MessageType.Coucou, HandleCoucouMessage);
        MessageHandler.RegisterHandler(MessageType.PaddleLeftMove, HandlePaddleMove);
        MessageHandler.RegisterHandler(MessageType.PaddleRightMove, HandlePaddleMove);
        MessageHandler.RegisterHandler(MessageType.BallUpdate, HandleBallUpdate);

        PongLogger.Info("Server", "Message handlers registered.");
    }

    void Update()
    {
        if (Clients.Count >= REQUIRED_PLAYERS)
        {
            BroadcastGameState();
        }
    }

    /// <summary>
    /// Stops the server and notifies all clients.
    /// </summary>
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
                PongLogger.Info("Server", "Two players connected. Starting game.");
                ball.StartMoving();
                BroadcastUDPMessage(MessageType.GameStart);
            }
        }
        else
        {
            PongLogger.Warning("Server", $"Duplicate 'coucou' from client: {addr}");
        }

        UDP.SendUDPMessage(MessageType.Welcome, sender);
    }

    private void HandlePaddleMove(string data, IPEndPoint sender)
    {
        string[] tokens = data.Split('|');
        string messageType = tokens[0];
        string json = tokens[1];

        PaddleMoveCommand command = JsonUtility.FromJson<PaddleMoveCommand>(json);
        Vector3 movement = Vector3.up * command.Direction * Time.deltaTime;

        if (messageType == MessageType.PaddleLeftMove)
        {
            leftPaddlePosition += movement;
            leftPaddlePosition.y = Mathf.Clamp(leftPaddlePosition.y, -4, 4);
        }
        else if (messageType == MessageType.PaddleRightMove)
        {
            rightPaddlePosition += movement;
            rightPaddlePosition.y = Mathf.Clamp(rightPaddlePosition.y, -4, 4);
        }
    }

    private void HandleBallUpdate(string data, IPEndPoint sender)
    {
        BallState state = JsonUtility.FromJson<BallState>(data);
        ball.transform.position = state.Position;
        PongLogger.Verbose("Server", $"Updated ball position to {state.Position}");
    }


    private void BroadcastGameState()
    {
        BallState ballState = new BallState { Position = ball.transform.position };
        string ballJson = JsonUtility.ToJson(ballState);

        PaddleState leftPaddleState = new PaddleState { Position = leftPaddlePosition };
        string leftPaddleJson = JsonUtility.ToJson(leftPaddleState);

        PaddleState rightPaddleState = new PaddleState { Position = rightPaddlePosition };
        string rightPaddleJson = JsonUtility.ToJson(rightPaddleState);

        BroadcastUDPMessage($"{MessageType.BallUpdate}|{ballJson}");
        BroadcastUDPMessage($"{MessageType.PaddleLeftUpdate}|{leftPaddleJson}");
        BroadcastUDPMessage($"{MessageType.PaddleRightUpdate}|{rightPaddleJson}");
    }

    public void BroadcastUDPMessage(string message)
    {
        foreach (var client in Clients.Values)
        {
            UDP.SendUDPMessage(message, client);
            PongLogger.Info("Server", $"Broadcasted message to {client.Address}:{client.Port} => {message}");
        }
    }
}
