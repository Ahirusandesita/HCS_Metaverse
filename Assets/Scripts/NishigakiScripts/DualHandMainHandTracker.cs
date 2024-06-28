using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class DualHandMainHandTracker : MonoBehaviour
{
    [SerializeField, Tooltip("OffHandTracker‚ÌGrabbable")]
    private Grabbable _offHandsGrabbable = default;

    public void Select()
    {
        // 
        _offHandsGrabbable.enabled = true;
    }

    public void UnSelect()
    {
        // 
        _offHandsGrabbable.enabled = false;
    }
}
