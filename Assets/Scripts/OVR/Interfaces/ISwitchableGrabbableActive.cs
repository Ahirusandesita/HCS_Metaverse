using UnityEngine;
public interface ISwitchableGrabbableActive
{
    void Active(IGrabbableActiveChangeRequester grabbableActiveChangeRequester);
    void Inactive(IGrabbableActiveChangeRequester grabbableActiveChangeRequester);
    GameObject gameObject { get; }

    void Regist(IGrabbableActiveChangeRequester grabbableActiveChangeRequester);
    void Cancellation(IGrabbableActiveChangeRequester grabbableActiveChangeRequester);
}