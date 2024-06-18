using Oculus.Interaction;
using UnityEngine;

[RequireComponent(typeof(OutlineManager))]
public class DisplayItem : MonoBehaviour, IDisplayItem
{
    [SerializeField] private PointableUnityEventWrapper onGrabbed = default;
    private ItemSelectArgs itemSelectArgs = default;
    private Outline outline = default;


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
        onGrabbed.WhenSelect.AddListener(_ =>
        {
            sn.Select(itemSelectArgs);
        });

        onGrabbed.WhenUnselect.AddListener(_ =>
        {
            sn.Unselect(itemSelectArgs);
        });

        onGrabbed.WhenHover.AddListener(_ =>
        {
            sn.Hover(itemSelectArgs);
            outline.enabled = true;
        });

        onGrabbed.WhenUnhover.AddListener(_ =>
        {
            sn.Unhover(itemSelectArgs);
            outline.enabled = false;
        });
    }

    void IDisplayItem.InjectPointableUnityEventWrapper(PointableUnityEventWrapper puew)
    {
        onGrabbed = puew;
    }
}
