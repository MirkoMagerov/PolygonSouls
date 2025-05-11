using System;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Action<int> OnPlayerDamaged;
    public Action<int> OnPlayerHealed;
    public static Action OnPlayerDied;

    [SerializeField] private int potionHealAmount = 20;
    [SerializeField] private int initialPotions = 5;
    [SerializeField] private int minPotions = 5;
    [SerializeField] private int currentPotionLevel = 0;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float healingParticlesDuration = 1.65f;
    [SerializeField] TextMeshProUGUI potionAmountText;
    [SerializeField] TextMeshProUGUI potionLevelText;
    [SerializeField] GameObject healingEffect;
    private int currentHealth;
    private int potionQuantity;
    private bool isHealingApplied = false;
    private PlayerDodge dodge;
    private PlayerEquipmentReference equipmentReference;
    private Animator animator;
    private StarterAssetsInputs input;

    private bool isHealing = false;

    void Start()
    {
        dodge = GetComponent<PlayerDodge>();
        equipmentReference = GetComponent<PlayerEquipmentReference>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        input = GetComponent<StarterAssetsInputs>();
        input.OnHealPerformed += StartHealing;
        currentHealth = maxHealth;
        potionQuantity = initialPotions;

        UpdateUI();
    }

    void OnDestroy()
    {
        input.OnHealPerformed -= StartHealing;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(150);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!dodge.IsInvincible() && currentHealth > 0)
        {
            OnPlayerDamaged?.Invoke(damage);

            currentHealth = Mathf.Max(currentHealth - damage, 0);
            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            if (isHealing)
            {
                InterruptHealing();
            }
            else
            {
                animator.SetTrigger("Hit");
            }
        }
    }

    private void StartHealing()
    {
        if (PlayerStateManager.Instance.GetCurrentState().CanHeal && !isHealing && potionQuantity > 0)
        {
            input.isHealing = false;
            isHealing = true;
            isHealingApplied = false;
            PlayerStateManager.Instance.SetState(PlayerStateType.Healing);
            equipmentReference.GetShield().SetActive(false);
            equipmentReference.GetHealthPotion().SetActive(true);
            animator.SetTrigger("Heal");
        }

    }

    private void InterruptHealing()
    {
        animator.ResetTrigger("Heal");
        animator.SetTrigger("Hit");

        isHealing = false;

        equipmentReference.GetShield().SetActive(true);
        equipmentReference.GetHealthPotion().SetActive(false);
        PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
    }

    public void ApplyHealEffect()
    {
        if (isHealing && !isHealingApplied)
        {
            Heal(potionHealAmount);
            isHealingApplied = true;
        }
    }

    public void Heal(int healAmount)
    {
        GameObject healingParticles = Instantiate(healingEffect, transform);
        Destroy(healingParticles, healingParticlesDuration);
        potionQuantity -= 1;
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        OnPlayerHealed?.Invoke(healAmount);
        UpdateUI();
    }

    public void LevelUpHealPotion()
    {
        if (currentPotionLevel == 5) return;
        potionHealAmount += currentPotionLevel * 5;
        currentPotionLevel++;
        UpdateUI();
    }

    public void UpdateUI()
    {
        potionAmountText.text = potionQuantity.ToString();

        if (currentPotionLevel == 0) return;
        potionLevelText.text = $"+{currentPotionLevel}";
    }

    public void AddHealthPotion(int amount)
    {
        potionQuantity += amount;
        UpdateUI();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnPlayerHealed?.Invoke(currentHealth);
        UpdateUI();
    }

    public void Die()
    {
        potionQuantity = minPotions;
        OnPlayerDied?.Invoke();
    }

    private void CompleteHealingAnimation()
    {
        isHealing = false;
        isHealingApplied = false;

        equipmentReference.GetShield().SetActive(true);
        equipmentReference.GetHealthPotion().SetActive(false);
        PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
    }

    public int GetMaxHealth() { return maxHealth; }
    public int GetCurrentHealth() { return currentHealth; }
}