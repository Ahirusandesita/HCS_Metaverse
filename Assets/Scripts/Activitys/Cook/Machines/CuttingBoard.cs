using UnityEngine;
using Oculus.Interaction;
using Cysharp.Threading.Tasks;

public class CuttingBoard : Machine
{
    [SerializeField]
    Collider _cuttingBoardCollider = default;

    ConnectionChecker _connentionChecker = new ConnectionChecker();

    CookActivitySound _sound = default;

    Knife _hittingKnife = default;

    LayerMask _itemLayer = 1 >> 7;

    protected override void Start()
    {
        base.Start();
        _sound = FindObjectOfType<CookActivitySound>();
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
                        Debug.LogWarning($"<color=blue>包丁当たったよん</color>");
                        ProcessEvent(_processingValue);
                        _hittingKnife = knife;
                        _sound.RPC_PlayOneShotSE(CookActivitySound.SEName_Cook.cut, transform.position);
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