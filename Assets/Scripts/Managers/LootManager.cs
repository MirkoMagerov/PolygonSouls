using UnityEngine;
using System.Collections.Generic;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;
    private Dictionary<int, LootableItemSO> activeLoots = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterLoot(int lootId, LootableItemSO item)
    {
        activeLoots[lootId] = item;
    }

    public LootableItemSO GetLoot(int id)
    {
        if (activeLoots.TryGetValue(id, out LootableItemSO loot))
        {
            activeLoots.Remove(id);
            switch(loot.itemType)
            {
                case ItemType.VitalEssence:
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().AddHealthPotion(loot.dropAmount);
                    break;
                case ItemType.VitalEssenceUpgrade:
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().LevelUpHealPotion();
                    break;
            }
            
            return loot;
        }
        return null;
    }
}