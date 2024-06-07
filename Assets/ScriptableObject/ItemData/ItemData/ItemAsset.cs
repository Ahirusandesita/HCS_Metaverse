using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemAsset")]
public class ItemAsset : ScriptableObject
{
    [SerializeField] private int itemID = default;
    [SerializeField] private string itemName = default;
    [SerializeField] private string itemText = default;
    [SerializeField] private GameObject prefab = default;
    [SerializeField] private bool allowVisualCatalog = false;
    [SerializeField, InterfaceType(typeof(IDisplayItem)), HideWhile(nameof(allowVisualCatalog))]
    private Object displayItem = default;

    public int ItemID => itemID;
    public string ItemName => itemName;
    public string ItemText => itemText;
    public GameObject Prefab => prefab;
    public IDisplayItem DisplayItem => displayItem as IDisplayItem;
}
