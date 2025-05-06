using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public int damage = 15;
    public int staminaCost = 15;

    private NormalSkeletonStateMachine skeletonStateMachine;

    void Start()
    {
        skeletonStateMachine = GetComponentInParent<NormalSkeletonStateMachine>();
    }

    public int GetDamage() => damage;
    public int GetStaminaCost() => staminaCost;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(transform.root) || other.transform == transform.root) return;
        if (!other.CompareTag("Player") || !skeletonStateMachine.GetAttackWindow()) return;
        skeletonStateMachine.SetAttackWindowActive(0);
        other.TryGetComponent(out ShieldBlock shieldBlock);

        if (shieldBlock.IsBlockingActive() && shieldBlock.IsInBlockAngle(transform.root.position))
        {
            shieldBlock.ApplyReducedDamage(damage, staminaCost);
            return;
        }

        other.TryGetComponent(out PlayerHealth playerHealth);
        playerHealth.TakeDamage(damage);
        other.TryGetComponent(out PlayerAttack playerAttack);
        playerAttack.SetAttackWindowActive(0);
    }
}