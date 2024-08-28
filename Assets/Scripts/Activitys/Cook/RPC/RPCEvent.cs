using UnityEngine;
using Fusion;
public interface IPracticableRPCEvent
{
    void RPC_Event<TInterface>(NetworkObject networkObject) where TInterface : IAction;
    void RPC_Event<TInterface, TParameter>(NetworkObject targetNetworkObject, NetworkObject parameterNetworkObject) where TInterface : IAction<TParameter> where TParameter : MonoBehaviour;
    void RPC_Event<TInterface, TParameter>(NetworkObject targetNetworkObject, TParameter parameter) where TInterface : IAction<TParameter>;
}
public class NullPracticableRPCEvent : IPracticableRPCEvent
{
    public void RPC_Event<TInterface>(NetworkObject networkObject) where TInterface : IAction
    {
        Debug.LogWarning("Not connected to Photon");
        networkObject.GetComponent<TInterface>().Action();
    }

    public void RPC_Event<TInterface, TParameter>(NetworkObject targetNetworkObject, NetworkObject parameterNetworkObject)
        where TInterface : IAction<TParameter>
        where TParameter : MonoBehaviour
    {
        Debug.LogWarning("Not connected to Photon");
        targetNetworkObject.GetComponent<TInterface>().Action(parameterNetworkObject.GetComponent<TParameter>());
    }

    public void RPC_Event<TInterface, TParameter>(NetworkObject targetNetworkObject, TParameter parameter) where TInterface : IAction<TParameter>
    {
        Debug.LogWarning("Not connected to Photon");
        targetNetworkObject.GetComponent<TInterface>().Action(parameter);
    }
}
public static class RPCEventExpansion
{
    public static void RPC_Event<TInterface>(this IPracticableRPCEvent practicableRPCEvent, GameObject targetObject) where TInterface : IAction
    {
        practicableRPCEvent.RPC_Event<TInterface>(targetObject.GetComponent<NetworkObject>());
    }
    public static void RPC_Event<TInterface, TParameter>(this IPracticableRPCEvent practicableRPCEvent, GameObject targetObject, GameObject parameterObject) where TInterface : IAction<TParameter> where TParameter : MonoBehaviour
    {
        practicableRPCEvent.RPC_Event<TInterface, TParameter>(targetObject.GetComponent<NetworkObject>(), parameterObject.GetComponent<NetworkObject>());
    }
    public static void RPC_Event<TInterface, TParameter>(this IPracticableRPCEvent practicableRPCEvent, GameObject targetObject, TParameter parameter) where TInterface : IAction<TParameter>
    {
        practicableRPCEvent.RPC_Event<TInterface, TParameter>(targetObject.GetComponent<NetworkObject>(), parameter);
    }
}
public class RPCEvent : NetworkBehaviour, IPracticableRPCEvent
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Event<TInterface>(NetworkObject targetNetworkObject) where TInterface : IAction
    {
        targetNetworkObject.GetComponent<TInterface>().Action();
    }
    public void RPC_Event<TInterface, TParameter>(NetworkObject targetNetworkObject, NetworkObject parameterNetworkObject) where TInterface : IAction<TParameter> where TParameter : MonoBehaviour
    {
        targetNetworkObject.GetComponent<TInterface>().Action(parameterNetworkObject.GetComponent<TParameter>());
    }
    public void RPC_Event<TInterface, TParameter>(NetworkObject targetNetworkObject, TParameter parameter) where TInterface : IAction<TParameter>
    {
        targetNetworkObject.GetComponent<TInterface>().Action(parameter);
    }
}
