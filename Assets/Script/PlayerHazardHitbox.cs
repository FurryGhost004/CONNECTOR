using UnityEngine;

// ✅ ĐẶT SCRIPT NÀY VÀO OBJECT CON CÓ BOX COLLIDER
public class PlayerHazardHitbox : MonoBehaviour
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
    private HazardTileController hazardController;
    private bool hasTriggered = false;

    void Start()
    {
        // ✅ LẤY HEALTHSYSTEM TỪ PARENT (Player chính)
        healthSystem = GetComponentInParent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("⚠️ Không tìm thấy HealthSystem trên Player parent!");
        }

        hazardController = FindObjectOfType<HazardTileController>();
        if (hazardController == null)
        {
            Debug.LogError("⚠️ Không tìm thấy HazardTileController trong scene!");
        }

        if (showDebugLogs)
        {
            Collider2D col = GetComponent<Collider2D>();
            Debug.Log($"✅ Hitbox Collider: {col?.GetType().Name}, IsTrigger={col?.isTrigger}");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (showDebugLogs)
            Debug.Log($"🔍 [HITBOX] Trigger: {collision.gameObject.name}, Tag={collision.tag}");

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
                Debug.Log("⚠️ HITBOX chạm vào HAZARD!");

            if (hazardController != null)
            {
                int damage = hazardController.GetDamageAmount();

                if (showDebugLogs)
                    Debug.Log($"💔 Mất {damage} máu!");

                if (healthSystem != null)
                {
                    // ✅ LẤY VỊ TRÍ TỪ PARENT (Player chính)
                    Vector2 attackSource = (Vector2)transform.parent.position + knockbackOffset;
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
                Debug.Log("✅ Hitbox rời khỏi hazard");
        }
    }
}