using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public GameOverManager gameOverManager; // Reference to the GameOverManager
    private PlayerHealth playerHealth; // Reference to the PlayerHealth component
    private ScoreManagement scoreManagement;
    private AudioManager audioManager; // Reference to AudioManager


    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        scoreManagement = FindObjectOfType<ScoreManagement>();
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
            gameOverManager.ShowGameOverScreen("Game Over", "Try Again");
            audioManager.PlaySFX(audioManager.GameOver);
        }
    }
    public void GameWin()
    {
        audioManager.PlaySFX(audioManager.Victory);
        scoreManagement.AddWinPoints(1000);
        gameOverManager.ShowGameOverScreen("Victory", "Play Again");
        PlayerPrefs.SetInt("Level1Completed", 1);

    }
}
