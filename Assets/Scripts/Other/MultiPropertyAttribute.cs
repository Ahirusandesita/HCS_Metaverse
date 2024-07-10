using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// GetPropertyHeightを使用するクラスはこれを付ける
/// </summary>
public interface IGetPropertyHeight
{
#if UNITY_EDITOR
    float GetPropertyHeight(SerializedProperty property, GUIContent label);
#endif
}

/// <summary>
/// 複数のアトリビュートが競合しないように管理するクラス
/// <br>基本的にすべてのPropertyAttributeはこのクラスを継承し、記述することをお勧めします。</br>
/// <br>See <see href="https://light11.hatenadiary.com/entry/2021/08/16/201543"/></br>
/// </summary>
public abstract class MultiPropertyAttribute : PropertyAttribute
{
    public MultiPropertyAttribute[] Attributes;
    public IGetPropertyHeight[] GetPropertyHeights;

#if UNITY_EDITOR
    public abstract void OnGUI(Rect position, SerializedProperty property, GUIContent label);

    // アトリビュートのうち一つでもfalseだったらそのGUIは非表示になる
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

        // 非表示の場合
        if (attributes.Any(attr => !attr.IsVisible(property)))
        {
            return;
        }

        // 描画
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

        // 非表示の場合
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