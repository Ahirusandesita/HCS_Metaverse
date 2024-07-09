using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoperObject : MonoBehaviour
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

    // 
    private enum DebugMode_StoperObject
    {
        FixedUpdate,
        Update,
        LateUpdate
    }

    [SerializeField]
    DebugMode_StoperObject mode = DebugMode_StoperObject.Update;

    private void Start()
    {
        // 
        _hitBoxCenter = _stoperColliter.bounds.center;

        // 
        _hitBoxSize = _stoperColliter.bounds.size / 2;

        // 
        _hitBoxRotation = this.transform.rotation;
    }

    private void Update()
    {
        if (mode == DebugMode_StoperObject.Update)
        {
            // 
            Collider[] hitColliders = Physics.OverlapBox(_hitBoxCenter, _hitBoxSize, _hitBoxRotation);

            bool onFlag = false;

            if (hitColliders is null)
            {
                Debug.Log($"Ç»Ç…Ç‡ìñÇΩÇ¡ÇƒÇ»Ç¢ÇÊÇÒ");
                return;
            }

            // 
            foreach(Collider hitCollider in hitColliders)
            {
                // 
                if (!hitCollider.TryGetComponent<Stoppable>(out var tmp))
                {
                    // 
                    continue;
                }

                onFlag = true;

                // 
                if (hitCollider.TryGetComponent<StopData>(out var stopData))
                {
                    // 
                    stopData.SetIsHitStopper(true);

                    Debug.Log($"{hitCollider.gameObject.name} is Stopping now");
                }
                // 
                else
                {
                    // 
                    hitCollider.gameObject.AddComponent<StopData>();

                    tmp.StoppingEvent();

                    Debug.Log($"{hitCollider.gameObject.name} Ç… StopDataí«â¡ÇµÇΩÇÊÇÒ");
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (mode == DebugMode_StoperObject.FixedUpdate)
        {
            // 
            Collider[] hitColliders = Physics.OverlapBox(_hitBoxCenter, _hitBoxSize, _hitBoxRotation);

            bool onFlag = false;

            // 
            foreach (Collider hitCollider in hitColliders)
            {
                // 
                if (!hitCollider.TryGetComponent<Stoppable>(out var tmp))
                {
                    // 
                    continue;
                }

                onFlag = true;

                // 
                if (hitCollider.TryGetComponent<StopData>(out var stopData))
                {
                    // 
                    stopData.SetIsHitStopper(true);

                    Debug.Log($"{hitCollider.gameObject.name} is Stopping now");
                }
                // 
                else
                {
                    // 
                    hitCollider.gameObject.AddComponent<StopData>();

                    Debug.Log($"{hitCollider.gameObject.name} Ç… StopDataí«â¡ÇµÇΩÇÊÇÒ");
                }
            }

            if (!onFlag)
            {
                Debug.Log($"Ç»Ç…Ç‡Ç∆ÇﬂÇƒÇ»Ç¢ÇÊÇÒ");
            }
        }
    }

    private void LateUpdate()
    {
        if (mode == DebugMode_StoperObject.LateUpdate)
        {
            // 
            Collider[] hitColliders = Physics.OverlapBox(_hitBoxCenter, _hitBoxSize, _hitBoxRotation);

            bool onFlag = false;

            // 
            foreach (Collider hitCollider in hitColliders)
            {
                // 
                if (!hitCollider.TryGetComponent<Stoppable>(out var tmp))
                {
                    // 
                    continue;
                }

                onFlag = true;

                // 
                if (hitCollider.TryGetComponent<StopData>(out var stopData))
                {
                    // 
                    stopData.SetIsHitStopper(true);

                    Debug.Log($"{hitCollider.gameObject.name} is Stopping now");
                }
                // 
                else
                {
                    // 
                    hitCollider.gameObject.AddComponent<StopData>();

                    Debug.Log($"{hitCollider.gameObject.name} Ç… StopDataí«â¡ÇµÇΩÇÊÇÒ");
                }
            }

            if (!onFlag)
            {
                Debug.Log($"Ç»Ç…Ç‡Ç∆ÇﬂÇƒÇ»Ç¢ÇÊÇÒ");
            }
        }
    }
}
