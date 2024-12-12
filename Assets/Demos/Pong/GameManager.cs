using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton pour un accès global

    public PongWinUI pongWinUI;

    PongBall Ball;

    public int blueTeamScore = 0;
    public int redTeamScore = 0;
    public int scoreToWin = 3;

    public PongBallState currentState = PongBallState.Playing;

    void Awake()
    {
        // Assure que seul un GameManager existe
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Garde le GameManager entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        pongWinUI = FindFirstObjectByType<PongWinUI>();
        Ball = GameObject.FindFirstObjectByType<PongBall>();
    }

    public void AddPointToBlue()
    {
        if (currentState == PongBallState.Playing)
        {
            blueTeamScore++;
            CheckWinCondition();
        }
    }

    public void AddPointToRed()
    {
        if (currentState == PongBallState.Playing)
        {
            redTeamScore++;
            CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        if (blueTeamScore >= scoreToWin)
        {
            Time.timeScale = 0;
            currentState = PongBallState.BlueTeamWin;
            pongWinUI.ShowWinPanel(PongBallState.BlueTeamWin);
        }
        else if (redTeamScore >= scoreToWin)
        {
            Time.timeScale = 0;
            currentState = PongBallState.RedTeamWin;
            pongWinUI.ShowWinPanel(PongBallState.RedTeamWin);
        }
    }
}
