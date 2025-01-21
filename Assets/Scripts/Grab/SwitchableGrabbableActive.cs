using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using UnityEngine;
public interface IGrabbableActiveChangeRequester
{
}


public class SwitchableGrabbableActive : MonoBehaviour, ISwitchableGrabbableActive
{
    private class RequesterInformation
    {
        public readonly IGrabbableActiveChangeRequester GrabbableActiveChangeRequester;
        public bool IsActive { get; set; }
        
        public RequesterInformation(IGrabbableActiveChangeRequester grabbableActiveChangeRequester)
        {
            this.GrabbableActiveChangeRequester = grabbableActiveChangeRequester;
            IsActive = true;
        }
    }
    private List<MonoBehaviour> interactables = new List<MonoBehaviour>();
    private List<RequesterInformation> grabbableActiveChangeRequesters = new List<RequesterInformation>();
    private void Awake()
    {
        foreach (MonoBehaviour item in this.GetComponentsInChildren<Grabbable>())
        {
            interactables.Add(item);
        }
        foreach (MonoBehaviour item in this.gameObject.GetComponentsInChildren<DistanceHandGrabInteractable>())
        {
            interactables.Add(item);
        }
        foreach (MonoBehaviour item in this.gameObject.GetComponentsInChildren<DistanceGrabInteractable>())
        {
            interactables.Add(item);
        }
        foreach (MonoBehaviour item in this.gameObject.GetComponentsInChildren<HandGrabInteractable>())
        {
            interactables.Add(item);
        }
        foreach (MonoBehaviour item in this.gameObject.GetComponentsInChildren<GrabInteractable>())
        {
            interactables.Add(item);
        }

        foreach (IInject<ISwitchableGrabbableActive> inject in this.GetComponentsInChildren<IInject<ISwitchableGrabbableActive>>())
        {
            inject.Inject(this);
        }

    }
    void ISwitchableGrabbableActive.Active(IGrabbableActiveChangeRequester grabbableActiveChangeRequester)
    {
        Search(grabbableActiveChangeRequester).IsActive = true;

        int i = 0;
        for(i = 0; i < grabbableActiveChangeRequesters.Count; i++)
        {
            if (!grabbableActiveChangeRequesters[i].IsActive)
            {
                break;
            }
        }

        if(i == grabbableActiveChangeRequesters.Count)
        {
            foreach (MonoBehaviour item in interactables)
            {
                item.enabled = true;
            }
        }
    }

    void ISwitchableGrabbableActive.Inactive(IGrabbableActiveChangeRequester grabbableActiveChangeRequester)
    {
        Search(grabbableActiveChangeRequester).IsActive = false;

        foreach(RequesterInformation requesterInformation in grabbableActiveChangeRequesters)
        {
            if (!requesterInformation.IsActive)
            {
                foreach (MonoBehaviour item in interactables)
                {
                    item.enabled = false;
                }
                break;
            }
        }
    }

    void ISwitchableGrabbableActive.Regist(IGrabbableActiveChangeRequester grabbableActiveChangeRequester)
    {
        this.grabbableActiveChangeRequesters.Add(new RequesterInformation(grabbableActiveChangeRequester));
    }
    void ISwitchableGrabbableActive.Cancellation(IGrabbableActiveChangeRequester grabbableActiveChangeRequester)
    {
        this.grabbableActiveChangeRequesters.Remove(Search(grabbableActiveChangeRequester));
    }

    private RequesterInformation Search(IGrabbableActiveChangeRequester grabbableActiveChangeRequester)
    {
        foreach(RequesterInformation requesterInformation in grabbableActiveChangeRequesters)
        {
            if(requesterInformation.GrabbableActiveChangeRequester == grabbableActiveChangeRequester)
            {
                return requesterInformation;
            }
        }

        return null; //!!!!!!!!!!!!!!!!!!!
    }
}
