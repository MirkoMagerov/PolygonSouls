using System;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Action<int> OnPlayerDamaged;
    public Action<int> OnPlayerHealed;
    public static Action OnPlayerDied;

    [SerializeField] private int potionHealAmount = 30;
    [SerializeField] private int initialPotions = 5;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] TextMeshProUGUI potionAmountText;
    private int currentHealth;
    private int potionQuantity;
    private bool isHealingApplied = false;
    private PlayerDodge dodge;
    private PlayerEquipmentReference equipmentReference;
    private Animator animator;
    private StarterAssetsInputs input;

    [SerializeField] private float healingThreshold = 0.6f;
    private bool isHealing = false;
    private float healingProgress = 0f;
    private float healingDuration = 0f;

    void Start()
    {
        dodge = GetComponent<PlayerDodge>();
        equipmentReference = GetComponent<PlayerEquipmentReference>();
        animator = GetComponent<Animator>();
        input = GetComponent<StarterAssetsInputs>();
        currentHealth = maxHealth;
        potionQuantity = initialPotions;

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Contains("Drink potion"))
            {
                healingDuration = clip.length;
                break;
            }
        }

        UpdateUI();
    }

    void Update()
    {
        if (input.isHealing && PlayerStateManager.Instance.GetCurrentState().CanHeal && !isHealing && potionQuantity > 0)
        {
            input.isHealing = false;
            StartHealing();
        }

        if (isHealing)
        {
            healingProgress += Time.deltaTime / healingDuration;

            if (healingProgress >= healingThreshold && !isHealingApplied)
            {
                Heal(potionHealAmount);
                isHealingApplied = true;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!dodge.IsInvincible())
        {
            OnPlayerDamaged?.Invoke(damage);

            if (isHealing)
            {
                InterruptHealing();
            }
            else
            {
                animator.SetTrigger("Hit");
            }

            currentHealth = Mathf.Max(currentHealth - damage, 0);
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void StartHealing()
    {
        isHealing = true;
        healingProgress = 0f;
        isHealingApplied = false;
        PlayerStateManager.Instance.SetState(PlayerStateType.Healing);
        equipmentReference.GetShield().SetActive(false);
        equipmentReference.GetHealthPotion().SetActive(true);
        animator.SetTrigger("Heal");
    }

    private void InterruptHealing()
    {
        animator.ResetTrigger("Heal");
        animator.SetTrigger("Hit");

        isHealing = false;
        healingProgress = 0f;

        equipmentReference.GetShield().SetActive(true);
        equipmentReference.GetHealthPotion().SetActive(false);
        PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
    }

    public void Heal(int healAmount)
    {
        potionQuantity -= 1;
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        OnPlayerHealed?.Invoke(healAmount);
        UpdateUI();
    }

    private void CompleteHealingAnimation()
    {
        isHealing = false;
        healingProgress = 0f;
        equipmentReference.GetShield().SetActive(true);
        equipmentReference.GetHealthPotion().SetActive(false);
        PlayerStateManager.Instance.SetState(PlayerStateType.Idle);
    }

    public void Die()
    {
        OnPlayerDied?.Invoke();
    }

    public void UpdateUI()
    {
        potionAmountText.text = potionQuantity.ToString();
    }

    public int GetMaxHealth() { return maxHealth; }
    public int GetCurrentHealth() { return currentHealth; }
}