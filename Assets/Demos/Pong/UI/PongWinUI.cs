using UnityEngine;
using UnityEngine.SceneManagement;
using Pong.Constants;

namespace Pong.UI
{
  public class PongWinUI : MonoBehaviour
  {
    public GameObject Panel;
    public GameObject PlayerLeft;
    public GameObject PlayerRight;

    private PongBallState currentBallState = PongBallState.Playing;

    void Start()
    {
      Panel.SetActive(false);
      PlayerLeft.SetActive(false);
      PlayerRight.SetActive(false);
    }

    void Update()
    {
      UpdateUI();
    }

    /// <summary>
    /// Call this method when the ball state changes (e.g., from the server logic).
    /// </summary>
    public void SetBallState(PongBallState newState)
    {
      currentBallState = newState;
    }

    private void UpdateUI()
    {
      switch (currentBallState)
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
}
