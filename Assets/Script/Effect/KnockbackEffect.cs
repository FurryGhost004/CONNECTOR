using UnityEngine;

[CreateAssetMenu(fileName = "New Knockback Effect", menuName = "ItemEffects/Knockback")]
public class KnockbackEffect : ItemEffect
{
    public float force = 5f;

    public override void ApplyEffect(GameObject target, Vector3 sourcePosition)
    {
        var enemy = target.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            Vector2 direction = target.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition).normalized;

        }
    }
}
