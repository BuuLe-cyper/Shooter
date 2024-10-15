using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Transform target;
    public float attackDistance = 5f;
    public Animator animator;
    public AudioManager audioManager;
    public string type;


    private bool isAttacking = false;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
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

        // Make the enemy face the target
        if (distanceToTarget <= attackDistance)
        {
            // Calculate the direction to the target
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Create a rotation that looks in the direction of the target
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            // Smoothly rotate towards the target
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Adjust speed as needed

            if (!isAttacking)
            {
                animator.SetBool("Attack", true);
                isAttacking = true;
                audioManager.Audio(type);
            }
        }
        else if (distanceToTarget > attackDistance && isAttacking)
        {
            animator.SetBool("Attack", false);
            isAttacking = false;
        }
    }
}
