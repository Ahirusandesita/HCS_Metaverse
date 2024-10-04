using UnityEngine;

public class SpectatorButton : MonoBehaviour
{
    public void Click()
	{
		RelatedParties.Instance.ActivityRelatedPartiesState = ActivityRelatedPartiesState.Spectators;
	}
}
