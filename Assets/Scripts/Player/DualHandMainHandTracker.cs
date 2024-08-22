using UnityEngine;
using Oculus.Interaction;

public class DualHandMainHandTracker : MonoBehaviour
{
    [SerializeField, Tooltip("OffHandTracker‚ÌGrabbable")]
    private Grabbable _offHandsGrabbable = default;

    public void Select()
    {
        // OffHandTracker‚ð’Í‚ß‚é‚æ‚¤‚É‚·‚é
        _offHandsGrabbable.enabled = true;
    }

    public void UnSelect()
    {
        // OffHandTracker‚ð’Í‚ß‚È‚¢‚æ‚¤‚É‚·‚é
        _offHandsGrabbable.enabled = false;
    }
}
