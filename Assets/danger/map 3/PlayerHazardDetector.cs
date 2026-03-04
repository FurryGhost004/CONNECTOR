using UnityEngine;

public class PlayerHazardDetector : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag của Hazard Tilemap")]
    public string hazardTag = "Hazard";

    [Header("Knockback")]
    [Tooltip("Vị trí knockback (dưới player một chút)")]
    public Vector2 knockbackOffset = new Vector2(0, -1f);

    [Header("Debug")]
    public bool showDebugLogs = true;

    private HealthSystem healthSystem;
    private HazardTileController hazardController; // ✅ Cache lại
    private bool hasTriggered = false;

    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("⚠️ Không tìm thấy HealthSystem trên Player!");
        }

        // ✅ TÌM HazardTileController 1 LẦN DUY NHẤT
        hazardController = FindObjectOfType<HazardTileController>();
        if (hazardController == null)
        {
            Debug.LogError("⚠️ Không tìm thấy HazardTileController trong scene!");
        }

        if (showDebugLogs)
        {
            Collider2D col = GetComponent<Collider2D>();
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Debug.Log($"✅ Player Collider: {col?.GetType().Name}, IsTrigger={col?.isTrigger}");
            Debug.Log($"✅ Player Rigidbody: {rb?.bodyType}");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (showDebugLogs)
            Debug.Log($"🔍 Trigger: {collision.gameObject.name}, Tag={collision.tag}");

        if (collision.CompareTag(hazardTag))
        {
            if (hasTriggered)
            {
                if (showDebugLogs)
                    Debug.Log("⏭️ Đã trigger rồi, bỏ qua!");
                return;
            }

            hasTriggered = true;

            if (showDebugLogs)
                Debug.Log("⚠️ Player chạm vào HAZARD!");

            if (hazardController != null)
            {
                int damage = hazardController.GetDamageAmount();

                if (showDebugLogs)
                    Debug.Log($"💔 Mất {damage} máu!");

                if (healthSystem != null)
                {
                    Vector2 attackSource = (Vector2)transform.position + knockbackOffset;
                    healthSystem.TakeDamage(damage, attackSource);
                }
            }
            else
            {
                Debug.LogError("⚠️ HazardController = null!");
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(hazardTag))
        {
            hasTriggered = false;
            if (showDebugLogs)
                Debug.Log("✅ Rời khỏi hazard");
        }
    }
}