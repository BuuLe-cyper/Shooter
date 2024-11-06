using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManagement : MonoBehaviour
{
    public TextMeshProUGUI Point;
    private float pointTotal;
    private float previousScore = 0; // Initial value to ensure the UI updates at start

    void Start()
    {
        // Initialize total score based on Enemy's score at the beginning of the game
        pointTotal = 0;
        UpdateScoreDisplay();
    }

    void Update()
    {
        // Only update the UI if Enemy.totalScore has changed
        if (pointTotal != previousScore)
        {
            UpdateScoreDisplay();
            previousScore = pointTotal;
        }
    }

    public void AddPoint(float point)
    {
        pointTotal += point;
    }

    public void ResetPoint()
    {
        pointTotal = 0;
    }

    void UpdateScoreDisplay()
    {
        // Display the updated score
        Point.text = pointTotal.ToString("F0");
    }

    // Method to add points when the game is won
    public void AddWinPoints(int points)
    {
        // Add the win points to the current point in CharacterStats
        CharacterStatsManager.CurrentStats.point.SetValue(CharacterStatsManager.CurrentStats.point.GetValue() + points);

        // Immediately update the displayed score
        UpdateScoreDisplay();

        // Save the updated character stats
        CharacterStatsManager.SaveCharacterStats(CharacterStatsManager.CurrentStats);
    }

    public void AddPointsToCurrentStats()
    {
        // Add the current pointTotal (Score) to the saved stats
        CharacterStatsManager.CurrentStats.point.SetValue(CharacterStatsManager.CurrentStats.point.GetValue() + (int)pointTotal);

        // Save the updated character stats
        CharacterStatsManager.SaveCharacterStats(CharacterStatsManager.CurrentStats);
    }

    public float GetTotalPoint()
    {
        return pointTotal;
    }
}

