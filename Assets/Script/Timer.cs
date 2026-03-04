using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainTime;
    [SerializeField] GameObject gameOverPanel;


    bool isWin = false;
    public void Initialize(float time)
    {
        this.remainTime = time;
        this.isWin = false;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    public float GetRemainTime()
    {
        return remainTime;
    }
    public void StopTimer()
    {
        isWin = true;
    }

    void Update()
    {
        if (remainTime > 0 && !isWin)
        {
            remainTime -= Time.deltaTime;
        }
        else if (remainTime > 0 && isWin)
        {
            return;
        }
        else
        {
            remainTime = 0;
            Failed();
        }


        int minutes = Mathf.FloorToInt(remainTime / 60);
        int seconds = Mathf.FloorToInt(remainTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void Failed()
    {
        gameOverPanel.SetActive(true);
    }
}