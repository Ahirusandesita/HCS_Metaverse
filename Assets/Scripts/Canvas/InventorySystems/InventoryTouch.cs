using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryTouch : MonoBehaviour, IInventoryOneFrame
{
    [SerializeField]
    private EventTrigger eventTrigger;
    private InventoryManager inventoryManager;
    private IItem item;
    private ItemAsset itemAsset;
    private InventoryOneFrame inventoryOneFrame;

    private bool hasItem;
    public bool HasItem
    {
        get
        {
            return hasItem;
        }
    }
    public bool MatchItem(ItemAsset itemAsset)
    {
        return itemAsset.ID == this.itemAsset.ID;
    }

    private int hasItemValue = 0;

    //public void PutAway(IItem item)
    //{
    //    this.item = item;
    //    inventoryOneFrame.PutAway(item);
    //    hasItem = true;
    //}
    public void PutAway(ItemAsset itemAsset)
    {

        this.item = itemAsset.DisplayItem as IItem;
        this.itemAsset = itemAsset;
        inventoryOneFrame.PutAway(itemAsset);

        hasItemValue++;

        if(hasItemValue >= item.MaxInventoryCapacity)
        {
            hasItem = true;
        }
    }

    public void TakeOut()
    {
        inventoryOneFrame.TakeOut();
        hasItemValue--;
        if(hasItemValue <= 0)
        {
            hasItem = false;
        }
        inventoryManager.ReturnItem(itemAsset);
        item = null;
        itemAsset = null;
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
