using UnityEngine;

public enum PongBallState {
  Playing = 0,
  RedTeamWin = 1,
  BlueTeamWin = 2,
}

public class PongBall : MonoBehaviour
{
    public ScoreText scoreText;

    public float Speed = 1;
    private bool canMove = false; //contrôler le mouvement

    Vector3 Direction;
    PongBallState _State = PongBallState.Playing;

    public PongBallState State {
      get {
        return _State;
      }
    } 

    void Awake() {
      if (!Globals.IsServer) {
        enabled = false;
      }

    }

    void Start() {
      ResetBall();
    }

    public void ResetBall()
    {
        // Réinitialiser la position et la direction
        transform.position = Vector3.zero;
        Direction = new Vector3(
            Random.Range(0.5f, 1),
            Random.Range(-0.5f, 0.5f),
            0
        );
        Direction.x *= Mathf.Sign(Random.Range(-100, 100));
        Direction.Normalize();
    }

    void Update() {
      if (State != PongBallState.Playing || !canMove) {
        return;
      }

      transform.position = transform.position + (Direction * Speed * Time.deltaTime);
    }

    // Méthode appelée par le ServerManager quand deux clients sont connectés
    public void StartMoving()
    {
        Debug.Log("[BALL] Starting movement - Two players connected!");
        canMove = true;
    }

    // Méthode appelée si un joueur se déconnecte
    public void StopMoving()
    {
        Debug.Log("[BALL] Stopping movement - Waiting for players...");
        canMove = false;
        ResetBall();
    }

    void OnCollisionEnter(Collision c) {
      switch (c.collider.name) {
        case "BoundTop":
        case "BoundBottom":
          Direction.y = -Direction.y;
          break;

        case "PaddleLeft":
        case "PaddleRight":
          Direction.x = -Direction.x;
          break;
        case "BoundLeft":
          Direction.x = -Direction.x;
          GameManager.Instance.AddPointToBlue();
          break;
        case "BoundRight":
          Direction.x = -Direction.x;
          GameManager.Instance.AddPointToRed();
          break;

                /*
                case "BoundLeft":
                  _State = PongBallState.RedTeamWin;
                  break;

                case "BoundRight":
                  _State = PongBallState.BlueTeamWin;
                  break;
                */

        }
    }

}
