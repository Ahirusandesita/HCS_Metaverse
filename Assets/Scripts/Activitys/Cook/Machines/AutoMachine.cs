using System;
using UnityEngine;
using Fusion;
using Oculus.Interaction;

public class AutoMachine : Machine
{
    [SerializeField, Tooltip("�I�u�W�F�N�g�̎擾�͈͂��w�肷��Collider")]
    private Collider _cuttingAreaCollider = default;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (_processingIngrodientsView != default)
        {
            bool isEndProcess = _processingIngrodientsView.NetworkView.GetComponent<NetworkIngrodients>().SubToIngrodientsDetailInformationsTimeItTakes(_processingType, Time.deltaTime);

            if (isEndProcess)
            {
                _processingIngrodientsView.NetworkView.GetComponent<NetworkIngrodients>().ProcessingStart(_processingType, _processerTransform);
            }
        }
    }
}