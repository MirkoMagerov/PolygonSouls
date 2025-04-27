using UnityEngine;

public enum ItemType { Weapon, Armor, Consumable, Key, Material }

[CreateAssetMenu(fileName = "New Item", menuName = "Loot/Item")]
public class LootableItemSO : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;

    [TextArea(2, 4)]
    public string description;
    public int value;
    public bool isStackable;
    public int maxStack = 1;
    public int dropAmount = 1;

    [Header("Equipment Stats")]
    public bool isEquippable;
    public float damage;
    public float defense;

    [Header("Consumable Stats")]
    public float effectAmount;
    public float effectDuration;

    [Header("Loot Settings")]
    public bool isRare;
    public float dropChance = 5f;

    public virtual void OnLoot(GameObject user)
    {
        Debug.Log($"Looted {itemName}");
    }
}