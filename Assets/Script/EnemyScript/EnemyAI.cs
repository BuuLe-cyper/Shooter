using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Seeker Seeker;
    public Transform Target;
    private Path Path;
    private Coroutine MoveCoroutine;
    public float MoveSpeed = 2f;
    public float NextWPDistance = 0.5f;
    public float RepathRate = 0.5f;
    public SpriteRenderer CharacterSR;

    public Animator animator;

    public float ReachDistance = 1.5f;

    private void Start()
    {
        // Start coroutine to find the player
        StartCoroutine(FindPlayerCoroutine());
    }

    private IEnumerator FindPlayerCoroutine()
    {
        GameObject player = null;

        // Loop until the player is found
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("character");

            if (player != null)
            {
                Target = player.transform;

                // Recalculate the path at regular intervals
                InvokeRepeating(nameof(CalculatePath), 0f, RepathRate);
                Debug.Log("Player found!");
            }
            else
            {
                Debug.LogWarning("Player not found! Waiting for player to be assigned...");
            }

            // Wait for a short period before checking again
            yield return new WaitForSeconds(0.5f);
        }
    }

    void KillEnemy()
    {
        animator.Play("Death");
    }
    void CalculatePath()
    {
        // Check if the Seeker is available for a new path
        if (Seeker != null && Seeker.IsDone())
        {
            if (Target != null) // Check if Target is not null
            {
                Seeker.StartPath(transform.position, Target.position, OnPathCallback);
            }
            else
            {
                Debug.LogWarning("Target is null. Cannot calculate path.");
            }
        }
        else if (Seeker == null)
        {
            Debug.LogWarning("Seeker is not assigned or has been destroyed.");
        }
    }


    void OnPathCallback(Path path)
    {
        if (path.error) return;
        Path = path;

        // Start moving towards the target
        MoveToTarget();
    }

    void MoveToTarget()
    {
        if (MoveCoroutine != null)
        {
            StopCoroutine(MoveCoroutine);
        }
        MoveCoroutine = StartCoroutine(MoveToTargetCoroutine());
    }

    IEnumerator MoveToTargetCoroutine()
    {
        int currentWP = 0;

        while (currentWP < Path.vectorPath.Count)
        {
            // Check if Target is not null
            if (Target == null)
            {
                Debug.LogWarning("Target has been destroyed or is not assigned.");
                yield break; // Exit the coroutine if the target is no longer valid
            }

            float distanceToTarget = Vector2.Distance(transform.position, Target.position);

            if (distanceToTarget <= ReachDistance)
            {
                UpdateAnimatorSpeed(0);
                yield break;
            }

            // Move toward the next waypoint in the path
            Vector2 direction = ((Vector2)Path.vectorPath[currentWP] - (Vector2)transform.position).normalized;
            Vector3 force = direction * MoveSpeed * Time.deltaTime;
            transform.position += force;

            UpdateAnimatorSpeed(force.magnitude);

            float distance = Vector2.Distance(transform.position, Path.vectorPath[currentWP]);
            if (distance < NextWPDistance)
            {
                currentWP++;
            }

            if (direction.x != 0)
            {
                if (CharacterSR != null) // Check if CharacterSR is not null
                {
                    CharacterSR.transform.localScale = new Vector3(Mathf.Sign(direction.x) * Mathf.Abs(CharacterSR.transform.localScale.x), CharacterSR.transform.localScale.y, CharacterSR.transform.localScale.z);
                }
            }

            // Check if the script's transform is valid before yielding
            if (this == null)
            {
                yield break; // Exit the coroutine if the script instance is no longer valid
            }

            yield return null;
        }


        UpdateAnimatorSpeed(0);
    }

    private void UpdateAnimatorSpeed(float speed)
    {
        if (animator != null && animator.parameters != null)
        {
            foreach (var param in animator.parameters)
            {
                if (param.name == "Speed" && param.type == AnimatorControllerParameterType.Float)
                {
                    animator.SetFloat("Speed", speed);
                    return;
                }
            }
        }
    }
}
