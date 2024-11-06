using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponDamage : MonoBehaviour
{
    public float damage = 10f;
    public bool isDestroyable = true;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            if (isDestroyable)
            {
                Destroy(gameObject);
            }
        }
        else if (collision.collider is CircleCollider2D && collision.gameObject.CompareTag("character") && collision.gameObject.name == "character2")
        {
            if (collision.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
            if (isDestroyable)
            {
                Destroy(gameObject);
            }
        }
    }

}
