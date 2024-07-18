using Oculus.Interaction.HandGrab;
using Oculus.Interaction;
using UnityEngine;
public class OriginalHandGrabInteractor : HandGrabInteractor
{
    public void SetInteractable(DistanceGrabInteractable distanceGrabInteractable)
    {
        Debug.LogError("BBBBBBB");
        HandGrabInteractable handGrabInteractable = distanceGrabInteractable.transform.parent.GetComponentInChildren<HandGrabInteractable>();
        base.InteractableSet(handGrabInteractable);
    }
    public void UnSetInteractable(DistanceGrabInteractable distanceGrabInteractable)
    {
        Debug.LogError("BBBBBBB");
        HandGrabInteractable handGrabInteractable = distanceGrabInteractable.transform.parent.GetComponentInChildren<HandGrabInteractable>();
        base.InteractableUnset(handGrabInteractable);
    }

    public void SelectIntaractable(DistanceGrabInteractable interactable)
    {
        Debug.LogError("BBBBBBB");
        base.InteractableSelected(GetHandGrabInteractable(interactable));
    }
    public void UnSelectIntaractable(DistanceGrabInteractable interactable)
    {
        Debug.LogError("BBBBBBB");
        base.InteractableUnselected(GetHandGrabInteractable(interactable));
    }

    private HandGrabInteractable GetHandGrabInteractable(DistanceGrabInteractable interactable)
    {
        return interactable.transform.parent.GetComponentInChildren<HandGrabInteractable>();
    }
}
