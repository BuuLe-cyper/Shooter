using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float health;
    private float lerpSpeed = 0.05f;
    private float defense = 50f;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = health;

        easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0) return; // Prevent taking damage if already dead

        // Calculate actual damage considering defense
        float actualDamage = damage / (1 + (defense / 100f));

        // Debugging logs
        Debug.Log("Max Health: " + maxHealth);
        Debug.Log("Current Health: " + health);
        Debug.Log("Damage: " + actualDamage);

        // Apply damage and prevent negative health
        health -= actualDamage;
        health = Mathf.Clamp(health, 0, maxHealth); // Ensure health doesn't go below zero

        // Check for death
        if (health <= 0)
        {
            animator.SetBool("Death", true);
            Debug.Log("Death");
            health = 0; // Ensure health is set to zero
        }
    }
}
