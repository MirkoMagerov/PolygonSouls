using UnityEngine;

public class PlayerEquipmentReference : MonoBehaviour
{
    [SerializeField] private GameObject skin;
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject healthPotion;

    public GameObject GetSkin() => skin;
    public GameObject GetWeapon() => weapon;
    public GameObject GetShield() => shield;
    public GameObject GetHealthPotion() => healthPotion;
}
