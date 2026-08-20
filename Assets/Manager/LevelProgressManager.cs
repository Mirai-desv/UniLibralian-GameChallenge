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

//Check Level unlock chưa
    public bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex <= progress.highestUnlockedLevel;
    }

// Nhận Level cao nhất đã unlock
    public int HighestUnlockedLevel()
    {
        return progress.highestUnlockedLevel;
    }

// Cập nhật Level cao nhất đã unlock
    public void CompleteLevel(int levelIndex)
    {
        if (levelIndex + 1 > progress.highestUnlockedLevel)
        {
            progress.highestUnlockedLevel = levelIndex + 1;
            SaveSystem.Save(progress);
        }

    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}