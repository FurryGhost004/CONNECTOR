using UnityEngine;
using UnityEngine.SceneManagement;
// ============================================
// FAILED MANAGER
// ============================================
public class FailedManager : MonoBehaviour
{
    [Header("Failed Panel")]
    public GameObject failedPanel;

    void Start()
    {
        // Ẩn panel lúc bắt đầu
        failedPanel.SetActive(false);
    }

    // Gọi hàm này khi thua
    public void ShowFailed()
    {
        failedPanel.SetActive(true);
        Time.timeScale = 0f; // Dừng game

    }

    // === CÁC HÀM CHO BUTTONS ===

    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickWaittingRoom()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("WaittingRoom");
    }
}