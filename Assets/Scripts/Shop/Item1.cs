using Oculus.Interaction;
using UnityEngine;

public class Item1 : MonoBehaviour, IDisplayItem
{
    [SerializeField] private PointableUnityEventWrapper onGrabbed = default;

    public int ID { get; private set; }
    public string Name { get; private set; }

    void IDisplayItem.SetIDAndItemName(int id, string name)
    {
        ID = id;
        Name = name;
    }

    void IDisplayItem.InjectSelectedNotification(ISelectedNotification sn)
    {
        onGrabbed.WhenHover.AddListener(_ =>
        {
            sn.Select(new ItemSelectArgs(ID));
        });
    }

    void IDisplayItem.InjectPointableUnityEventWrapper(PointableUnityEventWrapper puew)
    {
        onGrabbed = puew;
    }
}
