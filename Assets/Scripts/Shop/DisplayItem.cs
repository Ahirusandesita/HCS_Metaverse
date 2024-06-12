using Oculus.Interaction;
using UnityEngine;

public class DisplayItem : MonoBehaviour, IDisplayItem
{
    [SerializeField] private PointableUnityEventWrapper onGrabbed = default;

    private ItemSelectArgs itemSelectArgs = default;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        try
        {
            onGrabbed ??= GetComponent<PointableUnityEventWrapper>();
        }
        catch (System.NullReferenceException) { }
    }

    void IDisplayItem.InjectItemSelectArgs(ItemSelectArgs itemSelectArgs)
    {
        this.itemSelectArgs = itemSelectArgs;
    }

    void IDisplayItem.InjectSelectedNotification(ISelectedNotification sn)
    {
        onGrabbed.WhenHover.AddListener(_ =>
        {
            sn.Select(itemSelectArgs);
        });
    }

    void IDisplayItem.InjectPointableUnityEventWrapper(PointableUnityEventWrapper puew)
    {
        onGrabbed = puew;
    }
}
