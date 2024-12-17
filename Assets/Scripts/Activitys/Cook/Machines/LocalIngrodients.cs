using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Oculus.Interaction;

public class LocalIngrodients : MonoBehaviour
{
    private Machine _hitMachine = default;

    private MachineIDManager _machineIDManager = default;

    private LocalView _localView = default;

    private void Start()
    {
        _machineIDManager = FindObjectOfType<MachineIDManager>();

        _localView = GetComponent<LocalView>();

        PointableUnityEventWrapper pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();

        pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Machine>(out var hitMachine))
        {
            _localView.NetworkView.GetComponent<NetworkIngrodients>().RPC_PutIngrodients(hitMachine.MachineID);
        }
    }

    public void PutMachine(int machineID)
    {
        _hitMachine = _machineIDManager.GetMachine(machineID);

        _hitMachine.SetProcessingIngrodient(GetComponent<LocalView>());

        // ã≠êßÇ≈íÕÇ›âèú Å´
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
