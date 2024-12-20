using System;
using UnityEngine;
using Fusion;
using Oculus.Interaction;

public class AutoMachine : Machine
{
    [SerializeField, Tooltip("オブジェクトの取得範囲を指定するCollider")]
    private Collider _cuttingAreaCollider = default;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!GateOfFusion.Instance.IsActivityConnected)
        {
            return;
        }

        if (!GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            return;
        }

        if (_processingIngrodientsView != default)
        {
            ProcessEvent(_processingValue * Time.deltaTime);
        }
    }
}