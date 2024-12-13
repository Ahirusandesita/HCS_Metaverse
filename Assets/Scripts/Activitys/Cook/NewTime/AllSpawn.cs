using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;

public class AllSpawn : NetworkBehaviour, IAfterSpawned
{
    private int playerCount = 0;
    private bool isAllSpawned = false;
    void IAfterSpawned.AfterSpawned()
    {
        Debug.Log("AferSpawned");
        RPC_Spawned();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, InvokeLocal = true)]
    private void RPC_Spawned()
    {
        playerCount++;
        if (GateOfFusion.Instance.NetworkRunner.SessionInfo.PlayerCount - playerCount <= 0)
        {
            isAllSpawned = true;
        }
    }

    public async UniTask<bool> Async()
    {
        await UniTask.WaitUntil(() => isAllSpawned);
        return true;
    }
}
