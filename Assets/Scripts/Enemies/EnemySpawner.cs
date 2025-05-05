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
    [SerializeField] private Transform[] patrolPoints;
    private string enemyID;

    private GameObject spawnedEnemy;
    private bool enemyIsDead = false;

    void Awake()
    {
        GenerateUniqueID();
    }

    private void Start()
    {
        SpawnEnemy();
    }

    private void GenerateUniqueID()
    {
        string posHash = transform.position.GetHashCode().ToString("X8");
        string nameHash = gameObject.name.GetHashCode().ToString("X8");
        enemyID = $"{nameHash}-{posHash}";
    }

    public void SpawnEnemy()
    {
        if (!enemyIsDead)
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
        if (patrolPoints.Length > 0)
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
