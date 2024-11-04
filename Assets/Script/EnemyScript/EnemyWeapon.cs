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
        else if (collision.gameObject.tag == "character" && collision.gameObject.name == "character2")
        {
            CircleCollider2D circleCollider = collision.gameObject.GetComponent<CircleCollider2D>();

            if (circleCollider != null)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    Debug.Log(damage);
                    playerHealth.TakeDamage(damage);
                }
            }
            if (isDestroyable)
            {
                Destroy(gameObject);
            }
        }
    }

}
