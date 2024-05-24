using UnityEngine;

/// <summary>
/// �����ŗ^������������C���X�y�N�^�ɒǉ��\��������
/// </summary>
public class CustomFieldAttribute : PropertyAttribute
{
    public readonly string addDisplayName = default;

    public CustomFieldAttribute(string addDisplayName)
    {
        this.addDisplayName = addDisplayName;
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(CustomFieldAttribute))]
    public class CustomFieldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var customFieldAttribute = attribute as CustomFieldAttribute;
            label.text += $" ({customFieldAttribute.addDisplayName})";
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
#endif