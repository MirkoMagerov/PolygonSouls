using UnityEngine;

public class DogHitState : DogEnemyState
{
    private float hitStunDuration = 1f;
    private float timer = 0f;

    public DogHitState(DogStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.isStopped = true;
        stateMachine.Animator.SetTrigger("Hit");
        timer = 0f;
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;

        if (timer >= hitStunDuration)
        {
            stateMachine.FinishHit();
        }
    }

    public override void ExitState()
    {
        stateMachine.Agent.isStopped = false;
    }
}