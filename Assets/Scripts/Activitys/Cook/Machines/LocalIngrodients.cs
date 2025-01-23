using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Oculus.Interaction;

public class LocalIngrodients : Ingrodients, IGrabbableActiveChangeRequester
{
    [SerializeField]
    private Collider _collider = default;

    private LocalView _localView = default;

    public LocalView LocalView => _localView;

    private void Start()
    {
        _localView = GetComponent<LocalView>();

        PointableUnityEventWrapper pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();

        pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });
    }

    private void Update()
    {
        if (!GateOfFusion.Instance.IsActivityConnected && !GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            return;
        }

        Collider[] hitColliders = Physics.OverlapBox(_collider.bounds.center, _collider.bounds.extents, this.transform.rotation);

        if (hitColliders.Length == 0)
        {
            _hitMachine = default;

            return;
        }
        else
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject.TryGetComponent<Machine>(out var hitMachine) && hitMachine != _hitMachine)
                {
                    _localView.NetworkView.GetComponent<NetworkIngrodients>().RPC_PutIngrodients(hitMachine.MachineID);
                    Debug.LogWarning($"<color=red>LocalIngあたったよ->{hitMachine.gameObject.name}</color>");
                }
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.TryGetComponent<Machine>(out var hitMachine) && hitMachine != _hitMachine)
    //    {
    //        _localView.NetworkView.GetComponent<NetworkIngrodients>().RPC_PutIngrodients(hitMachine.MachineID);
    //        Debug.LogWarning($"<color=red>LocalIngあたったよ->{hitMachine.gameObject.name}</color>");
    //    }
    //}

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

        // 物理判定を止める
        GetComponent<Rigidbody>().isKinematic = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
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

        GetComponent<Rigidbody>().isKinematic = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_Destroy()
    {
        Destroy(gameObject);
    }
}
