using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryTouch : MonoBehaviour, IInventoryOneFrame
{
    [SerializeField]
    private EventTrigger eventTrigger;
    private InventoryManager inventoryManager;
    private IDisplayItem displayItem;
    private ItemAsset itemAsset;
    private InventoryOneFrame inventoryOneFrame;
    private SelectItem selectItem;

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
        if(this.itemAsset == null)
        {
            return true;
        }
        return itemAsset.ID == this.itemAsset.ID;
    }

    private int hasItemValue = 0;
    public void PutAway(ItemAsset itemAsset)
    {
        displayItem = itemAsset.DisplayItem;

        this.itemAsset = itemAsset;
        inventoryOneFrame.PutAway(itemAsset);

        hasItemValue++;

        if(hasItemValue >= displayItem.MaxInventoryCapacity)
        {
            hasItem = true;
        }
    }

    public void TakeOut()
    {
        if(itemAsset == null)
        {
            return;
        }

        if (!inventoryManager.IsAvailableItem(itemAsset))
        {
            selectItem.NotAvailable(itemAsset);
            return;
        }

        inventoryOneFrame.TakeOut();
        hasItemValue--;
        if(hasItemValue <= 0)
        {
            hasItem = false;
        }
        inventoryManager.ReturnItem(itemAsset);
        displayItem = null;
        itemAsset = null;
    }

    private void Awake()
    {
        inventoryOneFrame = this.GetComponent<InventoryOneFrame>();

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((x) => PointerUp());

        EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry();
        entryPointerEnter.eventID = EventTriggerType.PointerEnter;
        entryPointerEnter.callback.AddListener((x) => PointerEnter());

        EventTrigger.Entry entryPointerExit = new EventTrigger.Entry();
        entryPointerExit.eventID = EventTriggerType.PointerExit;
        entryPointerExit.callback.AddListener((x) => PointerExit());

        eventTrigger.triggers.Add(entryPointerUp);
        eventTrigger.triggers.Add(entryPointerEnter);
        eventTrigger.triggers.Add(entryPointerExit);
    }
    private void PointerUp()
    {
        if (!HasItem)
        {
            return;
        }
        TakeOut();
    }
    private void PointerEnter()
    {
        if(itemAsset == null)
        {
            return;
        }
        selectItem.Select(itemAsset);
    }
    private void PointerExit()
    {
        selectItem.UnSelect();
    }
    public void Inject(InventoryManager inventoryManager)
    {
        this.inventoryManager = inventoryManager;
    }
    public void SelectItemInject(SelectItem selectItem,NotExistIcon notExistIcon)
    {
        this.selectItem = selectItem;
        selectItem.NotExistIconInject(notExistIcon);
    }
}