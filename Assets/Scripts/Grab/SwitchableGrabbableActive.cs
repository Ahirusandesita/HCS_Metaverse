using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using UnityEngine;
public interface IInject<T>
{
    void Inject(T t);
}
public class SwitchableGrabbableActive : MonoBehaviour,ISwitchableGrabbableActive
{
    private List<MonoBehaviour> interactables = new List<MonoBehaviour>();
    private void Awake()
    {
        foreach(MonoBehaviour item in this.GetComponentsInChildren<Grabbable>())
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

        foreach(IInject<ISwitchableGrabbableActive> inject in this.GetComponentsInChildren<IInject<ISwitchableGrabbableActive>>())
        {
            inject.Inject(this);
        }

    }
    void ISwitchableGrabbableActive.Active()
    {
        foreach (MonoBehaviour item in interactables)
        {
            item.enabled = true;
        }
    }

    void ISwitchableGrabbableActive.Inactive()
    {
        foreach (MonoBehaviour item in interactables)
        {
            item.enabled = false;
        }
    }
}
