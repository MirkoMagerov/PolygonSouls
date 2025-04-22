using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public int damage = 15;
    public int staminaCost = 15;

    private NormalSkeletonStateMachine skeletonStateMachine;
    private bool hasHitPlayerThisAttack = false;
    private HashSet<Collider> hitColliders = new();

    void Start()
    {
        transform.root.TryGetComponent(out skeletonStateMachine);
    }

    void Update()
    {
        if (skeletonStateMachine != null && !skeletonStateMachine.GetAttackWindow())
        {
            hasHitPlayerThisAttack = false;
            hitColliders.Clear();
        }
    }

    public int GetDamage() => damage;
    public int GetStaminaCost() => staminaCost;

    void OnTriggerEnter(Collider other)
    {
        // Evitar colisiones con el propio enemigo
        if (other.transform.IsChildOf(transform.root) || other.transform == transform.root) return;

        // Comprobar si podemos atacar y no hemos impactado ya
        if (!skeletonStateMachine.GetAttackWindow() || hasHitPlayerThisAttack) return;

        // Evitar golpear al mismo collider múltiples veces en el mismo ataque
        if (hitColliders.Contains(other)) return;

        // Añadir collider a la lista de impactados
        hitColliders.Add(other);

        if (other.CompareTag("Player"))
        {
            ShieldBlock shield = other.GetComponentInChildren<ShieldBlock>();

            // Comprobar bloqueo con escudo
            if (shield != null && shield.IsBlockingActive() && shield.IsInBlockAngle(transform.position))
            {
                // Marcar ataque como procesado pero no como hit directo al jugador
                skeletonStateMachine.SetAttackWindowActive(0);
                Debug.Log("Ataque bloqueado por escudo");
                return;
            }

            // Aplicar daño al jugador
            if (other.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
                hasHitPlayerThisAttack = true;
                skeletonStateMachine.SetAttackWindowActive(0);
                Debug.Log("Daño directo al jugador: " + damage);
            }
        }
    }
}