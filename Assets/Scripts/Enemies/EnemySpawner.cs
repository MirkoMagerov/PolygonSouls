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
    [SerializeField] private GameObject spawnedEnemy;
    private Vector3 deathPosition;
    private Quaternion deathRotation;
    private string enemyID;
    private bool enemyIsDead = false;
    private bool isBeingDestroyed = false;

    private void OnDestroy()
    {
        isBeingDestroyed = true;
        CleanupEnemy();
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(enemyID))
        {
            GenerateUniqueID();
        }
        SpawnEnemy();
    }

    public bool IsEnemyDead() => enemyIsDead;
    public string GetEnemyID() => enemyID;

    private void GenerateUniqueID()
    {
        string posStr = $"{transform.position.x:F1}_{transform.position.y:F1}_{transform.position.z:F1}";
        enemyID = $"Enemy-{gameObject.name}-{posStr}";
    }

    public void SpawnEnemy()
    {
        if (isBeingDestroyed)
        {
            return;
        }

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
        if (enemy == null) return;

        IPatrolPointUser patrolUser = enemy.GetComponent<IPatrolPointUser>();
        if (patrolUser != null && patrolPoints != null && patrolPoints.Length > 0)
        {
            patrolUser.SetPatrolPoints(patrolPoints);
        }
    }

    private void CleanupEnemy()
    {
        if (spawnedEnemy.TryGetComponent<IEnemyDeathNotifier>(out var deathNotifier))
        {
            deathNotifier.OnDeath -= HandleEnemyDeath;
        }
        Destroy(spawnedEnemy);
        spawnedEnemy = null;
    }

    public void RespawnEnemy()
    {
        if (isBeingDestroyed) return;

        CleanupEnemy();

        enemyIsDead = false;
        SpawnEnemy();
    }

    private void HandleEnemyDeath()
    {
        if (isBeingDestroyed) return;

        if (spawnedEnemy != null)
        {
            deathPosition = spawnedEnemy.transform.position;
            deathRotation = spawnedEnemy.transform.rotation;

            if (spawnedEnemy.TryGetComponent<IEnemyDeathNotifier>(out var deathNotifier))
            {
                deathNotifier.OnDeath -= HandleEnemyDeath;
            }
        }

        enemyIsDead = true;
    }

    public void ForceKillWithPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        if (isBeingDestroyed) return;
        CleanupEnemy();

        if (spawnedEnemy != null)
        {
            Destroy(spawnedEnemy);
            spawnedEnemy = null;
        }

        enemyIsDead = true;

        spawnedEnemy = Instantiate(enemyPrefab, position, rotation);

        if (spawnedEnemy.TryGetComponent<EnemyIdentifier>(out var identifier))
        {
            identifier.SetID(enemyID);
        }
        else
        {
            identifier = spawnedEnemy.AddComponent<EnemyIdentifier>();
            identifier.SetID(enemyID);
        }

        deathPosition = position;
        deathRotation = rotation;

        DisableEnemyComponents(spawnedEnemy);
    }

    private void DisableEnemyComponents(GameObject enemy)
    {
        if (enemy == null) return;

        if (enemy.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            enemyHealth.InstaKill();
        }

        MonoBehaviour[] scripts = enemy.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }
    }

    public Vector3 GetDeathPosition()
    {
        return deathPosition;
    }

    public Quaternion GetDeathRotation()
    {
        return deathRotation;
    }
}