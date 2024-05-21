using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private IInventory[] inventories;

    private void Awake()
    {
        inventories = GetComponentsInChildren<IInventory>(true);
    }

    public void SendItem(IItem item)
    {
        foreach(IInventory inventory in inventories)
        {
            if (!inventory.HasItem)
            {
                inventory.PutAway(item);
                item.CleanUp();
                break;
            }
        }
    }
}
