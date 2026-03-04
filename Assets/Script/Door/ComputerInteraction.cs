using UnityEngine;

public class ComputerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MonoBehaviour minigameScript;
    [SerializeField] private BossHealth bossHealth;

    private IMinigame currentMinigame;

    private bool playerNear = false;
    private bool isPlaying = false;
    private bool isCompleted = false;

    void Start()
    {
        currentMinigame = minigameScript as IMinigame;

        if (currentMinigame == null)
        {
            Debug.LogError("Assigned object does not implement IMinigame!");
            return;
        }

        minigameScript.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.F))
        {
            if (!isPlaying && !isCompleted)
            {
                StartMinigame();
            }
            else if (isPlaying)
            {
                ResumeMinigame();
            }
        }
    }

    // ================= START =================

    void StartMinigame()
    {
        isPlaying = true;

        minigameScript.gameObject.SetActive(true);

        currentMinigame.SetWinCallback(OnMinigameCompleted);
        currentMinigame.StartMinigame();
    }

    void ResumeMinigame()
    {
        minigameScript.gameObject.SetActive(true);
        currentMinigame.ResumeGame();
    }

    // ================= COMPLETE =================

    void OnMinigameCompleted()
    {
        if (isCompleted) return; // tránh gọi 2 lần

        Debug.Log("Minigame Completed!");

        isCompleted = true;
        isPlaying = false;

        minigameScript.gameObject.SetActive(false);

        // Trừ 1 phase boss
        if (bossHealth != null)
        {
            bossHealth.DamageOnePhase();
        }

        // Optional: khóa máy luôn sau khi hoàn thành
        GetComponent<Collider2D>().enabled = false;
    }

    // ================= TRIGGER =================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;

            // Chỉ pause nếu đang chơi và chưa hoàn thành
            if (isPlaying && !isCompleted)
            {
                currentMinigame.PauseGame();
                minigameScript.gameObject.SetActive(false);
            }
        }
    }
}