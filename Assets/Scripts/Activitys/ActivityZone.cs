using UnityEngine;
using Fusion;
public class ActivityZone : MonoBehaviour
{
    private string sessionName;
    public string SessionName => sessionName;


    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RPC_SessionNaming()
    {
        this.sessionName = sessionName;
        Debug.LogWarning("daadad");
    }
}
