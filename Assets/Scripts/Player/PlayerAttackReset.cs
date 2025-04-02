using UnityEngine;

public class PlayerAttackReset : StateMachineBehaviour
{
    [SerializeField] private string triggerName;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);
    }
}
