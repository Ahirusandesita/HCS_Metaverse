using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClick : MonoBehaviour
{
    private CanvasFixed canvasFixed;
    private bool isActive = false;
    private void Awake()
    {
        canvasFixed = this.GetComponent<CanvasFixed>();

        EventTrigger eventTrigger = this.GetComponent<EventTrigger>();

        EventTrigger.Entry entryPointerClick = new EventTrigger.Entry();
        entryPointerClick.eventID = EventTriggerType.PointerClick;
        entryPointerClick.callback.AddListener((x) => PointerClick());

        eventTrigger.triggers.Add(entryPointerClick);
    }
    private void PointerClick()
    {
        isActive = !isActive;
        canvasFixed.Fixed(isActive);
    }
}
