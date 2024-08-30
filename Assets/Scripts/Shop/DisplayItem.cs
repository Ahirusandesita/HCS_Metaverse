using Oculus.Interaction;
using UnityEngine;

/// <summary>
/// GrabbableかつCloneなオブジェクト。生成元にVR特有の通知（Hover, Select）を送信する役割。
/// </summary>
[RequireComponent(typeof(OutlineManager))]
public class DisplayItem : MonoBehaviour, IDisplayItem
{
    [SerializeField] private PointableUnityEventWrapper onGrabbed = default;
    private ItemSelectArgs itemSelectArgs = default;
    private ISelectedNotification sn = default;


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        onGrabbed ??= GetComponent<PointableUnityEventWrapper>();
    }

    void IDisplayItem.Inject_ItemSelectArgsAndSelectedNotification(ItemSelectArgs itemSelectArgs, ISelectedNotification sn)
    {
        this.itemSelectArgs = itemSelectArgs;
        this.sn = sn;

        onGrabbed.WhenSelect.AddListener(WhenSelect);
        onGrabbed.WhenUnselect.AddListener(WhenUnselect);
        onGrabbed.WhenHover.AddListener(WhenHover);
        onGrabbed.WhenUnhover.AddListener(WhenUnhover);
    }

    void IDisplayItem.InjectPointableUnityEventWrapper(PointableUnityEventWrapper puew)
    {
        onGrabbed = puew;
    }

    private void WhenSelect(PointerEvent pointerEvent)
    {
        sn.Select(itemSelectArgs);

        onGrabbed.WhenSelect.RemoveListener(WhenSelect);
        onGrabbed.WhenHover.RemoveListener(WhenHover);
        onGrabbed.WhenUnhover.RemoveListener(WhenUnhover);
    }

    private void WhenUnselect(PointerEvent pointerEvent)
    {
        sn.Unselect(itemSelectArgs);
        onGrabbed.WhenUnselect.RemoveListener(WhenUnselect);
    }

    private void WhenHover(PointerEvent pointerEvent)
    {
        sn.Hover(itemSelectArgs);
    }

    private void WhenUnhover(PointerEvent pointerEvent)
    {
        sn.Unhover(itemSelectArgs);
    }
}