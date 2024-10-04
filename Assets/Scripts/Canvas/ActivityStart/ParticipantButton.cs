using UnityEngine;
using UnityEngine.EventSystems;
public class ParticipantButton : MonoBehaviour,IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogWarning("dadada");
        RelatedParties.Instance.ActivityRelatedPartiesState = ActivityRelatedPartiesState.Participants;
    }
}
