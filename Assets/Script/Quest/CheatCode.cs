using UnityEngine;

public class CheatCode : MonoBehaviour
{
    void Update()
    {
        // Nhấn Left Shift + B để mở khóa màn Boss
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.B))
        {
            UnlockBossLevel();
        }
    }

    void UnlockBossLevel()
    {
        // 1. Cập nhật chỉ số cao nhất
        PlayerPrefs.SetInt("HighestMissionIndex", 15);

        // 2. Mở khóa luôn các mốc level khác để tránh lỗi logic
        PlayerPrefs.SetInt("CurrentMaxLevel", 3);

        PlayerPrefs.Save();

        
        if (NPCManager.Instance != null)
        {
            NPCManager.Instance.RefreshNPCs();
        }


        Debug.Log("Hệ thống đã cập nhật tiến trình!");
    }
}
