using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UITimingMinigame : MonoBehaviour, IMinigame
{
    [Header("UI References")]
    public RectTransform movingBar;
    public RectTransform barBackground;
    public RectTransform[] successZones;
    public TextMeshProUGUI resultText;

    [Header("Settings")]
    public float speed = 300f;

    public System.Action OnMinigameWin;

    private bool movingRight;
    private bool isPlaying;
    private bool isStunned;

    private bool[] zoneCompleted;
    private float limit;
    private System.Action winCallback;

    public void SetWinCallback(System.Action onWin)
    {
        winCallback = onWin;
    }

    public void StartMinigame()
    {
        if (movingBar == null || barBackground == null || resultText == null)
        {
            Debug.LogError("Missing UI references in UITimingMinigame!");
            return;
        }

        if (successZones == null || successZones.Length == 0)
        {
            Debug.LogError("No SuccessZones assigned!");
            return;
        }

        isPlaying = true;
        isStunned = false;
        movingRight = true;
        resultText.text = "";

        limit = barBackground.rect.width / 2f - movingBar.rect.width / 2f;
        movingBar.anchoredPosition = new Vector2(-limit, 0);

        zoneCompleted = new bool[successZones.Length];

        for (int i = 0; i < successZones.Length; i++)
        {
            if (successZones[i] == null)
            {
                Debug.LogError("SuccessZone " + i + " is NULL");
                continue;
            }

            float randomX = Random.Range(-limit + 40f, limit - 40f);
            successZones[i].anchoredPosition = new Vector2(randomX, 0);

            zoneCompleted[i] = false;

            Image img = successZones[i].GetComponent<Image>();
            if (img != null)
            {
                img.color = Color.yellow;
            }
        }
    }

    void Update()
    {
        if (!isPlaying || isStunned)
            return;

        MoveBar();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryHitZone();
        }
    }

    void MoveBar()
    {
        float direction = movingRight ? 1f : -1f;

        movingBar.anchoredPosition +=
            Vector2.right * direction * speed * Time.deltaTime;

        if (movingBar.anchoredPosition.x >= limit)
            movingRight = false;

        if (movingBar.anchoredPosition.x <= -limit)
            movingRight = true;
    }

    void TryHitZone()
    {
        float barLeft = movingBar.anchoredPosition.x - movingBar.rect.width / 2f;
        float barRight = movingBar.anchoredPosition.x + movingBar.rect.width / 2f;

        bool hitSomething = false;

        for (int i = 0; i < successZones.Length; i++)
        {
            if (zoneCompleted[i]) continue;

            RectTransform zone = successZones[i];

            float zoneLeft = zone.anchoredPosition.x - zone.rect.width / 2f;
            float zoneRight = zone.anchoredPosition.x + zone.rect.width / 2f;

            bool overlap = barRight > zoneLeft && barLeft < zoneRight;

            if (overlap)
            {
                Image img = zone.GetComponent<Image>();
                if (img != null)
                    img.color = Color.green;

                zoneCompleted[i] = true;
                hitSomething = true;
                break;
            }
        }

        if (!hitSomething)
        {
            resultText.text = "MISS!";
            StartCoroutine(StunCoroutine());
        }

        CheckAllCompleted();
    }

    IEnumerator StunCoroutine()
    {
        isStunned = true;
        yield return new WaitForSeconds(1f);
        resultText.text = "";
        isStunned = false;
    }

    void CheckAllCompleted()
    {
        for (int i = 0; i < zoneCompleted.Length; i++)
        {
            if (!zoneCompleted[i])
                return;
        }

        resultText.text = "SUCCESS!";
        isPlaying = false;
        StartCoroutine(ExitAfterDelay());
    }

    IEnumerator ExitAfterDelay()
    {
        yield return new WaitForSeconds(0f);

        winCallback?.Invoke();
        gameObject.SetActive(false);
    }
    public void PauseGame()
    {
        isPlaying = false;
    }

    public void ResumeGame()
    {
        isPlaying = true;
    }
}