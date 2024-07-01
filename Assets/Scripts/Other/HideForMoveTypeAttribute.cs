using UnityEngine;

/// <summary>
/// �����ŗ^����VRMoveType�ϐ��̒l�ɂ���āA�ҏW�s��Ԃɂ���
/// </summary>
public class HideForMoveTypeAttribute : PropertyAttribute
{
    public readonly string referenceVariable = default;
    public readonly VRMoveType condition = default;

    public HideForMoveTypeAttribute(string referenceVariable, VRMoveType condition)
    {
        this.referenceVariable = referenceVariable;
        this.condition = condition;
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(HideForMoveTypeAttribute))]
    public class HideForMoveTypeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var hideForMoveTypeAttribute = attribute as HideForMoveTypeAttribute;
            var referenceVariableProperty = property.serializedObject.FindProperty(hideForMoveTypeAttribute.referenceVariable);
            if (referenceVariableProperty is null)
            {
                return;
            }

            EditorGUI.BeginDisabledGroup(referenceVariableProperty.enumValueIndex == (int)hideForMoveTypeAttribute.condition);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif