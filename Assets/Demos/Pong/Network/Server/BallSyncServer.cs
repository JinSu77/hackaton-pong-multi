using UnityEngine;
using Pong.Constants;
using Pong.Core;
using Pong.Core.Data;

namespace Pong.Core.Objects
{
    /// <summary>
    /// Synchronizes the ball's state on the server.
    /// </summary>
    public class BallSyncServer : MonoBehaviour
    {
        private ServerManager serverManager;
        private PongBall ball;
        private float nextUpdateTimeout = -1;

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
            ball = GetComponent<PongBall>();
        }

        void Update()
        {
            if (Time.time > nextUpdateTimeout && serverManager != null && ball != null)
            {
                BallState state = new BallState
                {
                    Position = ball.transform.position
                };

                string json = JsonUtility.ToJson(state);
                serverManager.BroadcastUDPMessage($"{MessageType.BallUpdate}|{json}");
                nextUpdateTimeout = Time.time + 0.03f; // Update every 30ms

                if (ball.State != PongBallState.Playing)
                {
                    serverManager.BroadcastUDPMessage($"{MessageType.GameOver}|{ball.State}");
                }
            }
        }
    }
}
