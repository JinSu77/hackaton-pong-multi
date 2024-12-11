using UnityEngine;
using Pong.Constants;
using Pong.Utils;

namespace Pong.Core.Objects
{
  public class PongBall : MonoBehaviour
  {
    public float Speed = 1;
    private bool canMove = false;

    private Vector3 direction;
    private PongBallState _state = PongBallState.Playing;

    /// <summary>
    /// Gets the current state of the Pong ball.
    /// </summary>
    public PongBallState State => _state;

    void Awake()
    {
      if (!Globals.IsServer)
      {
        enabled = false;
      }
    }

    void Start()
    {
      ResetBall();
      PongLogger.Info("PongBall", "Ball initialized and reset.");
    }

    /// <summary>
    /// Resets the ball's position and direction.
    /// </summary>
    public void ResetBall()
    {
      transform.position = Vector3.zero;
      direction = new Vector3(
          Random.Range(0.5f, 1),
          Random.Range(-0.5f, 0.5f),
          0
      );
      direction.x *= Mathf.Sign(Random.Range(-100, 100));
      direction.Normalize();
    }

    void Update()
    {
      if (_state != PongBallState.Playing || !canMove)
      {
        return;
      }

      transform.position += direction * Speed * Time.deltaTime;
    }

    /// <summary>
    /// Starts the ball's movement.
    /// </summary>
    public void StartMoving()
    {
      PongLogger.Info("PongBall", "Starting movement - Two players connected.");
      canMove = true;
    }

    /// <summary>
    /// Stops the ball's movement.
    /// </summary>
    public void StopMoving()
    {
      PongLogger.Info("PongBall", "Stopping movement - Waiting for players.");
      canMove = false;
      ResetBall();
    }

    void OnCollisionEnter(Collision collision)
    {
      PongLogger.Verbose("PongBall", $"Collision detected with {collision.collider.name}");

      switch (collision.collider.name)
      {
        case "BoundTop":
        case "BoundBottom":
          direction.y = -direction.y;
          PongLogger.Verbose("PongBall", "Direction inverted on Y-axis.");
          break;

        case "PaddleLeft":
        case "PaddleRight":
          // case "BoundLeft":
          // case "BoundRight":
          direction.x = -direction.x;
          PongLogger.Verbose("PongBall", "Direction inverted on X-axis.");
          break;

        case "BoundLeft":
          _state = PongBallState.PlayerRightWin;
          canMove = false;
          PongLogger.Info("PongBall", "PlayerRight wins.");
          break;

        case "BoundRight":
          _state = PongBallState.PlayerLeftWin;
          canMove = false;
          PongLogger.Info("PongBall", "PlayerLeft wins.");
          break;
      }
    }
  }
}
