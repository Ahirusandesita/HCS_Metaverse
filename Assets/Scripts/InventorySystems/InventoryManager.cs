using UnityEngine;
/// <summary>
/// ‚P‚Â‚ÌƒCƒ“ƒxƒ“ƒgƒŠŠÇ—
/// </summary>
public class InventoryManager : MonoBehaviour
{
    private IInventoryOneFrame[] inventories;

    private void Awake()
    {
        inventories = GetComponentsInChildren<IInventoryOneFrame>(true);
    }

    public void SendItem(IItem item)
    {
        foreach(IInventoryOneFrame inventory in inventories)
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
