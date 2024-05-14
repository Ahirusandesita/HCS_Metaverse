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

        flickManager = this.transform.parent.GetComponent<FlickManager>();
    }

    private void PointerDown()
    {
        if (!flickManager.IsPushScreen)
        {
            LetButtonMe();
            isStartPush = true;
        }
        LetButton();
    }

    private void PointerUp()
    {
        if (isStartPush)
        {
            isStartPush = false;
            StartPushButton();
        }

        PushButton();
    }

    protected abstract void StartPushButton();
    protected abstract void PushButton();
    protected abstract void LetButton();
    protected virtual void LetButtonMe()
    {

    }
}
