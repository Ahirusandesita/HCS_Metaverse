using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class Item1 : MonoBehaviour, IDisplayItem
{
    [SerializeField] private InteractableUnityEventWrapper handGrab = default;
    [SerializeField] private InteractableUnityEventWrapper distanceHandGrab = default;
    [SerializeField] private InteractableUnityEventWrapper distanceGrab = default;

    public string Name => "item1";

    void IDisplayItem.InjectSelectedNotification(ISelectedNotification sn)
    {
        handGrab.WhenHover.AddListener(() =>
        {
            sn.Select(SelectArgs.Empty);
        });
    }
}
