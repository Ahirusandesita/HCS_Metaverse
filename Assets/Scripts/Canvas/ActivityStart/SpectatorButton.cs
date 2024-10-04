using UnityEngine;
using UnityEngine.EventSystems;
public class SpectatorButton : MonoBehaviour,IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        RelatedParties.Instance.ActivityRelatedPartiesState = ActivityRelatedPartiesState.Spectators;
    }
}
