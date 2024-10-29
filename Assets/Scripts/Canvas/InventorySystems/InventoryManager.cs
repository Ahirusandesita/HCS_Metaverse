using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
/// <summary>
/// ÇPÇ¬ÇÃÉCÉìÉxÉìÉgÉää«óù
/// </summary>
public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private NotExistMaterial notExistMaterial;
    private IInventoryOneFrame[] inventories;

    private void Awake()
    {
        inventories = GetComponentsInChildren<IInventoryOneFrame>(true);

        foreach(IInventoryOneFrame inventoryOneFrame in inventories)
        {
            inventoryOneFrame.Inject(this);
        }

        NotExistMaterial oject = Instantiate(notExistMaterial);

        foreach(InventoryOneFrame inventoryOneFrame in GetComponentsInChildren<InventoryOneFrame>(true))
        {
            inventoryOneFrame.Inject(oject);
        }
    }

    public void ReturnItem(ItemAsset itemAsset)
    {
        SpawnItem(itemAsset).Forget();
    }
    private async UniTaskVoid SpawnItem(ItemAsset itemAsset)
    {
        GameObject item = await GateOfFusion.Instance.SpawnAsync(itemAsset.DisplayItem.gameObject, this.transform.position);    
        GameObject.FindObjectOfType<PlayerInteraction>().Add(item.GetComponent<IItem>() as ISelectedNotificationInjectable);
    }

    [SerializeField]
    private ItemBundleAsset itemBundle;
    public void SendItem(int id)
    {
        foreach (IInventoryOneFrame inventory in inventories)
        {
            if (!inventory.HasItem && inventory.MatchItem(itemBundle.GetItemAssetByID(id)))
            {
                inventory.PutAway(itemBundle.GetItemAssetByID(id));
                break;
            }
        }
    }
}
