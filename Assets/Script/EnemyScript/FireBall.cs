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
            // Move towards the target enemy
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, target.position) < 0.5f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
