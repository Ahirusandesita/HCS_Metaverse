using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class NetworkView : NetworkBehaviour, IAfterSpawned
{
    [Networked]
    public bool OneGrab { get; set; } = false;

    private LocalView localView;
    public LocalView LocalView => localView;
    private MeshRenderer[] meshRenderers;
    public void AfterSpawned()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].enabled = false;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_Position(Vector3 position, Vector3 rotation)
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            this.transform.position = position;
            this.transform.rotation = Quaternion.Euler(rotation);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_ExcludeOthersInactive()
    {
        localView.Inactive();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].enabled = true;
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_OneGrab()
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            OneGrab = true;
            Debug.LogError($"OneGrab True");
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_ExcludeOthersActive()
    {
        localView.Active(this.transform.position, this.transform.rotation.eulerAngles);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].enabled = false;
        }
    }

    public void LocalViewInject(LocalView localView)
    {
        Debug.Log("LocalView Injected");
        this.localView = localView;
    }

    private void OnDestroy()
    {
        Debug.LogError(gameObject.name + "Destory");
    }
}
