using UnityEngine;

public class AttackState : EnemyBaseState
{
    private float attackDuration = 1f;
    private float attackTimer = 0f;

    public AttackState(NormalSkeletonStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.isStopped = true;
        stateMachine.Agent.velocity = Vector3.zero;

        stateMachine.Animator.SetBool("IsRunning", false);
        stateMachine.Animator.SetTrigger("Attack");

        Vector3 directionToPlayer = stateMachine.Player.position - stateMachine.transform.position;
        directionToPlayer.y = 0;
        stateMachine.transform.rotation = Quaternion.LookRotation(directionToPlayer);

        attackTimer = 0f;
    }

    public override void UpdateState()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackDuration)
        {
            stateMachine.FinishAttack();
        }
    }

    public override void ExitState()
    {

    }
}