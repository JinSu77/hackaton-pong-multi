using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI blueScoreText; 
    public TextMeshProUGUI redScoreText; 

    void Update()
    {
        blueScoreText.text = GameManager.Instance.blueTeamScore.ToString();
        redScoreText.text = GameManager.Instance.redTeamScore.ToString();
    }
}
