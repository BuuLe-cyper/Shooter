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
        if (health <= 0) return;
        float actualDamage = damage / (1 + (defense / 100f));


        health -= actualDamage;
        health = Mathf.Clamp(health, 0, maxHealth);

        // Check for death
        if (health <= 0)
        {
            animator.SetBool("Death", true);
            health = 0;
        }
    }
}
