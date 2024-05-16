using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private Key childKey;

    private IFlickButtonChild[] flickButtonChilds;
    private void Awake()
    {
        flickManager = this.transform.root.GetComponent<FlickManager>();
        flickButtonChilds = this.GetComponentsInChildren<IFlickButtonChild>(true);

        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((x) => PointerDown());

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((x) => PointerUp());

        EventTrigger.Entry entryPointerClick = new EventTrigger.Entry();
        entryPointerClick.eventID = EventTriggerType.PointerClick;
        entryPointerClick.callback.AddListener((x) => PointerClick());

        eventTrigger.triggers.Add(entryPointerDown);
        eventTrigger.triggers.Add(entryPointerUp);
        eventTrigger.triggers.Add(entryPointerClick);
    }

    private void PointerDown()
    {
        if (!canButtonDown)
        {
            return;
        }

        flickManager.StartFlick(this);
        foreach (IFlickButtonChild item in flickButtonChilds)
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

        if (childKey.canUseKey)
        {
            flickManager.SendMessage(childKey.keyChar);
        }
        foreach (IFlickButtonChild item in flickButtonChilds)
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
        this.childKey = key;
    }
}
