using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Action<int> OnPlayerDamaged;
    public Action<int> OnPlayerHealed;
    public static Action OnPlayerDied;

    private int maxHealth = 100;
    private int currentHealth;
    private PlayerDodge dodge;
    private Animator animator;

    void Start()
    {
        dodge = GetComponent<PlayerDodge>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(20);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!dodge.IsInvincible())
        {
            OnPlayerDamaged?.Invoke(damage);
            animator.SetTrigger("Hit");
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        OnPlayerHealed?.Invoke(healAmount);
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public int GetMaxHealth() { return maxHealth; }
}
