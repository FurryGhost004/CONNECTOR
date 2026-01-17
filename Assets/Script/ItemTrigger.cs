using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    public Item itemData;
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
            foreach (var effect in itemData.effects)
            {
                effect.ApplyEffect(collision.gameObject, transform.position);
            }

            Destroy(gameObject);
        }
    }
}
