using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================
// VICTORY MANAGER
// ============================================
public class VictoryManager : MonoBehaviour
{
    [Header("Victory Panel")]
    public GameObject victoryPanel;


    void Start()
    {
        // Ẩn panel lúc bắt đầu
        victoryPanel.SetActive(false);
    }

    // Gọi hàm này khi thắng
    public void ShowVictory()
    {
        victoryPanel.SetActive(true);
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