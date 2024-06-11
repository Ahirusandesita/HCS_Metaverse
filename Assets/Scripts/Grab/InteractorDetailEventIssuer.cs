using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using System;
public enum HandType
{
    Left,
    Right
}
public enum InteractorType
{
    Select,
    UnSelect
}
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
}
public class InteractorDetailEventIssuer : MonoBehaviour
{
    public delegate void InteractorDetailEventHandler(InteractorDetailEventArgs ida);
    public event InteractorDetailEventHandler OnInteractor;

    [SerializeField]
    private List<InteractorUnityEventWrapper> leftInteractors;
    [SerializeField]
    private List<InteractorUnityEventWrapper> rightInteractors;

    private void Awake()
    {
        foreach (InteractorUnityEventWrapper interactorUnityEventWrapper in leftInteractors)
        {
            interactorUnityEventWrapper.WhenSelect.AddListener(() => { OnInteractor?.Invoke(new InteractorDetailEventArgs(HandType.Left, InteractorType.Select));
            });
            interactorUnityEventWrapper.WhenUnselect.AddListener(() => OnInteractor?.Invoke(new InteractorDetailEventArgs(HandType.Left, InteractorType.UnSelect)));
        }
        foreach (InteractorUnityEventWrapper interactorUnityEventWrapper in rightInteractors)
        {
            interactorUnityEventWrapper.WhenSelect.AddListener(() => {
                OnInteractor?.Invoke(new InteractorDetailEventArgs(HandType.Right, InteractorType.Select));
            });
            interactorUnityEventWrapper.WhenUnselect.AddListener(() => OnInteractor?.Invoke(new InteractorDetailEventArgs(HandType.Right, InteractorType.UnSelect)));
        }
    }
}
