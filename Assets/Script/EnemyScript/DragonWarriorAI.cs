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
    public float repathRate = 0.5f;
    public float nextWPDistance = 0.5f;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    private DragonDamage dragonDamage;

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

            // Nếu ở trong khoảng cách theo dõi và ngoài khoảng cách tối thiểu
            if (distanceToTarget > minFollowDistance && distanceToTarget <= followDistance)
            {
                if (path == null || path.vectorPath == null)
                    return;

                // Kiểm tra xem đã đến waypoint cuối cùng chưa
                if (currentWaypoint >= path.vectorPath.Count)
                {
                    if (!reachedEndOfPath) // Chỉ khi vừa đến cuối đường dẫn
                    {
                        reachedEndOfPath = true;
                        SetIdleAnimation(); // Chuyển sang idle
                    }
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
                // Nếu tốc độ gần bằng 0, dừng việc cập nhật đường đi và chuyển sang idle
                if (reachedEndOfPath || GetCurrentMovementSpeed() < 0.01f)
                {
                    reachedEndOfPath = true;
                    SetIdleAnimation();
                }
            }
        }
        else if (!isAttacking)
        {
            if (!reachedEndOfPath) // Đảm bảo không gọi SetIdleAnimation liên tục
            {
                reachedEndOfPath = true;
                SetIdleAnimation();
            }
        }
    }

    // Hàm tính toán tốc độ hiện tại của DragonWarrior
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
        float moveSpeed = direction.magnitude * movementSpeed; // Tính toán tốc độ di chuyển dựa trên hướng
        Vector3 force = direction * moveSpeed * Time.deltaTime; // Áp dụng tốc độ di chuyển vào lực

        transform.position += force; // Di chuyển DragonWarrior

        // Kiểm tra nếu khoảng cách tới waypoint hiện tại nhỏ hơn khoảng cách waypoint tiếp theo
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWPDistance)
        {
            currentWaypoint++;
        }

        // Kiểm tra tốc độ di chuyển có đủ lớn để chuyển animation sang trạng thái "Walk"
        if (moveSpeed >= 0.001f)
        {
            // Thiết lập hướng di chuyển dựa trên hướng đi
            transform.localScale = direction.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            animator.Play("Walk"); // Phát animation đi bộ
        }
        else
        {
            SetIdleAnimation(); // Nếu tốc độ quá thấp, phát animation "Idle"
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

        // Kiểm tra xem DragonWarrior có đang di chuyển hay không
        if (GetCurrentMovementSpeed() < 0.01f)
        {
            SetIdleAnimation(); // Chuyển về Idle nếu không còn di chuyển
        }
    }

    private IEnumerator PerformCloseRangeAttack(GameObject enemy)
    {
        isAttacking = true;

        int attackType = Random.Range(0, 2); // 0 là FlyKick, 1 là Strike
        string attackAnimation = attackType == 0 ? "FlyKick" : "Strike";

        dragonDamage.damage = attackType == 0 ? 20f : 30f; // Thiết lập sát thương dựa vào kiểu tấn công

        animator.Play(attackAnimation);

        Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;

        transform.localScale = directionToEnemy.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

        float attackMoveSpeed = movementSpeed * 1.5f; // Tốc độ lao vào có thể nhanh hơn di chuyển bình thường
        float attackDuration = animator.GetCurrentAnimatorStateInfo(0).length;

        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            float step = attackMoveSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position, step);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        isAttacking = false;

        // Kiểm tra xem DragonWarrior có đang di chuyển hay không
        if (GetCurrentMovementSpeed() < 0.01f)
        {
            SetIdleAnimation(); // Chuyển về Idle nếu không còn di chuyển
        }
    }
}
