using UnityEngine;
using UnityEngine.InputSystem;
using Pong.Core.Data;
using Pong.Constants;
using Pong.Utils;
using Pong.Core.Objects; // Ensure this is included for PongPlayer

namespace Pong.Core.Objects
{
  /// <summary>
  /// Manages the paddle logic, including input and position synchronization.
  /// </summary>
  public class PongPaddle : MonoBehaviour
  {
    public PongPlayer Player = PongPlayer.PlayerLeft;
    public float Speed = 1;
    public float MinY = -4;
    public float MaxY = 4;

    private PongInput inputActions;
    private InputAction PlayerAction;

    private bool isServer => Globals.IsServer;

    void Start()
    {
      if (isServer)
      {
        PongLogger.Info("PongPaddle", $"Paddle controls disabled for {Player} (Server mode).");
        enabled = false;
        return;
      }

      inputActions = new PongInput();
      switch (Player)
      {
        case PongPlayer.PlayerLeft:
          PlayerAction = inputActions.Pong.Player1;
          break;
        case PongPlayer.PlayerRight:
          PlayerAction = inputActions.Pong.Player2;
          break;
      }

      PlayerAction.Enable();
      PongLogger.Info("PongPaddle", $"{Player} controls enabled.");
    }

    void Update()
    {
      if (isServer)
      {
        return;
      }

      float direction = PlayerAction.ReadValue<float>();
      if (direction != 0)
      {
        SendPaddleMoveCommand(direction);
      }
    }

    private void SendPaddleMoveCommand(float direction)
    {
      string messageType = Player == PongPlayer.PlayerLeft ? MessageType.PaddleLeftMove : MessageType.PaddleRightMove;
      PaddleMoveCommand command = new PaddleMoveCommand { Direction = direction };
      string json = JsonUtility.ToJson(command);

      ClientManager clientManager = FindFirstObjectByType<ClientManager>();
      if (clientManager != null)
      {
        clientManager.UDP.SendUDPMessage($"{messageType}|{json}", clientManager.ServerEndpoint);
        PongLogger.Verbose("PongPaddle", $"Sent {messageType} command with direction {direction}");
      }
    }

    void OnDisable()
    {
      PlayerAction?.Disable();
    }
  }
}
