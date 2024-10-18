using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneTrackingController : MonoBehaviour
{
    [SerializeField]
    private Transform _trackingTransform = default;

    void Update()
    {
        transform.position = _trackingTransform.position;

        transform.rotation = _trackingTransform.rotation;
    }
}
