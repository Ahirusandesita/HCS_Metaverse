using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public interface ILeader
{
    void Inject(NetworkBehaviour networkBehaviour);
}
public interface IMember
{
    void Inject(NetworkBehaviour networkBehaviour);
}
public interface IRPCComponent
{
    void LeaderInject(MonoBehaviour monoBehaviour);
    void MemberInject(MonoBehaviour monoBehaviour);
    int InstanceCode { get; set; }
    bool Epauls(IRPCComponent left,IRPCComponent right)
    {
        return left.InstanceCode == right.InstanceCode;
    }
    NetworkBehaviour NetworkBehaviour => this as NetworkBehaviour;
}
