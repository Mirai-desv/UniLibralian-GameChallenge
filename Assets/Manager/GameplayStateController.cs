using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayStateController : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        Win,
        Lose
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;

    [Header("UI")]
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private GameObject LosePanel;

    private void Awake()
    {
        ResetResultPanels();
    }

    public void ResetGameState()
    {
        SetState(GameState.Playing);
    }

    public void SetState(GameState state)
    {
        CurrentState = state;

        if (WinPanel != null)
        {
            WinPanel.SetActive(state == GameState.Win);
        }

        if (LosePanel != null)
        {
            LosePanel.SetActive(state == GameState.Lose);
        }
    }

// Reset các panel kết quả về trạng thái ban đầu - Tắt các panel
    private void ResetResultPanels()
    {
        if (WinPanel != null)
        {
            WinPanel.SetActive(false);
        }

        if (LosePanel != null)
        {
            LosePanel.SetActive(false);
        }
    }

    // Chơi level tiếp theo nếu có, nếu không thì về Main Menu

    public void OnNextLevelButton()
    {
        if (CurrentState != GameState.Win)
            return;

        if (LevelLoader.Instance != null)
        {
            int currentLevel = LevelLoader.Instance.GetCurrentLevel();
            int nextLevel = currentLevel + 1;

            if (LevelProgressManager.Instance != null &&
                LevelProgressManager.Instance.IsLevelUnlocked(nextLevel))
            {
                LevelLoader.Instance.LoadLevel(nextLevel);
            }
            else
            {
                OnMainMenuButton();
            }
        }
        else
        {
            // Fallback khi test trực tiếp scene, chưa qua Bootstrap/Main Menu
            Debug.LogWarning("LevelLoader.Instance null — fallback dùng SceneManager để test trực tiếp.");

            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
            int nextLevel = currentLevel + 1;
            string nextSceneName = "Level " + (nextLevel + 1);

            if (Application.CanStreamedLevelBeLoaded(nextSceneName))
            {
                PlayerPrefs.SetInt("CurrentLevel", nextLevel);
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy scene '{nextSceneName}' trong Build Settings.");
            }
        }
    }
    
    // Chơi lại Level hiện tại

    public void OnRetryButton()
    {
        if (CurrentState != GameState.Lose)
            return;

        int currentLevel = LevelLoader.Instance != null
            ? LevelLoader.Instance.GetCurrentLevel()
            : PlayerPrefs.GetInt("CurrentLevel", 0);

        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.LoadLevel(currentLevel);
        }
        else
        {
            SceneManager.LoadScene("Level " + (currentLevel + 1));
        }
    }

    //Quay về Main Menu

    public void OnMainMenuButton()
    {
        if (LevelProgressManager.Instance != null)
        {
            LevelProgressManager.Instance.ReturnToMainMenu();
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}