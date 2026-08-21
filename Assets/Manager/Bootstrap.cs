using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    private void Start()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}