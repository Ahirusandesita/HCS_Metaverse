using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ReticleDependencyProvider : MonoBehaviour, IDependencyProvider<ReticleDependencyInformation>
{
    [SerializeField]
    ReticleDependencyInformation reticleDependencyInformation;
    [SerializeField]
    private DistanceHandGrabInteractor distanceHandGrabInteractor;
    [SerializeField]
    private DistanceGrabInteractor distanceGrabInteractor;
    [SerializeField]
    private HandRef handRef;
    [SerializeField]
    private Hand hand;
    [SerializeField]
    private Transform centerAye;

    public ReticleDependencyInformation Information =>
        new ReticleDependencyInformation(
        distanceHandGrabInteractor,
        distanceGrabInteractor,
        handRef,
        hand,
        centerAye
            );
}
