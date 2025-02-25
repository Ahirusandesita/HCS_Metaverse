using UnityEngine;
using Oculus.Interaction;

public class DualHandMainHandTracker : MonoBehaviour
{
    [SerializeField, Tooltip("OffHandTrackerのGrabbable")]
    private Grabbable _offHandsGrabbable = default;

    public void Select()
    {
        // OffHandTrackerを掴めるようにする
        _offHandsGrabbable.enabled = true;
    }

    public void UnSelect()
    {
        // OffHandTrackerを掴めないようにする
        _offHandsGrabbable.enabled = false;
    }
}
