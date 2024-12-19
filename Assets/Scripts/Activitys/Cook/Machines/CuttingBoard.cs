using UnityEngine;
using Oculus.Interaction;
using Fusion;

public class CuttingBoard : Machine
{
    [SerializeField]
    Collider _cuttingBoardCollider = default;

    Knife _hittingKnife = default;

    LayerMask _itemLayer = 1 >> 7;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            return;
        }

        Collider[] hitColliders = Physics.OverlapBox(_cuttingBoardCollider.bounds.center, _cuttingBoardCollider.bounds.extents, this.transform.rotation, _itemLayer);

        if (hitColliders.Length == 0)
        {
            _hittingKnife = default;

            return;
        }
        else
        {
            bool isHitKnife = false;

            for (int i = 0; i < hitColliders.Length; i++)
            {
                Knife knife = hitColliders[i].transform.root.GetComponentInChildren<Knife>();

                if (knife == null)
                {
                    if (_hittingKnife == null)
                    {
                        continue;
                    }
                }
                else
                {
                    isHitKnife = true;

                    if (_hittingKnife == default)
                    {
                        ProcessEvent(_processingValue);
                        _hittingKnife = knife;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            if (!isHitKnife)
            {
                _hittingKnife = default;
            }
        }
    }
}