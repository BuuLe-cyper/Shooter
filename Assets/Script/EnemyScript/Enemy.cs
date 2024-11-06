using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    //public float damageMultiplier = 1.0f;
    public float damageToPlayer = 1.0f;
    //public static float totalScore = 0f;
    public int expEnemy = 10;
    private GameObject gameOverManager;
    private ScoreManagement scoreManagement;
    public Animator animator;
    public AudioManager audioManager;
    private CharacterLevelManager characterLevelManager;
    private SpriteRenderer spriteRenderer;
    private BlinkAnimation blinkAnimation;
    private bool isDead = false;

    void Start()
    {
        gameOverManager = GameObject.Find("2D Canvas");
        currentHealth = maxHealth;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterLevelManager = GameObject.FindObjectOfType<CharacterLevelManager>();
        scoreManagement = FindObjectOfType<ScoreManagement>();
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return; // Prevent further damage processing if already dead

        if (audioManager != null)
        {
            audioManager.PlayZombieHitSound();
        }

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

            // Grant experience points for the enemy's defeat
            characterLevelManager.GainExperiences(expEnemy);
        }
    }

    IEnumerator WaitForDeathAnimation()
    {
        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);

        // Wait until the "Dead" animation starts
        while (!animationState.IsName("Dead"))
        {
            yield return null;
            animationState = animator.GetCurrentAnimatorStateInfo(0);
        }

        // Wait until the "Dead" animation completes
        while (animationState.IsName("Dead") && animationState.normalizedTime < 1.0f)
        {
            yield return null;
            animationState = animator.GetCurrentAnimatorStateInfo(0);
        }

        Die();
    }

    void Die()
    {
        if (isDead) return; // Exit if the enemy is already dead

        isDead = true; // Mark the enemy as dead

        float scoreFromDeath = (maxHealth * damageToPlayer) / 10f;
        scoreManagement.AddPoint(scoreFromDeath);

        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.ZombieDeath1);
        }

        // Destroy the enemy game object
        Destroy(gameObject);

        // Check if this is a specific enemy type to trigger game win condition
        if (gameObject.name.Contains("EnemySkeleton") && gameOverManager != null)
        {
            MainCanvasManager mainCanvasManager = gameOverManager.GetComponent<MainCanvasManager>();
            mainCanvasManager.GameWin();
        }
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
        else if (collision.gameObject.tag == "character" && collision.gameObject.name == "character2")
        {
            CircleCollider2D circleCollider = collision.gameObject.GetComponent<CircleCollider2D>();

            if (circleCollider != null && collision.collider == circleCollider)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageToPlayer);
                }
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
