using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkIngrodients : Ingrodients
{
    private NetworkView _networkView = default;

    private void Start()
    {
        _networkView = GetComponent<NetworkView>();
    }

    [Rpc]
    public void RPC_PutIngrodients(int machineID)
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            _hitMachine = FindObjectOfType<MachineIDManager>().GetMachine(machineID);

            _networkView.LocalView.GetComponent<LocalIngrodients>().RPC_PutMachine(machineID);
        }
    }

    [Rpc]
    public void RPC_IngrodientsSelect()
    {
        _networkView.LocalView.GetComponent<LocalIngrodients>().UnPutMachine();
    }

    [Rpc]
    public void RPC_ManualProcess()
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            SubToIngrodientsDetailInformationsTimeItTakes(_hitMachine.ProcessType, _hitMachine.ProcessingValue);
        }
    }
}
