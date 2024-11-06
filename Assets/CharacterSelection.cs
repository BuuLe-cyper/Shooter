using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class CharacterSelection : MonoBehaviour
{
    public GameObject character;  // Reference to the character GameObject you want to enable
    private string LoadScene;
    public void Start()
    {
        LoadScene = PlayerPrefs.GetString("LoadScene");
    }
    public void LoadLevel()
    {

        // Load the gameplay scene (Level 1 or Level 2)
        SceneManager.LoadScene(LoadScene);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnEnable()
    {
        // Subscribe to the event triggered when a new scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe when the object is disabled to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == LoadScene)
        {
            // Lấy tất cả root GameObjects trong scene hiện tại
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                // Kiểm tra nếu tên của đối tượng là "Player" hoặc "character2"
                if (obj.name == "Player")
                {
                    // Nếu character.name là "Player", enable Player và disable character2
                    if (character.name == "Player")
                    {
                        obj.SetActive(true); // Set Player active
                    }
                    else
                    {
                        obj.SetActive(false); // Set Player inactive
                    }
                }
                else if (obj.name == "character2")
                {
                    // Nếu character.name là "character2", enable character2 và disable Player
                    if (character.name == "character2")
                    {
                        obj.SetActive(true); // Set character2 active
                    }
                    else
                    {
                        obj.SetActive(false); // Set character2 inactive
                    }
                }
            }
        }

    }
}
