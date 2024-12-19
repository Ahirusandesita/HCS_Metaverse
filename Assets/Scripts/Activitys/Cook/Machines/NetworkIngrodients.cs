using Fusion;

public class NetworkIngrodients : NetworkBehaviour
{
    private Machine _hitMachine = default;

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
    public void RPC_ProcessEvent(float processValue)
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            bool isEndProcessing = _networkView.LocalView.GetComponent<LocalIngrodients>().SubToIngrodientsDetailInformationsTimeItTakes(_hitMachine.ProcessType, processValue);

            if (isEndProcessing)
            {
                _networkView.LocalView.GetComponent<LocalIngrodients>().ProcessingStart(_hitMachine.ProcessType, _hitMachine.ProcesserTransform);
            }
        }
    }
}
