using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class ItemIDView
{
    [SerializeField] protected int id = default;
    [SerializeField] protected string displayName = default;
    [SerializeField] protected int selectedIndex = default;

    public int ID => id;

    public static implicit operator int(ItemIDView itemID)
    {
        return itemID.id;
    }

    public static explicit operator ItemIDView(int id)
    {
        return new ItemIDView() { id = id };
    }
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
    [CustomPropertyDrawer(typeof(ItemIDView))]
    public class ItemIDViewDrawer : PropertyDrawer
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
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in allItemAsset.Items)
            {
                if (!item.IsDisplayable)
                {
                    stringBuilder.Append("NoPrefab : ");
                }
                else
                {
                    stringBuilder.Append($"{item.Genre} : ");
                }

                stringBuilder.Append(item.Name);
                tmpList.Add(stringBuilder.ToString());
                stringBuilder.Clear();
            }
            displayedOptions = tmpList.ToArray();
        }
    }
}
#endif