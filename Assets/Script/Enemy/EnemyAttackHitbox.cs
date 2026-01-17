using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    public int damage = 10;
    private Collider2D hitbox;
    private bool hasDealtDamage = false; // Biến cờ để ngăn chặn việc gây sát thương nhiều lần trong 1 đòn đánh

    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        if (hitbox != null)
        {
            hitbox.enabled = false;
            hitbox.isTrigger = true; // Đảm bảo luôn là Trigger
        }
    }

    public void EnableHitbox()
    {
        hasDealtDamage = false; // Reset cờ mỗi khi đòn đánh mới bắt đầu
        if (hitbox != null) hitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        if (hitbox != null) hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu đã đánh trúng rồi thì không xử lý nữa cho đến đòn tiếp theo
        if (hasDealtDamage) return;

        if (other.CompareTag("Player"))
        {
            // Thực hiện trừ máu
            bool hitSuccess = false;

            if (HealthSystem.Instance != null)
            {
                HealthSystem.Instance.TakeDamage(damage);
                hitSuccess = true;
            }
            else
            {
                HealthSystem hp = other.GetComponent<HealthSystem>();
                if (hp != null)
                {
                    hp.TakeDamage(damage);
                    hitSuccess = true;
                }
            }

            if (hitSuccess)
            {
                hasDealtDamage = true; // Đánh dấu đã gây sát thương xong
                Debug.Log($"<color=red>Player hit!</color> Damage: {damage}");

                // Tắt hitbox ngay lập tức để an toàn
                DisableHitbox();
            }
        }
    }
}