using Fusion;

public class NetworkIngrodients : NetworkBehaviour
{
    private Machine _hitMachine = default;

    private NetworkView _networkView = default;

    private ConnectionChecker _connectionChecker = new ConnectionChecker();

    public NetworkView NetworkView => _networkView;

    private void Start()
    {
        _networkView = GetComponent<NetworkView>();
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_PutIngrodients(int machineID)
    {
        if (_hitMachine != null)
        {
            return;
        }

        if (_connectionChecker.IsConnection)
        {
            _hitMachine = FindObjectOfType<MachineIDManager>().GetMachine(machineID);

            _networkView.LocalView.GetComponent<LocalIngrodients>().RPC_PutMachine(machineID);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_IngrodientsSelect()
    {
        _networkView.LocalView.GetComponent<LocalIngrodients>().UnPutMachine();
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_ProcessEvent(float processValue)
    {
        if (!_connectionChecker.IsConnection)
        {
            return;
        }

        LocalIngrodients ingrodients = _networkView.LocalView.GetComponent<LocalIngrodients>();

        bool isEndProcessing = ingrodients.SubToIngrodientsDetailInformationsTimeItTakes(_hitMachine.ProcessType, processValue);

        if (isEndProcessing)
        {
            ingrodients.ProcessingStart(_hitMachine.ProcessType, _hitMachine.ProcesserTransform);
            ingrodients.RPC_Destroy();
        }
    }
}
