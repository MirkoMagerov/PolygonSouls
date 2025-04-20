using UnityEngine;

public class HitState : EnemyBaseState
{
    private float hitDuration = 0.8f;
    private float hitTimer = 0f;

    public HitState(NormalSkeletonStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        // Detener movimiento
        stateMachine.Agent.isStopped = true;
        stateMachine.Agent.velocity = Vector3.zero;

        // Activar animación
        stateMachine.Animator.SetBool("IsRunning", false);
        stateMachine.Animator.SetTrigger("Hit");

        // Reiniciar timer
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