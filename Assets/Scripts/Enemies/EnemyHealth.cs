using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public interface IEnemyStateMachine
{
    void NotifyDeath();
}

public class EnemyHealth : MonoBehaviour
{
    public delegate void EnemyDeathHandler(Transform enemy);
    public event EnemyDeathHandler OnEnemyDeath;

    [SerializeField] private Canvas healthBarCanvas;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider damageBar;
    [SerializeField] private MonoBehaviour enemyStateMachine;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private float damageAnimationDuration = 0.5f;
    private Animator animator;
    private Coroutine damageAnimationCoroutine;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        healthBar.maxValue = maxHealth;
        damageBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        damageBar.value = maxHealth;

        if (maxHealth == currentHealth) healthBar.gameObject.SetActive(false);
    }

    void Update()
    {
        healthBarCanvas.transform.rotation = Quaternion.LookRotation(healthBarCanvas.transform.position - mainCamera.transform.position);
    }

    public void TakeDamage(int damage)
    {
        if (!healthBar.gameObject.activeInHierarchy) healthBar.gameObject.SetActive(true);

        int previousHealth = currentHealth;
        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
        }

        healthBar.value = currentHealth;

        if (damageAnimationCoroutine != null)
        {
            StopCoroutine(damageAnimationCoroutine);
        }

        damageBar.value = previousHealth;

        damageAnimationCoroutine = StartCoroutine(AnimateDamageBar(currentHealth));
    }

    private IEnumerator AnimateDamageBar(float targetValue)
    {
        yield return new WaitForSeconds(0.35f);

        float startValue = damageBar.value;
        float elapsed = 0f;

        while (elapsed < damageAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / damageAnimationDuration;

            float easedT = 1 - Mathf.Pow(1 - t, 3);
            damageBar.value = Mathf.Lerp(startValue, targetValue, easedT);

            yield return null;
        }

        damageBar.value = targetValue;
        damageAnimationCoroutine = null;
    }

    private void Die()
    {
        OnEnemyDeath?.Invoke(transform);
        healthBarCanvas.gameObject.SetActive(false);

        if (enemyStateMachine is IEnemyStateMachine stateMachine)
        {
            stateMachine.NotifyDeath();
        }
    }

    public void InstaKill()
    {
        currentHealth = 0;
        healthBar.value = currentHealth;
        Die();
    }
}
