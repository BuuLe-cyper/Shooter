using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 2f;
    public float destroyDelay = 2f;

    private Transform target;

    public void Initialize(Transform target)
    {
        this.target = target;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target != null)
        {
            // Target Enemy
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Caculate path
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
