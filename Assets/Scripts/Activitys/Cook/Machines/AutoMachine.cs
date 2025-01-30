using System;
using UnityEngine;
using Fusion;
using Oculus.Interaction;

public class AutoMachine : Machine
{
    [SerializeField, Tooltip("オブジェクトの取得範囲を指定するCollider")]
    private Collider _ProcessingAreaCollider = default;

    private CookActivitySound _sound = default;

    private float _seTimer = 0f;

    private const float SE_INTERVAL = 2.5f;

    protected override void Start()
    {
        base.Start();

        _sound = FindObjectOfType<CookActivitySound>();
    }

    protected override void Update()
    {
        base.Update();

        if (_seTimer >= 0)
        {
            _seTimer -= Time.deltaTime;
        }

        if (!GateOfFusion.Instance.IsActivityConnected || !GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            return;
        }

        if (_processingIngrodientsView != default)
        {
            ProcessEvent(_processingValue * Time.deltaTime);

            if (_seTimer <= 0)
            {
                _sound.PlayOneShotSE(CookActivitySound.SEName_Cook.bake, transform.position);
                _seTimer = SE_INTERVAL;
            }
        }
    }
}