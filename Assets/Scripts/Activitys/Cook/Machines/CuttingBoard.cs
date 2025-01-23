using UnityEngine;
using Oculus.Interaction;
using Cysharp.Threading.Tasks;

public class CuttingBoard : Machine
{
    [SerializeField]
    Collider _cuttingBoardCollider = default;

    ConnectionChecker _connentionChecker = new ConnectionChecker();

    Knife _hittingKnife = default;

    LayerMask _itemLayer = 1 >> 7;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!_connentionChecker.IsConnection)
        {
            return;
        }

        Collider[] hitColliders = Physics.OverlapBox(_cuttingBoardCollider.bounds.center, _cuttingBoardCollider.bounds.extents, this.transform.rotation);

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
                        Debug.LogWarning($"<color=blue>•ï’š“–‚½‚Á‚½‚æ‚ñ</color>");
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