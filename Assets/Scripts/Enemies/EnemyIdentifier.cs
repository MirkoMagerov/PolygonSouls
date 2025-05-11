using UnityEngine;

public class EnemyIdentifier : MonoBehaviour
{
    [SerializeField] private string enemyID;

    public string GetID() => enemyID;
    public void SetID(string id) => enemyID = id;
}
