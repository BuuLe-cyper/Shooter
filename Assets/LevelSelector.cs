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

        // Check if the level is locked
        if (level == 2 && level1Completed == 0)
        {
            lockImage.SetActive(true);
            levelButton.interactable = false;
        } 
        else
        {
            lockImage.SetActive(false);
            levelButton.interactable = true;
        }
    }

    public void OpenScene()
    {
        // Check if the level is accessible
        if (level == 1 || (level == 2 && PlayerPrefs.GetInt("Level1Completed", 0) == 1))
        {
            // Level 1 or Level 2 (if Level 1 completed)
            if (level == 1)
            {
                PlayerPrefs.SetString("LoadScene", "ShooterScenes");
            }
            else if (level == 2)
            {
                PlayerPrefs.SetString("LoadScene", "Level 2");
            }
            SceneManager.LoadScene("CharScene");
        }
        
    }
}
