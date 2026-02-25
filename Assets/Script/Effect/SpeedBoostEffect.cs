using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoostEffect", menuName = "ItemEffects/ConsumableEffects/SpeedBoostEffect")]
public class SpeedBoostEffect : ConsumableItemEffect
{
    public float speedMultiplier;
    public override void ApplyEffect(GameObject target)
    {
        CharaterController movement = target.GetComponent<CharaterController>();
        if (movement != null)
        {
            movement.ApplySpeedBoost(speedMultiplier, duration);
        }
    }

}
