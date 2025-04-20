using StarterAssets;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public bool isAttacking = false;

    private bool attackWindow;
    private bool hitRegisteredThisAttack;

    private Animator animator;
    private StarterAssetsInputs inputActions;

    void Start()
    {
        animator = GetComponent<Animator>();
        inputActions = GetComponent<StarterAssetsInputs>();
        AutomatizeAnimationEvents();
    }

    void Update()
    {
        if (inputActions.primaryAttack)
        {
            Attack();
            inputActions.primaryAttack = false;
        }
    }

    private void Attack()
    {
        if (!PlayerStateManager.Instance.GetCurrentState().CanAttack) return;

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
                    functionName = "ResetAttackState"
                };
                clip.AddEvent(endEvent);
            }
        }
    }

    public void ResetAttackState()
    {
        PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
        isAttacking = false;
    }

    public void SetAttackWindowActive(int isActive)
    {
        attackWindow = isActive == 1;
        if (isActive == 1)
        {
            hitRegisteredThisAttack = false;
        }
    }

    public bool GetAttackWindow() => attackWindow;
}
