using UnityEngine;
using System.Net;
using Pong.Constants;
using Pong.Core;
using Pong.Core.Data;
using Pong.Utils;

namespace Pong.Network.Server
{
    /// <summary>
    /// Synchronizes the paddle's state on the server.
    /// </summary>
    public class PaddleSyncServer : MonoBehaviour
    {
        private ServerManager serverManager;
        public bool IsLeftPaddle = true;

        private Vector3 currentPosition;
        private float nextUpdateTimeout = -1;
        private const float UpdateInterval = 0.03f; // 30ms interval between updates

        void Awake()
        {
            if (!Globals.IsServer)
            {
                enabled = false;
            }
        }

        void Start()
        {
            serverManager = FindFirstObjectByType<ServerManager>();

            // Initialize paddle position
            currentPosition = transform.position;

            // Register the paddle move handler
            string messageType = IsLeftPaddle ? MessageType.PaddleLeftMove : MessageType.PaddleRightMove;
            MessageHandler.RegisterHandler(messageType, HandlePaddleMove);
        }

        void Update()
        {
            if (Time.time > nextUpdateTimeout && serverManager != null)
            {
                SendPaddleState();
                nextUpdateTimeout = Time.time + UpdateInterval;
            }
        }

        /// <summary>
        /// Handles paddle move messages received from the clients.
        /// </summary>
        /// <param name="data">Serialized paddle move command.</param>
        /// <param name="sender">Sender's IP endpoint.</param>
        private void HandlePaddleMove(string data, IPEndPoint sender)
        {
            PaddleMoveCommand command = JsonUtility.FromJson<PaddleMoveCommand>(data);

            // Update paddle position based on the received direction
            Vector3 movement = Vector3.up * command.Direction * Time.deltaTime;
            currentPosition += movement;
            currentPosition.y = Mathf.Clamp(currentPosition.y, -4, 4); // Clamp within bounds

            PongLogger.Verbose(
                "PaddleSyncServer",
                $"{(IsLeftPaddle ? "Left" : "Right")} paddle moved to {currentPosition} by client {sender.Address}:{sender.Port}"
            );
        }

        /// <summary>
        /// Sends the current paddle position to all clients.
        /// </summary>
        private void SendPaddleState()
        {
            PaddleState state = new PaddleState { Position = currentPosition };
            string json = JsonUtility.ToJson(state);

            string messageType = IsLeftPaddle ? MessageType.PaddleLeftUpdate : MessageType.PaddleRightUpdate;
            serverManager.BroadcastUDPMessage($"{messageType}|{json}");

            PongLogger.Verbose("PaddleSyncServer", $"Broadcasted {(IsLeftPaddle ? "left" : "right")} paddle position: {currentPosition}");
        }

        void OnDestroy()
        {
            // Unregister the handler when the paddle is destroyed
            string messageType = IsLeftPaddle ? MessageType.PaddleLeftMove : MessageType.PaddleRightMove;
            MessageHandler.UnregisterHandler(messageType);
        }
    }
}
