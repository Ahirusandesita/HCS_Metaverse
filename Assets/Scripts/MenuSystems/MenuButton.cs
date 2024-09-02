using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MenuButton : MonoBehaviour
{
    private EventTrigger eventTrigger;
    private IMenuManager menuManager;
    private void Awake()
    {
        eventTrigger = this.GetComponent<EventTrigger>() == null ? this.gameObject.AddComponent<EventTrigger>() : this.GetComponent<EventTrigger>();

        EventTrigger.Entry entryPointerClick = new EventTrigger.Entry();
        entryPointerClick.eventID = EventTriggerType.PointerClick;
        entryPointerClick.callback.AddListener((x) => StartMenu());
        eventTrigger.triggers.Add(entryPointerClick);
    }
    public abstract void StartMenu();
    public abstract void EndMenu();

    private void ActiveMenu()
    {
        menuManager.ActiveMenu(this);
    }

    public void InjectMenuManager(IMenuManager menuManager)
    {
        this.menuManager = menuManager;
    }
}
