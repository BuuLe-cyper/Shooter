using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeaponDamage : MonoBehaviour
{
    public float damage = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }

        }
    }

    public void BoostDamage(float boostDamage)
    {
        damage += boostDamage;
    }

}
