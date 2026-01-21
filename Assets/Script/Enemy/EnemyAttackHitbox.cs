using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    public int damage = 10;
    private Collider2D hitbox;
    private bool hasDealtDamage = false;

    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        if (hitbox != null)
        {
            hitbox.enabled = false;
            hitbox.isTrigger = true;
        }
    }

    public void EnableHitbox()
    {
        hasDealtDamage = false;
        if (hitbox != null) hitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        if (hitbox != null) hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDealtDamage) return;

        if (other.CompareTag("Player"))
        {
            bool hitSuccess = false;

            Vector2 attackSource = transform.position; // 🔥 VỊ TRÍ ENEMY

            if (HealthSystem.Instance != null)
            {
                HealthSystem.Instance.TakeDamage(damage, attackSource);
                hitSuccess = true;
            }
            else
            {
                HealthSystem hp = other.GetComponent<HealthSystem>();
                if (hp != null)
                {
                    hp.TakeDamage(damage, attackSource);
                    hitSuccess = true;
                }
            }

            if (hitSuccess)
            {
                hasDealtDamage = true;
                Debug.Log($"<color=red>Player hit!</color> Damage: {damage}");

                DisableHitbox();
            }
        }
    }
}
