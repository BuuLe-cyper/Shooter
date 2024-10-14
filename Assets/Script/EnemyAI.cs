using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Seeker Seeker; // Seeker component for pathfinding
    public Transform Target; // Target the player
    private Path Path;
    private Coroutine MoveCoroutine;
    public float MoveSpeed = 2f;
    public float NextWPDistance = 0.5f; // How close the enemy should get to a waypoint before moving to the next one
    public float RepathRate = 0.5f; // How often to recalculate the path
    public SpriteRenderer CharacterSR;

    public Animator animator;

    public float ReachDistance = 1.5f;

    private void Start()
    {
        // Find the player
        GameObject player = GameObject.Find("character");
        if (player != null)
        {
            Target = player.transform;

            // Recalculate the path at regular intervals
            InvokeRepeating(nameof(CalculatePath), 0f, RepathRate);
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }

    void CalculatePath()
    {
        // Check if the seeker is available for a new path
        if (Seeker.IsDone())
        {
            Seeker.StartPath(transform.position, Target.position, OnPathCallback);
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
                CharacterSR.transform.localScale = new Vector3(Mathf.Sign(direction.x) * Mathf.Abs(CharacterSR.transform.localScale.x), CharacterSR.transform.localScale.y, CharacterSR.transform.localScale.z);
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
