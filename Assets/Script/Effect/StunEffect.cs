using UnityEngine;

[CreateAssetMenu(fileName = "New Stun Effect", menuName = "ItemEffects/Stun")]
public class StunEffect : ItemEffect
{
    public override void ApplyEffect(GameObject target, Vector3 sourcePosition)
    {
        var enemy = target.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.StunDuration(duration);
        }
    }
}