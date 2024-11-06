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
        int level1Completed = PlayerPrefs.GetInt("Level1Completed", 0);

        // Check if the level is locked (i.e., Level 2 should be locked if Level 1 is not completed)
        if (level == 3 && level1Completed == 0)
        {
            // Display the lock image and disable the button if Level 2 is locked
            lockImage.SetActive(true);
            levelButton.interactable = false; // Disable the button for Level 2
        }
        else
        {
            // Otherwise, hide the lock image and enable the button
            lockImage.SetActive(false);
            levelButton.interactable = true;  // Enable the button for this level
        }
    }

    public void OpenScene()
    {
        // Log the level to check which scene is being requested
        Debug.Log("Opening Scene: Level " + level.ToString());

        // Check if the level is accessible
        if (level == 2 || (level == 3 && PlayerPrefs.GetInt("Level1Completed", 0) == 1))
        {
            if (level == 2)
            {
                SceneManager.LoadScene("ShooterScenes");
            }
            else if (level == 3)
            {
                SceneManager.LoadScene("Level 2");
            }
        }

    }
}
