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
        public Animator animator;
        void Start()
        {
            currentHealth = maxHealth;
        }


    public virtual void TakeDamage(float damage)
    {
        //float actualDamage = damage * damageMultiplier;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if(animator != null)
            {
                animator.Play("Dead");

                // Start coroutine to wait for death animation to finish
                StartCoroutine(WaitForDeathAnimation());
            }
            else
            {
                Die();
            }
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

        // After the death animation finishes, destroy the object
        Die();
    }

    void Die()
    {
        //float scoreFromDeath = (maxHealth * damageMultiplier * damageToPlayer) / 10f;
        float scoreFromDeath = (maxHealth * damageToPlayer) / 10f;

        totalScore += scoreFromDeath;

        // Destroy the enemy game object
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
    }
