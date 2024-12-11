using UnityEngine;
using Pong.Constants;
using System.Net;
using Pong.Core;
using Pong.Core.Data;
using Pong.Utils;

/// <summary>
/// Synchronizes the left paddle state on the client.
/// </summary>
public class PaddleLeftSyncClient : MonoBehaviour
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

        // Register handler for paddle left updates
        MessageHandler.RegisterHandler(MessageType.PaddleLeftUpdate, HandlePaddleLeftUpdate);
    }

    void Update()
    {
        float direction = Input.GetAxisRaw("Vertical");
        PaddleMoveCommand command = new PaddleMoveCommand { Direction = direction };
        string json = JsonUtility.ToJson(command);

        ClientMan.UDP.SendUDPMessage($"{MessageType.PaddleLeftMove}|{json}", ClientMan.ServerEndpoint);
    }


    /// <summary>
    /// Handles paddle left position updates received from the server.
    /// </summary>
    /// <param name="data">Serialized paddle state.</param>
    /// <param name="sender">Sender's IP endpoint.</param>
    private void HandlePaddleLeftUpdate(string data, IPEndPoint sender)
    {
        PaddleState state = JsonUtility.FromJson<PaddleState>(data);

        transform.position = state.Position;

        PongLogger.Verbose("Client", $"Updated local left paddle position to {state.Position}");
    }
}
