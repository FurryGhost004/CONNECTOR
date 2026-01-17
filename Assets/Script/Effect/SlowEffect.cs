using UnityEngine;

[CreateAssetMenu(fileName = "New Slow Effect", menuName = "ItemEffects/Slow")]
public class SlowEffect : ItemEffect
{
    public float slowMultiplier = 0.5f;

    public override void ApplyEffect(GameObject target, Vector3 sourcePosition)
    {
        var enemy = target.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.SlowDuration(duration, slowMultiplier);
        }
    }

}


