using Fusion;
using Layer_lab._3D_Casual_Character;
using KumaDebug;
using UnityEngine;

public class CharacterRPCManager : NetworkBehaviour
{
    //public override void Spawned()
    //{
    //    XKumaDebugSystem.LogWarning($"CharacterRPCManager_Spawned", KumaDebugColor.SuccessColor);
    //    DontDestroyOnLoad(this.gameObject);
    //}
    //[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    //public void Rpc_ChangeWear(PartsType partsType, int index, NetworkObject remoteView)
    //{
    //    XKumaDebugSystem.LogWarning($"ChangeWear", KumaDebugColor.WarningColor);
    //    ICharacterBase characterBase = remoteView.GetComponentInChildren<ICharacterBase>();
    //    characterBase.SetItem(partsType, index);
    //}

    //[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    //public void Rpc_PlayEmote(AnimationControl.AnimType animType, int index, NetworkObject remoteView)
    //{
    //    ICharacterControl characterControl = remoteView.GetComponentInChildren<ICharacterControl>();
    //    characterControl.PlayAnimation(FindObjectOfType<AnimationControl>().GetAnimation(new AnimationControl.AnimData(animType, index)));
    //}

    //[Rpc(InvokeLocal = false)]
    //public void Rpc_AnimationBoolControl(NetworkObject remoteView,string boolName,bool value)
    //{
    //    remoteView.GetComponentInChildren<Animator>().SetBool(boolName,value);
    //}

    //[Rpc(InvokeLocal = false)]
    //public void Rpc_AnimationFloatControl(NetworkObject remoteView, string floatName, float value)
    //{
    //    remoteView.GetComponentInChildren<Animator>().SetFloat(floatName, value);
    //}
}
