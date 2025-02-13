using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class CharacterControlRPCManager : NetworkBehaviour
{
    private ChangeSkins skin;    
    private AnimationSelecter anim;
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_ChangeSkins(int machIndex,int i)
    {
        skin.RPCDressUp(machIndex, i);
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_TakeOffSkins(int partsIndex, int machIndex)
    {
        skin.RPCTakeOff(partsIndex, machIndex);
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_ChangeBody()
    {
        skin.ChangeBody();
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_AnimationSelect(int id,string name)
    {
        anim.RPCDressUP(id, name);
    }

    public void InjectChangeSkin(ChangeSkins skin)
    {
        this.skin = skin;
    }
    public void InjectAnimationSelect(AnimationSelecter anim)
    {
        this.anim = anim;
    }
}
