using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DragonWarriorAI : MonoBehaviour
{
    public Transform target;
    public float followDistance = 5f;
    public float attackDistance = 5f;
    public float movementSpeed = 3f;
    public float minFollowDistance = 3f;

    public GameObject fireBallPrefab;
    public Animator animator;

    public Seeker seeker;
    private Path path;
    public float repathRate = 0.5f;
    public float nextWPDistance = 0.5f;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    private DragonDamage dragonDamage;

    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    private bool isAttacking = false;
    private bool isDead = false;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        GameObject player = GameObject.FindGameObjectWithTag("character");
        if (player != null)
        {
            target = player.transform;

            InvokeRepeating(nameof(CalculatePath), 0f, repathRate);
        }

        dragonDamage = GetComponent<DragonDamage>();
    }

    private void CalculatePath()
    {
        if (seeker.IsDone() && target != null)
        {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (target != null && !isAttacking)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            GameObject nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, nearestEnemy.transform.position);

                if (distanceToEnemy <= 2f && Time.time - lastAttackTime >= attackCooldown)
                {
                    StartCoroutine(PerformCloseRangeAttack(nearestEnemy));
                    lastAttackTime = Time.time;
                    return;
                }

                if (distanceToEnemy <= attackDistance && Time.time - lastAttackTime >= attackCooldown)
                {
                    StartCoroutine(AttackEnemy(nearestEnemy));
                    lastAttackTime = Time.time;
                    return;
                }
            }

            if (distanceToTarget > minFollowDistance && distanceToTarget <= followDistance)
            {
                if (path == null || path.vectorPath == null)
                    return;

                if (currentWaypoint >= path.vectorPath.Count)
                {
                    if (!reachedEndOfPath)
                    {
                        reachedEndOfPath = true;
                        SetIdleAnimation();
                    }
                    return;
                }
                else
                {
                    reachedEndOfPath = false;
                }

                MoveAlongPath();
            }
            else
            {
                if (reachedEndOfPath || GetCurrentMovementSpeed() < 0.01f)
                {
                    reachedEndOfPath = true;
                    SetIdleAnimation();
                }
            }
        }
        else if (!isAttacking)
        {
            if (!reachedEndOfPath)
            {
                reachedEndOfPath = true;
                SetIdleAnimation();
            }
        }
    }

    private float GetCurrentMovementSpeed()
    {
        if (path == null || path.vectorPath == null || currentWaypoint >= path.vectorPath.Count)
            return 0f;

        Vector3 direction = ((Vector3)path.vectorPath[currentWaypoint] - transform.position).normalized;
        return direction.magnitude * movementSpeed;
    }

    private void MoveAlongPath()
    {
        Vector3 direction = ((Vector3)path.vectorPath[currentWaypoint] - transform.position).normalized;
        float moveSpeed = direction.magnitude * movementSpeed;
        Vector3 force = direction * moveSpeed * Time.deltaTime;

        transform.position += force; // Di chuyển DragonWarrior

        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWPDistance)
        {
            currentWaypoint++;
        }

        if (moveSpeed >= 0.001f)
        {
            transform.localScale = direction.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            animator.Play("Walk");
        }
        else
        {
            SetIdleAnimation();
        }
    }

    private void SetIdleAnimation()
    {
        animator.Play("Idle");
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private IEnumerator AttackEnemy(GameObject enemy)
    {
        isAttacking = true;
        animator.Play("Atack");
        
        if (audioManager != null)
        {
            audioManager.PlayDragonSound("Shoot");
        }


        Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
        transform.localScale = directionToEnemy.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

        yield return new WaitForSeconds(0.5f);

        GameObject fireBall = Instantiate(fireBallPrefab, transform.position, Quaternion.identity);
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        fireBall.GetComponent<FireBall>().Initialize(enemy.transform);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isAttacking = false;

        if (GetCurrentMovementSpeed() < 0.01f)
        {
            SetIdleAnimation();
        }
    }

    private IEnumerator PerformCloseRangeAttack(GameObject enemy)
    {
        isAttacking = true;

        int attackType = Random.Range(0, 2); // 0 is FlyKick, 1 is Strike
        string attackAnimation = attackType == 0 ? "FlyKick" : "Strike";

        dragonDamage.damage = attackType == 0 ? 20f : 30f; // Set damage based on attack type

        animator.Play(attackAnimation);

        Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
        transform.localScale = directionToEnemy.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

        float attackMoveSpeed = movementSpeed * 1.5f; // Move faster during attack
        float attackDuration = animator.GetCurrentAnimatorStateInfo(0).length;

        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            float step = attackMoveSpeed * Time.deltaTime;

            // Stop if very close to the enemy (to prevent overshooting)
            if (Vector3.Distance(transform.position, enemy.transform.position) < 0.1f)
            {
                break;
            }

            transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position, step);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(Mathf.Max(0, attackDuration - elapsedTime));

        isAttacking = false;

        if (GetCurrentMovementSpeed() < 0.01f)
        {
            SetIdleAnimation();
        }
    }



    public void DestroyDragon()
    {
        if (!isDead)
        {
            isDead = true;

            StopAllCoroutines();
            animator.Play("Dead");
            Destroy(gameObject, 2f);
        }
    }

}
