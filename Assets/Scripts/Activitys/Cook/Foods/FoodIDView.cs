using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FoodIDView : ItemIDView { }

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
    [CustomPropertyDrawer(typeof(FoodIDView))]
    public class FoodIDViewDrawer : PropertyDrawer
    {
        private SerializedProperty idProperty = default;
        private SerializedProperty nameProperty = default;
        private SerializedProperty selectedIndexProperty = default;
        private static ItemBundleAsset allItemAsset = default;
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
                allItemAsset = AssetDatabase.FindAssets($"t:{nameof(ItemBundleAsset)}")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<ItemBundleAsset>)
                    .Where(asset => asset.GenresHandled == ItemGenre.Food)
                    .First();
                UpdateDisplayOptions();
            }

            int newValue = EditorGUI.Popup(position, label.text, selectedIndexProperty.intValue, displayedOptions);

            // 更新
            if (EditorGUI.EndChangeCheck())
            {
                idProperty.intValue = allItemAsset.Items[newValue].ID;
                nameProperty.stringValue = allItemAsset.Items[newValue].Name;
                selectedIndexProperty.intValue = newValue;
            }
            EditorGUI.EndProperty();
        }

        public static void UpdateDisplayOptions()
        {
            var tmpList = new List<string>();
            foreach (var item in allItemAsset.Items)
            {
                string content = item.Name;

                if (!item.IsDisplayable)
                {
                    content += " (No Visual)";
                }
                tmpList.Add(content);
            }
            displayedOptions = tmpList.ToArray();
        }
    }
}
#endif