using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using System;

public class InteractorDetailEventArgs : EventArgs
{
    public readonly HandType HandType;
    public readonly InteractorType InteractorType;
    public readonly Transform HandTransform;

    public InteractorDetailEventArgs(HandType handType,InteractorType interactorType)
    {
        this.HandType = handType;
        this.InteractorType = interactorType;
    }
    public InteractorDetailEventArgs(HandType handType, InteractorType interactorType,Transform handTransform)
    {
        this.HandType = handType;
        this.InteractorType = interactorType;
        this.HandTransform = handTransform;
    }
}
public delegate void InteractorDetailEventHandler(InteractorDetailEventArgs ida);
public class InteractorDetailEventIssuer : MonoBehaviour
{
   
    public event InteractorDetailEventHandler OnInteractor;

    [SerializeField]
    private List<InteractorManager> interactorManagers = new List<InteractorManager>();

    private void Awake()
    {
        foreach(InteractorManager interactor in interactorManagers)
        {
            interactor.OnInteractor += (ida) => OnInteractor?.Invoke(ida);
        }
    }
}
