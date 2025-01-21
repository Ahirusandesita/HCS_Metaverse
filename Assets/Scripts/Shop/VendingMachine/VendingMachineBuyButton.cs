using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineBuyButton : MonoBehaviour
{
    [SerializeField]
    private EventTrigger eventTrigger;
    [SerializeField]
    private VendingMachineUI vendingMachineUI;
    private void Awake()
    {
        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((x) => OnPointerClick((PointerEventData)x));

        eventTrigger.triggers.Add(entryPointerUp);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        vendingMachineUI.Buy();
    }

    [ContextMenu("test")]
    public void Test()
    {
        vendingMachineUI.Buy();
    }
}
