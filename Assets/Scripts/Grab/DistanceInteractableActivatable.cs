using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction;


public class DistanceInteractableActivatable : MonoBehaviour, IActivatableDistance
{
    [SerializeField]
    private float activeDistance;
    public float ActiveDistance => activeDistance;

    private DistanceHandGrabInteractable[] distanceHandGrabInteractables;
    private DistanceGrabInteractable[] distanceGrabInteractables;

    private void Awake()
    {
        distanceHandGrabInteractables = this.gameObject.GetComponentsInChildren<DistanceHandGrabInteractable>();
        distanceGrabInteractables = this.gameObject.GetComponentsInChildren<DistanceGrabInteractable>();
    }

    void IActivatableDistance.Active()
    {
        InteractableEnable(true);
    }

    void IActivatableDistance.Passive()
    {
        InteractableEnable(false);
    }

    private void InteractableEnable(bool active)
    {
        foreach (DistanceGrabInteractable distanceGrabInteractable in distanceGrabInteractables)
        {
            distanceGrabInteractable.enabled = active;
        }
        foreach (DistanceHandGrabInteractable distanceHandGrabInteractable in distanceHandGrabInteractables)
        {
            distanceHandGrabInteractable.enabled = active;
        }
    }

}
