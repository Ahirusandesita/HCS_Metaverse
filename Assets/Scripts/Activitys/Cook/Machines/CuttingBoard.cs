using UnityEngine;
using Oculus.Interaction;
using Fusion;

public class CuttingBoard : Machine
{
    [SerializeField]
    Collider _cuttingBoardCollider = default;

    Knife _hittingKnife = default;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // ê⁄êGÇµÇΩColliderÇîªíËÇµÇƒäiî[Ç∑ÇÈ
        Collider[] hitColliders = Physics.OverlapBox(_cuttingBoardCollider.bounds.center, _cuttingBoardCollider.bounds.extents, this.transform.rotation);

        if (hitColliders.Length == 0)
        {
            _hittingKnife = default;

            return;
        }
        else
        {

        }
    }

    private void OnTriggerStay(Collider other)
    {
        Knife knife = other.transform.root.GetComponentInChildren<Knife>();

        if (knife == null)
        {
            if (_hittingKnife == null)
            {
                return;
            }
            else
            {
                _hittingKnife = null;
            }
        }
        else
        {
            if (_hittingKnife == null)
            {
                ManualProcessEvent();
                _hittingKnife = knife;
            }
            else
            {
                return;
            }
        }

        if (other.transform.root.GetComponentInChildren<Knife>() != null)
        {
            ManualProcessEvent();
        }
    }
}