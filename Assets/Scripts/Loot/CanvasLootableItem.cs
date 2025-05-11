using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLootableItem : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemQuantity;

    public void InitializeCanvas(LootableItemSO lootableItem)
    {
        itemIcon.sprite = lootableItem.itemIcon;

        itemName.text = lootableItem.itemName;

        itemQuantity.text = lootableItem.dropAmount.ToString();
    }
}