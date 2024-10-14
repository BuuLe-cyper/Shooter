using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public float damageMultiplier = 1.0f;
    public float damageToPlayer = 1.0f;
    private int i = 0;
    void Start()
    {
        currentHealth = maxHealth;
    }


    public virtual void TakeDamage(float damage)
    {
        float actualDamage = damage * damageMultiplier;

        currentHealth -= actualDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    void Die()
    {
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
                i++;
                Debug.Log("time" + i);
            }


        }
    }
}
