using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// GetPropertyHeight���g�p����N���X�͂����t����
/// </summary>
public interface IGetPropertyHeight
{
#if UNITY_EDITOR
    float GetPropertyHeight(SerializedProperty property, GUIContent label);
#endif
}

/// <summary>
/// �����̃A�g���r���[�g���������Ȃ��悤�ɊǗ�����N���X
/// <br>��{�I�ɂ��ׂĂ�PropertyAttribute�͂��̃N���X���p�����A�L�q���邱�Ƃ������߂��܂��B</br>
/// <br>See <see href="https://light11.hatenadiary.com/entry/2021/08/16/201543"/></br>
/// </summary>
public abstract class MultiPropertyAttribute : PropertyAttribute
{
    public MultiPropertyAttribute[] Attributes;
    public IGetPropertyHeight[] GetPropertyHeights;

#if UNITY_EDITOR
    public abstract void OnGUI(Rect position, SerializedProperty property, GUIContent label);

    // �A�g���r���[�g�̂�����ł�false�������炻��GUI�͔�\���ɂȂ�
    public virtual bool IsVisible(SerializedProperty property)
    {
        return true;
    }
#endif
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MultiPropertyAttribute), true)]
public class MultiPropertyAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attributes = GetAttributes();
        var propertyDrawers = GetPropertyHeights();

        // ��\���̏ꍇ
        if (attributes.Any(attr => !attr.IsVisible(property)))
        {
            return;
        }

        // �`��
        using var ccs = new EditorGUI.ChangeCheckScope();

        foreach (var attribute in attributes)
        {
            attribute.OnGUI(position, property, label);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attributes = GetAttributes();
        var propertyHeights = GetPropertyHeights();

        // ��\���̏ꍇ
        if (attributes.Any(attr => !attr.IsVisible(property)))
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        var height = propertyHeights.Length == 0
            ? base.GetPropertyHeight(property, label)
            : propertyHeights.Last().GetPropertyHeight(property, label);
        return height;
    }

    private MultiPropertyAttribute[] GetAttributes()
    {
        var attr = (MultiPropertyAttribute)attribute;

        if (attr.Attributes == null)
        {
            attr.Attributes = fieldInfo
                .GetCustomAttributes(typeof(MultiPropertyAttribute), false)
                .Cast<MultiPropertyAttribute>()
                .OrderBy(x => x.order)
                .ToArray();
        }

        return attr.Attributes;
    }

    private IGetPropertyHeight[] GetPropertyHeights()
    {
        var attr = (MultiPropertyAttribute)attribute;

        if (attr.GetPropertyHeights == null)
        {
            attr.GetPropertyHeights = fieldInfo
                .GetCustomAttributes(typeof(MultiPropertyAttribute), false)
                .OfType<IGetPropertyHeight>()
                .OrderBy(x => ((MultiPropertyAttribute)x).order)
                .ToArray();
        }

        return attr.GetPropertyHeights;
    }
}
#endif