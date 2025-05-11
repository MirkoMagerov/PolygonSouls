using UnityEngine;

public enum ItemType { VitalEssence, VitalEssenceUpgrade }

[CreateAssetMenu(fileName = "New Item", menuName = "Loot/Item")]
public class LootableItemSO : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public int dropAmount;
    public float dropChance;
}