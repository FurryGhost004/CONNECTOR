using UnityEngine;


public class FillingBarMiniGame : MonoBehaviour
{
    int maxbarPoint = 100;

    [Header("UI References")]
    [SerializeField] private RectTransform marker;
    [SerializeField] private RectTransform successZone;
    [SerializeField] private RectTransform perfectZone;
    [SerializeField] private RectTransform trackArea;

    [Header("Settings")]
    [SerializeField] private float markerSpeed = 400f;
    private bool isGameActive = false;
    private int direction = 1;

    [Header("Progress Settings")]
    [SerializeField] private UnityEngine.UI.Slider progressSlider; 
    [SerializeField] private float currentProgress = 0f;
    [SerializeField] private float successBonus = 10f;  
    [SerializeField] private float perfectBonus = 20f;  
    [SerializeField] private float failPenalty = 5f;
    [SerializeField] private float progressMax = 100f;

    private float trackMin;
    private float trackMax;
    void Start()
    {
        float trackHeight = trackArea.rect.height;
        trackMin = -trackHeight / 2;
        trackMax = trackHeight / 2;

        StartMiniGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameActive)
        {
            return;
        }
        MoveMarker();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSkillCheck();
        }
    }

    public void StartMiniGame()
    {
        gameObject.SetActive(true);
        isGameActive = true;
        marker.localPosition = new Vector2(marker.localPosition.x, trackMin);
        direction = 1;

        float padding = successZone.rect.height / 2;
        float randomY = Random.Range(trackMin + padding, trackMax - padding);

        successZone.localPosition = new Vector3(successZone.localPosition.x, randomY, 0);
        perfectZone.localPosition = new Vector3(perfectZone.localPosition.x, randomY, 0);
    }

    void MoveMarker()
    {
        float newY = marker.localPosition.y;
        newY += direction * markerSpeed * Time.deltaTime;
        if (newY >= trackMax || newY <= trackMin)
        {
            direction *= -1;
        }

        marker.localPosition = new Vector2(marker.localPosition.x, newY);


    }

    void CheckSkillCheck()
    {
        isGameActive = false;

        float markerPos = marker.localPosition.y;

        float successMin = successZone.localPosition.y - (successZone.rect.height / 2);
        float successMax = successZone.localPosition.y + (successZone.rect.height / 2);

        float perfectMin = perfectZone.localPosition.y - (perfectZone.rect.height / 2);
        float perfectMax = perfectZone.localPosition.y + (perfectZone.rect.height / 2);

        if (markerPos >= perfectMin && markerPos <= perfectMax)
        {
            Debug.Log("Perfect!");
            UpdateProgress(perfectBonus);
        }
        else if (markerPos >= successMin && markerPos <= successMax)
        {
            Debug.Log("Success!");
            UpdateProgress(successBonus);
        }
        else
        {
            Debug.Log("Failed!");
            UpdateProgress(-failPenalty);
        }

        if (currentProgress < 100f)
        {
            Invoke("StartMiniGame", 1.0f);
        }
    }
    public void ResetMinigame()
    {
        isGameActive = false;
        marker.localPosition = new Vector3(marker.localPosition.x, trackMin, 0);
        currentProgress = 0f;

    }
    void UpdateProgress(float amount)
    {
        currentProgress += amount;


        currentProgress = Mathf.Clamp(currentProgress, 0f, 100f);


        if (progressSlider != null)
        {
            progressSlider.value = currentProgress;
        }

        if (currentProgress >= 100f)
        {
            Debug.Log("Sửa chữa hoàn tất!");
            OnRepairComplete();
        }
    }
    public System.Action OnComplete;
    public void OnRepairComplete()
    {
        isGameActive = false;
        CancelInvoke("StartMiniGame");
        gameObject.SetActive(false);
        OnComplete?.Invoke();
        Destroy(gameObject, 0.1f);
        Debug.Log("Minigame completed successfully!");
        // Add win logic
    }
}
