using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCamera))]
[RequireComponent(typeof(PlayerStamina))]
[RequireComponent(typeof(PlayerDodge))]
public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private StarterAssetsInputs input;

    private PlayerMovement movement;
    private PlayerCamera cameraControl;
    private PlayerDodge dodge;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        input = GetComponent<StarterAssetsInputs>();

        movement = GetComponent<PlayerMovement>();
        cameraControl = GetComponent<PlayerCamera>();
        dodge = GetComponent<PlayerDodge>();
    }

    void Start()
    {
        anim.SetLayerWeight(0, 0f);
        anim.SetLayerWeight(1, 1f);
    }

    private void Update()
    {
        if (!dodge.IsDodging)
        {
            movement.GroundedCheck();

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
            else if (PlayerStateManager.Instance.GetCurrentState().currentState != PlayerStateType.Attacking)
            {
                movement.HandleJumpAndGravity();
                movement.HandleMovement();
            }

            input.dodge = false;
        }
    }

    private void LateUpdate()
    {
        cameraControl.UpdateCamera();
    }
}