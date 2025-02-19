using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class VendingMachine : SafetyInteractionObject, IGrabbableActiveChangeRequester
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
	[SerializeField]
	private RegisterSceneInInspector _grabbableScene;
	private ISwitchableGrabbableActive _switchableGrabbableActive;
	public int ShopID => _shopID;
	public bool IsAdminPlayer => _roomAdminID == PlayerData.PlayerID;

	public override void Select(SelectArgs selectArgs)
	{
	}

	public override void Unselect(SelectArgs selectArgs) { }

	public override void Close()
	{
		base.Close();
		SafetyClose();
	}

	public async UniTaskVoid Initialize(int shopID, int roomAdminID)
	{
		_switchableGrabbableActive = GetComponent<ISwitchableGrabbableActive>();
		if (_switchableGrabbableActive == null)
		{
			Debug.LogError($"ISwitchableGrabbableActiveがアタッチされていません" + this.gameObject.name);
			return;
		}
		_switchableGrabbableActive.Regist(this);
		if (SceneManager.GetActiveScene().name == _grabbableScene)
		{
			_switchableGrabbableActive.Active(this);
		}
		else
		{
			_switchableGrabbableActive.Inactive(this);
		}

		_shopID = shopID;
		_roomAdminID = roomAdminID;
		WebAPIRequester webAPIRequester = new WebAPIRequester();

		WebAPIRequester.OnVMProductData data = await webAPIRequester.PostVMEntry(_shopID);
		_uiManager.InitUI(data);
		_isOpen = data.Active;
		if (_isOpen)
		{
			_viewTransform.GetComponentInChildren<Renderer>().material.color = Color.white;
		}
		else
		{
			_viewTransform.GetComponentInChildren<Renderer>().material.color = Color.black;
		}
		_uiManager.CloseUI();
	}

	protected override void SafetyClose()
	{
		_uiManager.CloseUI();
	}

	protected override void SafetyOpenLooking()
	{
		if (!_isOpen) { return; }

		_uiManager.OpenBuyUI();
		_uiManager.OpenPageControlButton();
		if (IsAdminPlayer)
		{
			_uiManager.OpenEditerButtons();
		}
		else
		{
			_uiManager.OpenBuyButton();
		}
	}

	protected override void SafetyOpen()
	{

	}
}
