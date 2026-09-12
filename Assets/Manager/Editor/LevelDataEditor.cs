using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var level = (LevelData)target;
        if (GUILayout.Button("Auto Sort Order (theo thứ tự list)"))
        {
            for (int i = 0; i < level.BookLayout.Count; i++)
                level.BookLayout[i].SortingOrder = i;

            EditorUtility.SetDirty(level);
        }
    }
}