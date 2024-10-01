using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MarkViewEventArgs : System.EventArgs
{

}
public delegate void MarkClickHandler(MarkViewEventArgs markEventArgs);
public class MarkView : MonoBehaviour, IPointerClickHandler
{
    public event MarkClickHandler OnMarkClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        OnMarkClick?.Invoke(new MarkViewEventArgs());
    }
}
