using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

public struct Key
{
    public char keyChar;
    public bool canUseKey;
    public Key(char keyChar,bool canUseKey)
    {
        this.keyChar = keyChar;
        this.canUseKey = canUseKey;
    }
}
public class FlickParent : MonoBehaviour, IFlickButtonParent, IFlickButtonOpeningAndClosing
{
    [SerializeField]
    private char keyChar;

    private bool canButtonDown = true;
    private FlickManager flickManager;
    [SerializeField]
    private EventTrigger eventTrigger;

    bool canUseChildKey = false;
    private Key childKey;

    private IFlickButtonChild[] flickButtonChildren;

    [Inject]
    public void FlickChildInject(List<IFlickButtonChild> flickButtonChildren)
    {
        this.flickButtonChildren = flickButtonChildren.ToArray();
    }

    private void Awake()
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
    }

    private void PointerEnter()
    {
        canUseChildKey = true;
    }
    private void PointerDown()
    {
        if (!canButtonDown)
        {
            return;
        }

        flickManager.StartFlick(this);
        foreach (IFlickButtonChild item in flickButtonChildren)
        {
            item.ButtonDeployment();
        }
    }
    private void PointerUp()
    {
        if (!canButtonDown)
        {
            return;
        }

        flickManager.EndFlick(this);

        if (!canUseChildKey)
        {
            flickManager.SendMessage(childKey.keyChar);
        }
        foreach (IFlickButtonChild item in flickButtonChildren)
        {
            item.ButtonClose();
        }
    }
    private void PointerClick()
    {
        if (!canButtonDown)
        {
            return;
        }
        flickManager.SendMessage(keyChar);
    }

    void IFlickButtonOpeningAndClosing.Open()
    {
        canButtonDown = true;
    }
    void IFlickButtonOpeningAndClosing.Close()
    {
        canButtonDown = false;
    }
    public void SendMessage(Key key)
    {
        canUseChildKey = false;
        this.childKey = key;
    }
}
