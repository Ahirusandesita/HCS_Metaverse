using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllItemData", menuName = "ScriptableObjects/AllItemAsset")]
public class AllItemAsset : ScriptableObject
{
    [SerializeField] private List<ItemAsset> items = default;

    public IReadOnlyList<ItemAsset> Items => items;

    public ItemAsset GetItemAssetByID(int id)
    {
        return items.Where(item => item.ItemID == id).First();
    }
}
