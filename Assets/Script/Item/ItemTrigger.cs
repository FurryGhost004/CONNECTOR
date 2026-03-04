using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    public Item itemData;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {

        if (itemData.type == ItemType.ThrowItem)
        {
            foreach (var effect in itemData.effects)
            {
                effect.ApplyEffect(gameObject, transform.position);
            }


            Destroy(gameObject, itemData.destroyTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (itemData.isAoE)
            {
                Explode();
            }
            else
            {
                Activate(collision.gameObject);
                Destroy(gameObject);
            }

        }
    }

    void Activate ( GameObject target)
    {
        if (itemData.triggerSound != null)
        {
            AudioSource.PlayClipAtPoint(itemData.triggerSound, transform.position);
        }
        if (anim != null && !string.IsNullOrEmpty(itemData.triggerAnimationTrigger))
        {
            anim.SetTrigger(itemData.triggerAnimationTrigger);
        }
        foreach (var effect in itemData.effects)
        {
            effect.ApplyEffect(target, transform.position);
        }
    }

    void Explode()
    {
        if (itemData.triggerSound != null)
        {
            AudioSource.PlayClipAtPoint(itemData.triggerSound, transform.position);
        }
        if (anim != null && !string.IsNullOrEmpty(itemData.triggerAnimationTrigger))
        {
            anim.SetTrigger(itemData.triggerAnimationTrigger);
        }
        foreach (var effect in itemData.effects)
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, effect.radius);
            foreach (var target in targets)
            {
                effect.ApplyEffect(target.gameObject, transform.position);
            }

        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (itemData != null)
        {
            Gizmos.color = Color.red;
            foreach (var effect in itemData.effects)
            {
                Gizmos.DrawWireSphere(transform.position, effect.radius);
            }
        }
    }
}
