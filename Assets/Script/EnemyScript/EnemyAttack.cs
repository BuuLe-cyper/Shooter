using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Transform target;
    public float attackDistance = 5f;
    public Animator animator;

    private bool isAttacking = false;

    private void Start()
    {
        GameObject player = GameObject.Find("character");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackDistance && !isAttacking)
        {
            animator.SetBool("Attack", true);
            isAttacking = true;
        }
        else if (distanceToTarget > attackDistance && isAttacking)
        {
            animator.SetBool("Attack", false);
            isAttacking = false;
        }
    }
}
