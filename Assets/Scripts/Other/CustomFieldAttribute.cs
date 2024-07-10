using UnityEngine;

/// <summary>
/// 引数で与えた文字列をインスペクタに追加表示させる
/// </summary>
public class CustomFieldAttribute : PropertyAttribute
{
    public enum DisplayType
    {
        Append,
        Replace,
    }

    public readonly string displayName = default;
    public readonly DisplayType displayType = default;

    public CustomFieldAttribute(string displayName, DisplayType displayType = DisplayType.Append)
    {
        this.displayName = displayName;
        this.displayType = displayType;
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
            if (customFieldAttribute.displayType == CustomFieldAttribute.DisplayType.Append)
            {
                label.text += $" ({customFieldAttribute.displayName})";
            }
            else
            {
                label.text = customFieldAttribute.displayName;
            }

            EditorGUI.PropertyField(position, property, label);
        }
    }
}
#endif