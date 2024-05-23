using UnityEngine;
/// <summary>
/// ƒCƒ“ƒxƒ“ƒgƒŠ‚P˜gŠÇ—
/// </summary>
public class InventoryOneFrame : MonoBehaviour, IInventoryOneFrame
{
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private MeshFilter meshFilter;

    private bool hasItem;
    public bool HasItem
    {
        get
        {
            return hasItem;
        }
    }

    private IInventoryRetractable inventory_Mesh;
    public IInventoryRetractable Inventory_Mesh => inventory_Mesh;

    private AppearanceInfo_Mesh appearanceInfo_Mesh;

    private void Awake()
    {
        meshRenderer.enabled = false;
    }

    public void PutAway(IItem item)
    {
        inventory_Mesh = item as IInventoryRetractable;
        appearanceInfo_Mesh = inventory_Mesh.Appearance();
        InventoryView();
        hasItem = true;
    }

    public IItem TakeOut()
    {
        meshRenderer.enabled = false;
        hasItem = false;
        return inventory_Mesh as IItem;
    }

    private void InventoryView()
    {
        meshRenderer.enabled = true;
        meshRenderer.material = appearanceInfo_Mesh.Material;
        meshFilter.mesh = appearanceInfo_Mesh.Mesh;
    }
}
