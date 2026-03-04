using UnityEngine;
public enum EffectType
{
    Stun,
    Taunt,
    Knockback,
    Slow
}
public abstract class ItemEffect : ScriptableObject
{
    public string effectName;
    public float duration;
    public float radius;
    public EffectType effectType;

    public abstract void ApplyEffect(GameObject target, Vector3 sourcePosition);
}

