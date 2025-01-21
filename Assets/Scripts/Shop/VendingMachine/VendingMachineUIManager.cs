using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KumaDebug;
using UnityEngine;

public class VendingMachineUIManager : MonoBehaviour
{
	[SerializeField]
	private ItemBundleAsset _itemBundleAsset = default;
	[SerializeField]
	private VendingMachineUI _vendingMachineUIPrefab = default;
	[SerializeField]
	private RectTransform _startRectTransform;

	[SerializeField]
	private VendingMachineOpenAdminMenu _vendingMachineOpenAdminMenu;

	[SerializeField]
	private VendingMachine _vendingMachine = default;
	[SerializeField]
	private GameObject _buyPanel = default;
	[SerializeField]
	private GameObject _adminPanel = default;
	private Dictionary<int, VendingMachineUI> _vendingUIs = new();
	[Header("ÉpÉâÉÅÅ[É^")]
	[SerializeField]
	private int _rowMax = 3;
	[SerializeField]
	private Vector2 _mergin;

	public void InitBuyUI(WebAPIRequester.OnVMEntryData data)
	{
		XDebug.LogWarning("init");
		foreach (VendingMachineUI uI in _vendingUIs.Values)
		{
			Destroy(uI.gameObject);
		}
		_vendingUIs.Clear();
		foreach (WebAPIRequester.ItemLineup lineup in data.ItemList)
		{
			int discountedPrice = Mathf.FloorToInt(lineup.Price * (1 - lineup.Discount));
			AddBuyUI(lineup.ItemID, discountedPrice);
			UpdateUI(lineup.ItemID, lineup.Stock);
		}
		CloseBuyUI();
	}

	public void OpenBuyUI()
	{
		XDebug.LogWarning("open");
		foreach (VendingMachineUI ui in _vendingUIs.Values)
		{
			ui.gameObject.SetActive(true);
		}
	}

	public void OpenBuyButton()
	{
		foreach (VendingMachineUI ui in _vendingUIs.Values)
		{
			ui.BuyButton.SetActive(true);
		}
	}

	public void CloseBuyUI()
	{
		foreach (VendingMachineUI ui in _vendingUIs.Values)
		{
			ui.gameObject.SetActive(false);
			ui.BuyButton.SetActive(false);
		}
	}

	public void OpenAdminManuButton()
	{
		_vendingMachineOpenAdminMenu.gameObject.SetActive(true);
	}
	public void CloseAdminManuButton()
	{
		_vendingMachineOpenAdminMenu.gameObject.SetActive(false);
	}
	public void OpenBuyCanvas()
	{
		_buyPanel.SetActive(true);
	}

	public void CloseBuyCanvas()
	{
		CloseBuyUI();
		_buyPanel.SetActive(false);
	}

	public void OpenAdminMenuUI()
	{
		CloseBuyCanvas();
		_adminPanel.SetActive(true);

	}
	public void CloseAdminMenuUI()
	{
		_adminPanel.SetActive(false);
	}

	[ContextMenu("adada")]
	private void Reload()
	{
		FindAnyObjectByType<MyRoomLoader>().Load().Forget();
	}

	private void AddBuyUI(int id, int discountedPrice)
	{
		if(id > 20000)
		{
			id += 20000;
		}
		VendingMachineUI ui = Instantiate(_vendingMachineUIPrefab, _buyPanel.transform);
		RectTransform rectTransform = ui.transform as RectTransform;
		RectTransform prefabRectTransform = _vendingMachineUIPrefab.transform as RectTransform;
		int productCount = _vendingUIs.Count;
		Vector2 prefabSize = prefabRectTransform.sizeDelta + _mergin;

		rectTransform.localPosition =
			_startRectTransform.localPosition +
				Vector3.right * (prefabSize.x * (productCount % _rowMax) ) +
				Vector3.down * (prefabSize.y * (productCount / _rowMax));
		ItemAsset itemAsset = _itemBundleAsset.GetItemAssetByID(id);
		//XDebug.LogWarning($"{ui}:{Resources.Load<Sprite>("NotExistIcon")}");
		//XDebug.LogWarning($"{id}:{discountedPrice}:{this}:{itemAsset}");
		//XDebug.LogWarning($"{itemAsset.ItemIcon}:{itemAsset.name}");
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
			result = await webAPIRequester.PostVMPayment(id, int.MaxValue, _vendingMachine.ShopID);
			if (result.GetBody.UpdateFlg)
			{
				WebAPIRequester.OnVMEntryData entryData =
					await webAPIRequester.PostVMEntry(_vendingMachine.ShopID);
				InitBuyUI(entryData);
				XKumaDebugSystem.LogError("BuyÇ≈Ç´Ç‹ÇπÇÒÇ≈ÇµÇΩ");
				return;
			}
		}
		catch
		{
			XKumaDebugSystem.LogError("BuyÇ≈Ç´Ç‹ÇπÇÒÇ≈ÇµÇΩ");
			return;
		}
		//ï°êîîÉÇ¢ÇÕñ¢é¿ëïÇÃÇ‡ÇÃÇ∆Ç∑ÇÈ
		UpdateUI(id, result.GetBody.StockData[0].Amount);
	}
}
