using System;
using UnityEngine;

public interface IEnemyDeathNotifier
{
    event Action OnDeath;
}

public interface IPatrolPointUser
{
    void SetPatrolPoints(Transform[] points);
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private string enemyID;
    [SerializeField] private Transform[] patrolPoints;

    private GameObject spawnedEnemy;
    private bool enemyIsDead = false;

    private void Start()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        if (spawnedEnemy == null && !enemyIsDead)
        {
            spawnedEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);

            if (!spawnedEnemy.TryGetComponent<EnemyIdentifier>(out var identifier))
            {
                identifier = spawnedEnemy.AddComponent<EnemyIdentifier>();
            }
            identifier.SetID(enemyID);

            ConfigureEnemyData(spawnedEnemy);

            if (spawnedEnemy.TryGetComponent<IEnemyDeathNotifier>(out var deathNotifier))
            {
                deathNotifier.OnDeath += HandleEnemyDeath;
            }
        }
    }

    private void ConfigureEnemyData(GameObject enemy)
    {
        IPatrolPointUser patrolUser = enemy.GetComponent<IPatrolPointUser>();
        if (patrolUser != null && patrolPoints.Length > 0)
        {
            patrolUser.SetPatrolPoints(patrolPoints);
        }
    }

    private void HandleEnemyDeath()
    {
        enemyIsDead = true;

        if (spawnedEnemy != null)
        {
            if (spawnedEnemy.TryGetComponent<IEnemyDeathNotifier>(out var deathNotifier))
            {
                deathNotifier.OnDeath -= HandleEnemyDeath;
            }
        }

        spawnedEnemy = null;
    }

    public void RespawnEnemy()
    {
        if (spawnedEnemy != null)
        {
            if (spawnedEnemy.TryGetComponent<IEnemyDeathNotifier>(out var deathNotifier))
            {
                deathNotifier.OnDeath -= HandleEnemyDeath;
            }

            Destroy(spawnedEnemy);
            spawnedEnemy = null;
        }

        enemyIsDead = false;
        SpawnEnemy();
    }

    public void ForceKill()
    {
        if (spawnedEnemy != null)
        {
            if (spawnedEnemy.TryGetComponent<IEnemyDeathNotifier>(out var deathNotifier))
            {
                deathNotifier.OnDeath -= HandleEnemyDeath;
            }

            Destroy(spawnedEnemy);
        }

        spawnedEnemy = null;
        enemyIsDead = true;
    }

    public bool IsEnemyDead() => enemyIsDead;
    public string GetEnemyID() => enemyID;
}
