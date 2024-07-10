using UnityEngine;
using UnityEditor;

/// <summary>
/// 引数で与えたbool変数の値によって、編集不可状態にする
/// </summary>
public class HideForBoolAttribute : MultiPropertyAttribute
{
    public readonly string referenceVariable = default;
    public readonly bool condition = default;

    public HideForBoolAttribute(string referenceVariable, bool condition)
    {
        this.referenceVariable = referenceVariable;
        this.condition = condition;
    }

#if UNITY_EDITOR
    public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
    {
        var referenceVariableProperty = property.serializedObject.FindProperty(referenceVariable);
        if (referenceVariableProperty is null)
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(referenceVariableProperty.boolValue == condition);
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndDisabledGroup();
    }
#endif
}

