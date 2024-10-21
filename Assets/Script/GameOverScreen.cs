using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    private GameOverManager gameOverManager;

    void Start()
    {
        gameOverManager = FindObjectOfType<GameOverManager>();
    }

    public void RestartButton()
    {
        if (gameOverManager != null)
        {
            gameOverManager.RestartGame(); // Call restart method from GameOverManager
        }
    }

    public void MainMenuButton()
    {
        if (gameOverManager != null)
        {
            gameOverManager.GoToMainMenu(); // Call main menu method from GameOverManager
        }
    }
}
