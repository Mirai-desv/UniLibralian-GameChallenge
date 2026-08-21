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

    [Header("Sprites theo từng Level (kéo theo đúng thứ tự 1, 2, 3...)")]
    [SerializeField] private Sprite[] lockedSprites;
    [SerializeField] private Sprite[] unlockedSprites;

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

        bool usingSceneButtonTemplate = levelButtonPrefab == null;
        if (usingSceneButtonTemplate)
        {
            LevelButton sceneButtonTemplate =
                contentContainer.GetComponentInChildren<LevelButton>(true);
            if (sceneButtonTemplate == null)
            {
                Debug.LogError("MenuLevelSelectionManager: Chưa gán Level Button Prefab.");
                return;
            }

            levelButtonPrefab = sceneButtonTemplate.gameObject;
        }

        if (usingSceneButtonTemplate)
        {
            ConfigureLevelButton(levelButtonPrefab, 0);
            ClearOldButtons(levelButtonPrefab.transform);
        }
        else
        {
            ClearOldButtons();
        }

        int totalGameLevels = CountLevelScenes();
        if (totalGameLevels == 0)
        {
            Debug.LogError("Không tìm thấy scene Level nào trong Build Settings.");
            return;
        }

        int firstGeneratedLevel = usingSceneButtonTemplate ? 1 : 0;
        for (int levelIndex = firstGeneratedLevel;
             levelIndex < totalGameLevels;
             levelIndex++)
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

        ConfigureLevelButton(buttonObject, levelIndex);
    }

    private void ConfigureLevelButton(GameObject buttonObject, int levelIndex)
    {
        // Ẩn hẳn text số cũ (nếu prefab còn sót lại), tránh đè lên sprite
        TMP_Text levelText = buttonObject.GetComponentInChildren<TMP_Text>(true);
        if (levelText != null)
        {
            levelText.gameObject.SetActive(false);
        }

        LevelButton levelButton =
            buttonObject.GetComponent<LevelButton>();

        if (levelButton != null)
        {
            Sprite locked = (levelIndex < lockedSprites.Length) ? lockedSprites[levelIndex] : null;
            Sprite unlocked = (levelIndex < unlockedSprites.Length) ? unlockedSprites[levelIndex] : null;

            if (unlocked == null)
            {
                Debug.LogWarning($"Thiếu sprite Unlock cho Level {levelIndex + 1}.");
            }

            levelButton.SetSprites(locked, unlocked);
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

    private void ClearOldButtons(Transform template)
    {
        foreach (Transform child in contentContainer)
        {
            if (child != template)
            {
                Destroy(child.gameObject);
            }
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