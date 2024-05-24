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

        IPointerDragRegistrable[] pointerDragRegistrables = this.GetComponents<IPointerDragRegistrable>();
        EventTrigger.Entry entryPointerDrag = new EventTrigger.Entry();
        entryPointerDrag.eventID = EventTriggerType.Drag;
        entryPointerDrag.callback.AddListener((data) =>
        {
            foreach (IPointerDragRegistrable pointerDrag in pointerDragRegistrables)
            {
                pointerDrag.OnPointerDrag((PointerEventData)data);
            }
        }
        );

        IPointerDragRegistrableToParent[] pointerDragRegistrableToParents = this.GetComponentsInChildren<IPointerDragRegistrableToParent>(true);
        EventTrigger.Entry entryDragToParent = new EventTrigger.Entry();
        entryDragToParent.eventID = EventTriggerType.Drag;
        entryDragToParent.callback.AddListener((data) =>
        {
            foreach (IPointerDragRegistrableToParent pointerDragRegistrableToParent in pointerDragRegistrableToParents)
            {
                pointerDragRegistrableToParent.OnParentPointerDrag((PointerEventData)data);
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
}
