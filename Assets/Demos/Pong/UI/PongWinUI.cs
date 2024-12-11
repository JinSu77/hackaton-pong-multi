using UnityEngine;
using UnityEngine.SceneManagement;
using Pong.Constants;
using Pong.Core.Objects;

public class PongWinUI : MonoBehaviour
{
  public GameObject Panel;
  public GameObject PlayerLeft;
  public GameObject PlayerRight;

  private PongBall ball;

  void Start()
  {
    Panel.SetActive(false);
    PlayerLeft.SetActive(false);
    PlayerRight.SetActive(false);
    ball = GameObject.FindFirstObjectByType<PongBall>();
  }

  void Update()
  {
    if (ball == null) return;

    switch (ball.State)
    {
      case PongBallState.Playing:
        Panel.SetActive(false);
        break;

      case PongBallState.PlayerLeftWin:
        Panel.SetActive(true);
        PlayerLeft.SetActive(true);
        PlayerRight.SetActive(false);
        break;

      case PongBallState.PlayerRightWin:
        Panel.SetActive(true);
        PlayerLeft.SetActive(false);
        PlayerRight.SetActive(true);
        break;
    }
  }

  public void OnReplay()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}
