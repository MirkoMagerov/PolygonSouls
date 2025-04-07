using System.Collections;
using StarterAssets;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    public float JumpTimeout = 0.50f;

    [Header("Dodge Roll")]
    public float dodgeTimer;
    [SerializeField] AnimationCurve dodgeCurve;

    [Header("Player Grounded")]
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    [Header("Camera")]
    public GameObject CameraTarget;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;

    // Camera fields
    private float targetYaw;
    private float targetPitch;

    // Movement fields
    private float speed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float verticalVelocity;
    private float jumpTimeoutDelta;

    // Dodge roll fields
    private bool isDodging = false;
    private float dodgeTimeRemaining = 0f;
    private float dodgeCooldownRemaining = 0f;
    private Vector3 dodgeDirection;
    private bool isInvincible = false;

    // Animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDMotionSpeed;
    private int animIDDodge;

    // References
    private Animator animator;
    private CharacterController controller;
    private StarterAssetsInputs input;
    private GameObject mainCamera;
    private bool hasAnimator;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main.gameObject;
        }
    }

    private void Start()
    {
        targetYaw = CameraTarget.transform.rotation.eulerAngles.y;

        hasAnimator = TryGetComponent(out animator);
        controller = GetComponent<CharacterController>();
        input = GetComponent<StarterAssetsInputs>();

        AssignAnimationIDs();
        jumpTimeoutDelta = JumpTimeout;
        Keyframe dodge_lastFrame = dodgeCurve[dodgeCurve.length - 1];
        dodgeTimer = dodge_lastFrame.time;
    }

    private void Update()
    {
        hasAnimator = TryGetComponent(out animator);

        GroundedCheck();

        if (!isDodging)
        {
            if (input.dodge && Grounded)
            {
                StartCoroutine(DodgeRoll());
                input.dodge = false;
            }
            else if (PlayerStateManager.Instance.GetCurrentState().currentState != PlayerStateType.Attacking)
            {
                JumpAndGravity();
                Move();
            }
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private IEnumerator DodgeRoll()
    {
        PlayerStateManager.Instance.SetState(PlayerStateType.Dodging);
        animator.SetTrigger(animIDDodge);
        isDodging = true;
        float timer = 0;
        while (timer < dodgeTimer)
        {
            float dodgeSpeed = dodgeCurve.Evaluate(timer) * 13f;
            Vector3 dir = transform.forward * dodgeSpeed;
            controller.Move(dir * Time.deltaTime + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        isDodging = false;
        PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDDodge = Animator.StringToHash("Roll");
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        if (hasAnimator)
        {
            animator.SetBool(animIDGrounded, Grounded);
        }
    }

    private void CameraRotation()
    {
        const float threshold = 0.01f;

        if (input.look.sqrMagnitude >= threshold)
        {
            float deltaTime = IsUsingMouse() ? 1.0f : Time.deltaTime;
            targetYaw += input.look.x * deltaTime;
            targetPitch += input.look.y * deltaTime;
        }

        targetYaw = ClampAngle(targetYaw, float.MinValue, float.MaxValue);
        targetPitch = ClampAngle(targetPitch, BottomClamp, TopClamp);

        CameraTarget.transform.rotation = Quaternion.Euler(targetPitch, targetYaw, 0.0f);
    }

    private void Move()
    {
        float targetSpeed = input.sprint ? SprintSpeed : MoveSpeed;

        if (input.move == Vector2.zero) targetSpeed = 0.0f;

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

        // Update animation blend
        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

        if (input.move != Vector2.zero)
        {
            if (PlayerStateManager.Instance.GetCurrentState().currentState != PlayerStateType.InAir)
            {
                PlayerStateManager.Instance.SetState(PlayerStateType.Moving);
            }
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        else if (PlayerStateManager.Instance.GetCurrentState().currentState == PlayerStateType.Moving)
        {
            PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

        // Update animator
        if (hasAnimator)
        {
            animator.SetFloat(animIDSpeed, animationBlend);
            animator.SetFloat(animIDMotionSpeed, inputMagnitude);
        }
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            if (PlayerStateManager.Instance.GetCurrentState().currentState == PlayerStateType.InAir)
            {
                PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
            }
            if (hasAnimator)
            {
                animator.SetBool(animIDJump, false);
                animator.SetBool(animIDFreeFall, false);
            }

            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            if (input.jump && jumpTimeoutDelta <= 0.0f)
            {
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                if (hasAnimator)
                {
                    animator.SetBool(animIDJump, true);
                }
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

            if (hasAnimator)
            {
                animator.SetBool(animIDFreeFall, true);
            }

            input.jump = false;
        }

        verticalVelocity += Gravity * Time.deltaTime;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    private bool IsUsingMouse()
    {
        return GetComponent<PlayerInput>().currentControlScheme == "KeyboardMouse";
    }
}