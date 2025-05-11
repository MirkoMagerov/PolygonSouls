using UnityEngine;

public class DogChaseState : DogEnemyState
{
    private float attackCooldown = 0f;
    private float positionUpdateInterval = 0.1f;
    private float lastPositionUpdateTime = 0f;
    private Vector3 lastPlayerPosition;
    private float lungeBoostTimer = 0f;
    private bool isLunging = false;

    public DogChaseState(DogStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.speed = stateMachine.ChaseSpeed;
        stateMachine.Animator.SetBool("IsRunning", true);

        if (stateMachine.Player != null)
        {
            lastPlayerPosition = stateMachine.Player.position;
            stateMachine.Agent.SetDestination(lastPlayerPosition);
        }

        isLunging = false;
    }

    public override void UpdateState()
    {
        if (stateMachine.Player == null)
        {
            stateMachine.LosePlayer();
            return;
        }

        // Actualizar cooldown de ataque
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        // Actualizar destino hacia el jugador más frecuentemente (más agresivo)
        if (Time.time > lastPositionUpdateTime + positionUpdateInterval)
        {
            lastPlayerPosition = stateMachine.Player.position;
            stateMachine.Agent.SetDestination(lastPlayerPosition);
            lastPositionUpdateTime = Time.time;
        }

        // Calcular distancia al jugador
        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, stateMachine.Player.position);

        // Si está fuera del radio de detección, volver a patrullar
        if (distanceToPlayer > stateMachine.DetectionRadius * 1.5f)
        {
            stateMachine.LosePlayer();
            return;
        }

        // Lunge mechanics (acometida rápida) - característica distintiva de los perros de DS3
        if (!isLunging && distanceToPlayer < stateMachine.AttackRange * 2.0f && distanceToPlayer > stateMachine.AttackRange * 1.2f)
        {
            // Probabilidad de iniciar una acometida
            if (Random.value < 0.05f)
            {
                isLunging = true;
                lungeBoostTimer = 0.6f; // Duración de la acometida
                stateMachine.Agent.speed = stateMachine.ChaseSpeed * 1.3f; // Boost de velocidad moderado
            }
        }

        // Actualizar el temporizador de lunge
        if (isLunging)
        {
            lungeBoostTimer -= Time.deltaTime;
            if (lungeBoostTimer <= 0)
            {
                isLunging = false;
                stateMachine.Agent.speed = stateMachine.ChaseSpeed;
            }
        }

        // Si está en rango para circling, cambiar a ese estado
        if (distanceToPlayer <= stateMachine.AttackRange * 1.5f && distanceToPlayer > stateMachine.AttackRange * 0.8f && !isLunging)
        {
            // 70% probabilidad de hacer circling antes de atacar (como en DS3)
            if (Random.value < 0.7f)
            {
                stateMachine.TransitionToCirclingState();
                return;
            }
        }

        // Si está en rango de ataque y no hay cooldown, atacar directamente
        if (distanceToPlayer <= stateMachine.AttackRange && attackCooldown <= 0)
        {
            // Cara a cara, más mordiscos que zarpazos
            if (Random.value > 0.35f)
            {
                stateMachine.PerformBiteAttack();
            }
            else
            {
                stateMachine.PerformClawAttack();
            }

            attackCooldown = stateMachine.AttackCooldown;
        }

        // Siempre mantener la vista en el jugador mientras persigue
        if (stateMachine.Player != null)
        {
            Vector3 lookDirection = stateMachine.Player.position - stateMachine.transform.position;
            lookDirection.y = 0;

            // No usar el NavMeshAgent para rotar, hacerlo directamente
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            stateMachine.transform.rotation = Quaternion.Slerp(
                stateMachine.transform.rotation,
                targetRotation,
                Time.deltaTime * 8f);
        }
    }

    public override void ExitState()
    {
        stateMachine.Animator.SetBool("IsRunning", false);
    }
}