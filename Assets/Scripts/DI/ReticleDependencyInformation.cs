using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ReticleDependencyInformation : DependencyInformation
{
    public readonly DistanceHandGrabInteractor DistanceHandGrabInteractor;
    public readonly DistanceGrabInteractor DistanceGrabInteractor;
    public readonly HandRef HandRef;
    public readonly Hand Hand;
    public readonly Transform CenterAye;

    public ReticleDependencyInformation(DistanceHandGrabInteractor distanceHandGrabInteractor, DistanceGrabInteractor distanceGrabInteractor, HandRef handRef, Hand hand, Transform centerAye)
    {
        this.DistanceHandGrabInteractor = distanceHandGrabInteractor;
        this.DistanceGrabInteractor = distanceGrabInteractor;
        this.HandRef = handRef;
        this.Hand = hand;
        this.CenterAye = centerAye;
    }
}
