using UnityEngine;
using UnityEngine.SceneManagement;

public class PongWinUI : MonoBehaviour
{
    public GameObject Panel;
    public GameObject PlayerLeft;
    public GameObject PlayerRight;

    PongBall Ball;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Panel.SetActive(false);
        PlayerLeft.SetActive(false);
        PlayerRight.SetActive(false);
        Ball = GameObject.FindFirstObjectByType<PongBall>();
    }

    public void ShowWinPanel(PongBallState winState)
    {
      switch (winState)
      {
        case PongBallState.Playing:
            Panel.SetActive(false);
            break;
        case PongBallState.RedTeamWin:
            Panel.SetActive(true);
            PlayerLeft.SetActive(true);  // Affiche l'équipe rouge comme gagnante
            break;
        case PongBallState.BlueTeamWin:
            Panel.SetActive(true);
            PlayerRight.SetActive(true);  // Affiche l'équipe bleue comme gagnante
            break;
      }
    }

    public void OnReplay() {
      Time.timeScale = 1;
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
