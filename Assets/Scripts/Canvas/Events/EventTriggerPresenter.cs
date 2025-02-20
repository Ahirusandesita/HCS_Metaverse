using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerPresenter : MonoBehaviour
{
    private EventTrigger eventTrigger;

    private void Awake()
    {
        eventTrigger = this.GetComponent<EventTrigger>();

        IPointerUpRegistrable[] pointerUpRegistrables = this.GetComponents<IPointerUpRegistrable>();
        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((data) =>
        {
            foreach (IPointerUpRegistrable pointerUp in pointerUpRegistrables)
            {
                pointerUp.OnPointerUp((PointerEventData)data);
            }
        }
        );

        IPointerDownRegistrable[] pointerDownRegistrables = this.GetComponents<IPointerDownRegistrable>();
        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((data) =>
        {
            foreach (IPointerDownRegistrable pointerDown in pointerDownRegistrables)
            {
                pointerDown.OnPointerDown((PointerEventData)data);
            }
        }
        );

        IPointerEnterRegistrable[] pointerEnterRegistrables = this.GetComponents<IPointerEnterRegistrable>();
        EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry();
        entryPointerEnter.eventID = EventTriggerType.PointerEnter;
        entryPointerEnter.callback.AddListener((data) =>
        {
            foreach (IPointerEnterRegistrable pointerEnter in pointerEnterRegistrables)
            {
                pointerEnter.OnPointerEnter((PointerEventData)data);
            }
        }
        );

        IPointerExitRegistrable[] pointerExitRegistrables = this.GetComponents<IPointerExitRegistrable>();
        EventTrigger.Entry entryPointerExit = new EventTrigger.Entry();
        entryPointerExit.eventID = EventTriggerType.PointerExit;
        entryPointerExit.callback.AddListener((data) =>
        {
            foreach (IPointerExitRegistrable pointerExit in pointerExitRegistrables)
            {
                pointerExit.OnPointerExit((PointerEventData)data);
            }
        }
        );

        IPointerClickRegistrable[] pointerClickRegistrables = this.GetComponents<IPointerClickRegistrable>();
        EventTrigger.Entry entryPointerClick = new EventTrigger.Entry();
        entryPointerClick.eventID = EventTriggerType.PointerClick;
        entryPointerClick.callback.AddListener((data) =>
        {
            foreach (IPointerClickRegistrable pointerClick in pointerClickRegistrables)
            {
                pointerClick.OnPointerClick((PointerEventData)data);
            }
        }
        );

        IDragRegistrable[] pointerDragRegistrables = this.GetComponents<IDragRegistrable>();
        EventTrigger.Entry entryPointerDrag = new EventTrigger.Entry();
        entryPointerDrag.eventID = EventTriggerType.Drag;
        entryPointerDrag.callback.AddListener((data) =>
        {
            foreach (IDragRegistrable pointerDrag in pointerDragRegistrables)
            {
                pointerDrag.OnDrag((PointerEventData)data);
            }
        }
        );

        IDragRegistrableToParent[] pointerDragRegistrableToParents = this.GetComponentsInChildren<IDragRegistrableToParent>(true);
        EventTrigger.Entry entryDragToParent = new EventTrigger.Entry();
        entryDragToParent.eventID = EventTriggerType.Drag;
        entryDragToParent.callback.AddListener((data) =>
        {
            foreach (IDragRegistrableToParent pointerDragRegistrableToParent in pointerDragRegistrableToParents)
            {
                pointerDragRegistrableToParent.OnParentDrag((PointerEventData)data);
            }
        }
        );

        eventTrigger.triggers.Add(entryPointerUp);
        eventTrigger.triggers.Add(entryPointerDown);
        eventTrigger.triggers.Add(entryPointerEnter);
        eventTrigger.triggers.Add(entryPointerExit);
        eventTrigger.triggers.Add(entryPointerClick);
        eventTrigger.triggers.Add(entryPointerDrag);
        eventTrigger.triggers.Add(entryDragToParent);
    }
    public void Add(IPointerClickRegistrable registrable)
    {
        EventTrigger.Entry entryPointerClick = new EventTrigger.Entry();
        entryPointerClick.eventID = EventTriggerType.PointerClick;
        entryPointerClick.callback.AddListener((data) =>
        {

            registrable.OnPointerClick((PointerEventData)data);

        }
        );

        eventTrigger.triggers.Add(entryPointerClick);
    }
}

public static class EventRegisterExtends
{
    public static void Retister(this IPointerClickRegistrable registrable, MonoBehaviour me)
    {
        me.transform.root.GetComponentInChildren<EventTriggerPresenter>().Add(registrable);
    }
}
