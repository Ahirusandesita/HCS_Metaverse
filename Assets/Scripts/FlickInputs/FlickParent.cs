using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class FlickParent : MonoBehaviour
{
    protected FlickManager flickManager;
    [SerializeField]
    private EventTrigger eventTrigger;


    private Image image;
    private Color startColor;
    private Vector3 startSize;
    private Vector3 pushSize;
    protected float push_xSize = 0.37f;

    protected virtual void Awake()
    {
        flickManager = this.transform.root.GetComponentInChildren<FlickManager>(true);

        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((x) => PointerDown());

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((x) => PointerUp());

        EventTrigger.Entry entryPointerClick = new EventTrigger.Entry();
        entryPointerClick.eventID = EventTriggerType.PointerClick;
        entryPointerClick.callback.AddListener((x) => PointerClick());


        EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry();
        entryPointerEnter.eventID = EventTriggerType.PointerEnter;
        entryPointerEnter.callback.AddListener((x) => PointerEnter());
        eventTrigger.triggers.Add(entryPointerEnter);

        eventTrigger.triggers.Add(entryPointerDown);
        eventTrigger.triggers.Add(entryPointerUp);
        eventTrigger.triggers.Add(entryPointerClick);

        image = GetComponent<Image>();
        startColor = image.color;

        startSize = transform.localScale;
        pushSize = startSize;
        pushSize.x = push_xSize;
    }

    protected void PointerDownAnimation()
    {
        image.color = ButtonColor.PushColor;
        transform.localScale = pushSize;
    }

    protected void PointerUpAnimation()
    {
        image.color = startColor;
        transform.localScale = startSize;
    }

    protected abstract void PointerEnter();
    protected abstract void PointerDown();

    protected abstract void PointerUp();
    protected abstract void PointerClick();
}
