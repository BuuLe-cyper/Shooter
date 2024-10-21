using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public GameOverManager gameOverManager; // Reference to the GameOverManager
    private PlayerHealth playerHealth; // Reference to the PlayerHealth component

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found in the scene!");
        }

        if (gameOverManager == null)
        {
            Debug.LogError("GameOverManager component not assigned in the inspector!");
        }
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.health <= 0)
        {
            gameOverManager.ShowGameOverScreen(); // Show the Game Over screen
        }
    }
}
