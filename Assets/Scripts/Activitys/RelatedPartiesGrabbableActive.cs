using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelatedPartiesGrabbableActive : MonoBehaviour, IGrabbableActiveChangeRequester
{
    private ISwitchableGrabbableActive switchableGrabbableActive;
    private void Start()
    {
        this.switchableGrabbableActive = GetComponent<ISwitchableGrabbableActive>();
        switchableGrabbableActive.Regist(this);

        switch (RelatedParties.Instance.ActivityRelatedPartiesState)
        {
            case ActivityRelatedPartiesState.Participants:
                switchableGrabbableActive.Active(this);
                break;
            case ActivityRelatedPartiesState.Spectators:
                switchableGrabbableActive.Inactive(this);
                break;
        }
    }
}
