using UnityEngine;
using Pong.Constants;
using System.Net;
using Pong.Core;
using Pong.Core.Data;

/// <summary>
/// Synchronizes the ball's state on the client.
/// </summary>
public class BallSyncClient : MonoBehaviour
{
  private ClientManager ClientMan;

  void Awake()
  {
    if (Globals.IsServer)
    {
      enabled = false;
    }
  }

  /// <summary>
  /// Registers a message handler for ball updates.
  /// </summary>
  void Start()
  {
    ClientMan = FindFirstObjectByType<ClientManager>();

    // Register handler for ball updates
    MessageHandler.RegisterHandler(MessageType.BallUpdate, HandleBallUpdate);
  }

  /// <summary>
  /// Handles ball position updates sent by the server.
  /// </summary>
  /// <param name="data">Serialized ball state data.</param>
  /// <param name="sender">Sender's IP endpoint.</param>
  private void HandleBallUpdate(string data, IPEndPoint sender)
  {
    BallState state = JsonUtility.FromJson<BallState>(data);
    transform.position = state.Position;
  }
}
