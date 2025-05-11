using UnityEngine;

public class HitState : EnemyBaseState
{
    private float hitDuration = 1f;
    private float hitTimer = 0f;

    public HitState(NormalSkeletonStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.SetAttackWindowActive(0);
        stateMachine.Agent.isStopped = true;
        stateMachine.Agent.velocity = Vector3.zero;

        stateMachine.Animator.SetBool("IsRunning", false);
        stateMachine.Animator.SetTrigger("Hit");

        hitTimer = 0f;
    }

    public override void UpdateState()
    {
        hitTimer += Time.deltaTime;

        if (hitTimer >= hitDuration)
        {
            stateMachine.FinishHit();
        }
    }

    public override void ExitState() { }
}