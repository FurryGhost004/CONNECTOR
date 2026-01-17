using UnityEngine;

public class ItemLanding : MonoBehaviour
{
    public Item itemData;
    public float stopThreshold = 0.1f;

    Rigidbody2D rb;
    bool hasSpawnedAOE = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!hasSpawnedAOE && rb.linearVelocity.magnitude < stopThreshold)
        {
            SpawnAOE();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasSpawnedAOE)
            SpawnAOE();
    }
    void SpawnAOE()
    {
        hasSpawnedAOE = true;
        rb.linearVelocity = Vector2.zero;

        foreach (var effect in itemData.effects)
        {
            effect.ApplyEffect(gameObject, transform.position);
        }
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnItemThrown();
        }

    }
    private void OnDrawGizmos()
    {
        if (itemData != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var effect in itemData.effects)
            {
                Gizmos.DrawWireSphere(transform.position, effect.radius);
            }
        }
    }
}
