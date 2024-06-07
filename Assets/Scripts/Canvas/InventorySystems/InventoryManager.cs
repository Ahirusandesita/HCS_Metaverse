using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ÇPÇ¬ÇÃÉCÉìÉxÉìÉgÉää«óù
/// </summary>
public class InventoryManager : MonoBehaviour
{
    private IInventoryOneFrame[] inventories;

    private List<IItem> items = new List<IItem>();

    private void Awake()
    {
        inventories = GetComponentsInChildren<IInventoryOneFrame>(true);

        foreach(IInventoryOneFrame inventoryOneFrame in inventories)
        {
            inventoryOneFrame.Inject(this);
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
