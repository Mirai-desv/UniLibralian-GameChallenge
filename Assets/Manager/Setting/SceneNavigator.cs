using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    public string mainMenuSceneName = "Main Menu";

    // Biến static giữ tên scene trước khi vào Setting
    // static để giữ nguyên giá trị khi đổi scene
    public static string previousScene;

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void GoBack()
    {
        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            // Nếu không có scene trước đó (ví dụ mở Setting đầu tiên) thì về Main Menu
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}