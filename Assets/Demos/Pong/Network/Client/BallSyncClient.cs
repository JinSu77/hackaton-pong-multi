using UnityEngine;
using System.Net;
using Pong.Constants;
using Pong.Core;
using Pong.Core.Data;
using Pong.Utils;

namespace Pong.Network.Client
{
  /// <summary>
  /// Synchronizes the ball's state on the client.
  /// </summary>
  public class BallSyncClient : MonoBehaviour
  {
    private ClientManager clientManager;
    private Vector3 ballPosition;
    private PongBallState ballState = PongBallState.Playing;

    void Awake()
    {
      if (Globals.IsServer)
      {
        enabled = false;
      }
    }

    void Start()
    {
      clientManager = FindFirstObjectByType<ClientManager>();
      MessageHandler.RegisterHandler(MessageType.BallUpdate, HandleBallUpdate);
      MessageHandler.RegisterHandler(MessageType.GameOver, HandleGameOver);
    }

    /// <summary>
    /// Handles updates to the ball's position from the server.
    /// </summary>
    private void HandleBallUpdate(string data, IPEndPoint sender)
    {
      BallState state = JsonUtility.FromJson<BallState>(data);
      ballPosition = state.Position;
      transform.position = ballPosition;
    }

    /// <summary>
    /// Handles game-over messages from the server.
    /// </summary>
    private void HandleGameOver(string data, IPEndPoint sender)
    {
      if (System.Enum.TryParse(data, out PongBallState newState))
      {
        ballState = newState;

        if (ballState != PongBallState.Playing)
        {
          // Stop any local ball-related behavior
          PongLogger.Info("BallSyncClient", $"Game over! State: {ballState}");
        }
      }
    }
  }
}
