using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemBundleData", menuName = "ScriptableObjects/ItemAsset/Bundle")]
public class ItemBundleAsset : ScriptableObject
{
    [SerializeField] private List<ItemAsset> items = default;

    public IReadOnlyList<ItemAsset> Items => items;

    public ItemAsset GetItemAssetByID(int id)
    {
        return items.Where(item => item.ID == id).First();
    }
}
