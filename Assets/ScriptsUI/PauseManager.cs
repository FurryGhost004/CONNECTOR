using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    void Start()
    {
        // Ẩn panel lúc bắt đầu
        pausePanel.SetActive(false);
    }

    void Update()
    {
        // Nhấn ESC để pause/resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Dừng thời gian game
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Chạy lại game
        isPaused = false;
    }

    public void RestartModel()
    {
        Time.timeScale = 1f; // Nhớ reset timeScale
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackWaittingRoom()
    {
        Time.timeScale = 1f; // Nhớ reset timeScale
        SceneManager.LoadScene("MainMenu");
    }
}