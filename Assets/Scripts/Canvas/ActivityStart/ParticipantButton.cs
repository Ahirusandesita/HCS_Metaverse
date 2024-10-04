using UnityEngine;

public class ParticipantButton : MonoBehaviour
{
    public void Click()
	{
		RelatedParties.Instance.ActivityRelatedPartiesState = ActivityRelatedPartiesState.Participants;
	}
}
