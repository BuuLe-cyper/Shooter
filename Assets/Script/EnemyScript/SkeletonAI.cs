using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAi : MonoBehaviour
{
    private Animator animator;
    public Transform target;

    public float movementSpeed = 2f;
    public float attackRange = 1.5f;
    public float detectionRange = 10f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    private bool isDead = false;
    private bool isAttacking = false;

    private enum State { Idle, Running, Attacking, Shielding, TakingHit, Dead }
    private State currentState;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentState = State.Idle;
        GameObject player = GameObject.Find("character");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void Update()
    {
        if (isDead)
        {
            Die();
            return;
        }

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget >= detectionRange)
            {
                MoveTowardsTarget();
            }
            else if (distanceToTarget <= attackRange)
            {
                Debug.Log("Attack Range: " + attackRange);
                Debug.Log("Distance to Target: " + distanceToTarget);
                AttackTarget();
            }
            else
            {
                Idle();
            }
        }
        else
        {
            Idle(); // If there's no target, idle
        }
    }

    // Play idle animation
    void Idle()
    {
        if (currentState != State.Idle && !isAttacking)
        {
            currentState = State.Idle;
            animator.Play("IdleSkeleton");
        }
    }

    // Move towards target animation
    void MoveTowardsTarget()
    {
        if (currentState != State.Running && !isAttacking)
        {
            currentState = State.Running;
            animator.Play("Skeleton_Run");
        }
    }

    // Attack target method
    void AttackTarget()
    {
        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            isAttacking = true; // Mark as attacking
            currentState = State.Attacking;
            Debug.Log("Attack initiated.");

            // Choose randomly between two attack animations
            if (Random.Range(0, 2) == 0)
            {
                Debug.Log("Attack2");
                animator.Play("Skeleton_Attack");
            }
            else
            {
                Debug.Log("Attack3");
                animator.Play("Skeleton_Attack2");
            }

            lastAttackTime = Time.time;

            // Start coroutine to reset attack state
            StartCoroutine(ResetAttackAfterDelay());
        }
        else
        {
            Idle();
        }
    }

    // Coroutine to reset attack state after animation duration
    private IEnumerator ResetAttackAfterDelay()
    {
        // Wait for the duration of the attack animation
        // You can adjust the wait time if needed based on your animation length
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isAttacking = false; // Reset attack state
        Idle(); // Transition back to idle
    }

    // Play hit animation
    public void TakeHit()
    {
        if (!isDead)
        {
            currentState = State.TakingHit;
            animator.Play("Skeleton_TakeHit");
        }
    }

    // Play shield animation
    public void Shield()
    {
        if (!isDead)
        {
            currentState = State.Shielding;
            animator.Play("Skeleton_Shield");
        }
    }

    // Play death animation
    public void Die()
    {
        if (!isDead)
        {
            currentState = State.Dead;
            animator.Play("Skeleton_Death");
            isDead = true;
        }
    }
}
