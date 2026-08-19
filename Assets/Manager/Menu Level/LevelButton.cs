using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private int levelIndex;

    [Header("UI")]
    [SerializeField] private Image background;
    [SerializeField] private Button button;

    [Header("Colors")]
    [SerializeField] private Color notCompletedColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color completedColor = Color.green;

    private void Start()
    {
        RefreshState();
    }

    public void SetLevelIndex(int index)
    {
        levelIndex = index;
        RefreshState();
    }

    public void RefreshState()
    {
        if (LevelProgressManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy LevelProgressManager.");
            return;
        }

        int highestUnlocked = 
            LevelProgressManager.Instance.HighestUnlockedLevel();

        // Đã hoàn thành
        if (levelIndex < highestUnlocked)
        {
            background.color = completedColor;
            button.interactable = true;
        }
        // Đã mở nhưng chưa hoàn thành
        else if (levelIndex == highestUnlocked)
        {
            background.color = notCompletedColor;
            button.interactable = true;
        }
        // Chưa mở khóa
        else
        {
            background.color = lockedColor;
            button.interactable = false;
        }
    }

    public void OnLevelButtonClicked()
    {
        if (LevelProgressManager.Instance == null)
            return;

        if (!LevelProgressManager.Instance.IsLevelUnlocked(levelIndex))
        {
            Debug.Log("Level chưa được mở khóa.");
            return;
        }

        LevelLoader.Instance.LoadLevel(levelIndex);
    }
}