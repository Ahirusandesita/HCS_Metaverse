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
	private int _rowMax = 3;
	private int _shopID = default;
	private Dictionary<int, VendingMachineUI> _vendingUIs = new();

	public async UniTaskVoid Initialize(int shopID)
	{
		_shopID = shopID;
		await ShopUpdate();
	}

	private async UniTask ShopUpdate()
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();

		WebAPIRequester.OnShopEntryData data = await webAPIRequester.PostShopEntry(_shopID);
		foreach (WebAPIRequester.ItemLineup lineup in data.GetBody.ItemList)
		{
			int discountedPrice = Mathf.FloorToInt(lineup.Price * (1 - lineup.Discount));
			AddVendingMachineUI(lineup.ItemID, discountedPrice);
			UpdateUI(lineup.ItemID, lineup.Stock);
		}
	}

	private void AddVendingMachineUI(int id, int discountedPrice)
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

	public void UpdateUI(int id,int stock)
	{
		XKumaDebugSystem.LogWarning($"{id}:{stock}");
		if (stock <= 0)
		{
			XKumaDebugSystem.LogWarning($"{id}:hide");
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

		// UpdateUI();
	}
}
