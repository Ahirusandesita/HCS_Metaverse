using HCSMeta.Function.Injection;
using HCSMeta.Player;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ÇPÇ¬ÇÃÉCÉìÉxÉìÉgÉää«óù
/// </summary>
public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private NotExistMaterial notExistMaterial;

    private IInventoryOneFrame[] inventories;

    private List<IItem> items = new List<IItem>();

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

    public void SendItem(IItem item)
    {
        foreach(IInventoryOneFrame inventory in inventories)
        {
            if (!inventory.HasItem)
            {
                items.Add(item);
                inventory.PutAway(item);
                item.CleanUp();

                GameObject.FindObjectOfType<PlayerInteraction>().Add(item as ISelectedNotificationInjectable);
                break;
            }
        }
    }

    public void ReturnItem(IItem item)
    {
        item.TakeOut(this.transform.position);
        item.Use();
        items.Remove(item);
    }
}
