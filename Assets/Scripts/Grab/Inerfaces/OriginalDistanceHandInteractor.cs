using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
public class OriginalDistanceHandInteractor : DistanceGrabInteractor
{
    [SerializeField]
    private OriginalHandGrabInteractor originalInteractor;
    protected override void InteractableSet(DistanceGrabInteractable interactable)
    {
        Debug.LogError("BBBBBBB");
        originalInteractor.SetInteractable(interactable);
    }

    protected override void InteractableUnset(DistanceGrabInteractable interactable)
    {
        Debug.LogError("BBBBBBB");
        originalInteractor.UnSetInteractable(interactable);
    }

    protected override void InteractableSelected(DistanceGrabInteractable interactable)
    {
        Debug.LogError("BBBBBBB");
        originalInteractor.SelectIntaractable(interactable);
    }
    protected override void InteractableUnselected(DistanceGrabInteractable interactable)
    {
        Debug.LogError("BBBBBBB");
        originalInteractor.UnSelectIntaractable(interactable);
    }
}
