using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "AllItemData", menuName = "ScriptableObjects/AllItemAsset")]
public class AllItemAsset : ScriptableObject
{
    [Serializable]
    public class ItemInfo
    {
        [SerializeField, Min(0)] private int itemID = default;
        [SerializeField] private string displayName = default;
        [SerializeField, InterfaceType(typeof(IDisplayItem))] private Object prefab = default;

        public int ItemID => itemID;
        public string DisplayName => displayName;
        public IDisplayItem Prefab => prefab as IDisplayItem;
    }

    [SerializeField] private List<ItemInfo> items = default;
    private Dictionary<int, IDisplayItem> itemDictionary = default;

    public IReadOnlyDictionary<int, IDisplayItem> ItemDictionary
    {
        get
        {
            itemDictionary ??= items.ToDictionary(items => items.ItemID, items => items.Prefab);
            return itemDictionary;
        }
    }

    public IReadOnlyList<ItemInfo> ItemInfos => items;
}
