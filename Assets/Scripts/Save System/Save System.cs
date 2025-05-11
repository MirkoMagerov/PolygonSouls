using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [SerializeField] private string saveFileName = "savegame.dat";
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(List<string> deadEnemyIDs, List<EnemyDeathData> enemyDeathData, Transform playerTransform, int playerHealth = 100)
    {
        string activeBonfireID = null;
        if (BonfireManager.Instance != null)
        {
            activeBonfireID = BonfireManager.Instance.GetLastActiveBonfireID();
        }

        SaveGameWithBonfire(deadEnemyIDs, enemyDeathData, playerTransform, playerHealth, activeBonfireID);
    }

    public void SaveGameWithBonfire(List<string> deadEnemyIDs, List<EnemyDeathData> enemyDeathData, Transform playerTransform, int playerHealth, string bonfireID)
    {
        try
        {
            BinaryFormatter formatter = new();

            List<string> litBonfireIDs = new List<string>();
            if (BonfireManager.Instance != null)
            {
                litBonfireIDs = BonfireManager.Instance.GetLitBonfireIDs();
            }

            GameData data = new()
            {
                deadEnemyIDs = deadEnemyIDs ?? new List<string>(),
                enemyDeathData = enemyDeathData ?? new List<EnemyDeathData>(),
                playerData = new PlayerData(playerTransform, playerHealth),
                lastActiveBonfireID = bonfireID,
                litBonfireIDs = litBonfireIDs
            };

            using FileStream file = File.Create(savePath);
            formatter.Serialize(file, data);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al guardar el juego: {e.Message}\n{e.StackTrace}");
        }
    }

    public GameData LoadGame()
    {
        GameData data = new();

        if (!File.Exists(savePath))
        {
            return data;
        }

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream file = File.Open(savePath, FileMode.Open))
            {
                data = (GameData)formatter.Deserialize(file);
            }

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar el juego: {e.Message}\n{e.StackTrace}");
            return data;
        }
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }

    public void CreateInitialSaveFile()
    {
        if (File.Exists(savePath)) return;

        var emptyData = new GameData
        {
            deadEnemyIDs = new List<string>(),
            enemyDeathData = new List<EnemyDeathData>(),
            playerData = new PlayerData(),
            litBonfireIDs = new List<string>(),
            lastActiveBonfireID = null
        };

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using FileStream file = File.Create(savePath);
            formatter.Serialize(file, emptyData);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al crear archivo inicial: {e.Message}");
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }
}