using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManagement : MonoBehaviour
{
    public TextMeshProUGUI Point;
    private float pointTotal;
    private float previousScore = -1; // Initial value to ensure the UI updates at start

    void Start()
    {
        // Initialize total score based on Enemy's score at the beginning of the game
        pointTotal = Enemy.totalScore;
        UpdateScoreDisplay();
    }

    void Update()
    {
        // Only update the UI if Enemy.totalScore has changed
        if (Enemy.totalScore != previousScore)
        {
            pointTotal = Enemy.totalScore;
            UpdateScoreDisplay();
            previousScore = Enemy.totalScore;
        }
    }

    void UpdateScoreDisplay()
    {
        // Display the updated score
        Point.text = pointTotal.ToString("F0");
    }

    // Method to add points when the game is won
    public void AddWinPoints(int points)
    {
        // Add the win points directly to Enemy.totalScore
        Enemy.totalScore += points;

        // Immediately update the displayed score
        UpdateScoreDisplay();
    }
}

