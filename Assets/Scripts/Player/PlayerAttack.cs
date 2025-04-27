using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private const float COMBO_WINDOW = 1.2f;
    public bool isAttacking = false;
    private int currentAttackIndex = 0;
    private bool attackWindow;
    private float lastAttackTime;

    [Header("Stamina Costs")]
    public int firstAttackCost = 6;
    public int secondAttackCost = 9;
    public int thirdAttackCost = 14;

    private Animator animator;
    private StarterAssetsInputs inputActions;
    private PlayerStamina playerStamina;

    void Start()
    {
        animator = GetComponent<Animator>();
        inputActions = GetComponent<StarterAssetsInputs>();
        playerStamina = GetComponent<PlayerStamina>();
        AutomatizeAnimationEvents();
    }

    void Update()
    {
        if (inputActions.primaryAttack)
        {
            Attack();
            inputActions.primaryAttack = false;
        }

        if (Time.time - lastAttackTime > COMBO_WINDOW && !isAttacking)
        {
            currentAttackIndex = 0;
        }
    }

    private void Attack()
    {
        if (!PlayerStateManager.Instance.GetCurrentState().CanAttack) return;

        if (!playerStamina.HasEnoughStamina(GetStaminaCostForAttack(currentAttackIndex))) return;

        if (currentAttackIndex == 0) ConsumeStamina();
        lastAttackTime = Time.time;
        PlayerStateManager.Instance.SetState(PlayerStateType.Attacking);
        isAttacking = true;
        animator.SetTrigger("Attack");
    }

    private void AutomatizeAnimationEvents()
    {
        var clips = animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (clip.name.Contains("Attack"))
            {
                AnimationEvent endEvent = new()
                {
                    time = clip.length,
                    functionName = "OnAttackAnimationComplete"
                };
                clip.AddEvent(endEvent);
            }
        }
    }

    public void OnAttackAnimationComplete()
    {
        isAttacking = false;
        currentAttackIndex = 0;
        PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
    }

    private int GetStaminaCostForAttack(int attackIndex)
    {
        return attackIndex switch
        {
            0 => firstAttackCost,
            1 => secondAttackCost,
            2 => thirdAttackCost,
            _ => firstAttackCost,
        };
    }

    public void ConsumeStamina()
    {
        playerStamina.UseStamina(GetStaminaCostForAttack(currentAttackIndex));
        currentAttackIndex++;
        if (currentAttackIndex > 2) currentAttackIndex = 0;
    }

    public void SetAttackWindowActive(int isActive)
    {
        attackWindow = isActive == 1;
    }

    public bool GetAttackWindow() => attackWindow;
}
