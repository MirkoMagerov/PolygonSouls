using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<EnemySpawner> enemySpawners = new();

    private void Awake()
    {
        enemySpawners.AddRange(FindObjectsOfType<EnemySpawner>());

        if (GameManager.Instance != null) GameManager.Instance.RegisterEnemyManager(this);
    }

    public void RespawnAllEnemies()
    {
        foreach (EnemySpawner spawner in enemySpawners) { spawner.RespawnEnemy(); }
    }

    public List<string> GetDeadEnemyIDs()
    {
        List<string> deadEnemies = new();

        foreach (EnemySpawner spawner in enemySpawners)
        {
            if (spawner.IsEnemyDead())
                deadEnemies.Add(spawner.GetEnemyID());
        }

        return deadEnemies;
    }

    public List<EnemyDeathData> GetEnemyDeathData()
    {
        List<EnemyDeathData> deathDataList = new();

        foreach (EnemySpawner spawner in enemySpawners)
        {
            if (spawner.IsEnemyDead())
            {
                string id = spawner.GetEnemyID();
                Vector3 position = spawner.GetDeathPosition();
                Quaternion rotation = spawner.GetDeathRotation();
                deathDataList.Add(new EnemyDeathData(id, position, rotation));
            }
        }

        return deathDataList;
    }

    public void RestoreEnemyState(List<string> deadEnemyIDs, List<EnemyDeathData> deathDataList)
    {
        Debug.Log($"Restoring enemy state, dead enemies: {deadEnemyIDs?.Count ?? 0}, death data: {deathDataList?.Count ?? 0}");

        foreach (EnemySpawner spawner in enemySpawners)
        {
            spawner.RespawnEnemy();
        }

        if (deathDataList != null && deathDataList.Count > 0)
        {
            foreach (EnemyDeathData deathData in deathDataList)
            {
                Debug.Log($"Processing death data for enemy ID: {deathData.enemyID}");
                EnemySpawner spawner = enemySpawners.Find(s => s.GetEnemyID() == deathData.enemyID);

                if (spawner != null)
                {
                    Debug.Log($"Found spawner for enemy ID: {deathData.enemyID}, applying death state");
                    spawner.ForceKillWithPositionAndRotation(
                        deathData.deathPosition.ToVector3(),
                        deathData.deathRotation.ToQuaternion()
                    );
                }
                else
                {
                    Debug.LogWarning($"No spawner found for enemy ID: {deathData.enemyID}");
                }
            }
        }
        else
        {
            Debug.Log("No death data available to restore");
        }
    }
}
