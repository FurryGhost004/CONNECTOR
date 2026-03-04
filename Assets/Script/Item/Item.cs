using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    ThrowItem,
    SetUpItem,
    ConsumeableItem,
    KeyItem,
    QuestItem
}
[CreateAssetMenu(fileName = "New item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string name;

    public int level;
    public int price;
    public Sprite icon;
    public ItemType type;
    public GameObject prefab;
    public List<ItemEffect> effects;
    public List<ConsumableItemEffect> consumableEffects;
    public float destroyTime;
    public bool isAoE;
    [Header("Audio Settings")]
    public AudioClip useSound;      
    public AudioClip triggerSound;
    [Header("Animation Settings")]
    public string useAnimationTrigger;      
    public string triggerAnimationTrigger;  
    public float consumableDuration;
    [TextArea]
    public string description;

}

