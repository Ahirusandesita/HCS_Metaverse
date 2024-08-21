using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace HCSMeta.Activity
{
    /// <summary>
    /// このInterfaceはエディタクラスからのみアクセスすること
    /// </summary>
    public interface IEditorItemBundleAsset
    {
        List<ItemAsset> EditorItems { set; }
        ItemGenre GenresHandled { get; }
    }

    [CreateAssetMenu(fileName = "ItemBundleData", menuName = "ScriptableObjects/ItemAsset/Bundle")]
    public class ItemBundleAsset : ScriptableObject, IEditorItemBundleAsset
    {
        [SerializeField] private List<ItemAsset> items = default;
        [SerializeField] private ItemGenre genresHandled = default;

        public IReadOnlyList<ItemAsset> Items => items;
        public ItemGenre GenresHandled => genresHandled;

        List<ItemAsset> IEditorItemBundleAsset.EditorItems { set => items = value; }

        public ItemAsset GetItemAssetByID(int id)
        {
            return items.Where(item => item.ID == id).First();
        }
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    using HCSMeta.Activity;

    [CustomEditor(typeof(ItemBundleAsset))]
    public class ItemBundleAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(12f);

            if (GUILayout.Button("Set Selected Genre Items"))
            {
                try
                {
                    var itemBundleAsset = target as IEditorItemBundleAsset;
                    List<ItemAsset> itemAsset = default;

                    if (itemBundleAsset.GenresHandled == ItemGenre.All)
                    {
                        itemAsset = AssetDatabase.FindAssets($"t:{nameof(ItemAsset)}")
                            .Select(AssetDatabase.GUIDToAssetPath)
                            .Select(AssetDatabase.LoadAssetAtPath<ItemAsset>)
                            .ToList();
                    }
                    else
                    {
                        itemAsset = AssetDatabase.FindAssets($"t:{nameof(ItemAsset)}")
                            .Select(AssetDatabase.GUIDToAssetPath)
                            .Select(AssetDatabase.LoadAssetAtPath<ItemAsset>)
                            .Where(asset => asset.Genre == itemBundleAsset.GenresHandled)
                            .ToList();
                    }

                    itemBundleAsset.EditorItems = itemAsset;
                }
                // 要素ない状態でボタン押すと例外出る→うざいので握りつぶす
                catch (System.NullReferenceException) { }
            }
        }
    }
}
#endif