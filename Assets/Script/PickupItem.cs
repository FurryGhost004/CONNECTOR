using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item itemData;
    [HideInInspector] public bool isInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isInRange = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            foreach (var effect in itemData.effects)
            {
                effect.ApplyEffect(collision.gameObject, transform.position);
            }
        }

        Destroy(gameObject);
    }
}