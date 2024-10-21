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
    private SpriteRenderer spriteRenderer;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        health = maxHealth;
        GameObject player = GameObject.Find("character");
        spriteRenderer = player.GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = health;

        easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(BlinkPlayer(spriteRenderer));
        if (audioManager != null)
    {
        audioManager.PlayPlayerHitSound();
    }

        if (health <= 0) return;
        float actualDamage = damage / (1 + (defense / 100f));


        health -= actualDamage;
        health = Mathf.Clamp(health, 0, maxHealth);

        // Check for death
        if (health <= 0)
        {
            PlayerDie();
        }
    }
    private IEnumerator BlinkPlayer(SpriteRenderer spriteRenderer)
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

    private void PlayerDie()
    {
        animator.SetBool("Death", true);
        health = 0;
    }
}
