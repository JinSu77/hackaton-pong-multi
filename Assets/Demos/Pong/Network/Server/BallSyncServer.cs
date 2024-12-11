using UnityEngine;
using Pong.Constants;
using Pong.Core;
using Pong.Core.Data;

namespace Pong.Network.Server
{
    /// <summary>
    /// Synchronizes the ball's state on the server.
    /// </summary>
    public class BallSyncServer : MonoBehaviour
    {
        private ServerManager serverManager;
        private Vector3 ballPosition;
        private PongBallState ballState = PongBallState.Playing;
        private float nextUpdateTimeout = -1;

        public delegate void BallStateChangedHandler(PongBallState newState, Vector3 position);
        public static event BallStateChangedHandler BallStateChanged;

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
        }

        void Update()
        {
            if (Time.time > nextUpdateTimeout && serverManager != null)
            {
                BroadcastBallState();
                nextUpdateTimeout = Time.time + UpdateInterval;
            }
        }

        /// <summary>
        /// Updates the ball's position and state, and triggers the BallStateChanged event if necessary.
        /// </summary>
        public void UpdateBallState(Vector3 position, PongBallState state)
        {
            if (ballState != state)
            {
                ballState = state;
                BallStateChanged?.Invoke(state, position);
                NotifyGameOver();
            }

            ballPosition = position;
        }

        /// <summary>
        /// Sends the ball's position to all clients.
        /// </summary>
        private void BroadcastBallState()
        {
            BallState state = new BallState { Position = ballPosition };
            string json = JsonUtility.ToJson(state);

            serverManager.BroadcastUDPMessage($"{MessageType.BallUpdate}|{json}");
        }

        /// <summary>
        /// Notifies all clients of a game-over state.
        /// </summary>
        private void NotifyGameOver()
        {
            if (ballState != PongBallState.Playing)
            {
                serverManager.BroadcastUDPMessage($"{MessageType.GameOver}|{ballState}");
            }
        }
    }
}
