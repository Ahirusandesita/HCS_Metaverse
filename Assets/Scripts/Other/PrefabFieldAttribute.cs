using UnityEngine;

/// <summary>
/// アタッチできるオブジェクトをPrefab Onlyにする
/// </summary>
public class PrefabFieldAttribute : PropertyAttribute { }

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(PrefabFieldAttribute))]
    public class PrefabFieldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var oldValue = property.objectReferenceValue;
            label.text += " (Prefab Only)";
            EditorGUI.PropertyField(position, property, label);

            if (property.objectReferenceValue is not null)
            {
                var prefabType = PrefabUtility.GetPrefabAssetType(property.objectReferenceValue);
                if (prefabType == PrefabAssetType.NotAPrefab)
                {
                    property.objectReferenceValue = oldValue;
                    Debug.LogWarning("このプロパティにPrefab以外のGameObjectはアタッチできません。");
                }
            }
        }
    }
}
#endif