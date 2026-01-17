using UnityEngine;

public class SettingBtn : MonoBehaviour
{
    [Header("References")]
    public SettingManager settingManager;

    // === NÚT APPLY ===
    public void Apply()
    {
        if (settingManager != null)
        {
            settingManager.Apply();
            Debug.Log("✅ Settings đã được lưu!");
        }
    }

    // === NÚT CLOSE ===
    public void ClosePanel()
    {
        if (settingManager != null)
        {
            settingManager.ClosePanel();
        }
    }
}
