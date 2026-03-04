using UnityEngine;

public enum ConsumableItemEffectType
{
    Heal,
    SpeedBoost,
    EnergyRestore,


}

[CreateAssetMenu(fileName = "ConsumableItemEffect", menuName = "Inventory/ConsumableItemEffect")]
public abstract class ConsumableItemEffect : ScriptableObject
{
    public string effectName;
    public float duration;
    public ConsumableItemEffectType consumableItemEffectType;

    public abstract void ApplyEffect(GameObject target);
}
