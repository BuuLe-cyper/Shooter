using UnityEngine;

public class DragonWarriorAI : MonoBehaviour
{
    public Transform player;
    public GameObject fireBallPrefab; // Prefab of the fireball
    public Transform firePoint; // Where the fireball spawns
    public float attackCooldown = 2f; // Time between attacks
    private float lastAttackTime = 0f;

    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Example of checking if enough time has passed for an attack
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.Play("Dragon_Attack"); // Trigger attack animation

            // Fire the fireball after a delay to sync with animation
            Invoke("ShootFireBall", 0.5f); // Adjust based on animation timing
            lastAttackTime = Time.time;
        }
    }

    void ShootFireBall()
    {
        // Instantiate fireball
        GameObject fireBall = Instantiate(fireBallPrefab, firePoint.position, firePoint.rotation);

        // Get the FireBall component and set the target
        FireBall fireBallScript = fireBall.GetComponent<FireBall>();
        if (fireBallScript != null)
        {
            // Set the nearest enemy or target here
            Transform enemyTarget = FindNearestEnemy(); // Implement this function
            fireBallScript.SetTarget(enemyTarget);
        }

        isAttacking = false;
    }

    // Example of finding the nearest enemy
    Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }
}
