using UnityEngine;

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

        int currentLevel = LevelLoader.Instance.GetCurrentLevel();
        int nextLevel = currentLevel + 1;

        // Kiểm tra xem còn level tiếp theo không
        if (LevelProgressManager.Instance != null &&
            LevelProgressManager.Instance.IsLevelUnlocked(nextLevel))
        {
            LevelLoader.Instance.LoadLevel(nextLevel);
        }
        else
        {
            // Nếu đã hết level thì về Main Menu
            OnMainMenuButton();
        }
    }

    // Chơi lại Level hiện tại

    public void OnRetryButton()
    {
        if (CurrentState != GameState.Lose)
            return;

        int currentLevel = LevelLoader.Instance.GetCurrentLevel();

        LevelLoader.Instance.LoadLevel(currentLevel);
    }

    //Quay về Main Menu

    public void OnMainMenuButton()
    {
        if (LevelProgressManager.Instance != null)
        {
            LevelProgressManager.Instance.ReturnToMainMenu();
        }
    }
}