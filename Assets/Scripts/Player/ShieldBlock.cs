using System;
using StarterAssets;
using UnityEngine;

public class ShieldBlock : MonoBehaviour
{
    public float blockAngle = 45f;
    public float damageReduction = 0.7f;
    private Animator animator;
    private StarterAssetsInputs input;
    private PlayerStamina playerStamina;
    private PlayerHealth playerHealth;

    void Start()
    {
        animator = GetComponent<Animator>();
        input = GetComponent<StarterAssetsInputs>();
        playerStamina = GetComponentInParent<PlayerStamina>();
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    void Update()
    {
        animator.SetBool("Blocking", input.isBlocking);
    }

    public bool IsBlockingActive()
    {
        return input.isBlocking;
    }

    public bool IsInBlockAngle(Vector3 attackPosition)
    {
        Vector3 directionToAttack = (attackPosition - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToAttack);
        return angle <= blockAngle;
    }

    public void ApplyReducedDamage(int damage, int staminaCost)
    {
        float reducedDamage = damage * (1 - damageReduction);

        playerStamina.UseStamina(staminaCost);

        playerHealth.TakeDamage((int)Math.Round(reducedDamage));

        PlayBlockEffects();
    }

    private void PlayBlockEffects()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null) audioSource.Play();

        ParticleSystem particles = GetComponent<ParticleSystem>();
        if (particles != null) particles.Play();

        //Camera.main.GetComponent<CameraShake>()?.Shake(0.1f, 0.1f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -blockAngle, 0) * transform.forward * 2);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, blockAngle, 0) * transform.forward * 2);
    }
}