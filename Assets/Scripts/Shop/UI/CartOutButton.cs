using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CartOutButton : MonoBehaviour
{
    [SerializeField]
    private UIIcon uiIconController = default;
    [SerializeField]
    private EventTrigger eventTrigger;
    private void Awake()
    {
        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((x) => OnPointerClick((PointerEventData)x));

        eventTrigger.triggers.Add(entryPointerUp);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        uiIconController.Delete();
    }
    [ContextMenu("test")]
    private void OnPointerClickTest()
	{
        uiIconController.Delete();
	}
}
