using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private StarterAssetsInputs input;
    private PlayerMovement movement;
    private PlayerCamera cameraControl;
    private PlayerDodge dodge;
    private PlayerHealth health;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        input = GetComponent<StarterAssetsInputs>();

        movement = GetComponent<PlayerMovement>();
        cameraControl = GetComponent<PlayerCamera>();
        dodge = GetComponent<PlayerDodge>();
        health = GetComponent<PlayerHealth>();
    }

    void Start()
    {
        anim.SetLayerWeight(0, 0f);
        anim.SetLayerWeight(1, 1f);
        input.OnDodgePerformed += HandleDodge;
    }

    void OnDestroy()
    {
        input.OnDodgePerformed -= HandleDodge;
    }

    private void Update()
    {
        movement.GroundedCheck();
        movement.HandleJumpAndGravity();
        movement.HandleMovement();
    }

    private void HandleDodge()
    {
        if (!dodge.IsDodging)
        {
            if (input.dodge && dodge.CanDodge() && PlayerStateManager.Instance.GetCurrentState().CanDodge)
            {
                if (input.move.x != 0 || input.move.y != 0)
                {
                    dodge.StartDodge();
                }
                else
                {
                    dodge.StartBackStep();
                }
            }
            input.dodge = false;
        }
    }

    public void PlayDeathAnimation()
    {
        anim.SetTrigger("Die");
    }

    public void ResetHealth()
    {
        health.ResetHealth();
    }

    private void LateUpdate()
    {
        cameraControl.UpdateCamera();
    }
}