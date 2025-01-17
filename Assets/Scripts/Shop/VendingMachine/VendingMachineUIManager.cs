using System.Collections;
using System.Collections.Generic;
using KumaDebug;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class VendingMachineUIManager : MonoBehaviour
{
	[SerializeField]
	private ItemBundleAsset _itemBundleAsset = default;
	[SerializeField]
	private VendingMachineUI _vendingMachineUIPrefab = default;
	[SerializeField]
	private Transform _parent = default;
	[SerializeField]
	private RectTransform _startRectTransform;
	[SerializeField]
	private Transform _viewTransform;
	[SerializeField]
	private int _rowMax = 3;
	private int _shopID = -1;
	private int _roomAdminID = -1;
	private Dictionary<int, VendingMachineUI> _vendingUIs = new();
	private PlayerDontDestroyData PlayerData => PlayerDontDestroyData.Instance;

	public async UniTaskVoid Initialize(int shopID,int roomAdminID)
	{
		_shopID = shopID;
		_roomAdminID = roomAdminID;
		await ShopUpdate();
	}

	private async UniTask ShopUpdate()
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();

		WebAPIRequester.OnVMEntryData data = await webAPIRequester.PostVMEntry(_shopID);

		//if (data.Active)
		//{
		//	Active(data);
		//}
		//else
		//{
		//	Inactive();
		//}
		Active(data);
		if (IsAdminPlayer(PlayerData.PlayerID))
		{
			AddAdminManuButton();
		}
	}

	private bool IsAdminPlayer(int playerID)
	{
		return _roomAdminID == playerID;
	}

	private void Active(WebAPIRequester.OnVMEntryData data)
	{
		_viewTransform.GetComponentInChildren<Renderer>().material.color = Color.white;

		foreach (WebAPIRequester.ItemLineup lineup in data.ItemList)
		{
			int discountedPrice = Mathf.FloorToInt(lineup.Price * (1 - lineup.Discount));
			AddBuyButtonUI(lineup.ItemID, discountedPrice);
			UpdateUI(lineup.ItemID, lineup.Stock);
		}
	}

	private void Inactive()
	{
		_viewTransform.GetComponentInChildren<Renderer>().material.color = Color.black;
		foreach (VendingMachineUI uI in _vendingUIs.Values)
		{
			Destroy(uI);
		}
		_vendingUIs.Clear();
	}

	private void AddAdminManuButton()
	{

	}

	private void OpenAdminManu()
	{

	}

	private void AddBuyButtonUI(int id, int discountedPrice)
	{
		VendingMachineUI ui = Instantiate(_vendingMachineUIPrefab, _parent);
		RectTransform rectTransform = ui.transform as RectTransform;
		RectTransform prefabRectTransform = _vendingMachineUIPrefab.transform as RectTransform;
		int productCount = _vendingUIs.Count;
		Vector2 prefabSize = prefabRectTransform.sizeDelta;
		rectTransform.localPosition =
			_startRectTransform.localPosition +
				Vector3.right * (prefabSize.x * (productCount % _rowMax)) +
				Vector3.down * (prefabSize.y * (productCount / _rowMax));
		ItemAsset itemAsset = _itemBundleAsset.GetItemAssetByID(id);

		ui.Init(id
			, discountedPrice
			, this
			, itemAsset.ItemIcon
			, itemAsset.name);
		_vendingUIs.Add(id, ui);
	}

	public void UpdateUI(int id, int stock)
	{
		if (stock <= 0)
		{
			_vendingUIs[id].SoldOut();
		}
	}

	public async void Buy(int id)
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		WebAPIRequester.OnVMPaymentData result;
		try
		{
			result = await webAPIRequester.PostVMPayment(id, int.MaxValue, int.MinValue);
		}
		catch
		{
			XKumaDebugSystem.LogError("Buy‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½");
			return;
		}
		//•¡””ƒ‚¢‚Í–¢ŽÀ‘•‚Ì‚à‚Ì‚Æ‚·‚é
		UpdateUI(id, result.GetBody.StockData[0].Amount);
	}


}
