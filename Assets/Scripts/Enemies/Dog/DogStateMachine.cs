using UnityEngine;
using UnityEngine.AI;

public class DogStateMachine : MonoBehaviour
{
    // Referencias
    public NavMeshAgent Agent { get; private set; }
    public Animator Animator { get; private set; }
    public Transform Player { get; private set; }
    public Transform[] PatrolPoints;

    // Parámetros de comportamiento
    public float PatrolSpeed = 2.0f;
    public float ChaseSpeed = 3.5f;
    public float AttackRange = 2.0f;
    public float AttackCooldown = 2.0f;
    public float DetectionRadius = 10.0f;
    public float DetectionAngle = 120.0f;

    // Estados
    private DogEnemyState currentState;
    private DogPatrolState patrolState;
    private DogChaseState chaseState;
    private DogCirclingState circlingState;
    private DogBiteAttackState biteAttackState;
    private DogClawAttackState clawAttackState;
    private DogHitState hitState;
    private DogDeathState deathState;

    // Componentes de ataque
    public int AttackDamage = 20;
    public LayerMask PlayerLayer;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        // Inicializar estados
        patrolState = new DogPatrolState(this);
        chaseState = new DogChaseState(this);
        circlingState = new DogCirclingState(this);
        biteAttackState = new DogBiteAttackState(this);
        clawAttackState = new DogClawAttackState(this);
        hitState = new DogHitState(this);
        deathState = new DogDeathState(this);
    }

    private void Start()
    {
        // Buscar al jugador
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        // Iniciar en estado de patrulla
        ChangeState(patrolState);
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }

    public void ChangeState(DogEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.EnterState();
        }
    }

    // Métodos de transición
    public void DetectPlayer()
    {
        ChangeState(chaseState);
    }

    public void LosePlayer()
    {
        ChangeState(patrolState);
    }

    public void PerformBiteAttack()
    {
        ChangeState(biteAttackState);
    }

    public void PerformClawAttack()
    {
        ChangeState(clawAttackState);
    }

    public void FinishAttack()
    {
        // En lugar de volver directamente a Chase, usar circling a veces (comportamiento DS3)
        TransitionToCirclingState();
    }

    public void TransitionToCirclingState()
    {
        ChangeState(circlingState);
    }

    public void SwitchToChaseState()
    {
        ChangeState(chaseState);
    }

    public void FinishHit()
    {
        // Después de recibir un golpe, perseguir agresivamente
        if (Player != null)
        {
            ChangeState(chaseState);
        }
        else
        {
            ChangeState(patrolState);
        }
    }

    public void Die()
    {
        ChangeState(deathState);

        // Desactivar componentes después de un tiempo
        Invoke("DisableEnemy", 5.0f);
    }

    private void DisableEnemy()
    {
        Agent.enabled = false;
        enabled = false;
    }

    public void NotifyDeath()
    {
        // Evento de muerte que puede ser usado por otros sistemas
    }

    public void DealDamage()
    {
        if (Player == null) return;

        // Calcular daño basado en distancia y ángulo
        Vector3 directionToPlayer = Player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= AttackRange * 1.2f)
        {
            // Comprobar si estamos mirando hacia el jugador
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer.normalized);

            if (angleToPlayer <= 45f)
            {
                // Enviar mensaje de daño al jugador
                Player.GetComponent<PlayerHealth>()?.TakeDamage(AttackDamage);

                // Efecto de sonido de mordisco/zarpazo
                TryGetComponent<AudioSource>(out var audioSource);
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
        }
    }
}