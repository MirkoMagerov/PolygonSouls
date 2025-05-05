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

    public void RestoreEnemyState(List<string> deadEnemyIDs)
    {
        foreach (EnemySpawner spawner in enemySpawners) spawner.RespawnEnemy();

        foreach (EnemySpawner spawner in enemySpawners)
        {
            if (deadEnemyIDs.Contains(spawner.GetEnemyID())) spawner.ForceKill();
        }
    }
}
