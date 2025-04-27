using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLootableItem : MonoBehaviour
{
    public LootableItemSO lootableItemSO;
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemQuantity;

    public void InitializeCanvas(LootableItemSO lootableItem)
    {
        lootableItemSO = lootableItem;

        itemIcon.sprite = lootableItemSO.itemIcon;

        itemName.text = lootableItemSO.itemName;

        itemQuantity.text = lootableItem.dropAmount.ToString();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}