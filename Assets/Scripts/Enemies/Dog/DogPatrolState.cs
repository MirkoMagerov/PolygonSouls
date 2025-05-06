using UnityEngine;

public class DogPatrolState : DogEnemyState
{
    private int currentPointIndex = 0;

    public DogPatrolState(DogStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.speed = stateMachine.PatrolSpeed;
        stateMachine.Animator.SetBool("IsRunning", false);

        // Ir al primer punto de patrulla si hay puntos disponibles
        if (stateMachine.PatrolPoints.Length > 0)
        {
            stateMachine.Agent.SetDestination(stateMachine.PatrolPoints[currentPointIndex].position);
        }
    }

    public override void UpdateState()
    {
        // Verificar si el jugador está dentro del rango de detección
        if (CanSeePlayer())
        {
            stateMachine.DetectPlayer();
            return;
        }

        // Lógica de patrulla - ir al siguiente punto sin espera
        if (stateMachine.Agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (stateMachine.PatrolPoints.Length == 0)
            return;

        currentPointIndex = (currentPointIndex + 1) % stateMachine.PatrolPoints.Length;
        stateMachine.Agent.SetDestination(stateMachine.PatrolPoints[currentPointIndex].position);
        stateMachine.Animator.SetBool("IsWalking", true);
    }

    private bool CanSeePlayer()
    {
        if (stateMachine.Player == null)
            return false;

        Vector3 directionToPlayer = stateMachine.Player.position - stateMachine.transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Comprobar si el jugador está dentro del radio de detección
        if (distanceToPlayer <= stateMachine.DetectionRadius)
        {
            // Comprobar si el jugador está dentro del ángulo de visión
            float angleToPlayer = Vector3.Angle(stateMachine.transform.forward, directionToPlayer);
            if (angleToPlayer <= stateMachine.DetectionAngle / 2f)
            {
                // Realizar un raycast para comprobar si hay obstáculos entre el enemigo y el jugador
                if (Physics.Raycast(stateMachine.transform.position + Vector3.up, directionToPlayer.normalized, out RaycastHit hit, distanceToPlayer))
                {
                    if (hit.transform == stateMachine.Player)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override void ExitState()
    {
    }
}