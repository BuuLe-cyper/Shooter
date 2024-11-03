using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GameObject bullet;
    public float bulletSpeed;
    public float timeBtwFire;
    private float fireCoolDown;
    public float ShootDistance; // Distance at which the enemy will shoot

    void Start()
    {
        fireCoolDown = timeBtwFire; // Initialize the cooldown
    }

    void Update()
    {
        fireCoolDown -= Time.deltaTime; // Subtract time correctly using deltaTime
        GameObject player = GameObject.FindGameObjectWithTag("character");

        if (fireCoolDown <= 0 && player != null)
        {
            Vector3 playerPos = player.transform.position;
            float distanceToPlayer = Vector2.Distance(transform.position, playerPos); // Calculate distance to player

            // If player is within shooting range, fire a bullet
            if (distanceToPlayer <= ShootDistance)
            {
                fireCoolDown = timeBtwFire; // Reset cooldown after firing
                EnemyFireBullet();
            }
        }
    }

    void EnemyFireBullet()
    {
        var bulletTmp = Instantiate(bullet, transform.position, Quaternion.identity);
        Rigidbody2D rb = bulletTmp.GetComponent<Rigidbody2D>();

        // Disable gravity on the bullet to prevent it from falling
        rb.gravityScale = 0;

        // Get the player's position
        GameObject player = GameObject.FindGameObjectWithTag("character");
        if (player != null)
        {
            Vector3 playerPos = player.transform.position;
            Vector3 direction = playerPos - transform.position; // Get direction to the player

            // Apply force in the direction of the player
            rb.AddForce(direction.normalized * bulletSpeed, ForceMode2D.Impulse);
        }
    }
}
