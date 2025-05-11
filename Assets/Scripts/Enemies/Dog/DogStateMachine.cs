using System;
using UnityEngine;
using UnityEngine.AI;

public class DogStateMachine : MonoBehaviour, IEnemyStateMachine, IPatrolPointUser, IEnemyDeathNotifier
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

    public int AttackDamage = 10;

    // Estados
    private DogEnemyState currentState;
    private DogPatrolState patrolState;
    private DogChaseState chaseState;
    private DogCirclingState circlingState;
    private DogBiteAttackState biteAttackState;
    private DogClawAttackState clawAttackState;
    private DogHitState hitState;
    private DogDeathState deathState;
    private DogAttackManager attackManager;

    public event Action OnDeath;

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
        attackManager = GetComponent<DogAttackManager>();
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        ChangeState(patrolState);
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    public void SetPatrolPoints(Transform[] points)
    {
        PatrolPoints = points;
    }

    public void ChangeState(DogEnemyState newState)
    {
        currentState?.ExitState();

        currentState = newState;

        currentState?.EnterState();

        if (!(currentState is DogBiteAttackState || currentState is DogClawAttackState))
        {
            if (attackManager != null) attackManager.InterruptAttack();
        }
    }

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
        if (Player != null)
        {
            ChangeState(chaseState);
        }
        else
        {
            ChangeState(patrolState);
        }
    }

    public void NotifyDeath()
    {
        OnDeath?.Invoke();
        if (attackManager != null) attackManager.InterruptAttack();
        ChangeState(deathState);
    }

    public void DealDamage()
    {
        if (Player.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.TakeDamage(AttackDamage);

            TryGetComponent<AudioSource>(out var audioSource);
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);

        Gizmos.color = Color.red;
        Vector3 rightEdge = Quaternion.Euler(0, DetectionAngle / 2f, 0) * transform.forward;
        Vector3 leftEdge = Quaternion.Euler(0, -DetectionAngle / 2f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightEdge * DetectionRadius);
        Gizmos.DrawRay(transform.position, leftEdge * DetectionRadius);
        Gizmos.DrawRay(transform.position, transform.forward * DetectionRadius);
    }
}