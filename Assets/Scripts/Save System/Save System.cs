using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "savegame.dat");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(List<string> deadEnemyIDs, List<EnemyDeathData> enemyDeathData, Transform playerTransform, int playerHealth = 100)
    {
        try
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            BinaryFormatter formatter = new();
            FileStream file = null;

            try
            {
                file = File.Create(savePath);

                GameData data = new GameData
                {
                    deadEnemyIDs = deadEnemyIDs,
                    enemyDeathData = enemyDeathData,
                    playerData = playerTransform != null
                    ? new PlayerData(playerTransform, playerHealth)
                    : null
                };

                formatter.Serialize(file, data);
                Debug.Log($"Juego guardado en: {savePath}");
            }
            finally
            {
                file?.Close();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al guardar el juego: {e.Message}");
        }
    }

    public GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = null;

            try
            {
                file = File.Open(savePath, FileMode.Open);
                GameData data = (GameData)formatter.Deserialize(file);
                Debug.Log($"Juego cargado desde: {savePath}");
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error al cargar el juego: {e.Message}");
                return new GameData();
            }
            finally
            {
                file?.Close();
            }
        }

        Debug.Log("No se encontró ningún archivo de guardado. Iniciando nuevo juego.");
        return new GameData();
    }

    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }
}