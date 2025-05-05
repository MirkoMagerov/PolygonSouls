using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<string> deadEnemyIDs = new List<string>();
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    // Puedes añadir más datos aquí
}

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

    public void SaveGame(List<string> deadEnemyIDs, Transform playerTransform)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(savePath);

        GameData data = new GameData
        {
            deadEnemyIDs = deadEnemyIDs,
            playerPosition = playerTransform.position,
            playerRotation = playerTransform.rotation
        };

        formatter.Serialize(file, data);
        file.Close();
        Debug.Log($"Juego guardado en: {savePath}");
    }

    public GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);

            GameData data = (GameData)formatter.Deserialize(file);
            file.Close();
            Debug.Log($"Juego cargado desde: {savePath}");
            return data;
        }

        Debug.Log("No se encontró ningún archivo de guardado. Iniciando nuevo juego.");
        return new GameData();
    }

    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }
}