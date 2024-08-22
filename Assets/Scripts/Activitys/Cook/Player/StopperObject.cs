using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopperObject : MonoBehaviour
{
    [SerializeField, Tooltip("ê⁄êGîªíËÇçsÇ§Collider")]
    private Collider _stoperColliter = default;

    // 
    private Vector3 _hitBoxCenter = default;

    // 
    private Vector3 _hitBoxSize = default;

    // 
    private Quaternion _hitBoxRotation = default;

    // 
    private string _stoppableObjectsTag = "StoppableObject";

    private void Start()
    {
        // 
        _hitBoxCenter = _stoperColliter.bounds.center;

        // 
        _hitBoxSize = _stoperColliter.bounds.size;

        // 
        _hitBoxRotation = this.transform.rotation;
    }

    private void Update()
    {
        // 
        Collider[] hitColliders = Physics.OverlapBox(_hitBoxCenter, _hitBoxSize, _hitBoxRotation);

        // 
        foreach(Collider hitCollider in hitColliders)
        {
            // 
            if (!hitCollider.TryGetComponent<Stoppable>(out var tmp))
            {
                // 
                continue;
            }

            // 
            if (hitCollider.TryGetComponent<StopData>(out var stopData))
            {
                // 
                stopData.SetIsHitStopper(true);
            }
            // 
            else
            {
                // 
                hitCollider.gameObject.AddComponent<StopData>();
            }
        }
    }
}
