using System.Collections;
using UnityEngine;
using StarterAssets;

public class PlayerDodge : MonoBehaviour
{
    [Header("Dodge Roll")]
    public float dodgeTimer = 0.5f;
    [SerializeField] AnimationCurve dodgeCurve;
    public float dodgeCooldown = 1.5f;
    public float dodgeCost = 15f;

    [Header("Back Step")]
    public float backStepTimer = 0.4f;
    public float backStepExtraDistance = 1.5f;

    public bool IsDodging { get; private set; }
    public float dodgeCooldownRemaining;
    [SerializeField] private bool isInvincible = false;
    private int animIDDodge;

    private CharacterController controller;
    private StarterAssetsInputs input;
    private Animator animator;
    private PlayerStamina stamina;
    private PlayerMovement movement;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        stamina = GetComponent<PlayerStamina>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        animIDDodge = Animator.StringToHash("Dodge");

        Keyframe dodge_lastFrame = dodgeCurve[dodgeCurve.length - 1];
        dodgeTimer = dodge_lastFrame.time;
    }

    private void Update()
    {
        if (dodgeCooldownRemaining > 0f)
        {
            dodgeCooldownRemaining -= Time.deltaTime;
        }
    }

    public bool CanDodge()
    {
        return !IsDodging && movement.Grounded && dodgeCooldownRemaining <= 0f && stamina.HasEnoughStamina(dodgeCost);
    }

    public void StartDodge()
    {
        if (CanDodge())
        {
            input.dodge = false;
            StartCoroutine(DodgeRoll());
        }
    }

    public void StartBackStep()
    {
        if (CanDodge())
        {
            input.dodge = false;
            StartCoroutine(BackStep());
        }
    }

    private IEnumerator BackStep()
    {
        isInvincible = true;
        PlayerStateManager.Instance.SetState(PlayerStateType.Dodging);
        animator.SetTrigger(animIDDodge);
        IsDodging = true;
        dodgeCooldownRemaining = dodgeCooldown;

        float timer = 0;
        while (timer < backStepTimer)
        {
            float backSpeed = backStepExtraDistance / backStepTimer;
            Vector3 dir = -transform.forward * backSpeed;

            controller.Move(dir * Time.deltaTime + new Vector3(0.0f, movement.verticalVelocity, 0.0f) * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        IsDodging = false;
        isInvincible = false;
        PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
    }

    private IEnumerator DodgeRoll()
    {
        isInvincible = true;
        PlayerStateManager.Instance.SetState(PlayerStateType.Dodging);
        animator.SetTrigger(animIDDodge);
        IsDodging = true;
        dodgeCooldownRemaining = dodgeCooldown;

        float timer = 0;
        while (timer < dodgeTimer)
        {
            float dodgeSpeed = dodgeCurve.Evaluate(timer) * 13f;
            Vector3 dir = transform.forward * dodgeSpeed;
            controller.Move(dir * Time.deltaTime + new Vector3(0.0f, movement.verticalVelocity, 0.0f) * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        IsDodging = false;
        isInvincible = false;
        PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}