using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MarkViewEventArgs : System.EventArgs
{

}
public delegate void MarkClickHandler(MarkViewEventArgs markEventArgs);
public class MarkView : MonoBehaviour
{
    private EventTrigger eventTrigger;

    public event MarkClickHandler OnMarkClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogError("é¿çs");
        if (this.GetComponent<RectTransform>().Contains(eventData))
        {
            OnMarkClick?.Invoke(new MarkViewEventArgs());
            Debug.LogError("îÕàÕì‡");
        }
        else
        {
            Debug.LogError("îÕàÕäO");
        }
    }

    private void Start()
    {
        eventTrigger = transform.parent.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerClick;
        entryPointerUp.callback.AddListener((x) => OnPointerClick((PointerEventData)x));
        eventTrigger.triggers.Add(entryPointerUp);
    }
}

public static class RectTransformExtension
{
    private static readonly Vector3[] s_Corners = new Vector3[4];

    public static bool Contains(this RectTransform self, PointerEventData eventData)
    {
        var selfBounds = GetBounds(self);
        var worldPos = Vector3.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            self,
            eventData.position,
            eventData.pressEventCamera,
            out worldPos);
        worldPos.z = 0f;
        return selfBounds.Contains(worldPos);
    }

    public static bool Contains(this RectTransform self, RectTransform target)
    {
        var selfBounds = GetBounds(self);
        var targetBounds = GetBounds(target);
        return selfBounds.Contains(new Vector3(targetBounds.min.x, targetBounds.min.y, 0f)) &&
               selfBounds.Contains(new Vector3(targetBounds.max.x, targetBounds.max.y, 0f)) &&
               selfBounds.Contains(new Vector3(targetBounds.min.x, targetBounds.max.y, 0f)) &&
               selfBounds.Contains(new Vector3(targetBounds.max.x, targetBounds.min.y, 0f));
    }

    /// <summary>
    /// BoundsÇéÊìæ
    /// </summary>
    private static Bounds GetBounds(this RectTransform self)
    {
        var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        self.GetWorldCorners(s_Corners);
        for (var index2 = 0; index2 < 4; ++index2)
        {
            min = Vector3.Min(s_Corners[index2], min);
            max = Vector3.Max(s_Corners[index2], max);
        }

        max.z = 0f;
        min.z = 0f;

        Bounds bounds = new Bounds(min, Vector3.zero);
        bounds.Encapsulate(max);
        return bounds;
    }
}
