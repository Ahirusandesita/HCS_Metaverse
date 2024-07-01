using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemAsset/Item")]
public class ItemAsset : ScriptableObject
{
    [SerializeField] private int itemID = default;
    [SerializeField] private string itemName = default;
    [SerializeField] private string itemText = default;
    [SerializeField] private ItemGenre itemGenre = default;
    [SerializeField] private GameObject prefab = default;
    [Space(10)]
    [SerializeField] private bool allowVisualCatalog = false;
    [SerializeField, InterfaceType(typeof(IDisplayItem)), HideForBool(nameof(allowVisualCatalog), false)]
    private Object displayItem = default;

    public int ID => itemID;
    public string Name => itemName;
    public string Text => itemText;
    public ItemGenre Genre => itemGenre;
    public GameObject Prefab => prefab;
    public bool AllowVisualCatalog => allowVisualCatalog;
    public IDisplayItem DisplayItem => displayItem as IDisplayItem;
}

public enum ItemGenre
{
    All,
    Usable,
    Food,
}