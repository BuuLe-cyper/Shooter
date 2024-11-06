using TMPro;
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

    public void ShowGameOverScreen(string textStatus, string textRetry)
    {
        if (gameOverScreen != null && !gameOverScreen.activeSelf)
        {
            gameOverScreen.SetActive(true);

            GameObject gameStatusObject = GameObject.FindGameObjectWithTag("GameStatus");
            GameObject gameRetryObject = GameObject.FindGameObjectWithTag("Retry");

            if (gameStatusObject != null)
            {
                TextMeshProUGUI gameStatus = gameStatusObject.GetComponent<TextMeshProUGUI>();

                if (gameStatus != null)
                {
                    gameStatus.text = textStatus;
                }
            }
            if (gameRetryObject != null)
            {
                TextMeshProUGUI gameRetry = gameRetryObject.GetComponent<TextMeshProUGUI>();

                if (gameRetry != null)
                {
                    gameRetry.text = textRetry;
                }
            }

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
