using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class TestRPCL : NetworkBehaviour,IRPCComponent
{
    [SerializeField]
    private MonoBehaviour leader;
    [SerializeField]
    private MonoBehaviour member;
    public int InstanceCode { get; set; }

    public void LeaderInject(MonoBehaviour monoBehaviour)
    {
        this.leader = monoBehaviour;
        Debug.LogError(monoBehaviour.GetComponent<TestLeadre>());
    }

    public void MemberInject(MonoBehaviour monoBehaviour)
    {
        this.member = monoBehaviour;
        Debug.LogError(monoBehaviour.GetComponent<TestMember>());
    }
}
