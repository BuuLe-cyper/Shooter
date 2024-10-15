using System.Collections;
using UnityEngine;

public class DragonWarriorAI : MonoBehaviour
{
    public Transform target; // Đối tượng để theo dõi
    public float followDistance = 5f; // Khoảng cách tối đa để theo dõi
    public float attackDistance = 5f; // Khoảng cách để tấn công kẻ thù
    public float movementSpeed = 3f; // Tốc độ di chuyển của DragonWarrior
    public float minFollowDistance = 3f; // Khoảng cách tối thiểu để duy trì từ đối tượng

    public GameObject fireBallPrefab; // Prefab FireBall để khởi tạo
    public Animator animator; // Animator cho DragonWarrior

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
        }
    }

    private void Update()
    {
        if (target != null && !isAttacking) // Kiểm tra không đang tấn công
        {
            // Tính khoảng cách đến mục tiêu
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Tìm kiếm kẻ thù gần nhất để tấn công
            GameObject nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, nearestEnemy.transform.position);

                // Tấn công nếu trong khoảng cách tấn công và cooldown đã hết
                if (distanceToEnemy <= attackDistance && Time.time - lastAttackTime >= attackCooldown)
                {
                    AttackEnemy(nearestEnemy);
                    lastAttackTime = Time.time;
                    return; // Thoát khỏi Update để tránh theo dõi mục tiêu
                }
            }

            // Theo dõi đối tượng nếu trong khoảng cách theo dõi và ngoài khoảng cách tối thiểu
            if (distanceToTarget > minFollowDistance && distanceToTarget <= followDistance)
            {
                MoveTowardsTarget();
            }
            else
            {
                SetIdleAnimation(); // Đặt trạng thái nghỉ nếu ra ngoài khoảng cách theo dõi
            }
        }
        else
        {
            SetIdleAnimation();
        }
    }

    private void MoveTowardsTarget()
    {
        // Tính toán hướng đến mục tiêu
        Vector3 direction = (target.position - transform.position).normalized;

        // Di chuyển nhân vật lên, xuống và sang trái, phải
        Vector3 newPosition = transform.position + direction * movementSpeed * Time.deltaTime;
        transform.position = newPosition; // Cập nhật vị trí của DragonWarrior

        // Chỉ thay đổi hướng nhìn khi có chuyển động ngang
        if (Mathf.Abs(direction.x) > 0.01f) // Kiểm tra xem có chuyển động ngang không
        {
            if (direction.x > 0)
            {
                // Nhìn sang phải
                transform.localScale = new Vector3(1, 1, 1); // Nhìn mặc định
            }
            else if (direction.x < 0)
            {
                // Nhìn sang trái
                transform.localScale = new Vector3(-1, 1, 1); // Lật lại để nhìn sang trái
            }
        }

        animator.Play("Walk"); // Chơi hoạt ảnh đi bộ
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

    private void AttackEnemy(GameObject enemy)
    {
        isAttacking = true; // Đặt trạng thái tấn công

        animator.Play("Atack"); // Chơi hoạt ảnh tấn công

        // Tính toán hướng đến kẻ thù
        Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;

        // Cập nhật hướng nhìn của DragonWarrior dựa trên vị trí của kẻ thù
        if (directionToEnemy.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Nhìn sang phải
        }
        else if (directionToEnemy.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Nhìn sang trái
        }

        StartCoroutine(InstantiateFireBall(enemy)); // Bắt đầu coroutine để tạo fireball
        StartCoroutine(WaitForAttackAnimation()); // Đợi cho hoạt ảnh tấn công hoàn tất
    }

    private IEnumerator WaitForAttackAnimation()
    {
        // Đợi cho hoạt ảnh tấn công hoàn tất
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isAttacking = false; // Kết thúc trạng thái tấn công
    }

    private IEnumerator InstantiateFireBall(GameObject enemy)
    {
        yield return new WaitForSeconds(0.5f); // Delay before firing

        // Instantiate the FireBall
        GameObject fireBall = Instantiate(fireBallPrefab, transform.position, Quaternion.identity);

        // Rotate fireball towards the enemy and fix the Z-axis
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        Quaternion fireBallRotation = Quaternion.LookRotation(Vector3.forward, direction);
        fireBall.transform.rotation = Quaternion.Euler(0, 0, fireBallRotation.eulerAngles.z); // Fix Z-axis rotation

        fireBall.GetComponent<FireBall>().Initialize(enemy.transform);
    }
}
