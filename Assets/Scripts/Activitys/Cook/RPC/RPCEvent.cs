using UnityEngine;
using Fusion;
public interface IPracticableRPCEvent
{
    void RPC_Event<TInterface>(NetworkObject networkObject) where TInterface : IAction;
    void RPC_Event<TInterface, TParameter>(NetworkObject targetNetworkObject, NetworkObject parameterNetworkObject) where TInterface : IAction<TParameter> where TParameter : MonoBehaviour;
    //void RPC_Event<TInterface, TParameter>(NetworkObject targetNetworkObject, TParameter parameter) where TInterface : IAction<TParameter>;
    void RPC_Event<TInterface>(NetworkObject targetNetworkObject, int parameter) where TInterface : IAction<int>;
    void RPC_Event<TInterface>(NetworkObject targetNetworkObject, float parameter) where TInterface : IAction<float>;
    void RPC_Event<TInterface>(NetworkObject targetNetworkObject, bool parameter) where TInterface : IAction<bool>;
    void RPC_Event<TInterface>(NetworkObject targetNetworkObject, Vector3 parameter) where TInterface : IAction<Vector3>;
}
public class NullPracticableRPCEvent : IPracticableRPCEvent
{
    public void RPC_Event<TInterface>(NetworkObject networkObject) where TInterface : IAction
    {
        Debug.LogWarning("Not connected to Photon");
        networkObject.GetComponentInChildren<TInterface>().Action();
    }
    public void RPC_Event<TInterface, TParameter>(NetworkObject targetNetworkObject, NetworkObject parameterNetworkObject)
        where TInterface : IAction<TParameter>
        where TParameter : MonoBehaviour
    {
        Debug.LogWarning("Not connected to Photon");
        targetNetworkObject.GetComponentInChildren<TInterface>().Action(parameterNetworkObject.GetComponent<TParameter>());
    }
    public void RPC_Event<TInterface, TParameter>(NetworkObject targetNetworkObject, TParameter parameter) where TInterface : IAction<TParameter>
    {
        Debug.LogWarning("Not connected to Photon");
        targetNetworkObject.GetComponentInChildren<TInterface>().Action(parameter);
    }
    public void RPC_Event<TInterface>(NetworkObject targetNetworkObject, int parameter) where TInterface : IAction<int>
    {
        Debug.LogWarning("Not connected to Photon");
        targetNetworkObject.GetComponentInChildren<TInterface>().Action(parameter);
    }
    public void RPC_Event<TInterface>(NetworkObject targetNetworkObject, float parameter) where TInterface : IAction<float>
    {
        Debug.LogWarning("Not connected to Photon");
        targetNetworkObject.GetComponentInChildren<TInterface>().Action(parameter);
    }
    public void RPC_Event<TInterface>(NetworkObject targetNetworkObject, bool parameter) where TInterface : IAction<bool>
    {
        Debug.LogWarning("Not connected to Photon");
        targetNetworkObject.GetComponentInChildren<TInterface>().Action(parameter);
    }

    public void RPC_Event<TInterface>(NetworkObject targetNetworkObject, Vector3 parameter) where TInterface : IAction<Vector3>
    {
        Debug.LogWarning("Not connected to Photon");
        targetNetworkObject.GetComponentInChildren<TInterface>().Action(parameter);
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
    public static void RPC_Event<TInterface>(this IPracticableRPCEvent practicableRPCEvent, GameObject targetObject, int parameter) where TInterface : IAction<int>
    {
        practicableRPCEvent.RPC_Event<TInterface>(targetObject.GetComponent<NetworkObject>(), parameter);
    }
    public static void RPC_Event<TInterface>(this IPracticableRPCEvent practicableRPCEvent, GameObject targetObject, float parameter) where TInterface : IAction<float>
    {
        practicableRPCEvent.RPC_Event<TInterface>(targetObject.GetComponent<NetworkObject>(), parameter);
    }
    public static void RPC_Event<TInterface>(this IPracticableRPCEvent practicableRPCEvent, GameObject targetObject, bool parameter) where TInterface : IAction<bool>
    {
        practicableRPCEvent.RPC_Event<TInterface>(targetObject.GetComponent<NetworkObject>(), parameter);
    }
    public static void RPC_Event<TInterface>(this IPracticableRPCEvent practicableRPCEvent, GameObject targetObject, Vector3 parameter) where TInterface : IAction<Vector3>
    {
        practicableRPCEvent.RPC_Event<TInterface>(targetObject.GetComponent<NetworkObject>(), parameter);
    }
}
public class RPCEvent : NetworkBehaviour, IPracticableRPCEvent
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Event<TInterface>(NetworkObject targetNetworkObject) where TInterface : IAction
    {
        targetNetworkObject.GetComponentInChildren<TInterface>().Action();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Event<TInterface, TParameter>(NetworkObject targetNetworkObject, NetworkObject parameterNetworkObject) where TInterface : IAction<TParameter> where TParameter : MonoBehaviour
    {
        targetNetworkObject.GetComponentInChildren<TInterface>().Action(parameterNetworkObject.GetComponent<TParameter>());
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Event<TInterface>(NetworkObject targetNetworkObject, int parameter) where TInterface : IAction<int>
    {
        targetNetworkObject.GetComponentInChildren<TInterface>().Action(parameter);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Event<TInterface>(NetworkObject targetNetworkObject, float parameter) where TInterface : IAction<float>
    {
        targetNetworkObject.GetComponentInChildren<TInterface>().Action(parameter);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Event<TInterface>(NetworkObject targetNetworkObject, bool parameter) where TInterface : IAction<bool>
    {
        targetNetworkObject.GetComponentInChildren<TInterface>().Action(parameter);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Event<TInterface>(NetworkObject targetNetworkObject, Vector3 parameter) where TInterface : IAction<Vector3>
    {
        targetNetworkObject.GetComponentInChildren<TInterface>().Action(parameter);
    }
}
