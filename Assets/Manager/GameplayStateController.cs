using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

    // WIN ANIMATION
    [Header("Win Animation")]
    [SerializeField] private CanvasGroup winPanelCanvasGroup;

    [SerializeField] private RectTransform winTitle;
    [SerializeField] private RectTransform nextButton;
    [SerializeField] private RectTransform mainMenuButton;

    [SerializeField] private float winFadeDuration = 0.25f;
    [SerializeField] private float winPopupDuration = 0.4f;


    // LOSE ANIMATION
    [Header("Lose Animation")]
    [SerializeField] private CanvasGroup losePanelCanvasGroup;

    [SerializeField] private RectTransform loseContent;

    [SerializeField] private float loseFadeDuration = 0.25f;
    [SerializeField] private float losePopupDuration = 0.4f;


    private Sequence winAnimationSequence;
    private Sequence loseAnimationSequence;



    private void Awake()
    {
        ResetResultPanels();
    }


    // GAME STATE
    public void ResetGameState()
    {
        SetState(GameState.Playing);
    }


    public void SetState(GameState state)
    {
        CurrentState = state;

        // WIN
        if (WinPanel != null)
        {
            if (state == GameState.Win)
            {
                ShowWinPanel();
            }
            else
            {
                HideWinPanel();
            }
        }


        // LOSE
        if (LosePanel != null)
        {
            if (state == GameState.Lose)
            {
                ShowLosePanel();
            }
            else
            {
                HideLosePanel();
            }
        }
    }


    // WIN PANEL
    private void ShowWinPanel()
    {
        if (WinPanel == null)
            return;

        WinPanel.SetActive(true);

        if (winAnimationSequence != null)
        {
            winAnimationSequence.Kill();
        }


        // Reset visual

        if (winPanelCanvasGroup != null)
        {
            winPanelCanvasGroup.alpha = 0f;
        }

        if (winTitle != null)
        {
            winTitle.localScale = Vector3.one * 0.7f;
        }

        if (nextButton != null)
        {
            nextButton.localScale = Vector3.one * 0.8f;
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.localScale = Vector3.one * 0.8f;
        }


        // Create sequence

        winAnimationSequence = DOTween.Sequence();


        // Background fade

        if (winPanelCanvasGroup != null)
        {
            winAnimationSequence.Join(
                winPanelCanvasGroup
                    .DOFade(1f, winFadeDuration)
                    .SetEase(Ease.OutQuad)
            );
        }


        // Title

        if (winTitle != null)
        {
            winAnimationSequence.Join(
                winTitle
                    .DOScale(Vector3.one, winPopupDuration)
                    .SetEase(Ease.OutBack)
            );
        }


        // Next Button

        if (nextButton != null)
        {
            winAnimationSequence.Insert(
                0.1f,
                nextButton
                    .DOScale(Vector3.one, winPopupDuration)
                    .SetEase(Ease.OutBack)
            );
        }


        // Main Menu Button

        if (mainMenuButton != null)
        {
            winAnimationSequence.Insert(
                0.15f,
                mainMenuButton
                    .DOScale(Vector3.one, winPopupDuration)
                    .SetEase(Ease.OutBack)
            );
        }
    }


    private void HideWinPanel()
    {
        if (WinPanel == null)
            return;

        if (winAnimationSequence != null)
        {
            winAnimationSequence.Kill();
            winAnimationSequence = null;
        }

        WinPanel.SetActive(false);

        ResetWinVisual();
    }


    private void ResetWinVisual()
    {
        if (winPanelCanvasGroup != null)
        {
            winPanelCanvasGroup.alpha = 0f;
        }

        if (winTitle != null)
        {
            winTitle.localScale = Vector3.one * 0.7f;
        }

        if (nextButton != null)
        {
            nextButton.localScale = Vector3.one * 0.8f;
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.localScale = Vector3.one * 0.8f;
        }
    }


    // LOSE PANEL
    private void ShowLosePanel()
    {
        if (LosePanel == null)
            return;

        LosePanel.SetActive(true);

        if (loseAnimationSequence != null)
        {
            loseAnimationSequence.Kill();
        }


        // Reset visual

        if (losePanelCanvasGroup != null)
        {
            losePanelCanvasGroup.alpha = 0f;
        }

        if (loseContent != null)
        {
            loseContent.localScale = Vector3.one * 0.8f;
        }


        // Create sequence

        loseAnimationSequence = DOTween.Sequence();


        // Background fade

        if (losePanelCanvasGroup != null)
        {
            loseAnimationSequence.Join(
                losePanelCanvasGroup
                    .DOFade(1f, loseFadeDuration)
                    .SetEase(Ease.OutQuad)
            );
        }


        // Lose content popup

        if (loseContent != null)
        {
            loseAnimationSequence.Join(
                loseContent
                    .DOScale(Vector3.one, losePopupDuration)
                    .SetEase(Ease.OutBack)
            );
        }
    }


    private void HideLosePanel()
    {
        if (LosePanel == null)
            return;

        if (loseAnimationSequence != null)
        {
            loseAnimationSequence.Kill();
            loseAnimationSequence = null;
        }

        LosePanel.SetActive(false);

        ResetLoseVisual();
    }


    private void ResetLoseVisual()
    {
        if (losePanelCanvasGroup != null)
        {
            losePanelCanvasGroup.alpha = 0f;
        }

        if (loseContent != null)
        {
            loseContent.localScale = Vector3.one * 0.8f;
        }
    }


    // RESET UI
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

        ResetWinVisual();
        ResetLoseVisual();
    }


    // NEXT LEVEL
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
            Debug.LogWarning(
                "LevelLoader.Instance null — fallback dùng SceneManager để test trực tiếp."
            );

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
                Debug.LogWarning(
                    $"Không tìm thấy scene '{nextSceneName}' trong Build Settings."
                );
            }
        }
    }


    // RETRY
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


    // MAIN MENU
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


    // CLEANUP
    private void OnDestroy()
    {
        if (winAnimationSequence != null)
        {
            winAnimationSequence.Kill();
        }

        if (loseAnimationSequence != null)
        {
            loseAnimationSequence.Kill();
        }
    }
}