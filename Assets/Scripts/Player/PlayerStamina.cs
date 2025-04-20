using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenerationRate = 5f;
    public float staminaRegenerationDelay = 1f;
    public Slider staminaSlider;

    [Header("Stamina Costs")]
    public float sprintCostPerSecond = 12.5f;
    public float dodgeCost = 20f;

    private float lastStaminaUseTime = 0f;
    private bool dodgeStaminaDeducted = false;
    private StarterAssetsInputs input;
    private PlayerMovement movement;
    private PlayerDodge dodge;

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
        movement = GetComponent<PlayerMovement>();
        dodge = GetComponent<PlayerDodge>();
    }

    private void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
    }

    private void Update()
    {
        if (movement.IsSprinting())
        {
            UseStamina(sprintCostPerSecond * Time.deltaTime);

            if (currentStamina <= 0)
            {
                input.sprint = false;
            }
        }

        if (dodge.IsDodging)
        {
            if (!dodgeStaminaDeducted && currentStamina >= dodgeCost)
            {
                UseStamina(dodgeCost);
                dodgeStaminaDeducted = true;
            }
        }
        else
        {
            dodgeStaminaDeducted = false;
        }

        if (Time.time > lastStaminaUseTime + staminaRegenerationDelay)
        {
            currentStamina = Mathf.Min(currentStamina + staminaRegenerationRate * Time.deltaTime, maxStamina);
        }

        staminaSlider.value = currentStamina;
    }

    public void UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            lastStaminaUseTime = Time.time;
        }
    }

    public bool HasEnoughStamina(float amount)
    {
        return currentStamina >= amount;
    }

    public float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }
}