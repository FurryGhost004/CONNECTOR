using UnityEngine;

public class SettingBtn : MonoBehaviour
{
    [Header("References")]
    public SettingManager settingManager;

    // === NÚT CLOSE ===
    public void ClosePanel()
    {
        if (settingManager != null)
        {
            settingManager.ClosePanel();
            Debug.Log("❌ Đóng Setting Panel");
        }
    }
}
