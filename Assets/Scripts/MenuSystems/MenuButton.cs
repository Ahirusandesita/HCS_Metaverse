using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    //private EventTrigger eventTrigger;
    private IMenuManager menuManager;
    public Action action;
    private void Awake()
    {
        //eventTrigger = this.GetComponent<EventTrigger>() == null ? this.gameObject.AddComponent<EventTrigger>() : this.GetComponent<EventTrigger>();

        //EventTrigger.Entry entryPointerClick = new EventTrigger.Entry();
        //entryPointerClick.eventID = EventTriggerType.PointerClick;
        //entryPointerClick.callback.AddListener((x) => StartMenu());
        //entryPointerClick.callback.AddListener((x) => ActiveMenu());
        //eventTrigger.triggers.Add(entryPointerClick);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        StartMenu();
        ActiveMenu();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.GetComponent<Image>().color = Color.grey;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.GetComponent<Image>().color = Color.white;
    }
}
