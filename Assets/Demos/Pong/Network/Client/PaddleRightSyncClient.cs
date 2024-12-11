using UnityEngine;
using Pong.Constants;
using System.Net;
using Pong.Core;
using Pong.Core.Data;
using Pong.Utils;

/// <summary>
/// Synchronizes the right paddle state on the client.
/// </summary>
public class PaddleRightSyncClient : MonoBehaviour
{
    private ClientManager ClientMan;
    private float NextUpdateTimeout = -1;

    void Awake()
    {
        if (Globals.IsServer)
        {
            enabled = false;
        }
    }

    void Start()
    {
        ClientMan = FindFirstObjectByType<ClientManager>();

        // Register handler for paddle right updates
        MessageHandler.RegisterHandler(MessageType.PaddleRightUpdate, HandlePaddleRightUpdate);
    }

    void Update()
    {
        float direction = Input.GetAxisRaw("Vertical");
        PaddleMoveCommand command = new PaddleMoveCommand { Direction = direction };
        string json = JsonUtility.ToJson(command);

        ClientMan.UDP.SendUDPMessage($"{MessageType.PaddleRightMove}|{json}", ClientMan.ServerEndpoint);
    }


    /// <summary>
    /// Handles paddle right position updates received from the server.
    /// </summary>
    /// <param name="data">Serialized paddle state.</param>
    /// <param name="sender">Sender's IP endpoint.</param>
    private void HandlePaddleRightUpdate(string data, IPEndPoint sender)
    {
        PaddleState state = JsonUtility.FromJson<PaddleState>(data);

        transform.position = state.Position;

        PongLogger.Verbose("Client", $"Updated local right paddle position to {state.Position}");
    }
}
