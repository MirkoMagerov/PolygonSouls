using UnityEngine;

public class DogCirclingState : DogEnemyState
{
    private float circlingRadius = 3.5f; // Radio para orbitar al jugador
    private float circlingSpeed;
    private bool clockwise;
    private Vector3 targetPosition;
    private float nextPositionUpdateTime = 0f;
    private float repositionInterval = 0.3f;
    private float attackReadinessTimer = 0f;
    private float minCircleTimeBeforeAttack = 1f;
    private float maxCircleTimeBeforeAttack = 1.5f;
    private float targetCircleTime;

    // Dirección de circling se establece al entrar al estado y se mantiene constante
    private bool directionEstablished = false;

    public DogCirclingState(DogStateMachine stateMachine) : base(stateMachine)
    {
        ResetCirclingBehavior();
    }

    private void ResetCirclingBehavior()
    {
        // La dirección solo se establece al entrar al estado
        if (!directionEstablished)
        {
            clockwise = Random.value > 0.5f;
            directionEstablished = true;
        }

        circlingSpeed = Random.Range(2.2f, 3.2f);
        targetCircleTime = Random.Range(minCircleTimeBeforeAttack, maxCircleTimeBeforeAttack);
        attackReadinessTimer = 0f;
    }

    public override void EnterState()
    {
        stateMachine.Agent.speed = stateMachine.ChaseSpeed * 0.8f;
        stateMachine.Animator.SetBool("IsRunning", true);

        // Desactivar NavMeshAgent para control directo del movimiento
        stateMachine.Agent.isStopped = true;

        // Resetear la bandera de dirección establecida al entrar al estado
        directionEstablished = false;
        ResetCirclingBehavior();
    }

    public override void UpdateState()
    {
        if (stateMachine.Player == null)
        {
            stateMachine.LosePlayer();
            return;
        }

        attackReadinessTimer += Time.deltaTime;

        // Obtener dirección al jugador
        Vector3 dirToPlayer = stateMachine.Player.position - stateMachine.transform.position;
        dirToPlayer.y = 0;
        float distanceToPlayer = dirToPlayer.magnitude;

        // Verificar si el perro está demasiado lejos o cerca
        if (distanceToPlayer > stateMachine.DetectionRadius * 1.5f)
        {
            stateMachine.SwitchToChaseState();
            return;
        }
        else if (distanceToPlayer < stateMachine.AttackRange * 0.7f)
        {
            // Demasiado cerca - retrocede ligeramente antes de atacar
            Vector3 backwardDirection = -dirToPlayer.normalized;
            stateMachine.transform.position += backwardDirection * Time.deltaTime * stateMachine.ChaseSpeed * 0.5f;
        }

        // Actualizar posición de circling
        if (Time.time >= nextPositionUpdateTime)
        {
            // Calcular posición orbital alrededor del jugador
            Vector3 right = Vector3.Cross(Vector3.up, dirToPlayer.normalized);
            float targetRadius = Mathf.Clamp(distanceToPlayer, 2.0f, circlingRadius);

            if (clockwise)
                targetPosition = stateMachine.Player.position + right * targetRadius;
            else
                targetPosition = stateMachine.Player.position - right * targetRadius;

            nextPositionUpdateTime = Time.time + repositionInterval;
        }

        // Mover hacia la posición calculada
        Vector3 moveDirection = (targetPosition - stateMachine.transform.position).normalized;
        stateMachine.transform.position += moveDirection * Time.deltaTime * circlingSpeed;

        // Mantener mirada fija en el jugador - característica clave de los perros DS3
        Vector3 lookDirection = stateMachine.Player.position - stateMachine.transform.position;
        lookDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        stateMachine.transform.rotation = Quaternion.Slerp(
            stateMachine.transform.rotation,
            targetRotation,
            Time.deltaTime * 10f);

        // Decidir atacar después de cierto tiempo
        if (attackReadinessTimer >= targetCircleTime)
        {
            // Si estamos a la distancia correcta, atacar
            if (distanceToPlayer <= stateMachine.AttackRange * 1.1f)
            {
                if (Random.value > 0.6f)
                    stateMachine.PerformBiteAttack();
                else
                    stateMachine.PerformClawAttack();
                return;
            }
            // Si estamos fuera de rango, volver a persecución
            else if (distanceToPlayer > stateMachine.AttackRange * 1.2f)
            {
                stateMachine.SwitchToChaseState();
                return;
            }
        }

        // Eliminar el cambio aleatorio de dirección durante el circling
        // Ya no tenemos: if (Random.value < 0.01f) { clockwise = !clockwise; }
    }

    public override void ExitState()
    {
        stateMachine.Agent.isStopped = false;
        // Resetear la bandera para que la próxima vez elija una nueva dirección
        directionEstablished = false;
    }
}