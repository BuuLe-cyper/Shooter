using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    //public float damageMultiplier = 1.0f;
    public float damageToPlayer = 1.0f;
    public static float totalScore = 0f;
    public int expEnemy = 10;

    public Animator animator;
    public AudioManager audioManager;
    private CharacterLevelManager characterLevelManager;
    private SpriteRenderer spriteRenderer;
    private BlinkAnimation blinkAnimation;

    void Start()
    {
        currentHealth = maxHealth;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterLevelManager = GameObject.FindObjectOfType<CharacterLevelManager>();
    }

    public virtual void TakeDamage(float damage)
    {
        if (audioManager != null)
        {
            audioManager.PlayZombieHitSound();
        }
        //float actualDamage = damage * damageMultiplier;
        StartCoroutine(BlinkEnemy(spriteRenderer));

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if (animator != null)
            {
                animator.Play("Dead");

                StartCoroutine(WaitForDeathAnimation());
            }
            else
            {
                Die();
            }
            characterLevelManager.GainExperiences(expEnemy);
        }
    }

    IEnumerator WaitForDeathAnimation()
    {
        // Wait for the animation to start playing the "Dead" animation
        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
        while (!animationState.IsName("Dead"))
        {
            yield return null;  // Wait for the next frame
            animationState = animator.GetCurrentAnimatorStateInfo(0);  // Update the animation state info
        }

        // Now the death animation is playing, wait until it completes
        while (animationState.IsName("Dead") && animationState.normalizedTime < 1.0f)
        {
            yield return null;  // Wait for the next frame
            animationState = animator.GetCurrentAnimatorStateInfo(0);  // Update the animation state info
        }

        Die();
    }

    void Die()
    {
        float scoreFromDeath = (maxHealth * damageToPlayer) / 10f;

        totalScore += scoreFromDeath;
        audioManager.PlaySFX(audioManager.ZombieDeath1);

        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
            }

        }
    }
    private IEnumerator BlinkEnemy(SpriteRenderer spriteRenderer)
    {
        float elaspedTime = 0f;

        while (elaspedTime < 0.1f)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(0.05f);

            elaspedTime += 0.05f;
        }
        spriteRenderer.enabled = true;
    }
}
