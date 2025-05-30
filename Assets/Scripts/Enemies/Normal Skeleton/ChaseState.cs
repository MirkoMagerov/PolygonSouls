using UnityEngine;

public class ChaseState : EnemyBaseState
{
    private float lastAttackTime = 0f;
    private float lastChargeTime = 0f;

    public ChaseState(NormalSkeletonStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.speed = stateMachine.ChaseSpeed;
        stateMachine.Agent.isStopped = false;
        stateMachine.Animator.SetBool("IsRunning", true);
    }

    public override void UpdateState()
    {
        if (stateMachine.Player == null) return;

        stateMachine.Agent.SetDestination(stateMachine.Player.position);

        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, stateMachine.Player.position);

        if (distanceToPlayer <= stateMachine.ChargeAttackRange && distanceToPlayer > stateMachine.CloseAttackRange &&
            Time.time > lastChargeTime)
        {
            lastChargeTime = Time.time + stateMachine.ChargeAttackRange;
            stateMachine.PerformChargeAttack();
        }
        else if (distanceToPlayer <= stateMachine.CloseAttackRange && Time.time > lastAttackTime)
        {
            lastAttackTime = Time.time + stateMachine.CloseAttackRange;
            stateMachine.PerformCloseAttack();
        }
    }

    public override void ExitState()
    {

    }
}