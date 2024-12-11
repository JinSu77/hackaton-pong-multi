using UnityEngine;
using Pong.Constants;
using Pong.Network.Server;
using Pong.Utils; // Import PongLogger

namespace Pong.Core.Physics
{
    public class BallCollisionHandler : MonoBehaviour
    {
        private Vector3 direction = new Vector3(1, 0, 0);
        public float Speed = 1f;

        private BallSyncServer ballSyncServer;

        void Start()
        {
            direction = new Vector3(Random.Range(0.5f, 1), Random.Range(-0.5f, 0.5f), 0);

            ballSyncServer = FindFirstObjectByType<BallSyncServer>();
            if (ballSyncServer == null)
            {
                PongLogger.Error("BallCollisionHandler", "BallSyncServer not found in the scene!");
            }
            else
            {
                PongLogger.Info("BallCollisionHandler", "BallCollisionHandler initialized and BallSyncServer connected.");
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            switch (collision.collider.tag)
            {
                case "Paddle":
                    HandlePaddleCollision(collision);
                    break;

                case "Boundary":
                    HandleBoundaryCollision(collision);
                    break;

                case "GoalLeft":
                    HandleGoalCollision(PongBallState.PlayerRightWin);
                    break;

                case "GoalRight":
                    HandleGoalCollision(PongBallState.PlayerLeftWin);
                    break;

                default:
                    PongLogger.Warning("BallCollisionHandler", $"Unhandled collision with object: {collision.collider.name}");
                    break;
            }
        }

        private void HandlePaddleCollision(Collision collision)
        {
            // Reverse the X direction to reflect off the paddle
            direction.x = -direction.x;

            // Optional: Add variation to the direction based on the collision point
            float hitFactor = (transform.position.y - collision.collider.transform.position.y) / collision.collider.bounds.size.y;
            direction.y += hitFactor;

            direction.Normalize();

            PongLogger.Verbose("BallCollisionHandler", $"Paddle collision detected. New direction: {direction}");

            // Inform BallSyncServer of the updated ball state
            ballSyncServer.UpdateBallState(transform.position, PongBallState.Playing);
        }

        private void HandleBoundaryCollision(Collision collision)
        {
            // Reverse the Y direction to reflect off the boundary
            direction.y = -direction.y;

            PongLogger.Verbose("BallCollisionHandler", $"Boundary collision detected. New direction: {direction}");

            // Inform BallSyncServer of the updated ball state
            ballSyncServer.UpdateBallState(transform.position, PongBallState.Playing);
        }

        private void HandleGoalCollision(PongBallState newState)
        {
            // Stop the ball
            direction = Vector3.zero;

            PongLogger.Info("BallCollisionHandler", $"Goal collision detected. Ball state updated to {newState}");

            // Inform BallSyncServer of the game-over state
            ballSyncServer.UpdateBallState(transform.position, newState);
        }

        void Update()
        {
            // Move the ball in the current direction
            transform.position += direction * Speed * Time.deltaTime;
        }
    }
}
