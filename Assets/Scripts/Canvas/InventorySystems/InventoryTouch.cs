using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryTouch : MonoBehaviour,IInventoryOneFrame
{
    [SerializeField]
    private EventTrigger eventTrigger;
    private InventoryManager inventoryManager;
    private IItem item;
    private InventoryOneFrame inventoryOneFrame;

    private bool hasItem;
    public bool HasItem
    {
        get
        {
            return hasItem;
        }
    }

    public void PutAway(IItem item)
    {
        this.item = item;
        inventoryOneFrame.PutAway(item);
        hasItem = true;
    }

    public void TakeOut()
    {
        inventoryOneFrame.TakeOut();
        hasItem = false;
        inventoryManager.ReturnItem(item);
        item = null;
    }

    private void Awake()
    {
        inventoryOneFrame = this.GetComponent<InventoryOneFrame>();

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((x) => PointerUp());

        eventTrigger.triggers.Add(entryPointerUp);
    }
    private void PointerUp()
    {
        if (!HasItem)
        {
            return;
        }
        TakeOut();
    }
    public void Inject(InventoryManager inventoryManager)
    {
        this.inventoryManager = inventoryManager;
    }
}
