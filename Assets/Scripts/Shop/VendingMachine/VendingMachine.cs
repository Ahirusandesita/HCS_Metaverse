using UnityEngine;
using Cysharp.Threading.Tasks;

public class VendingMachine : SafetyInteractionObject
{
	private int _shopID = -1;
	private int _roomAdminID = -1;
	private PlayerDontDestroyData PlayerData => PlayerDontDestroyData.Instance;
	[SerializeField]
	private bool _isOpen;
	[SerializeField]
	private VendingMachineUIManager _uiManager = default;
	[SerializeField]
	private Transform _viewTransform;
	public int ShopID => _shopID;

	public override void Select(SelectArgs selectArgs) { }

	public override void Unselect(SelectArgs selectArgs) { }

	public override void Close()
	{
		base.Close();
		SafetyClose();
	}

	public async UniTaskVoid Initialize(int shopID, int roomAdminID)
	{
		_shopID = shopID;
		_roomAdminID = roomAdminID;
		WebAPIRequester webAPIRequester = new WebAPIRequester();

		WebAPIRequester.OnVMEntryData data = await webAPIRequester.PostVMEntry(_shopID);
		_uiManager.InitBuyUI(data);
		_isOpen = data.Active;
		if (_isOpen)
		{
			_viewTransform.GetComponentInChildren<Renderer>().material.color = Color.white;
		}
		else
		{
			//_isOpen = true;
			_viewTransform.GetComponentInChildren<Renderer>().material.color = Color.black;
		}
		XDebug.LogWarning($"IsOpen:{_isOpen}");
	}

	protected override void SafetyClose()
	{
		_uiManager.CloseBuyUI();
		_uiManager.CloseAdminManuButton();
	}

	protected override void SafetyOpen()
	{
		if (!_isOpen) { return; }
		_uiManager.OpenBuyUI();
		if (IsAdminPlayer(PlayerData.PlayerID))
		{
			_uiManager.OpenAdminManuButton();
		}
		else
		{
			_uiManager.OpenBuyButton();
		}
	}
	private bool IsAdminPlayer(int playerID)
	{
		return _roomAdminID == playerID;
	}
}
