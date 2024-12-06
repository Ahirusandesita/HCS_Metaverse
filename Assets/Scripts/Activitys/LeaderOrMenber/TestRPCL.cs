using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class TestRPCL : NetworkBehaviour,IRPCComponent
{
    public int InstanceCode { get; set; }

    public void LeaderInject(MonoBehaviour monoBehaviour)
    {
        Debug.LogError(monoBehaviour.GetComponent<TestLeadre>());
    }

    public void MemberInject(MonoBehaviour monoBehaviour)
    {
        Debug.LogError(monoBehaviour.GetComponent<TestMember>());
    }
}
