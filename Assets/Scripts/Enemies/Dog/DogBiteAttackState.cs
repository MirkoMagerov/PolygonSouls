using UnityEngine;

public class DogBiteAttackState : DogEnemyState
{
    private float attackDuration = 1.2f;
    private float timer = 0f;

    public DogBiteAttackState(DogStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.isStopped = true;
        stateMachine.Animator.SetTrigger("Bite");
        timer = 0f;

        FacePlayer();
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;

        if (timer < 0.5f)
        {
            FacePlayer();
        }

        if (timer >= attackDuration)
        {
            stateMachine.TransitionToCirclingState();
        }
    }

    private void FacePlayer()
    {
        if (stateMachine.Player != null)
        {
            Vector3 direction = stateMachine.Player.position - stateMachine.transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            stateMachine.transform.rotation = Quaternion.Lerp(
                stateMachine.transform.rotation,
                targetRotation,
                Time.deltaTime * 10f);
        }
    }

    public override void ExitState()
    {
        stateMachine.Agent.isStopped = false;
    }
}