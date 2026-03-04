using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    public float damage = 15f;
    public float damageInterval = 0.2f;

    private float timer;
    private Vector2 startPoint;
    private Vector2 direction;
    private float length;

    public void Setup(Vector2 start, Vector2 dir, float laserLength)
    {
        startPoint = start;
        direction = dir;
        length = laserLength;
    }

        void Update()
        {
            timer += Time.deltaTime;

            if (timer >= damageInterval)
            {
                timer = 0f;

                RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, direction, length);

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        HealthSystem health = hit.collider.GetComponent<HealthSystem>();
                        if (health != null)
                        {
                            health.TakeDamage(damage, startPoint);
                        }
                    }
                }
            }
        }
    
}