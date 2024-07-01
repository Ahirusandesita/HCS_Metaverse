using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemIDViewer
{
    [SerializeField] private int id = default;
    [SerializeField] private string displayName = default;
    [SerializeField] private int selectedIndex = default;

    public int ID => id;

    public static implicit operator int(ItemIDViewer itemID)
    {
        return itemID.id;
    }

    public static explicit operator ItemIDViewer(int id)
    {
        return new ItemIDViewer() { id = id };
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(ItemIDViewer))]
    public class ItemIDViewerDrawer : PropertyDrawer
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

        /// <summary>
        /// Popupの表示内容を更新する
        /// </summary>
        public static void UpdateDisplayOptions()
        {
            var tmpList = new List<string>();
            foreach (var item in allItemAsset.Items)
            {
                string content = item.Name;

                if (!item.AllowVisualCatalog)
                {
                    content += " (No Visual)";
                }
                else if (item.Genre != ItemGenre.All)
                {
                    content += $" ({item.Genre})";
                }
                tmpList.Add(content);
            }
            displayedOptions = tmpList.ToArray();
        }
    }
}
#endif