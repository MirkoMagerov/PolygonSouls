using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Player")]
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    public float JumpTimeout = 0.50f;

    [Header("Player Grounded")]
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    private float speed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    public float verticalVelocity { get; private set; }
    private float jumpTimeoutDelta;

    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDFreeFall;
    private int animIDMotionSpeed;

    private CharacterController controller;
    private StarterAssetsInputs input;
    private Animator animator;
    private GameObject mainCamera;
    private PlayerStamina stamina;
    private PlayerLockOn lockOn;

    private void Awake()
    {
        mainCamera = Camera.main.gameObject;
        controller = GetComponent<CharacterController>();
        input = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        stamina = GetComponent<PlayerStamina>();
        lockOn = GetComponent<PlayerLockOn>();
    }

    private void Start()
    {
        AssignAnimationIDs();
        jumpTimeoutDelta = JumpTimeout;
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    public void GroundedCheck()
    {
        Vector3 spherePosition = new(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        animator.SetBool(animIDGrounded, Grounded);
    }

    public void HandleMovement()
    {
        float canSprintSpeed = stamina.HasEnoughStamina(stamina.sprintCostPerSecond * Time.deltaTime) ? SprintSpeed : MoveSpeed;
        float targetSpeed = input.sprint ? canSprintSpeed : MoveSpeed;

        if (input.move == Vector2.zero) targetSpeed = 0.0f;
        animator.SetFloat("Horizontal", input.move.x);
        animator.SetFloat("Vertical", input.move.y);

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

        if (Mathf.Abs(currentHorizontalSpeed - targetSpeed) > 0.1f)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

        if (input.move != Vector2.zero)
        {
            if (PlayerStateManager.Instance.GetCurrentState().currentState != PlayerStateType.InAir)
            {
                PlayerStateManager.Instance.SetState(PlayerStateType.Moving);
            }
            if (!lockOn.GetEnemyLockedOn())
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            else
            {
                animator.SetFloat(animIDSpeed, animationBlend);
                MoveInLockOnMode();
                return;
            }
        }
        else if (PlayerStateManager.Instance.GetCurrentState().currentState == PlayerStateType.Moving)
        {
            PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
        }

        controller.Move(new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

        animator.SetFloat(animIDSpeed, animationBlend);
        animator.SetFloat(animIDMotionSpeed, inputMagnitude);
    }

    public void MoveInLockOnMode()
    {
        Vector3 verticalMovement = new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime;

        if (input.move.x != 0 && input.move.y != 0)
        {
            Vector3 moveDirection = new Vector3(input.move.x, 0, input.move.y).normalized;

            moveDirection = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0) * moveDirection;

            float targetSpeed = input.sprint ? SprintSpeed : MoveSpeed;

            float diagonalCompensation = 0.4f;
            controller.Move(diagonalCompensation * targetSpeed * Time.deltaTime * moveDirection + verticalMovement);
        }
        else
        {
            controller.Move(verticalMovement);
        }

        animator.SetFloat("Horizontal", input.move.x);
        animator.SetFloat("Vertical", input.move.y);
    }

    public void HandleJumpAndGravity()
    {
        if (Grounded)
        {
            if (PlayerStateManager.Instance.GetCurrentState().currentState == PlayerStateType.InAir)
            {
                PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
            }

            animator.SetBool(animIDFreeFall, false);

            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = 0f;
            }

            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            PlayerStateManager.Instance.SetState(PlayerStateType.InAir);
            jumpTimeoutDelta = JumpTimeout;

            animator.SetBool(animIDFreeFall, true);
        }

        verticalVelocity += Gravity * Time.deltaTime;
    }

    public bool IsMoving()
    {
        return input.move != Vector2.zero;
    }

    public bool IsSprinting()
    {
        return input.sprint && IsMoving();
    }
}