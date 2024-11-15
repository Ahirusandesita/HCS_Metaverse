using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// このInterfaceはエディタクラスからのみアクセスすること
/// </summary>
public interface IEditorItemBundleAsset
{
    List<ItemAsset> EditorItems { set; }
}

[CreateAssetMenu(fileName = "ItemBundleData", menuName = "ScriptableObjects/ItemAsset/Bundle")]
public class ItemBundleAsset : ScriptableObject, IEditorItemBundleAsset
{
    [SerializeField] private List<ItemAsset> items = default;

    public IReadOnlyList<ItemAsset> Items => items;

    List<ItemAsset> IEditorItemBundleAsset.EditorItems { set => items = value; }

    public ItemAsset GetItemAssetByID(int id)
    {
        return items.Where(item => item.ID == id).First();
    }
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
    [CustomEditor(typeof(ItemBundleAsset))]
    public class ItemBundleAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(12f);

            if (GUILayout.Button("Auto Set"))
            {
                try
                {
                    var itemBundleAsset = target as IEditorItemBundleAsset;
                    List<ItemAsset> itemAsset = default;

                    itemAsset = AssetDatabase.FindAssets($"t:{nameof(ItemAsset)}")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<ItemAsset>)
                        .OrderBy(asset => asset.Genre)
                        .ToList();

                    itemBundleAsset.EditorItems = itemAsset;
                }
                // 要素ない状態でボタン押すと例外出る→うざいので握りつぶす
                catch (System.NullReferenceException) { }
            }
        }
    }
}
#endif