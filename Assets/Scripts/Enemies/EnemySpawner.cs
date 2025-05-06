using System;
using UnityEngine;
using UnityEngine.AI;

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
    private GameObject spawnedEnemy;
    private Vector3 deathPosition;
    private Quaternion deathRotation;
    private string enemyID;
    private bool enemyIsDead = false;

    void Awake()
    {
        if (string.IsNullOrEmpty(enemyID))
        {
            GenerateUniqueID();
        }
    }

    private void Start()
    {
        SpawnEnemy();
    }

    public bool IsEnemyDead() => enemyIsDead;
    public string GetEnemyID() => enemyID;

    private void GenerateUniqueID()
    {
        string posStr = $"{transform.position.x:F1}_{transform.position.y:F1}_{transform.position.z:F1}";
        enemyID = $"Enemy-{gameObject.name}-{posStr}";
        Debug.Log($"Generated ID: {enemyID}");
    }

    public void SpawnEnemy()
    {
        if (!enemyIsDead)
        {
            if (enemyIsDead)
            {
                Debug.Log($"Not spawning enemy with ID {enemyID} because it's marked as dead");
                return;
            }

            Debug.Log($"Spawning enemy with ID: {enemyID}");
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

    private void HandleEnemyDeath()
    {
        if (spawnedEnemy != null)
        {
            deathPosition = spawnedEnemy.transform.position;
            deathRotation = spawnedEnemy.transform.rotation;
        }

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

    public void ForceKillWithPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        if (spawnedEnemy != null)
        {
            Destroy(spawnedEnemy);
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

        EnableDeathVisuals(spawnedEnemy);
        DisableEnemyComponents(spawnedEnemy);

        deathPosition = position;
        deathRotation = rotation;

        Debug.Log($"Enemy {enemyID} respawned as dead at position {position}");
    }

    private void DisableEnemyComponents(GameObject enemy)
    {
        enemy.TryGetComponent<CharacterController>(out var characterController);
        characterController.enabled = false;

        enemy.TryGetComponent<NavMeshAgent>(out var navMeshAgent);
        navMeshAgent.enabled = false;

        MonoBehaviour[] scripts = enemy.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.enabled = false;
        }
    }

    private void EnableDeathVisuals(GameObject enemy)
    {
        if (enemy.TryGetComponent<Animator>(out var animator))
        {
            animator.enabled = true;
            animator.Play("Death");
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
