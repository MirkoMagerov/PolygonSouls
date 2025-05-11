using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<IEnemyStateMachine> allEnemies = new();
    private Dictionary<string, IEnemyStateMachine> enemyMap = new();
    private List<EnemySpawner> enemySpawners = new();
    private List<EnemyDeathData> pendingDeathData = new();
    private bool isInitialized = false;
    private bool isRestoringState = false;

    private void Awake()
    {
        StartCoroutine(InitializeWithDelay());
    }

    void Start()
    {
        StartCoroutine(EnsureRegistration());
    }

    private IEnumerator EnsureRegistration()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterEnemyManager(this);
            yield break;
        }
        yield return new WaitForSeconds(0.2f);
        // int attempts = 0;
        // while (attempts < 5)
        // {
        //     if (GameManager.Instance != null)
        //     {
        //         GameManager.Instance.RegisterEnemyManager(this);
        //         yield break;
        //     }
        //     attempts++;
        //     yield return new WaitForSeconds(0.2f);
        // }
    }

    private IEnumerator InitializeWithDelay()
    {
        yield return new WaitForEndOfFrame();

        RefreshEnemySpawners();

        if (pendingDeathData.Count > 0)
        {
            isRestoringState = true;
            ApplyDeathData(pendingDeathData);
            pendingDeathData.Clear();
            isRestoringState = false;
        }

        isInitialized = true;
    }

    private void RefreshEnemySpawners()
    {
        enemySpawners.Clear();
        var foundSpawners = FindObjectsOfType<EnemySpawner>();
        enemySpawners.AddRange(foundSpawners);

        if (foundSpawners.Length > 0)
        {
            List<string> allIDs = new();
            foreach (var spawner in foundSpawners)
            {
                allIDs.Add(spawner.GetEnemyID());
            }
        }
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

    public void ResetAllEnemies()
    {
        RefreshEnemySpawners();

        foreach (EnemySpawner spawner in enemySpawners)
        {
            spawner.RespawnEnemy();
        }

        pendingDeathData.Clear();
    }

    public void RestoreEnemyState(List<EnemyDeathData> deathDataList)
    {
        if (GameManager.Instance.isNewGame)
        {
            ResetAllEnemies();
            return;
        }

        if (!isInitialized && deathDataList != null && deathDataList.Count > 0)
        {
            pendingDeathData.Clear();
            pendingDeathData.AddRange(deathDataList);
            return;
        }

        if (enemySpawners.Count == 0)
        {
            RefreshEnemySpawners();
        }

        if (!isRestoringState)
        {
            foreach (EnemySpawner spawner in enemySpawners)
            {
                spawner.RespawnEnemy();
            }
        }

        if (deathDataList != null && deathDataList.Count > 0)
        {
            isRestoringState = true;
            ApplyDeathData(deathDataList);
            isRestoringState = false;
        }
    }

    private void ApplyDeathData(List<EnemyDeathData> deathDataList)
    {
        if (deathDataList == null || deathDataList.Count == 0)
        {
            return;
        }

        RefreshEnemySpawners();

        Dictionary<string, EnemySpawner> spawnerLookup = new();
        foreach (EnemySpawner spawner in enemySpawners)
        {
            string id = spawner.GetEnemyID();
            if (!spawnerLookup.ContainsKey(id))
            {
                spawnerLookup.Add(id, spawner);
            }
        }

        foreach (EnemyDeathData deathData in deathDataList)
        {
            if (spawnerLookup.TryGetValue(deathData.enemyID, out EnemySpawner spawner))
            {
                StartCoroutine(DelayedForceKill(
                        spawner,
                        deathData.deathPosition.ToVector3(),
                        deathData.deathRotation.ToQuaternion()
                    ));
            }
        }
    }

    private IEnumerator DelayedForceKill(EnemySpawner spawner, Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(0.2f);
        spawner.ForceKillWithPositionAndRotation(position, rotation);
    }
}