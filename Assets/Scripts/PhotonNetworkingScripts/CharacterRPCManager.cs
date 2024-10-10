using Fusion;
using Layer_lab._3D_Casual_Character;
using KumaDebug;

public class CharacterRPCManager : NetworkBehaviour
{
	public override void Spawned()
	{
		XKumaDebugSystem.LogWarning($"CharacterRPCManager_Spawned", KumaDebugColor.SuccessColor);
		DontDestroyOnLoad(this.gameObject);
	}
	[Rpc(RpcSources.All,RpcTargets.All,InvokeLocal = false)]
    public void Rpc_ChangeWear(PartsType partsType,int index,NetworkObject remoteView)
	{
		XKumaDebugSystem.LogWarning($"ChangeWear", KumaDebugColor.WarningColor);
		CharacterBase characterBase = remoteView.GetComponentInChildren<CharacterBase>();
		characterBase.SetItem(partsType, index);
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public void Rpc_PlayEmote(AnimationControl.AnimType animType, int index, NetworkObject remoteView)
	{
		CharacterControl characterControl = remoteView.GetComponentInChildren<CharacterControl>();
		characterControl.PlayAnimation(animType, index);
	}
}
