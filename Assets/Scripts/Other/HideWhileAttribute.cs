using UnityEngine;

/// <summary>
/// 引数で与えたbool変数の値によって、編集不可状態にする
/// </summary>
public class HideWhileAttribute : PropertyAttribute
{
    public readonly string referenceVariable = default;
    public readonly bool condition = default;

    public HideWhileAttribute(string referenceVariable, bool condition)
    {
        this.referenceVariable = referenceVariable;
        this.condition = condition;
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
            var hideWhileAttribute = attribute as HideWhileAttribute;
            var referenceVariableProperty = property.serializedObject.FindProperty(hideWhileAttribute.referenceVariable);
            if (referenceVariableProperty is null)
            {
                return;
            }

            EditorGUI.BeginDisabledGroup(referenceVariableProperty.boolValue == hideWhileAttribute.condition);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif