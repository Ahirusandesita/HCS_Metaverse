using Oculus.Interaction;
using UnityEngine;

public class Item2 : MonoBehaviour, IDisplayItem
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
        distanceGrab.WhenHover.AddListener(() =>
        {
            sn.Select(new ItemSelectArgs(ID));
        });
        distanceHandGrab.WhenHover.AddListener(() =>
        {
            sn.Select(new ItemSelectArgs(ID));
        });
    }
}
