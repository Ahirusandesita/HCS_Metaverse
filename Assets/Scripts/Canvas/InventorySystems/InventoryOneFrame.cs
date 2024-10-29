using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ÉCÉìÉxÉìÉgÉäÇPògä«óù
/// </summary>
public class InventoryOneFrame : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private MeshFilter meshFilter;

    
    private NotExistMaterial notExistMaterial;
    public void Inject(NotExistMaterial notExistMaterial)
    {
        this.notExistMaterial = notExistMaterial;
    }

    private IInventoryRetractable inventory_Mesh;
    public IInventoryRetractable Inventory_Mesh => inventory_Mesh;

    private AppearanceInfo_Mesh appearanceInfo_Mesh;

    [SerializeField]
    private Image icon;

    private void Awake()
    {
        meshRenderer.enabled = false;
    }

    public void PutAway(IItem item)
    {
        
        inventory_Mesh = item is IInventoryRetractable ? item as IInventoryRetractable : notExistMaterial;
        appearanceInfo_Mesh = inventory_Mesh.Appearance();
        InventoryView();
    }

    private void InventoryView()
    {
        meshRenderer.enabled = true;
        meshRenderer.materials = appearanceInfo_Mesh.Material;
        meshRenderer.transform.localScale = appearanceInfo_Mesh.Size;
        meshFilter.mesh = appearanceInfo_Mesh.Mesh;
    }

    public void PutAway(ItemAsset itemAsset)
    {
        icon.sprite = itemAsset.ItemIcon;
    }
    public void TakeOut()
    {
        icon.sprite = null;
    }
}
