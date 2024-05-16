using UnityEngine;
using UnityEngine.EventSystems;

public abstract class FlickButton : MonoBehaviour
{
    [SerializeField]
    private EventTrigger eventTrigger;
    [SerializeField]
    protected char keyChar;

    private FlickManager flickManager;
    protected FlickManager FlickManager
    {
        get
        {
            return flickManager;
        }
    }
    private bool isStartPush = true;

    protected virtual void Awake()
    {
        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((x) => PointerDown());
        eventTrigger.triggers.Add(entryPointerDown);

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((x) => PointerUp());
        eventTrigger.triggers.Add(entryPointerUp);

        EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry();
        entryPointerEnter.eventID = EventTriggerType.PointerEnter;
        entryPointerEnter.callback.AddListener((x) => PointerEnter());
        eventTrigger.triggers.Add(entryPointerEnter);

        EventTrigger.Entry entryPointerExit = new EventTrigger.Entry();
        entryPointerExit.eventID = EventTriggerType.PointerExit;
        entryPointerExit.callback.AddListener((x) => PointerExit());
        eventTrigger.triggers.Add(entryPointerExit);



        EventTrigger.Entry entryPointerExitEnd = new EventTrigger.Entry();
        entryPointerExitEnd.eventID = EventTriggerType.PointerExit;
        entryPointerExitEnd.callback.AddListener((x) => EndCanvasEnterTouch());
        eventTrigger.triggers.Add(entryPointerExitEnd);

        EventTrigger.Entry entryPointerClickEnd = new EventTrigger.Entry();
        entryPointerClickEnd.eventID = EventTriggerType.PointerClick;
        entryPointerClickEnd.callback.AddListener((x) => EndClickCanvasTouch());
        eventTrigger.triggers.Add(entryPointerClickEnd);

        EventTrigger.Entry entryPointerDownEnd = new EventTrigger.Entry();
        entryPointerDownEnd.eventID = EventTriggerType.PointerUp;
        entryPointerDownEnd.callback.AddListener((x) => EndCanvasTouch());
        eventTrigger.triggers.Add(entryPointerDownEnd);

        flickManager = this.transform.root.GetComponent<FlickManager>();
    }

    private void EndClickCanvasTouch()
    {
        //if (!FlickManager.IsPushScreen)
        //{
            EndClickTouch();
        //}
    }

    private void EndCanvasTouch()
    {
        EndTouch();
    }

    private void EndCanvasEnterTouch()
    {
        //if (!FlickManager.IsPushScreen)
        //{
            EndEnterTouch();
        //}
    }

    protected virtual void PointerDown() { }
    protected virtual void PointerUp() { }
    protected virtual void PointerEnter() { }
    protected virtual void PointerExit() { }
    protected abstract void EndClickTouch();
    protected abstract void EndTouch();
    protected abstract void EndEnterTouch();
}
