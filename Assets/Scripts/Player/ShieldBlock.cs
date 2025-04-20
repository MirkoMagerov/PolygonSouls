using System;
using StarterAssets;
using UnityEngine;

public class ShieldBlock : MonoBehaviour
{
    public float blockAngle = 45f;
    public float damageReduction = 0.7f;
    private Animator animator;
    private StarterAssetsInputs input;

    void Start()
    {
        animator = GetComponent<Animator>();
        input = GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        if (input.isBlocking)
        {
            animator.SetBool("Blocking", true);
        }
        else
        {
            animator.SetBool("Blocking", false);
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (!input.isBlocking) return;

        if (other.TryGetComponent<EnemyWeapon>(out var enemyWeapon))
        {
            if (IsInBlockAngle(other.transform.position))
            {
                float reducedDamage = enemyWeapon.GetDamage() * (1 - damageReduction);

                PlayerStamina stamina = GetComponentInParent<PlayerStamina>();
                stamina.UseStamina(enemyWeapon.GetStaminaCost());

                TryGetComponent<PlayerHealth>(out var playerHealth);
                playerHealth.TakeDamage((int)Math.Round(reducedDamage));

                Debug.Log("¡Ataque bloqueado! Daño reducido a: " + reducedDamage);

                PlayBlockEffects();
            }
        }
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