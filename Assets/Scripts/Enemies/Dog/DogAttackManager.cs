using UnityEngine;

public class DogAttackManager : MonoBehaviour
{
    [SerializeField] private Collider mouthCollider;
    private DogStateMachine stateMachine;

    private bool attackWindowActive = false;

    private void Start()
    {
        stateMachine = GetComponentInParent<DogStateMachine>();
        mouthCollider.enabled = false;
    }

    public void OpenAttackWindow()
    {
        attackWindowActive = true;
        if (mouthCollider != null) mouthCollider.enabled = true;
    }

    public void CloseAttackWindow()
    {
        attackWindowActive = false;
        if (mouthCollider != null) mouthCollider.enabled = false;
    }

    public void InterruptAttack()
    {
        attackWindowActive = false;
        if (mouthCollider != null) mouthCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!attackWindowActive) return;

        if (other.CompareTag("Player"))
        {
            CloseAttackWindow();
            stateMachine.DealDamage();
        }
    }
}