using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    public ItemEffect itemEffect;
    public float radius = 3f;
    public float duration = 2f;

    private float timer;
    private bool hasApplied = false;
    public static bool isProcessingAOE = false;
    void Start()
    {
        timer = duration;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (!hasApplied)
        {
            ApplyEffectInArea();
            hasApplied = true;
            Destroy(gameObject, duration+0.5f);
        }
    }

    private void ApplyEffectInArea()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                itemEffect.ApplyEffect(hit.gameObject, transform.position);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
