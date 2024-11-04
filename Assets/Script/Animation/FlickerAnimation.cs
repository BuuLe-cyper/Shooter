using System.Collections;
using UnityEngine;

public class FlickerAnimation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    public float flickerDuration = 0.5f; // Duration of the flicker effect
    public int flickerCount = 10; // Number of flickers

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component if not assigned
        }
    }

    public void StartFlicker()
    {
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        for (int i = 0; i < flickerCount; i++)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Toggle visibility
            yield return new WaitForSeconds(flickerDuration / (flickerCount * 2)); // Wait before the next flicker
        }

        spriteRenderer.enabled = true; // Ensure the sprite is visible after flickering
    }
}