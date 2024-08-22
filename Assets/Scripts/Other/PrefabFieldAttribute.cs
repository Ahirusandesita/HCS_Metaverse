using UnityEngine;

/// <summary>
/// �A�^�b�`�ł���I�u�W�F�N�g��Prefab Only�ɂ���
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
                    Debug.LogWarning("���̃v���p�e�B��Prefab�ȊO��GameObject�̓A�^�b�`�ł��܂���B");
                }
            }
        }
    }
}
#endif