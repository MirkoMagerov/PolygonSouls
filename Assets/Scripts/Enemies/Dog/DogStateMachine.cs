using System;
using UnityEngine;
using UnityEngine.AI;

public class DogStateMachine : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;
    private DogEnemyState currentState;

    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float detectionAngle = 60f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    // Estados
    public DogPatrolState PatrolState { get; private set; }
    public DogChaseState ChaseState { get; private set; }
    public DogBiteAttackState BiteAttackState { get; private set; }
    public DogClawAttackState ClawAttackState { get; private set; }
    public DogHitState HitState { get; private set; }
    public DogDeathState DeathState { get; private set; }

    // Eventos
    public event Action OnDeath;
    public event Action OnPlayerDetected;
    public event Action OnPlayerLost;
    public event Action OnAttackFinished;
    public event Action OnHit;
    public event Action OnHitFinished;

    // Propiedades
    public NavMeshAgent Agent => agent;
    public Animator Animator => animator;
    public Transform Player => player;
    public Transform[] PatrolPoints => patrolPoints;
    public float DetectionRadius => detectionRadius;
    public float DetectionAngle => detectionAngle;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        agent.stoppingDistance = attackRange;

        // Inicializar estados
        PatrolState = new DogPatrolState(this);
        ChaseState = new DogChaseState(this);
        BiteAttackState = new DogBiteAttackState(this);
        ClawAttackState = new DogClawAttackState(this);
        HitState = new DogHitState(this);
        DeathState = new DogDeathState(this);
    }

    private void Start()
    {
        // Configurar eventos
        OnPlayerDetected += () => ChangeState(ChaseState);
        OnPlayerLost += () => ChangeState(PatrolState);
        OnAttackFinished += () => ChangeState(ChaseState);
        OnHit += () => ChangeState(HitState);
        OnHitFinished += () => ChangeState(ChaseState);

        // Estado inicial
        ChangeState(PatrolState);
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    public void ChangeState(DogEnemyState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    // Métodos para invocar eventos
    public void DetectPlayer() => OnPlayerDetected?.Invoke();
    public void LosePlayer() => OnPlayerLost?.Invoke();
    public void PerformBiteAttack() => ChangeState(BiteAttackState);
    public void PerformClawAttack() => ChangeState(ClawAttackState);
    public void FinishAttack() => OnAttackFinished?.Invoke();
    public void TakeHit(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            ChangeState(DeathState);
        }
        else
        {
            OnHit?.Invoke();
        }
    }
    public void FinishHit() => OnHitFinished?.Invoke();
    public void NotifyDeath() => OnDeath?.Invoke();

    // Método para habilitar ventana de daño (llamado desde animaciones)
    public void SetAttackWindowActive(bool isActive)
    {
        // Aquí se implementaría la lógica para activar/desactivar el collider de ataque
        if (isActive)
        {
            // Activar collider de ataque
        }
        else
        {
            // Desactivar collider de ataque
        }
    }

    // Visualización en el editor
    private void OnDrawGizmosSelected()
    {
        // Radio de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Ángulo de visión
        Gizmos.color = Color.red;
        Vector3 rightEdge = Quaternion.Euler(0, detectionAngle / 2f, 0) * transform.forward;
        Vector3 leftEdge = Quaternion.Euler(0, -detectionAngle / 2f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightEdge * detectionRadius);
        Gizmos.DrawRay(transform.position, leftEdge * detectionRadius);
        Gizmos.DrawRay(transform.position, transform.forward * detectionRadius);

        // Radio de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}