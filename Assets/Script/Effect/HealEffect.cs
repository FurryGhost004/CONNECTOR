using UnityEngine;

[CreateAssetMenu(fileName = "NewHealEffect", menuName = "ItemEffects/ConsumableEffects/HealEffect")]
public class HealEffect: ConsumableItemEffect
{
    public float healAmount;
    
    public override void ApplyEffect(GameObject target)
    {
        HealthSystem health = target.GetComponent< HealthSystem> ();
        if (health != null)
        {
            health.StartRecoverHealth(healAmount, duration); 
        }
        Debug.Log($"Applied HealEffect to {target.name}, healing {healAmount} over {duration} seconds.");
    }

    

}
