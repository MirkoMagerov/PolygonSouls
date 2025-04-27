using UnityEngine;
using System.Collections.Generic;

public class EnemyLootSystem : MonoBehaviour
{
    public GameObject lootEffectPrefab;
    public List<LootableItemSO> lootTable = new();

    public void SpawnLootEffect()
    {
        LootableItemSO lootableItem = DetermineLoot();
        if (lootableItem == null) return;

        GameObject effect = Instantiate(lootEffectPrefab, transform.position + new Vector3(0, .5f, 0), Quaternion.identity);

        int lootID = GetInstanceID();

        LootManager.Instance.RegisterLoot(lootID, lootableItem);

        if (!effect.TryGetComponent<LootVisual>(out var lootVisual)) lootVisual = effect.AddComponent<LootVisual>();
        lootVisual.lootID = lootID;
    }

    private LootableItemSO DetermineLoot()
    {
        if (lootTable.Count == 0) return null;

        float randomValue = Random.Range(0f, 100f);
        float cumulativeProbability = 0f;

        foreach (LootableItemSO item in lootTable)
        {
            cumulativeProbability += item.dropChance;
            if (randomValue <= cumulativeProbability) return item;
        }
        return null;
    }
}
