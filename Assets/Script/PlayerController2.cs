﻿using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    // Movement properties
    public float moveSpeed = 5f;
    public float rollBoost = 4f;
    public float flashDistance = 3;
    public float rollCooldownTime = 0.25f;
    private float rollCooldown;
    private bool isRolling = false;

    // Flash properties
    private float flashCooldown;
    private float flashCooldownTime = 10f;
    public string flashCooldownText;

    private Rigidbody2D rb;
    public Animator animator;
    private Vector2 moveInput;
    public SpriteRenderer charSR;

    // Combo attack properties
    private int comboCounter = 0;
    private float lastClickTime = 0f;
    private float comboDelay = 0.2f; // Time allowed between clicks to maintain combo

    //Shoot
    public GameObject bullet;
    public GameObject bullet1;
    public Transform firePos;
    public float TimeBtwFire = 0.2f;
    public float TimeBtwFire1 = 3f;
    public float bulletForce = 10f; // Default value
    private AudioManager audioManager; // Reference to AudioManager
    private float timeBtwFire;
    private float timeBtwFire1;

    //Damage
    public float damage = 10f;

    private bool isShooting = false;

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        moveSpeed = CharacterStatsManager.CurrentStats.speed.GetValue() / 20;
        GameObject vcam1 = GameObject.FindGameObjectWithTag("vcam1");
        if (vcam1 != null)
        {
            CinemachineVirtualCamera virtualCamera = vcam1.GetComponent<CinemachineVirtualCamera>();

            if (virtualCamera != null)
            {
                GameObject character2 = GameObject.FindGameObjectWithTag("character");

                // Check if Character 2 exists
                if (character2 != null)
                {
                    virtualCamera.Follow = character2.transform;
                }
                else
                {
                    Debug.LogWarning("Character 2 not found!");
                }
            }
            else
            {
                Debug.LogWarning("CinemachineVirtualCamera component not found on vcam1!");
            }
        }
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        this.flashCooldown = 0;
    }
    void Update()
    {
        HandleMovement();
        HandleRoll();
        HandleFlash();
        UpdateFlashCooldownUI();
        RotateTowardsMouse();
        HandleComboAttack();
        HandleShooting();
    }
    private void HandleShooting()
    {
        // Nếu đang bắn thì set cờ isShooting thành true
        if (Input.GetMouseButtonDown(0) && timeBtwFire <= 0)
        {
            isShooting = true;
            animator.SetBool("isShoot", true);
            animator.Play("Fire", -1, 0);
            audioManager.PlaySFX(audioManager.Shoot);
            FireBullet(bullet, TimeBtwFire);
            timeBtwFire = TimeBtwFire;
            StartCoroutine(ResetShootState("isShoot", 1.0f));
        }

        if (Input.GetKeyDown(KeyCode.F) && timeBtwFire1 <= 0)
        {
            isShooting = true;
            animator.SetBool("isShoot1", true);
            animator.CrossFade("Fire1", 0.1f);
            audioManager.PlaySFX(audioManager.Shoot);
            FireBullet(bullet1, TimeBtwFire1);
            timeBtwFire1 = TimeBtwFire1;
            StartCoroutine(ResetShootState("isShoot1", 1.0f));
        }

        // Giảm thời gian giữa các lần bắn
        if (timeBtwFire > 0) timeBtwFire -= Time.deltaTime;
        if (timeBtwFire1 > 0) timeBtwFire1 -= Time.deltaTime;

        // Chỉ set trạng thái di chuyển khi không bắn
        if (!isShooting)
        {
            animator.SetBool("isMoving", moveInput.sqrMagnitude > 0.001);
        }
    }
    private IEnumerator ResetShootState(string shootParameter, float delay)
    {
        yield return new WaitForSeconds(delay);

        animator.SetBool(shootParameter, false);
        isShooting = false; // Reset trạng thái bắn

        if (moveInput.sqrMagnitude < 0.001)
        {
            animator.CrossFade("idle", 0.2f);
        }
        else
        {
            animator.CrossFade("walk", 0.2f);
        }
    }
    private void FireBullet(GameObject bulletPrefab, float cooldown)
    {
        if (firePos == null || bulletPrefab == null)
        {
            return;
        }

        // Play sound effect if audioManager is assigned
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.Shoot);
        }

        // Instantiate bullet and apply force
        GameObject bulletInstance = Instantiate(bulletPrefab, firePos.position, Quaternion.identity);
        Rigidbody2D rb = bulletInstance.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;  // Disable gravity

            // Calculate direction towards the mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;  // Ensure it’s on the 2D plane
            Vector3 direction = (mousePos - firePos.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bulletInstance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180));

            // Apply force to the bullet
            if (bulletForce > 0)
            {
                rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
            }
        }
    }





    // Movement and Animation handling
    private void HandleMovement()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        rb.velocity = moveInput * moveSpeed;

        animator.SetFloat("Speed", moveInput.sqrMagnitude);

    }

    // Rolling mechanic
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
        animator.SetBool("isRolling", true);
        moveSpeed += rollBoost;
        rollCooldown = rollCooldownTime;
        isRolling = true;

    }

    private void EndRoll()
    {
        animator.SetBool("isRolling", false);
        moveSpeed -= rollBoost;
        isRolling = false;

    }

    // Flash mechanic
    private void HandleFlash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && flashCooldown <= 0)
        {
            Vector2 flashDirection = moveInput.normalized;
            if (flashDirection == Vector2.zero)
            {
                flashDirection = Vector2.right * Mathf.Sign(transform.localScale.x);
            }
            flashCooldown = flashCooldownTime;
            transform.position = rb.position + flashDirection * flashDistance;
        }

        if (flashCooldown > 0)
        {
            flashCooldown -= Time.deltaTime;
        }
    }

    // Update flash cooldown text
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

    // Rotate player sprite towards the mouse position
    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = (mousePos - transform.position).normalized;

        if (direction != Vector2.zero)
        {
            charSR.transform.localScale = new Vector3(-Mathf.Sign(direction.x) * Mathf.Abs(charSR.transform.localScale.x), charSR.transform.localScale.y, charSR.transform.localScale.z);
        }
    }

    // Combo Attack Mechanic
    private void HandleComboAttack()
    {
        // Bỏ qua khi đang bắn
        if (isShooting) return;

        if (Input.GetMouseButtonDown(1)) // Right-click
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (comboCounter == 0 || stateInfo.normalizedTime >= 1f)
            {
                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick <= comboDelay)
                {
                    comboCounter++;
                }
                else
                {
                    comboCounter = 1;
                }

                lastClickTime = Time.time;

                switch (comboCounter)
                {
                    case 1:
                        animator.Play("attack1");
                        audioManager.Audio("Sword");
                        break;
                    case 2:
                        animator.Play("attack2");
                        audioManager.Audio("Sword");
                        break;
                    case 3:
                        animator.Play("attack3");
                        audioManager.Audio("Sword");
                        break;
                    default:
                        comboCounter = 1;
                        animator.Play("attack1");
                        audioManager.Audio("Sword");
                        break;
                }
            }
        }

        // Nếu quá thời gian comboDelay, reset combo và kiểm tra điều kiện di chuyển
        if (Time.time - lastClickTime > comboDelay)
        {
            comboCounter = 0;

            if (moveInput.sqrMagnitude < 0.001)
            {
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttack", false);
                animator.Play("idle");
            }
            else
            {
                animator.SetBool("isMoving", true);
                animator.SetBool("isAttack", false);
                animator.SetFloat("Speed", moveInput.sqrMagnitude);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ObjectNoGo") || collision.gameObject.CompareTag("Floor"))
        {
            rb.velocity = Vector2.zero;
        }
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                float actualDamage = damage + CharacterStatsManager.CurrentStats.attack.GetValue();
                enemy.TakeDamage(actualDamage);
            }

        }
    }
    public void BoostDamage(float boostDamage)
    {
        damage += boostDamage;
    }
}
