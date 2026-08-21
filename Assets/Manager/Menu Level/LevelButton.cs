using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private int levelIndex;

    [Header("UI")]
    [SerializeField] private Image background;
    [SerializeField] private Button button;

    [Header("Sprites")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;

    private void Start()
    {
        RefreshState();
    }

    public void SetLevelIndex(int index)
    {
        levelIndex = index;
        RefreshState();
    }

    public void SetSprites(Sprite locked, Sprite unlocked)
    {
        lockedSprite = locked;
        unlockedSprite = unlocked;
    }

    public void RefreshState()
    {
        if (LevelProgressManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy LevelProgressManager.");
            return;
        }

        bool isUnlocked = LevelProgressManager.Instance.IsLevelUnlocked(levelIndex);

        if (background != null)
        {
            background.sprite = isUnlocked ? unlockedSprite : lockedSprite;
        }

        if (button != null)
        {
            button.interactable = isUnlocked;
        }
    }

    public void OnLevelButtonClicked()
    {
        if (LevelProgressManager.Instance != null &&
            !LevelProgressManager.Instance.IsLevelUnlocked(levelIndex))
        {
            Debug.Log("Level chưa được mở khóa.");
            return;
        }

        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.LoadLevel(levelIndex);
        }
        else
        {
            SceneManager.LoadScene("Level " + (levelIndex + 1));
        }
    }
}