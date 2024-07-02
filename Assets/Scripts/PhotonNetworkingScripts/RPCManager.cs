using Fusion;
using UnityEngine;

public delegate void SessionNameChanged(string name);

public class RPCManager : NetworkBehaviour
{
    public event SessionNameChanged SessionNameChangedHandler;

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SessionNaming(string sessionName)
    {
        Debug.LogWarning("RpcExecute");

        //é¿çs
        SessionNameChangedHandler?.Invoke(sessionName);
    }
}
