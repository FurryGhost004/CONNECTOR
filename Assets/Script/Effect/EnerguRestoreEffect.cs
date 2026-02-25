using UnityEngine;

[CreateAssetMenu(fileName = "EnerguRestoreEffect", menuName = "ItemEffects/ConsumableEffects/EnergyRestoreEffect")]
public class EnerguRestoreEffect : ConsumableItemEffect
{
    public float energyAmount;
    public override void ApplyEffect(GameObject target)
    {
        HealthSystem energy = target.GetComponent< HealthSystem> ();
        if (energy != null)
        {
            energy.RestoreEnergy(energyAmount); 
        }
    }

}
