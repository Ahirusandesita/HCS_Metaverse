using Fusion;
using Layer_lab._3D_Casual_Character;
using KumaDebug;

public class CharacterRPCManager : NetworkBehaviour
{
	[Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_ChangeWear(PartsType partsType,int index,NetworkObject remoteView)
	{
		XKumaDebugSystem.LogWarning($"ChangeWear", KumaDebugColor.WarningColor);
		CharacterBase characterBase = remoteView.GetComponentInChildren<CharacterBase>();
		characterBase.SetItem(partsType, index);
	}
}
