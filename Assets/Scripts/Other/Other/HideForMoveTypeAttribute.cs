using UnityEngine;
using UnityEditor;
using HCSMeta.Player;

/// <summary>
/// �����ŗ^����VRMoveType�ϐ��̒l�ɂ���āA�ҏW�s��Ԃɂ���
/// </summary>
public class HideForMoveTypeAttribute : MultiPropertyAttribute
{
    public readonly string referenceVariable = default;
    public readonly VRMoveType condition = default;

    public HideForMoveTypeAttribute(string referenceVariable, VRMoveType condition)
    {
        this.referenceVariable = referenceVariable;
        this.condition = condition;
    }

#if UNITY_EDITOR
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var referenceVariableProperty = property.serializedObject.FindProperty(referenceVariable);
        if (referenceVariableProperty is null)
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(referenceVariableProperty.enumValueIndex == (int)condition);
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndDisabledGroup();

    }
#endif
}
