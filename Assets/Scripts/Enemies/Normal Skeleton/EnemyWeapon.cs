using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public int damage = 15;
    public int staminaCost = 15;

    private NormalSkeletonStateMachine skeletonStateMachine;

    void Start()
    {
        transform.root.TryGetComponent(out skeletonStateMachine);
    }

    public int GetDamage() => damage;
    public int GetStaminaCost() => staminaCost;

    // void OnCollisionEnter(Collision collision)
    // {
    //     if (!skeletonStateMachine.GetAttackWindow() || skeletonStateMachine.GetHitRegisteredThisAttack()) return;

    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         ShieldBlock shield = collision.gameObject.GetComponentInChildren<ShieldBlock>();
    //         if (shield.IsBlockingActive() && shield.IsInBlockAngle(transform.position))
    //         {
    //             skeletonStateMachine.SetAttackWindowActive(0);
    //             return;
    //         }

    //         if (collision.gameObject.TryGetComponent(out PlayerHealth playerHealth))
    //         {
    //             Debug.Log("Daño directo al jugador: " + damage);
    //             playerHealth.TakeDamage(damage);
    //             skeletonStateMachine.SetAttackWindowActive(0);
    //         }
    //     }
    // }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(transform.root) || other.transform == transform.root) return;
        if (!skeletonStateMachine.GetAttackWindow() || skeletonStateMachine.GetHitRegisteredThisAttack()) return;

        if (other.CompareTag("Player"))
        {
            ShieldBlock shield = other.GetComponentInChildren<ShieldBlock>();
            if (shield.IsBlockingActive() && shield.IsInBlockAngle(transform.position))
            {
                return;
            }

            if (other.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
                skeletonStateMachine.SetAttackWindowActive(0);
                Debug.Log("Daño directo al jugador: " + damage);
            }
        }
    }
}