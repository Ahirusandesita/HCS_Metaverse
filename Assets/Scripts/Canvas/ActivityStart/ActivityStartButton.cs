using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ActivityStartButton : MonoBehaviour, IPointerClickHandler
{
    [ContextMenu("Click")]
    public void OnPointerClick(PointerEventData eventData)
    {
        GateOfFusion.Instance.ActivityStart();
    }
}
