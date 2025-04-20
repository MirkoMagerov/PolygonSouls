using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public int damage = 25;
    public int staminaCost = 15;
    private PlayerAttack playerAttack;

    // Start is called before the first frame update
    void Start()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(transform.root) || other.transform == transform.root) return;

        if (other.CompareTag("Enemy") && playerAttack.isAttacking && playerAttack.GetAttackWindow())
        {
            other.TryGetComponent(out EnemyHealth enemyHealth);
            enemyHealth.TakeDamage(damage);
            playerAttack.SetAttackWindowActive(0);
        }
    }
}
