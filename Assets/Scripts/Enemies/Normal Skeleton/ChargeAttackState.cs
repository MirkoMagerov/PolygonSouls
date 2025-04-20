using UnityEngine;

public class ChargeAttackState : EnemyBaseState
{
    private float attackDuration = 1.5f;
    private float stateTimer = 0f;

    public ChargeAttackState(NormalSkeletonStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.isStopped = true;
        stateMachine.Agent.velocity = Vector3.zero;

        if (stateMachine.Player != null)
        {
            Vector3 directionToPlayer = stateMachine.Player.position - stateMachine.transform.position;
            directionToPlayer.y = 0;
            stateMachine.transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }

        stateMachine.Animator.SetTrigger("ChargeAttack");
    }

    public override void UpdateState()
    {
        stateTimer += Time.deltaTime;

        if (stateTimer >= attackDuration)
        {
            stateMachine.FinishAttack();
        }
    }

    public override void ExitState()
    {
        stateMachine.Agent.nextPosition = stateMachine.transform.position;
    }
}