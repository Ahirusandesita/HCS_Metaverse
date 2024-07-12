using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class LockedCuttingObject : MonoBehaviour, IKnifeHitEvent
{
    [SerializeField, Tooltip("オブジェクトの取得範囲を指定するCollider")]
    private Collider _cuttingAreaCollider = default;

    [SerializeField, Tooltip("固定位置")]
    private Transform _machineTransform = default;

    [SerializeField, Tooltip("オブジェクトを固定する場所のTransform")]
    private Transform _objectLockPosition = default;

    // 
    private Vector3 _hitBoxCenter = default;

    // 
    private Vector3 _hitBoxSize = default;

    // 
    private Quaternion _hitBoxRotation = default;

    // 
    private bool _isLockedObject = default;

    // 
    private Ingrodients _lockingIngrodients = default;

    // 
    private Puttable _lockedPuttable = default;

    private void Start()
    {
        // 
        _hitBoxCenter = _cuttingAreaCollider.bounds.center;

        // 
        _hitBoxSize = _cuttingAreaCollider.bounds.size / 2;

        // 
        _hitBoxRotation = this.transform.rotation;
    }

    private void Update()
    {
        // 
        if (_isLockedObject)
        {
            // 
            return;
        }

        Collider[] hitColliders = Physics.OverlapBox(_hitBoxCenter, _hitBoxSize, _hitBoxRotation);

        if (hitColliders is null)
        {
            Debug.Log($"なにも当たってないよん");
            return;
        }

        // 
        foreach (Collider hitCollider in hitColliders)
        {
            // 
            if (hitCollider.transform.root.TryGetComponent<Ingrodients>(out var ingrodient))
            {
                // 
                if (ingrodient.GetComponent<Rigidbody>().isKinematic)
                {
                    // 
                    continue;
                }

                // 
                GameObject lockObject = ingrodient.gameObject;

                // 
                _lockingIngrodients = ingrodient;

                // 
                lockObject.GetComponent<Grabbable>().enabled = false;
                lockObject.GetComponent<Rigidbody>().isKinematic = true;

                // 
                lockObject.transform.position = _machineTransform.position;
                lockObject.transform.rotation = _machineTransform.rotation;

                // 
                lockObject.GetComponent<Grabbable>().enabled = true;

                // 
                _isLockedObject = true;

                // 
                _lockedPuttable = lockObject.AddComponent<Puttable>();

                // 
                _lockedPuttable.SetLockedCuttingObject(this);
            }
        }
    }

    public void KnifeHitEvent()
    {
        if (_isLockedObject)
        {
            // 
            bool isEndCut = _lockingIngrodients.SubToIngrodientsDetailInformationsTimeItTakes(ProcessingType.Cut, 1);

            // 
            if (isEndCut)
            {
                _lockingIngrodients.ProcessingStart(ProcessingType.Cut, _machineTransform);
                _isLockedObject = false;
                Destroy(_lockedPuttable);
            }            
        }
    }

    public void CanselCutting()
    {
        // 
        _isLockedObject = false;
    }
}
