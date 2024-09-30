using UnityEngine;
using Fusion;
using KumaDebug;

public delegate void TradeHandler(ItemOwnerState ownerState);

public class Tradable : NetworkBehaviour
{
	private PlayerRef _itemOwner = default;
	private ItemOwnerState OwnerState
	{
		get
		{
			if (_itemOwner == default)
			{
				return ItemOwnerState.Public;
			}
			else if (GateOfFusion.Instance.NetworkRunner.LocalPlayer == _itemOwner)
			{
				return ItemOwnerState.Mine;
			}
			else
			{
				return ItemOwnerState.NotMine;
			}

		}
	}
	public event TradeHandler OnTrade;
	private PlayerRef receiver = default;

	[ContextMenu("aaaaa")]
	private void TradeTest()
	{
		Trade(receiver);
	}

	public void Trade(PlayerRef receiver)
	{
		OnTrade?.Invoke(OwnerState);
		Rpc_ChangeItemOwner(receiver);
	}

	[ContextMenu("llllll")]
	private void TestTest()
	{
		Rpc_ChangeReceiver(Runner.LocalPlayer);
	}
	
	[Rpc(RpcSources.All,RpcTargets.All)]
	private void Rpc_ChangeReceiver(PlayerRef receiver)
	{
		this.receiver = receiver;
		//XKumaDebugSystem.LogWarning($"Receiver:{this.receiver}");
	}

	[Rpc(RpcSources.All,RpcTargets.All)]
	private void Rpc_ChangeItemOwner(PlayerRef nextItemOwner)
	{
		_itemOwner = nextItemOwner;
	}

	[ContextMenu("dadada")]
	private void Debug()
	{
		XKumaDebugSystem.LogWarning(OwnerState,KumaDebugColor.ErrorColor);
	}
}
