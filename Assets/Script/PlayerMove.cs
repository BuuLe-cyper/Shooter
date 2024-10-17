using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Movement properties
    public float moveSpeed = 5f;
    public float rollBoost = 4f;
    public float flashDistance = 3;
    public float rollCooldownTime = 0.25f;

    private float rollCooldown;
    private bool isRolling = false;
    private Rigidbody2D rb;
    public Animator animator;
    private Vector2 moveInput;
    public SpriteRenderer charSR;

    private float flashCooldown;  // Tracks flash cooldown time
    private float flashCooldownTime = 10f;
    public string flashCooldownText;  // Reference to the UI Text to show cooldown

    // Add a reference to the Abilities script
    public Abilities abilities;

    // DragonWarrior summon properties
    public GameObject dragonWarriorPrefab;
    public float dragonWarriorDuration = 30f; // 2 minutes
    public float summonCooldownTime = 120f; // 3 minutes
    private float summonCooldown = 0;
    private GameObject summonedDragonWarrior;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        this.flashCooldown = 0;
        this.summonCooldown = 0;
    }

    private void Update()
    {
        HandleMovement();
        HandleRoll();
        HandleFlash();
        HandleSummonDragonWarrior(); // Handle summoning DragonWarrior

        // Make the player face the mouse cursor
        RotateTowardsMouse();

        // Update the flash cooldown UI
        UpdateFlashCooldownUI();
    }

    // Handle basic movement and animation
    private void HandleMovement()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        rb.velocity = moveInput * moveSpeed;

        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    // Handle rolling action
    private void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space) && rollCooldown <= 0)
        {
            StartRoll();
        }

        if (isRolling)
        {
            rollCooldown -= Time.deltaTime;
            if (rollCooldown <= 0)
            {
                EndRoll();
            }
        }
    }

    private void StartRoll()
    {
        animator.SetBool("Roll", true);
        moveSpeed += rollBoost;
        rollCooldown = rollCooldownTime;
        isRolling = true;
    }

    private void EndRoll()
    {
        animator.SetBool("Roll", false);
        moveSpeed -= rollBoost;
        isRolling = false;
    }

    // Handle flash movement (teleport forward)
    private void HandleFlash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && this.flashCooldown <= 0)
        {
            Vector2 flashDirection = moveInput.normalized;
            if (flashDirection == Vector2.zero)
            {
                flashDirection = Vector2.right * Mathf.Sign(transform.localScale.x);
            }
            this.flashCooldown = this.flashCooldownTime;
            transform.position = rb.position + flashDirection * flashDistance;
        }

        if (flashCooldown > 0)
        {
            this.flashCooldown -= Time.deltaTime;
        }
    }

    // Handle summoning DragonWarrior
    private void HandleSummonDragonWarrior()
    {
        // Check if cooldown is over and the player pressed 'F'
        if (Input.GetKeyDown(KeyCode.F) && summonCooldown <= 0)
        {
            SummonDragonWarrior();
        }

        // Cooldown countdown
        if (summonCooldown > 0)
        {
            summonCooldown -= Time.deltaTime;
        }
    }

    private void SummonDragonWarrior()
    {
        // Summon DragonWarrior at player's position
        summonedDragonWarrior = Instantiate(dragonWarriorPrefab, transform.position, Quaternion.identity);

        // Start coroutine to destroy DragonWarrior after duration
        StartCoroutine(DestroyDragonWarriorAfterTime());

        // Set summon cooldown
        summonCooldown = summonCooldownTime;
    }

    private IEnumerator DestroyDragonWarriorAfterTime()
    {
        // Wait for the duration of DragonWarrior existence
        yield return new WaitForSeconds(dragonWarriorDuration);

        // Destroy the DragonWarrior after the duration is up
        if (summonedDragonWarrior != null)
        {
            summonedDragonWarrior.gameObject.GetComponent<DragonWarriorAI>().DestroyDragon();
            //Destroy(summonedDragonWarrior);
            //summonedDragonWarrior = null; // Reset reference
        }
    }

    // Stop movement and prevent rotation upon collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ObjectNoGo") || collision.gameObject.CompareTag("Floor"))
        {
            rb.velocity = Vector2.zero;
        }
    }

    // Update the flash cooldown UI text
    private void UpdateFlashCooldownUI()
    {
        if (flashCooldown > 0)
        {
            flashCooldownText = "Flash Cooldown: " + Mathf.Ceil(flashCooldown).ToString() + "s";
        }
        else
        {
            flashCooldownText = "Flash Ready!";
        }
    }

    // Rotate the character to face the mouse cursor
    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = (mousePos - transform.position).normalized;

        if (direction != Vector2.zero)
        {
            charSR.transform.localScale = new Vector3(Mathf.Sign(direction.x) * Mathf.Abs(charSR.transform.localScale.x), charSR.transform.localScale.y, charSR.transform.localScale.z);
        }
    }
}
