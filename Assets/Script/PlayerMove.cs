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
    private Animator animator;
    private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.freezeRotation = true;
    }

    private void Update()
    {
        HandleMovement();
        HandleRoll();
        HandleFlash();
    }

    // Handle basic movement and animation
    private void HandleMovement()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        rb.velocity = moveInput * moveSpeed;

        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        if (moveInput.x != 0)
        {
            Vector3 currentScale = transform.localScale;
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x) * Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
    }

    // Handle rolling action
    private void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.J) && rollCooldown <= 0)
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            Vector2 flashDirection = moveInput.normalized;
            if (flashDirection == Vector2.zero)
            {
                flashDirection = Vector2.right * Mathf.Sign(transform.localScale.x);
            }

            rb.MovePosition(rb.position + flashDirection * flashDistance);
        }
    }

    // Stop movement and prevent rotation upon collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("RockGroup"))
        {
            rb.velocity = Vector2.zero;
        }
    }
}
