using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBtn : MonoBehaviour
{
    [Header("References")]
    public GameObject settingsPanel; // Kéo Panel Setting vào đây

    void Start()
    {
        // Đảm bảo panel đóng khi vào MainMenu
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OnClickPlay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("WaittingRoom");
    }

    // ⭐ SỬA LẠI: MỞ PANEL THAY VÌ CHUYỂN SCENE
    public void OnClickSetting()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);

            // Nếu có SettingManager, gọi LoadSettings
            SettingManager settingManager = settingsPanel.GetComponent<SettingManager>();
            if (settingManager != null)
                settingManager.LoadSettings();
        }
        else
        {
            Debug.LogError("❌ Settings Panel chưa được gán!");
        }
    }
    public void OnClickStory()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TutorialScene");
    }

    public void OnClickQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}