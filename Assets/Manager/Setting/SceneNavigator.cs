using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneNavigator : MonoBehaviour
{
    public string mainMenuSceneName = "Main Menu";

    // Biến static giữ tên scene trước khi vào Setting
    // static để giữ nguyên giá trị khi đổi scene
    public static string previousScene;

    private void Awake()
    {
        AddButtonListener("Menu Button", GoToMainMenu);
        AddButtonListener("Back Button", GoBack);
    }

    private void AddButtonListener(string objectName, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = GameObject.Find(objectName);
        if (buttonObject == null)
        {
            Debug.LogWarning($"Không tìm thấy button '{objectName}' trong scene Setting.");
            return;
        }

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }

        button.onClick.AddListener(action);
    }

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