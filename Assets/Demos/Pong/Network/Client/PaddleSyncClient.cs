using UnityEngine;
using System.Net;
using Pong.Constants;
using Pong.Core;
using Pong.Core.Data;
using Pong.Utils;

namespace Pong.Network.Client
{
    /// <summary>
    /// Handles the paddle synchronization logic on the client.
    /// </summary>
    public class PaddleSyncClient : MonoBehaviour
    {
        public bool IsLeftPaddle = true;
        private ClientManager ClientMan;

        private float LastSendTime = 0;
        private bool IsMoving = false;
        private float CurrentDirection = 0;

        private const float SendInterval = 0.1f; // Minimum interval between updates

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

            string updateMessageType = IsLeftPaddle ? MessageType.PaddleLeftUpdate : MessageType.PaddleRightUpdate;
            MessageHandler.RegisterHandler(updateMessageType, HandlePaddleUpdate);
        }

        void Update()
        {
            float direction = Input.GetAxisRaw("Vertical");
            bool isCurrentlyMoving = direction != 0;

            // Send only on state change or at regular intervals
            if (isCurrentlyMoving != IsMoving || Time.time - LastSendTime > SendInterval)
            {
                IsMoving = isCurrentlyMoving;
                CurrentDirection = direction;

                PaddleMoveCommand command = new PaddleMoveCommand
                {
                    Direction = direction,
                    State = isCurrentlyMoving ? "Moving" : "Stopped"
                };

                string json = JsonUtility.ToJson(command);
                string messageType = IsLeftPaddle ? MessageType.PaddleLeftMove : MessageType.PaddleRightMove;

                ClientMan.UDP.SendUDPMessage($"{messageType}|{json}", ClientMan.ServerEndpoint);

                PongLogger.Verbose("PaddleSyncClient", $"Sent {messageType} with direction {direction} and state {command.State}");

                LastSendTime = Time.time;
            }
        }

        private void HandlePaddleUpdate(string data, IPEndPoint sender)
        {
            PaddleState state = JsonUtility.FromJson<PaddleState>(data);

            // Update local position of the paddle
            transform.position = state.Position;

            PongLogger.Verbose("PaddleSyncClient", $"Updated local paddle position to {state.Position}");
        }
    }
}
