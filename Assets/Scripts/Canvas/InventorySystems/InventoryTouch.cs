using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryTouch : MonoBehaviour
{
    [SerializeField]
    private EventTrigger eventTrigger;

    private IInventoryOneFrame inventory;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        inventory = this.GetComponent<IInventoryOneFrame>();
        inventoryManager = transform.root.GetComponent<InventoryManager>();

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((x) => PointerUp());

        eventTrigger.triggers.Add(entryPointerUp);
    }
    private void PointerUp()
    {
        IItem item = inventory.TakeOut();

        //‰¼
        item.TakeOut(this.transform.position);
    }
}
