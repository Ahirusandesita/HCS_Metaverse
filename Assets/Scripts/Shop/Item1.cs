using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class Item1 : MonoBehaviour, IDisplayItem
{
    [SerializeField] private InteractableUnityEventWrapper handGrab = default;
    [SerializeField] private InteractableUnityEventWrapper distanceHandGrab = default;
    [SerializeField] private InteractableUnityEventWrapper distanceGrab = default;

    public int ID { get; private set; }
    public string Name { get; private set; }

    void IDisplayItem.SetIDAndItemName(int id, string name)
    {
        ID = id;
        Name = name;
    }

    void IDisplayItem.InjectSelectedNotification(ISelectedNotification sn)
    {
        handGrab.WhenHover.AddListener(() =>
        {
            sn.Select(new ItemSelectArgs(ID));
        });
    }
}
