using UnityEngine;

public class HazardActivationTrigger : MonoBehaviour
{
    [Header("Hazard Controller")]
    [Tooltip("Kéo HazardController object vào đây")]
    public HazardTileController hazardController;

    [Header("Trigger Behavior")]
    [Tooltip("Bật hazard khi VÀO zone")]
    public bool activateOnEnter = true;

    [Tooltip("TẮT hazard khi RA zone")]
    public bool deactivateOnExit = true;

    [Tooltip("Chỉ trigger 1 lần duy nhất? (không tắt khi ra)")]
    public bool triggerOnce = false;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private bool hasTriggered = false;
    private bool isPlayerInside = false;

    void Start()
    {
        if (hazardController == null)
        {
            Debug.LogError("⚠️ Hazard Controller chưa gán! Kéo HazardController vào Inspector!");
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError("⚠️ Trigger Zone thiếu Collider2D! Thêm Box Collider 2D vào!");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning("⚠️ Collider2D phải là Trigger! Tick 'Is Trigger' vào!");
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Nếu đã trigger once và không cho trigger lại
            if (triggerOnce && hasTriggered)
            {
                if (showDebugLogs)
                    Debug.Log("⏭️ Đã trigger rồi, bỏ qua!");
                return;
            }

            isPlayerInside = true;

            if (activateOnEnter)
            {
                if (showDebugLogs)
                    Debug.Log("🚪 Player VÀO zone! BẬT HAZARDS!");

                if (hazardController != null)
                {
                    hazardController.ActivateHazards();
                    hasTriggered = true;
                }
                else
                {
                    Debug.LogError("⚠️ HazardController = null!");
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;

            // Nếu bật tắt khi exit VÀ không phải trigger once
            if (deactivateOnExit && !triggerOnce)
            {
                if (showDebugLogs)
                    Debug.Log("🚪 Player RA zone! TẮT HAZARDS!");

                if (hazardController != null)
                {
                    hazardController.DeactivateHazards();
                }
                else
                {
                    Debug.LogError("⚠️ HazardController = null!");
                }
            }
        }
    }

    // ✅ KIỂM TRA PLAYER CÓ TRONG ZONE KHÔNG
    public bool IsPlayerInside()
    {
        return isPlayerInside;
    }
}