using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemID
{
    [SerializeField] private int id = default;
    [SerializeField] private string displayName = default;
    [SerializeField] private int selectedIndex = default;

    public int ID => id;

    public static implicit operator int(ItemID itemID)
    {
        return itemID.id;
    }

    public static explicit operator ItemID(int id)
    {
        return new ItemID() { id = id };
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(ItemID))]
    public class ItemIDDrawer : PropertyDrawer
    {
        private SerializedProperty idProperty = default;
        private SerializedProperty nameProperty = default;
        private SerializedProperty selectedIndexProperty = default;
        private static AllItemAsset allItemAsset = default;
        private static string[] displayedOptions = default;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            // 拡張先の変数をSerializedPropertyに変換
            idProperty = property.FindPropertyRelative("id");
            nameProperty = property.FindPropertyRelative("displayName");
            selectedIndexProperty = property.FindPropertyRelative("selectedIndex");

            if (allItemAsset is null)
            {
                // AllItemAssetのインスタンスを取得
                allItemAsset = AssetDatabase.FindAssets($"t:{nameof(AllItemAsset)}")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<AllItemAsset>)
                    .First();
                UpdateDisplayOptions();
            }

            int newValue = EditorGUI.Popup(position, label.text, selectedIndexProperty.intValue, displayedOptions);

            // 更新
            if (EditorGUI.EndChangeCheck())
            {
                idProperty.intValue = allItemAsset.Items[newValue].ItemID;
                nameProperty.stringValue = allItemAsset.Items[newValue].DisplayName;
                selectedIndexProperty.intValue = newValue;
            }
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Popupの表示内容を更新する
        /// </summary>
        public static void UpdateDisplayOptions()
        {
            var tmpList = new List<string>();
            foreach (var item in allItemAsset.Items)
            {
                tmpList.Add(item.DisplayName);
            }
            displayedOptions = tmpList.ToArray();
        }
    }
}
#endif