using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 5f; // Speed of the FireBall
    public float lifetime = 2f;
    public float destroyDelay = 2f;

    private Transform target; // Target enemy to follow

    public void Initialize(Transform target)
    {
        this.target = target;
        Destroy(gameObject, lifetime); // Destroy the FireBall after its lifetime
    }

    void Update()
    {
        if (target != null)
        {
            // Di chuyển về phía kẻ địch
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Tính toán góc quay cho FireBall chỉ theo trục X và Y
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
