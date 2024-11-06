using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Awake()
    {
        TrackAppStart();
    }

    private void TrackAppStart()
    {
        // Track application start time and other info
        PlayerPrefs.SetInt("AppStarted", 1);
        PlayerPrefs.SetString("LastStartTime", System.DateTime.Now.ToString());
        PlayerPrefs.Save();

        // Optional: Check if it's the first time launching
        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            PlayerPrefs.SetInt("FirstLaunch", 1);
            Debug.Log("This is the first launch.");
            // Handle any first-launch-specific logic here
        }

        Debug.Log("Application has started and loaded the main menu.");

        // Initialize and load character stats
        CharacterStatsManager.InitializeCharacterStats();

        // Optionally, print out the loaded stats to confirm they are loaded
        Debug.Log("Character stats loaded:");
        Debug.Log($"Attack: {CharacterStatsManager.CurrentStats.attack.GetValue()}");
        Debug.Log($"Health: {CharacterStatsManager.CurrentStats.health.GetValue()}");
        Debug.Log($"Speed: {CharacterStatsManager.CurrentStats.speed.GetValue()}");
        Debug.Log($"Point: {CharacterStatsManager.CurrentStats.point.GetValue()}");
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
