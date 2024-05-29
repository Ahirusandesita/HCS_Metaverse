using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "AllItemData", menuName = "ScriptableObjects/AllItemAsset")]
public class AllItemAsset : ScriptableObject
{
    [SerializeField] private List<ItemInfo> items = default;
    private Dictionary<int, IBuyableItem> itemDictionary = default;

    public IReadOnlyDictionary<int, IBuyableItem> ItemDictionary
    {
        get
        {
            if (itemDictionary is null)
            {
                itemDictionary = items.ToDictionary(items => items.ItemID, items => items.Prefab);
            }

            return itemDictionary;
        }
    }
}

[Serializable]
public class ItemInfo
{
    [SerializeField, Min(0)] private int itemID = default;
    [SerializeField, InterfaceType(typeof(IBuyableItem))] private Object prefab = default;

    public int ItemID => itemID;
    public IBuyableItem Prefab => prefab as IBuyableItem;
}
