using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
