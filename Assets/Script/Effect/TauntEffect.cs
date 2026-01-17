using UnityEngine;

[CreateAssetMenu(fileName = "New Taunt Effect", menuName = "ItemEffects/Taunt")]

public class TauntEffect : ItemEffect
{
    public float Radius;
    public override void ApplyEffect(GameObject source, Vector3 position)
    {

        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {

                ITauntable tauntable = hit.GetComponent<ITauntable>();

                if (tauntable != null)
                {
                    tauntable.ApplyTaunt(duration, position);
                }
            }

            Debug.Log("Taunt");
        }
    }
}
