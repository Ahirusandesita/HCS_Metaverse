using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Oculus.Interaction;

public class LocalIngrodients : Ingrodients, IGrabbableActiveChangeRequester
{
    private LocalView _localView = default;

    private void Start()
    {
        _localView = GetComponent<LocalView>();

        PointableUnityEventWrapper pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();

        pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Machine>(out var hitMachine) && hitMachine != _hitMachine)
        {
            _localView.NetworkView.GetComponent<NetworkIngrodients>().RPC_PutIngrodients(hitMachine.MachineID);
        }
    }

    public void PutMachine(int machineID)
    {
        _hitMachine = FindObjectOfType<MachineIDManager>().GetMachine(machineID);

        _hitMachine.SetProcessingIngrodient(GetComponent<LocalView>());

        // 
        ISwitchableGrabbableActive grabbableActiveSwicher = GetComponent<ISwitchableGrabbableActive>();

        // 固定するオブジェクトのGrabbableをfalseにする
        grabbableActiveSwicher.Regist(this);
        grabbableActiveSwicher.Inactive(this);

        // 固定するオブジェクトの座標をマシンの座標に移動させる
        transform.position = _hitMachine.ProcesserTransform.position;
        transform.rotation = _hitMachine.ProcesserTransform.rotation;

        // 固定するオブジェクトのGrabbableをtrueにする
        grabbableActiveSwicher.Active(this);
        grabbableActiveSwicher.Cancellation(this);
    }

    [Rpc]
    public void RPC_PutMachine(int machineID)
    {
        PutMachine(machineID);
    }

    private void Select()
    {
        if (_hitMachine == null)
        {
            return;
        }

        _localView.NetworkView.GetComponent<NetworkIngrodients>().RPC_IngrodientsSelect();
    }

    public void UnPutMachine()
    {
        _hitMachine.UnSetProcessingIngrodient();

        _hitMachine = null;
    }
}
