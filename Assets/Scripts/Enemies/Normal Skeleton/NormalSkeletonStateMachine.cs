using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBaseState
{
    protected NormalSkeletonStateMachine stateMachine;

    public EnemyBaseState(NormalSkeletonStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}

public class NormalSkeletonStateMachine : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    [SerializeField] private Transform[] patrolPoints;

    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float detectionAngle = 60f;
    [SerializeField] private float closeAttackRange = 2f;
    [SerializeField] private float chargeAttackRange = 8f;

    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float chargeCooldown = 4f;
    private bool attackWindow;
    private bool hitRegisteredThisAttack;

    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chargeSpeed = 12f;

    [SerializeField] private EnemyBaseState currentState;
    public PatrolState PatrolState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public AttackState AttackState { get; private set; }
    public ChargeAttackState ChargeAttackState { get; private set; }
    public HitState HitState { get; private set; }

    public event Action OnPlayerDetected;
    public event Action OnPlayerLost;
    public event Action<float> OnCloseRangeAttack;
    public event Action<float> OnChargeAttack;
    public event Action OnAttackFinished;
    public event Action OnHit;
    public event Action OnHitFinished;

    // Getters para componentes y parámetros
    public NavMeshAgent Agent => agent;
    public Animator Animator => animator;
    public Transform Player => player;
    public Transform[] PatrolPoints => patrolPoints;
    public float DetectionRadius => detectionRadius;
    public float DetectionAngle => detectionAngle;
    public float CloseAttackRange => closeAttackRange;
    public float ChargeAttackRange => chargeAttackRange;
    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float ChargeSpeed => chargeSpeed;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

        // Crear estados
        PatrolState = new PatrolState(this);
        ChaseState = new ChaseState(this);
        AttackState = new AttackState(this);
        ChargeAttackState = new ChargeAttackState(this);
        HitState = new HitState(this);
    }

    private void Start()
    {
        SubscribeToEvents();

        ChangeState(PatrolState);
    }

    private void SubscribeToEvents()
    {
        OnPlayerDetected += () => ChangeState(ChaseState);
        OnCloseRangeAttack += (cooldown) => ChangeState(AttackState);
        OnChargeAttack += (cooldown) => ChangeState(ChargeAttackState);
        OnAttackFinished += () => ChangeState(ChaseState);
        OnHit += () => ChangeState(HitState);
        OnHitFinished += () => ChangeState(ChaseState);
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    public void ChangeState(EnemyBaseState newState)
    {
        currentState?.ExitState();

        currentState = newState;

        currentState.EnterState();
    }

    // Métodos para invocar eventos
    public void DetectPlayer() => OnPlayerDetected?.Invoke();
    public void LosePlayer() => OnPlayerLost?.Invoke();
    public void PerformCloseAttack() => OnCloseRangeAttack?.Invoke(attackCooldown);
    public void PerformChargeAttack() => OnChargeAttack?.Invoke(chargeCooldown);
    public void FinishAttack() => OnAttackFinished?.Invoke();
    public void TakeHit() => OnHit?.Invoke();
    public void FinishHit() => OnHitFinished?.Invoke();

    public bool GetAttackWindow() { return attackWindow; }
    public bool GetHitRegisteredThisAttack() { return hitRegisteredThisAttack; }

    public void SetAttackWindowActive(int isActive)
    {
        attackWindow = isActive == 1;
        if (isActive == 1)
        {
            hitRegisteredThisAttack = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Vector3 rightEdge = Quaternion.Euler(0, detectionAngle / 2f, 0) * transform.forward;
        Vector3 leftEdge = Quaternion.Euler(0, -detectionAngle / 2f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightEdge * detectionRadius);
        Gizmos.DrawRay(transform.position, leftEdge * detectionRadius);
        Gizmos.DrawRay(transform.position, transform.forward * detectionRadius);
    }
}