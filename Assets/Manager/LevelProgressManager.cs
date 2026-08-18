using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressManager : MonoBehaviour
{
    public static LevelProgressManager Instance;

    private LevelProgressData progress;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            progress = SaveSystem.Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex <= progress.highestUnlockedLevel;
    }

    public int HighestUnlockedLevel()
    {
        return progress.highestUnlockedLevel;
    }

    public void CompleteLevel(int levelIndex)
    {
        if (levelIndex >= progress.highestUnlockedLevel)
        {
            progress.highestUnlockedLevel = levelIndex + 1;
            SaveSystem.Save(progress);
        }

        ReturnToMainMenu();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}