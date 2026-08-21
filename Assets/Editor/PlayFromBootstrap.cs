using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayFromBootstrap
{
    private const string BootstrapScenePath = "Assets/Scenes/Bootstrap.unity";

    static PlayFromBootstrap()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (EditorSceneManager.GetActiveScene().path != BootstrapScenePath)
            {
                EditorSceneManager.OpenScene(BootstrapScenePath);
            }
        }
    }
}