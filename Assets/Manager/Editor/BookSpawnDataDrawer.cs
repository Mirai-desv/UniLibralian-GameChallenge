using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BookSpawnData))]
public class BookSpawnDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.BeginProperty(pos, label, prop);

        var typeProp = prop.FindPropertyRelative("Type");
        var posProp = prop.FindPropertyRelative("Position");
        var sortProp = prop.FindPropertyRelative("SortingOrder");

        float w = pos.width;
        var typeRect = new Rect(pos.x, pos.y, w * 0.28f, pos.height);
        var posRect  = new Rect(pos.x + w * 0.30f, pos.y, w * 0.5f, pos.height);
        var sortRect = new Rect(pos.x + w * 0.82f, pos.y, w * 0.18f, pos.height);

        EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);
        EditorGUI.PropertyField(posRect, posProp, GUIContent.none);
        EditorGUI.PropertyField(sortRect, sortProp, GUIContent.none);

        EditorGUI.EndProperty();
    }
}