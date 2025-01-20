using UnityEditor;
using UnityEngine;

/// <summary>
/// ���s���ɒl���ҏW�s�ɂȂ�
/// </summary>
public class HideAtPlaying : MultiPropertyAttribute
{
#if UNITY_EDITOR
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndDisabledGroup();
    }
#endif
}

public class Hide : MultiPropertyAttribute
{
#if UNITY_EDITOR
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndDisabledGroup();
    }
#endif
}
