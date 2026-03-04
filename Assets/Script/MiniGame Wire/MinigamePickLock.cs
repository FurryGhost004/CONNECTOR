using System.Collections.Generic;
using UnityEngine;

public class MinigamePickLock : MonoBehaviour, IMinigame
{
    [System.Serializable]
    public class Bar
    {
        public RectTransform redPin;
        public RectTransform greyPin;

        [HideInInspector] public int greyLength;
        [HideInInspector] public int redPosition;
        [HideInInspector] public int greyPosition;
        [HideInInspector] public bool completed;
    }

    public float stepHeight = 80f;
    public int maxStep = 5;
    public List<Bar> bars = new List<Bar>();
    public GameObject LockObject;
    public int minGreyLength = 1;
    public int maxGreyLength = 3;

    private int currentIndex = 0;
    private bool isPlaying = false;

    private System.Action winCallback;

    // =========================
    // IMinigame IMPLEMENTATION
    // =========================

    public void SetWinCallback(System.Action callback)
    {
        winCallback = callback;
    }

    public void StartMinigame()
    {
        gameObject.SetActive(true);
        isPlaying = true;
        GeneratePin();
        UpdateVisual();
    }

    public void PauseGame()
    {
        isPlaying = false;
    }

    public void ResumeGame()
    {
        isPlaying = true;
    }

    // =========================

    void Update()
    {
        if (!isPlaying) return;

        Push();
        UpdateVisual();
    }

    void GeneratePin()
    {
        currentIndex = 0;

        for (int i = 0; i < bars.Count; i++)
        {
            var bar = bars[i];
            bar.completed = false;

            bar.greyLength = Random.Range(minGreyLength, maxGreyLength);
            bar.greyPosition = 1;
            bar.redPosition = bar.greyPosition + bar.greyLength;

            if (bar.greyPin != null)
            {
                var s = bar.greyPin.sizeDelta;
                s.y = bar.greyLength * stepHeight;
                bar.greyPin.sizeDelta = s;
            }
        }
    }

    void ResetBar(Bar bar)
    {
        bar.greyPosition = 1;
        bar.redPosition = bar.greyPosition + bar.greyLength;
    }

    void Push()
    {
        if (currentIndex >= bars.Count) return;

        Bar currentBar = bars[currentIndex];

        if (currentBar.completed) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentBar.greyPosition++;
            currentBar.redPosition = currentBar.greyPosition + currentBar.greyLength;

            if (currentBar.redPosition > maxStep)
            {
                ResetBar(currentBar);
            }
        }

        if (currentBar.redPosition == maxStep && Input.GetKeyDown(KeyCode.F))
        {
            currentBar.completed = true;
            currentIndex++;

            Debug.Log("PIN COMPLETED!");

            if (currentIndex >= bars.Count)
            {
                Debug.Log("LOCK OPEN!");

                isPlaying = false;

                if (LockObject != null)
                    LockObject.SetActive(false);

                winCallback?.Invoke();     // 🔥 báo cho ComputerInteraction
                gameObject.SetActive(false); // 🔥 ẩn minigame
            }
        }
    }

    void UpdateVisual()
    {
        for (int i = 0; i < bars.Count; i++)
        {
            var bar = bars[i];

            float height = bar.greyLength * stepHeight;
            float bottomY = (bar.greyPosition - 1) * stepHeight;

            if (bar.greyPin != null)
            {
                var s = bar.greyPin.sizeDelta;
                s.y = height;
                bar.greyPin.sizeDelta = s;
                bar.greyPin.anchoredPosition =
                    new Vector2(bar.greyPin.anchoredPosition.x, bottomY);
            }

            if (bar.redPin != null)
            {
                bar.redPin.anchoredPosition =
                    new Vector2(bar.redPin.anchoredPosition.x,
                    (bar.redPosition - 0.47f) * stepHeight);
            }

            if (bar.completed)
                SetBarAlpha(bar, 0.5f);
            else if (i < currentIndex)
                SetBarAlpha(bar, 0.6f);
            else if (i > currentIndex)
                SetBarAlpha(bar, 1f);
        }
    }

    void SetBarAlpha(Bar bar, float a)
    {
        if (bar.redPin != null)
        {
            var img = bar.redPin.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                var c = img.color;
                c.a = a;
                img.color = c;
            }
        }

        if (bar.greyPin != null)
        {
            var img = bar.greyPin.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                var c = img.color;
                c.a = a;
                img.color = c;
            }
        }
    }
}