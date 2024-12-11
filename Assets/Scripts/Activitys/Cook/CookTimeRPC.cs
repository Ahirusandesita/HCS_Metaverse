using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public class CookTimeRPC : NetworkBehaviour ,IRPCComponent
{
    public int InstanceCode { get; set; }
    private ActivityProgressManagement_Leader leader;
    private ActivityProgressManagement_Member member;
    public void LeaderInject(MonoBehaviour monoBehaviour)
    {
        
    }

    public void MemberInject(MonoBehaviour monoBehaviour)
    {
        this.member = monoBehaviour.GetComponent<ActivityProgressManagement_Member>();
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_Time()
    {
           
    }
}
