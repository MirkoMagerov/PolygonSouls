using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarDamageEffect : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider damageSlider;

    [Header("Configuración")]
    [SerializeField] private float damageAnimationDuration = 0.5f;

    private int maxHealth;
    private int currentHealth;
    [SerializeField] private PlayerHealth playerHealth;
    private Coroutine damageAnimationCoroutine;

    private void Start()
    {
        maxHealth = playerHealth.GetMaxHealth();
        healthSlider.maxValue = maxHealth;
        damageSlider.maxValue = maxHealth;

        currentHealth = maxHealth;
        healthSlider.value = maxHealth;
        damageSlider.value = maxHealth;
    }

    void OnEnable()
    {
        playerHealth.OnPlayerDamaged += TakeDamage;
        playerHealth.OnPlayerHealed += Heal;

    }

    void OnDisable()
    {
        playerHealth.OnPlayerDamaged -= TakeDamage;
        playerHealth.OnPlayerHealed -= Heal;
    }

    public void TakeDamage(int damageAmount)
    {
        int previousHealth = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);

        healthSlider.value = currentHealth;

        if (damageAnimationCoroutine != null)
        {
            StopCoroutine(damageAnimationCoroutine);
        }

        damageSlider.value = previousHealth;

        damageAnimationCoroutine = StartCoroutine(AnimateDamageBar(currentHealth));
    }

    private IEnumerator AnimateDamageBar(float targetValue)
    {
        yield return new WaitForSeconds(0.35f);

        float startValue = damageSlider.value;
        float elapsed = 0f;

        while (elapsed < damageAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / damageAnimationDuration;

            float easedT = 1 - Mathf.Pow(1 - t, 3);
            damageSlider.value = Mathf.Lerp(startValue, targetValue, easedT);

            yield return null;
        }

        damageSlider.value = targetValue;
        damageAnimationCoroutine = null;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        healthSlider.value = currentHealth;
        damageSlider.value = currentHealth;
    }
}