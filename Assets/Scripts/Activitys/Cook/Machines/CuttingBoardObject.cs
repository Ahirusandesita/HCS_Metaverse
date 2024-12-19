using UnityEngine;

public class CuttingBoardObject : MonoBehaviour
{
    [SerializeField, Tooltip("切断できる範囲を指定するCollider")]
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

    public void ProcessEvent()
    {
        // 
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
            if (!hitCollider.transform.root.TryGetComponent<Ingrodients>(out var thisIngrodient))
            {
                // 
                continue;
            }

            bool isEndCut = thisIngrodient.SubToIngrodientsDetailInformationsTimeItTakes(ProcessingType.Cut, 1);

            // 
            if (isEndCut)
            {
                thisIngrodient.ProcessingStart(ProcessingType.Cut, _machineTransform);
            }
        }
    }
}