using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject PausePanel;

    private bool isPaused = false;

    private void Start()
    {
        ResumeGame();
    }

    public void PauseGame()
    {
        isPaused = true;

        if (PausePanel != null)
            PausePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (PausePanel != null)
            PausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // Chơi lại Level hiện tại
    public void RetryLevel()
    {
        // Luôn trả Time.timeScale về bình thường trước khi load scene
        Time.timeScale = 1f;

        if (LevelLoader.Instance != null)
        {
            int currentLevel = LevelLoader.Instance.GetCurrentLevel();
            LevelLoader.Instance.LoadLevel(currentLevel);
        }
        else
        {
            // Fallback nếu test trực tiếp scene
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
            string currentSceneName = "Level " + (currentLevel + 1);

            SceneManager.LoadScene(currentSceneName);
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Main Menu");
    }

    public void OpenSettings()
    {
        Time.timeScale = 1f;

        SceneNavigator.previousScene = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene("Setting");
    }
}