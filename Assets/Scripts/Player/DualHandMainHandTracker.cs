using UnityEngine;
using Oculus.Interaction;

public class DualHandMainHandTracker : MonoBehaviour
{
    [SerializeField, Tooltip("OffHandTracker��Grabbable")]
    private Grabbable _offHandsGrabbable = default;

    public void Select()
    {
        // OffHandTracker��͂߂�悤�ɂ���
        _offHandsGrabbable.enabled = true;
    }

    public void UnSelect()
    {
        // OffHandTracker��͂߂Ȃ��悤�ɂ���
        _offHandsGrabbable.enabled = false;
    }
}
