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