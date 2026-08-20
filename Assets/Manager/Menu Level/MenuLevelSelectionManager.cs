using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuLevelSelectionManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject levelButtonPrefab;

    private void Start()
    {
        GenerateLevels();
    }

    public void GenerateLevels()
    {
        if (contentContainer == null)
        {
            Debug.LogError("MenuLevelSelectionManager: Chưa gán Content Container.");
            return;
        }

        if (levelButtonPrefab == null)
        {
            Debug.LogError("MenuLevelSelectionManager: Chưa gán Level Button Prefab.");
            return;
        }

        if (LevelProgressManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy LevelProgressManager.");
            return;
        }

        ClearOldButtons();

        int highestUnlockedLevel =
            LevelProgressManager.Instance.HighestUnlockedLevel();

        int totalGameLevels = CountLevelScenes();
        if (totalGameLevels == 0)
        {
            Debug.LogError("Không tìm thấy scene Level nào trong Build Settings.");
            return;
        }

        int lastLevelIndex = Mathf.Min(
            totalGameLevels - 1,
            highestUnlockedLevel + 10);

        for (int levelIndex = 0; levelIndex <= lastLevelIndex; levelIndex++)
        {
            CreateLevelButton(levelIndex);
        }
    }

    private int CountLevelScenes()
    {
        HashSet<int> levelNumbers = new HashSet<int>();

        for (int buildIndex = 0;
             buildIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
             buildIndex++)
        {
            string scenePath =
                UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(buildIndex);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            if (sceneName.StartsWith("Level ") &&
                int.TryParse(sceneName.Substring("Level ".Length), out int levelNumber) &&
                levelNumber > 0)
            {
                levelNumbers.Add(levelNumber);
            }
        }

        int levelCount = 0;
        while (levelNumbers.Contains(levelCount + 1))
        {
            levelCount++;
        }

        if (levelCount != levelNumbers.Count)
        {
            Debug.LogError(
                "Các scene Level trong Build Settings phải được đánh số liên tục từ Level 1.");
        }

        return levelCount;
    }

    private void CreateLevelButton(int levelIndex)
    {
        GameObject buttonObject =
            Instantiate(levelButtonPrefab, contentContainer);

        // Hiển thị số Level
        TMP_Text levelText =
            buttonObject.GetComponentInChildren<TMP_Text>();

        if (levelText != null)
        {
            levelText.text = (levelIndex + 1).ToString();
        }

        // Cấu hình LevelButton
        LevelButton levelButton =
            buttonObject.GetComponent<LevelButton>();

        if (levelButton != null)
        {
            levelButton.SetLevelIndex(levelIndex);
        }
        else
        {
            Debug.LogWarning(
                $"Button Level {levelIndex + 1} chưa có LevelButton component."
            );
        }
    }

    private void ClearOldButtons()
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnBackButtonClicked()
    {
        if (LevelProgressManager.Instance != null)
        {
            LevelProgressManager.Instance.ReturnToMainMenu();
        }
    }
}