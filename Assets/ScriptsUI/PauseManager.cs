using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class PauseManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingPanel;

    private bool isPaused = false;

    void Awake()
    {
        // Đảm bảo game chạy bình thường khi vào scene
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingPanel != null)
            settingPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("ESC WORKS");

            if (settingPanel != null && settingPanel.activeSelf)
            {
                BackToPause();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }


    // ================= PAUSE / RESUME =================

    public void PauseGame()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(true);
        settingPanel?.SetActive(false);

        Time.timeScale = 0f;
        AudioListener.pause = true;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel?.SetActive(false);
        settingPanel?.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;
        isPaused = false;
    }

    // ================= SETTINGS =================

    // Gọi từ nút "Settings"
    public void OpenSettings()
    {
        if (pausePanel == null || settingPanel == null) return;

        pausePanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    // Gọi từ nút "Back" trong Settings
    public void BackToPause()
    {
        pausePanel?.SetActive(true);
        settingPanel?.SetActive(false);
    }

    // ================= OTHER BUTTONS =================

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackWaitingRoom()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("WaittingRoom");
    }
    public void BackMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }
}
