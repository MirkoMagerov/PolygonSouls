using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private int hashAttackCount = Animator.StringToHash("AttackCount");
    public bool isAttacking = false;

    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        InputActionsManager.Instance.GetPlayerControls().Attack.PrimaryAttack.performed += Attack;
        AutomatizeAnimationEvents();
    }

    private void OnDisable()
    {
        InputActionsManager.Instance.GetPlayerControls().Attack.PrimaryAttack.performed -= Attack;
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (!PlayerStateManager.Instance.stateInfo.CanAttack) return;

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
}
