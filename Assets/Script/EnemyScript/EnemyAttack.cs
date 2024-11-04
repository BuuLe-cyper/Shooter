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
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        while (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("character");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("Player found!");
            }
            else
            {
                Debug.LogWarning("Waiting for player to be assigned...");
            }

            // Wait for a short time before checking again
            yield return new WaitForSeconds(0.5f);
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

            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

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
