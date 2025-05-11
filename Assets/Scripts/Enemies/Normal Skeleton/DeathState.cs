using UnityEngine;

public class DeathState : EnemyBaseState
{
    public DeathState(NormalSkeletonStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.isStopped = true;
        stateMachine.Agent.enabled = false;
        stateMachine.Animator.SetTrigger("Dead");

        Collider[] colliders = stateMachine.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }
}