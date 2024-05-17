using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using TMPro;

public class FlickChild : MonoBehaviour, IFlickButtonChild
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;

    [SerializeField]
    private char keyChar;

    [SerializeField]
    private EventTrigger eventTrigger;
    private IFlickButtonParent flickButtonParent;

    [Inject]
    public void FlickParentInject(IFlickButtonParent flickButtonParent)
    {
        this.flickButtonParent = flickButtonParent;
    }

    private void Awake()
    {
        EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry();
        entryPointerEnter.eventID = EventTriggerType.PointerEnter;
        entryPointerEnter.callback.AddListener((x) => PointerEnter());

        EventTrigger.Entry entryPointerExit = new EventTrigger.Entry();
        entryPointerExit.eventID = EventTriggerType.PointerExit;
        entryPointerExit.callback.AddListener((x) => PointerExit());

        eventTrigger.triggers.Add(entryPointerExit);
        eventTrigger.triggers.Add(entryPointerEnter);

        textMeshProUGUI.enabled = false;
        image.enabled = false;
    }

    private void PointerEnter()
    {
        flickButtonParent.SendMessage(new Key(keyChar,true));
    }
    private void PointerExit()
    {
        flickButtonParent.SendMessage(new Key(keyChar, false));
    }

    void IFlickButtonChild.ButtonClose()
    {
        textMeshProUGUI.enabled = false;
        image.enabled = false;
    }

    void IFlickButtonChild.ButtonDeployment()
    {
        textMeshProUGUI.enabled = true;
        image.enabled = true;
    }
}
