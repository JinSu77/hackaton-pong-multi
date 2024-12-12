using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class StartCountdown : MonoBehaviour
{
    public PongBall pongBall;

    public TextMeshProUGUI countdownText; 
    public Image backgroundCountdown;
    public int countdownTime = 3;

    private Coroutine countdownCoroutine;

    void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    public void StartCountdownTimer()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine); // Arrêter toute ancienne coroutine
        }
        countdownCoroutine = StartCoroutine(CountdownToStart());
    }

    public IEnumerator CountdownToStart()
    {
        for (int i = countdownTime; i > 0; i--)
        {
            countdownText.text = i.ToString(); 
            yield return new WaitForSeconds(1); 
        }

        countdownText.gameObject.SetActive(false);
        backgroundCountdown.gameObject.SetActive(false);
        pongBall.StartMoving();
    }
}
