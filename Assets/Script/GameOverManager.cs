using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public MonoBehaviour[] playerInputScripts;

    private bool isGameOver = false;

    void Start()
    {
        if (gameOverScreen == null)
        {
            Debug.LogError("Game Over screen reference not assigned in the inspector!");
        }
    }

    public void ShowGameOverScreen()
    {
        if (gameOverScreen != null && !gameOverScreen.activeSelf)
        {
            gameOverScreen.SetActive(true);
            StopBackground();
            isGameOver = true;
        }
    }

    private void StopBackground()
    {
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        ResumeBackground();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
    }

    public void GoToMainMenu()
    {
        ResumeBackground();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }

    private void ResumeBackground()
    {
        Time.timeScale = 1f;
        isGameOver = false;
    }

    void Update()
    {
        if (isGameOver)
        {
            if (Input.anyKeyDown)
            {
                return;
            }
        }
    }
}
