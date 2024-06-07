using UnityEngine;

public class HideWhileAttribute : PropertyAttribute
{
    public readonly string referenceVariable = default;

    public HideWhileAttribute(string referenceVariable)
    {
        this.referenceVariable = referenceVariable;
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(HideWhileAttribute))]
    public class HideWhileAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //var hideWhileAttribute = attribute as HideWhileAttribute;
            //var referenceVariableProperty = property.serializedObject.FindProperty(hideWhileAttribute.referenceVariable);
            //if (referenceVariableProperty is null)
            //{
            //    return;
            //}
            //EditorGUI.BeginDisabledGroup(referenceVariableProperty.boolValue);
            //EditorGUI.PropertyField(position, property, label, true);
            //EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif