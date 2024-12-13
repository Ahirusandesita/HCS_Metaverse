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
    [SerializeField] private List<NetworkView> networkViews;
    public IReadOnlyList<ItemAsset> Items => items;
    public IReadOnlyList<NetworkView> NetworkViews => networkViews;
    List<ItemAsset> IEditorItemBundleAsset.EditorItems { set => items = value; }

    public ItemAsset GetItemAssetByID(int id)
    {
        return items.Where(item => item.ID == id).First();
    }
    public NetworkItemAsset GetNetworkItemAssetById(int id)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if(items[i].ID == id)
            {
                return new NetworkItemAsset(items[i], networkViews[i]);
            }
        }
        return null;
    }

}
public class NetworkItemAsset
{
    public readonly ItemAsset ItemAsset;
    public readonly NetworkView NetworkView;
    public NetworkItemAsset(ItemAsset itemAsset,NetworkView networkView)
    {
        this.ItemAsset = itemAsset;
        this.NetworkView = networkView;
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