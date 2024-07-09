using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoardObject : MonoBehaviour, IKnifeHitEvent
{
    [SerializeField, Tooltip("êÿífÇ≈Ç´ÇÈîÕàÕÇéwíËÇ∑ÇÈCollider")]
    private Collider _cuttingAreaCollider = default;

    [SerializeField, Tooltip("")]
    private Transform _machineTransform = default;

    // 
    private Vector3 _hitBoxCenter = default;

    // 
    private Vector3 _hitBoxSize = default;

    // 
    private Quaternion _hitBoxRotation = default;

    private void Start()
    {
        // 
        _hitBoxCenter = _cuttingAreaCollider.bounds.center;

        // 
        _hitBoxSize = _cuttingAreaCollider.bounds.size / 2;

        // 
        _hitBoxRotation = this.transform.rotation;
    }

    public void KnifeHitEvent()
    {
        // 
        Collider[] hitColliders = Physics.OverlapBox(_hitBoxCenter, _hitBoxSize, _hitBoxRotation);

        if (hitColliders is null)
        {
            Debug.Log($"Ç»Ç…Ç‡ìñÇΩÇ¡ÇƒÇ»Ç¢ÇÊÇÒ");
            return;
        }

        // 
        foreach (Collider hitCollider in hitColliders)
        {
            // 
            if (!hitCollider.TryGetComponent<Ingrodients>(out var thisIngrodient))
            {
                // 
                continue;
            }
            Debug.LogWarning("A");
            bool isEndCut = thisIngrodient.IngrodientsAsset.SubToIngrodientsDetailInformationsTimeItTakes(ProcessingType.Cut, 1);

            // 
            if (isEndCut)
            {
                thisIngrodient.ProcessingStart(ProcessingType.Cut, _machineTransform);
            }            
        }
    }
}
