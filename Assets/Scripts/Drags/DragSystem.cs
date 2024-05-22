using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSystem : MonoBehaviour
{
    private IScrollable[] scrollables;

    private Vector2 move;
    private bool canScroll = false;
    private void Awake()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entryDrag = new EventTrigger.Entry();
        entryDrag.eventID = EventTriggerType.Drag;
        entryDrag.callback.AddListener((data) => { OnDragDelegate((PointerEventData)data); });
        trigger.triggers.Add(entryDrag);

        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((data) => PointerDown((PointerEventData)data));
        trigger.triggers.Add(entryPointerDown);

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((x) => PointerUp());
        trigger.triggers.Add(entryPointerUp);

        scrollables = this.transform.GetComponentsInChildren<IScrollable>(true);
    }

    private void OnDragDelegate(PointerEventData data)
    {
        if (!canScroll)
        {
            return;
        }

        Vector3 scrollMove = move - data.position;
        foreach (IScrollable scrollable in scrollables)
        {
            scrollable.Scroll(scrollMove);
            Debug.Log(scrollMove);
        }

        move = data.position;
    }

    private void PointerDown(PointerEventData data)
    {
        move = data.position;
        canScroll = true;
    }

    private void PointerUp()
    {
        canScroll = false;
    }
}
