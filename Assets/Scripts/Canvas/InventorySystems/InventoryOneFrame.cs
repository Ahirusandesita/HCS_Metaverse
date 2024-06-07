using UnityEngine;
/// <summary>
/// ƒCƒ“ƒxƒ“ƒgƒŠ‚P˜gŠÇ—
/// </summary>
public class InventoryOneFrame : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private MeshFilter meshFilter;

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
    }

    public void TakeOut()
    {
        meshRenderer.enabled = false;
    }

    private void InventoryView()
    {
        meshRenderer.enabled = true;
        meshRenderer.material = appearanceInfo_Mesh.Material;
        meshFilter.mesh = appearanceInfo_Mesh.Mesh;
    }
}
