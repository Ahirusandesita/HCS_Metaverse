using UnityEngine;

/// <summary>
/// 引数で与えたbool変数の値によって、編集不可状態にする
/// </summary>
public class HideForBoolAttribute : PropertyAttribute
{
    public readonly string referenceVariable = default;
    public readonly bool condition = default;

    public HideForBoolAttribute(string referenceVariable, bool condition)
    {
        this.referenceVariable = referenceVariable;
        this.condition = condition;
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(HideForBoolAttribute))]
    public class HideForBoolAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var hideForBoolAttribute = attribute as HideForBoolAttribute;
            var referenceVariableProperty = property.serializedObject.FindProperty(hideForBoolAttribute.referenceVariable);
            if (referenceVariableProperty is null)
            {
                return;
            }

            EditorGUI.BeginDisabledGroup(referenceVariableProperty.boolValue == hideForBoolAttribute.condition);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif
