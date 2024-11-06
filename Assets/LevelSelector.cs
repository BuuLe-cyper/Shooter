using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public int level;                          // The level number (Level 1, Level 2, etc.)
    public GameObject lockImage;               // Reference to the lock image for the level
    public Button levelButton;                 // Reference to the level button

    void Start()
    {
        // Get the player's progress from PlayerPrefs
        int level1Completed = PlayerPrefs.GetInt("Level1Completed", 0);  // Default to 0 if not set

        // Check if the level is locked
        if (level == 2 && level1Completed == 0)
        {
            // Display the lock image and disable the button if Level 2 is locked
            lockImage.SetActive(true);
            levelButton.interactable = false; // Disable the button for Level 2
        }
        else if (level == 3 && level1Completed == 0)
        {
            // Display the lock image and disable the button for Level 3 if Level 1 isn't completed
            lockImage.SetActive(true);
            levelButton.interactable = false; // Disable the button for Level 3
        }
        else
        {
            // Otherwise, hide the lock image and enable the button for this level
            lockImage.SetActive(false);
            levelButton.interactable = true;  // Enable the button for this level
        }
    }

    public void OpenScene()
    {
        // Check if the level is accessible
        if (level == 2 || (level == 3 && PlayerPrefs.GetInt("Level1Completed", 0) == 1))
        {
            // Level 1 or Level 2 (if Level 1 completed)
            if (level == 2)
            {
                PlayerPrefs.SetString("LoadScene", "ShooterScenes");
            }
            else if (level == 3)
            {
                PlayerPrefs.SetString("LoadScene", "Level 2");
            }
            SceneManager.LoadScene("CharScene");
        }
        
    }
}
