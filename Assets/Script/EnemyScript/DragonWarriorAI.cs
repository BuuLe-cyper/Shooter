using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DragonWarriorAI : MonoBehaviour
{
    public Transform target; // Đối tượng để theo dõi
    public float followDistance = 5f; // Khoảng cách tối đa để theo dõi
    public float attackDistance = 5f; // Khoảng cách để tấn công kẻ thù
    public float movementSpeed = 3f; // Tốc độ di chuyển của DragonWarrior
    public float minFollowDistance = 3f; // Khoảng cách tối thiểu để duy trì từ đối tượng

    public GameObject fireBallPrefab; // Prefab FireBall để khởi tạo
    public Animator animator; // Animator cho DragonWarrior

    public Seeker seeker; // Sử dụng để tìm đường
    private Path path;
    private Coroutine moveCoroutine;
    public float repathRate = 0.5f;
    public float nextWPDistance = 0.5f;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    // Biến cooldown tấn công
    public float attackCooldown = 2f; // Thời gian giữa các đợt tấn công
    private float lastAttackTime = 0f; // Thời gian của lần tấn công trước

    private bool isAttacking = false; // Trạng thái tấn công

    private void Start()
    {
        // Tìm đối tượng để theo dõi
        GameObject player = GameObject.Find("character");
        if (player != null)
        {
            target = player.transform;

            // Bắt đầu tính toán đường đi đến mục tiêu
            InvokeRepeating(nameof(CalculatePath), 0f, repathRate);
        }
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
        if (target != null && !isAttacking)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Tìm kiếm kẻ thù gần nhất để tấn công
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

            // Theo dõi đối tượng nếu trong khoảng cách theo dõi và ngoài khoảng cách tối thiểu
            if (distanceToTarget > minFollowDistance && distanceToTarget <= followDistance)
            {
                if (path == null) return;

                if (currentWaypoint >= path.vectorPath.Count)
                {
                    reachedEndOfPath = true;
                    SetIdleAnimation();
                    return;
                }
                else
                {
                    reachedEndOfPath = false;
                }

                MoveAlongPath(); // Di chuyển dọc theo đường dẫn
            }
            else
            {
                SetIdleAnimation();
            }
        }
        else if (!isAttacking)
        {
            SetIdleAnimation();
        }
    }

    private void MoveAlongPath()
    {
        Vector3 direction = ((Vector3)path.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 force = direction * movementSpeed * Time.deltaTime;
        transform.position += force;

        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWPDistance)
        {
            currentWaypoint++;
        }

        if (Mathf.Abs(direction.x) > 0.01f)
        {
            transform.localScale = direction.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        }

        animator.Play("Walk");
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

        Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
        transform.localScale = directionToEnemy.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

        yield return new WaitForSeconds(0.5f);

        GameObject fireBall = Instantiate(fireBallPrefab, transform.position, Quaternion.identity);
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        Quaternion fireBallRotation = Quaternion.LookRotation(Vector3.forward, direction);
        fireBall.transform.rotation = Quaternion.Euler(0, 0, fireBallRotation.eulerAngles.z);

        fireBall.GetComponent<FireBall>().Initialize(enemy.transform);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.5f);

        isAttacking = false;
    }

    private IEnumerator PerformCloseRangeAttack(GameObject enemy)
    {
        isAttacking = true;

        int attackType = Random.Range(0, 2); // 0 là FlyKick, 1 là Strike
        string attackAnimation = attackType == 0 ? "FlyKick" : "Strike";
        animator.Play(attackAnimation);

        Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
        transform.localScale = directionToEnemy.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isAttacking = false;
    }
}
