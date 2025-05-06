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

        // Mirar hacia el jugador
        if (stateMachine.Player != null)
        {
            Vector3 direction = stateMachine.Player.position - stateMachine.transform.position;
            direction.y = 0;
            stateMachine.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;

        if (timer >= attackDuration)
        {
            stateMachine.FinishAttack();
        }
    }

    public override void ExitState()
    {
        stateMachine.Agent.isStopped = false;
    }
}