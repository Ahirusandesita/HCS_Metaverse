using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class InteractorManager : MonoBehaviour
{
    [SerializeField]
    private Transform handTransform;
    [SerializeField]
    private InteractorUnityEventWrapper interactorUnityEventWrapper;
    [SerializeField]
    private HandType handType;

    public event InteractorDetailEventHandler OnInteractor;

    private void Awake()
    {
        interactorUnityEventWrapper.WhenSelect.AddListener(() => OnInteractor?.Invoke(new InteractorDetailEventArgs(handType, InteractorType.Select, handTransform)));
        interactorUnityEventWrapper.WhenUnselect.AddListener(() => OnInteractor?.Invoke(new InteractorDetailEventArgs(handType, InteractorType.UnSelect, handTransform)));
    }
}
