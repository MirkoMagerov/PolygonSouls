using System.Collections.Generic;
using UnityEngine;

public class BonfireManager : MonoBehaviour
{
    public static BonfireManager Instance { get; private set; }

    [SerializeField] private List<string> debugLitBonfireIDs = new();
    [SerializeField] private string lastActiveBonfireID;

    private Dictionary<string, Transform> litBonfires = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsBonfireLit(string bonfireID)
    {
        return !string.IsNullOrEmpty(bonfireID) && litBonfires.ContainsKey(bonfireID);
    }

    public void LightBonfire(string bonfireID, Transform bonfireTransform)
    {
        if (string.IsNullOrEmpty(bonfireID) || bonfireTransform == null)
        {
            return;
        }

        if (litBonfires.ContainsKey(bonfireID))
        {
            litBonfires[bonfireID] = bonfireTransform;
        }
        else
        {
            litBonfires.Add(bonfireID, bonfireTransform);
            if (!debugLitBonfireIDs.Contains(bonfireID))
            {
                debugLitBonfireIDs.Add(bonfireID);
            }
        }

        lastActiveBonfireID = bonfireID;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetLastBonfirePosition(bonfireTransform);
            GameManager.Instance.SetLastBonfireID(bonfireID);
        }
        SavePermanentProgress();
    }

    public Transform GetBonfireTransform(string bonfireID)
    {
        if (string.IsNullOrEmpty(bonfireID))
        {
            return null;
        }

        if (litBonfires.TryGetValue(bonfireID, out Transform bonfireTransform))
        {
            if (bonfireTransform == null)
            {
                bonfireTransform = FindBonfireTransformByID(bonfireID);
                if (bonfireTransform != null)
                {
                    litBonfires[bonfireID] = bonfireTransform;
                }
            }
            return bonfireTransform;
        }
        return null;
    }

    private Transform FindBonfireTransformByID(string bonfireID)
    {
        Bonfire[] bonfires = FindObjectsOfType<Bonfire>();
        foreach (Bonfire bonfire in bonfires)
        {
            if (bonfire != null && bonfire.GetBonfireID() == bonfireID)
            {
                return bonfire.transform;
            }
        }
        return null;
    }

    public string GetLastActiveBonfireID()
    {
        return lastActiveBonfireID;
    }

    public void SetLastActiveBonfireID(string bonfireID)
    {
        if (!string.IsNullOrEmpty(bonfireID))
        {
            lastActiveBonfireID = bonfireID;
        }
    }

    public void RecoverLitBonfires()
    {
        Bonfire[] bonfires = FindObjectsOfType<Bonfire>();

        foreach (string id in new List<string>(litBonfires.Keys))
        {
            foreach (Bonfire bonfire in bonfires)
            {
                if (bonfire.GetBonfireID() == id)
                {
                    bonfire.ForceLight();
                    litBonfires[id] = bonfire.transform;

                    if (id == lastActiveBonfireID)
                    {
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.SetLastBonfirePosition(bonfire.transform);
                            GameManager.Instance.SetLastBonfireID(id);
                        }
                    }
                    break;
                }
            }
        }
    }

    public void InitializeLitBonfires(List<string> bonfireIDs)
    {
        litBonfires.Clear();
        debugLitBonfireIDs.Clear();

        if (bonfireIDs == null || bonfireIDs.Count == 0)
        {
            return;
        }

        foreach (string id in bonfireIDs)
        {
            if (!string.IsNullOrEmpty(id))
            {
                litBonfires[id] = null;
                debugLitBonfireIDs.Add(id);
            }
        }
    }

    private void SavePermanentProgress()
    {

        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        int playerHealth = 100;
        if (player.TryGetComponent<PlayerHealth>(out var healthComponent))
        {
            playerHealth = healthComponent.GetCurrentHealth();
        }

        SaveSystem.Instance.SaveGameWithBonfire(
            enemyManager.GetDeadEnemyIDs(),
            enemyManager.GetEnemyDeathData(),
            player.transform,
            playerHealth,
            lastActiveBonfireID
        );
    }

    public List<string> GetLitBonfireIDs()
    {
        debugLitBonfireIDs = new List<string>(litBonfires.Keys);
        return debugLitBonfireIDs;
    }
}