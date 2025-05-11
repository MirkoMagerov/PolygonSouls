using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<string> deadEnemyIDs = new List<string>();
    public List<EnemyDeathData> enemyDeathData = new List<EnemyDeathData>();
    public PlayerData playerData;
    public string lastActiveBonfireID;
    public List<string> litBonfireIDs = new();
}

[System.Serializable]
public class PlayerData
{
    public SerializationHelper.SerializableVector3 position;
    public SerializationHelper.SerializableQuaternion rotation;
    public int health;

    public PlayerData()
    {
        position = new SerializationHelper.SerializableVector3(Vector3.zero);
        rotation = new SerializationHelper.SerializableQuaternion(Quaternion.identity);
        health = 100;
    }

    public PlayerData(Transform transform, int currentHealth)
    {
        position = new SerializationHelper.SerializableVector3(transform.position);
        rotation = new SerializationHelper.SerializableQuaternion(transform.rotation);
        health = currentHealth;
    }
}

[System.Serializable]
public class EnemyDeathData
{
    public string enemyID;
    public SerializationHelper.SerializableVector3 deathPosition;
    public SerializationHelper.SerializableQuaternion deathRotation;

    public EnemyDeathData(string id, Vector3 position, Quaternion rotation)
    {
        enemyID = id;
        deathPosition = new SerializationHelper.SerializableVector3(position);
        deathRotation = new SerializationHelper.SerializableQuaternion(rotation);
    }
}