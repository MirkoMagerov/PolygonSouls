using UnityEngine;

public class DogChaseState : DogEnemyState
{
    private float attackCooldown = 0f;

    public DogChaseState(DogStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.speed = stateMachine.ChaseSpeed;
        stateMachine.Animator.SetBool("IsRunning", true);
    }

    public override void UpdateState()
    {
        if (stateMachine.Player == null)
        {
            stateMachine.LosePlayer();
            return;
        }

        // Actualizar destino hacia el jugador
        stateMachine.Agent.SetDestination(stateMachine.Player.position);

        // Reducir cooldown de ataque
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        // Calcular distancia al jugador
        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, stateMachine.Player.position);

        // Si está fuera del radio de detección, volver a patrullar
        if (distanceToPlayer > stateMachine.DetectionRadius * 1.5f)
        {
            stateMachine.LosePlayer();
            return;
        }

        // Si está en rango de ataque y no hay cooldown, atacar
        if (distanceToPlayer <= stateMachine.AttackRange && attackCooldown <= 0)
        {
            // Alternar entre mordisco y zarpazo
            if (UnityEngine.Random.value > 0.5f)
            {
                stateMachine.PerformBiteAttack();
            }
            else
            {
                stateMachine.PerformClawAttack();
            }

            attackCooldown = stateMachine.AttackCooldown;
        }
    }

    public override void ExitState()
    {
        stateMachine.Animator.SetBool("IsRunning", false);
    }
}