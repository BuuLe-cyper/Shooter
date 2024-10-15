using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 10f; // Tốc độ di chuyển của fireball
    public float attackRange = 10f; // Tầm tấn công của fireball
    private Transform target;
    private Vector3 startPosition; // Vị trí bắt đầu của fireball

    void Start()
    {
        startPosition = transform.position;

        // Nếu target đã được set trước đó, fireball sẽ bay đến mục tiêu
        if (target != null)
        {        }
        else
        {
            target = FindClosestEnemyInRange();
        }

        // Nếu không tìm thấy mục tiêu, hủy fireball
        if (target == null)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Nếu không có mục tiêu, phá hủy fireball
            return;
        }

        // Di chuyển fireball về phía mục tiêu
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Nếu khoảng cách đến mục tiêu nhỏ hơn một giá trị nhất định, thì coi như đã trúng mục tiêu
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            HitTarget();
        }
    }

    // Tìm đối tượng Enemy gần nhất trong tầm tấn công
    Transform FindClosestEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        // Lấy vị trí của fireball
        Vector3 fireballPosition = startPosition; // Bắt đầu tìm từ vị trí bắt đầu của fireball

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(fireballPosition, enemy.transform.position);
            // Chỉ xét các đối tượng trong tầm tấn công
            if (distanceToEnemy < minDistance && distanceToEnemy <= attackRange)
            {
                minDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        // Trả về transform của kẻ địch gần nhất, hoặc null nếu không có kẻ địch trong phạm vi
        if (closestEnemy != null)
        {
            return closestEnemy.transform;
        }

        return null; // Không có kẻ địch trong tầm tấn công
    }

    // Thêm phương thức SetTarget để chỉ định mục tiêu cho fireball
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Hàm xử lý khi fireball trúng mục tiêu
    void HitTarget()
    {
        // Logic khi trúng mục tiêu (có thể thêm hiệu ứng hoặc giảm máu)
        Destroy(gameObject); // Phá hủy fireball sau khi trúng mục tiêu
    }
}
