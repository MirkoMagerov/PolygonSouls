using UnityEngine;

public class DogDeathState : DogEnemyState
{
    public DogDeathState(DogStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.isStopped = true;
        stateMachine.Animator.SetTrigger("Death");

        Collider[] colliders = stateMachine.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        stateMachine.NotifyDeath();
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }
}