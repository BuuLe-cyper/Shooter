using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void StartBlinking(float duration, float blinkInterval)
    {
        StartCoroutine(Blink(duration, blinkInterval));
    }

    private IEnumerator Blink(float duration, float blinkInterval)
    {
        float elapsedTime = 0f;

        // Set the color to white
        spriteRenderer.color = Color.white;

        while (elapsedTime < duration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        // Reset the sprite's color to the original color
        spriteRenderer.color = originalColor;
        spriteRenderer.enabled = true;
    }
}
