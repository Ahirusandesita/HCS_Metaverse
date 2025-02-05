using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Linq;
using KumaDebug;
using UnityEngine;

public class VendingMachineUIManager : MonoBehaviour
{
	[SerializeField]
	private ItemBundleAsset _itemBundleAsset = default;
	[SerializeField]
	private VendingMachineUI _vendingMachineUIPrefab = default;
	[SerializeField]
	private VendingMachineEditUI _vendingMachineEditUIPrefab = default;
	[SerializeField]
	private RectTransform _startRectTransform;
	[SerializeField]
	private VendingMachine _vendingMachine = default;
	[SerializeField]
	private GameObject _mainPanel = default;
	[SerializeField]
	private GameObject _nextButton = default;
	[SerializeField]
	private GameObject _previousButton = default;
	[SerializeField]
	private GameObject _editPanel;
	[SerializeField]
	private GameObject _editReturnBuyMenuButton = default;
	[SerializeField]
	private VendingMachineEditPriceUI _vendingMachineEditPriceUI = default;
	private List<VendingMachineUI> _vendingUIs = new();
	private List<VendingMachineEditUI> _vendingEditUIs = new();
	private List<WebAPIRequester.ItemLineup> _previousItemLineups = new List<WebAPIRequester.ItemLineup>();
	private int _maxPageCount = 0;
	[Header("パラメータ")]
	[SerializeField]
	private int _buyMenuRowMax = 3;
	[SerializeField]
	private int _editMenuColMax = 3;
	[SerializeField]
	private int _displayUICount = 6;
	[SerializeField]
	private Vector2 _mergin;
	[SerializeField]
	private uint _currentPageCount = 1;


	public void InitUI(WebAPIRequester.OnVMProductData data)
	{
		foreach (VendingMachineUI uI in _vendingUIs)
		{
			Destroy(uI.gameObject);
		}
		_vendingUIs.Clear();
		_previousItemLineups = new List<WebAPIRequester.ItemLineup>(data.ItemList);
		foreach (WebAPIRequester.ItemLineup lineup in data.ItemList)
		{
			int discountedPrice = Mathf.FloorToInt(lineup.Price * (1 - lineup.Discount));
			VendingMachineUI ui = Instantiate(_vendingMachineUIPrefab, _mainPanel.transform);
			InitBuyUI(ui, lineup.ItemID, discountedPrice);
			_vendingUIs.Add(ui);
			ui.Stock = lineup.Stock;
			UpdateUI(lineup.ItemID, lineup.Stock);
		}
		_maxPageCount = _vendingUIs.Count / _displayUICount;
		if (_vendingUIs.Count % _displayUICount != 0) { _maxPageCount++; }
		CloseUI();
		ReplaceBuyUI();
	}

	private void ReplaceBuyUI()
	{
		for (int i = 0; i < _vendingUIs.Count; i++)
		{
			int productPositionCount = i % _displayUICount;
			RectTransform rectTransform = _vendingUIs[i].transform as RectTransform;
			RectTransform prefabRectTransform = _vendingMachineUIPrefab.transform as RectTransform;
			Vector2 prefabSize = prefabRectTransform.sizeDelta + _mergin;

			rectTransform.localPosition =
				_startRectTransform.localPosition +
					Vector3.right * (prefabSize.x * (productPositionCount % _buyMenuRowMax)) +
					Vector3.down * (prefabSize.y * (productPositionCount / _buyMenuRowMax));
		}
	}

	private VendingMachineUI InitBuyUI(VendingMachineUI ui, int id, int discountedPrice)
	{
		if (id > 20000)
		{
			id += 20000;
		}
		ItemAsset itemAsset = _itemBundleAsset.GetItemAssetByID(id);
		ui.Init(id
			, discountedPrice
			, this
			, itemAsset.ItemIcon
			, itemAsset.name);
		return ui;
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
				WebAPIRequester.OnVMProductData entryData =
					await webAPIRequester.PostVMEntry(_vendingMachine.ShopID);
				InitUI(entryData);
				XKumaDebugSystem.LogError("Buyできませんでした");
				return;
			}
		}
		catch
		{
			XKumaDebugSystem.LogError("Buyできませんでした");
			return;
		}
		//複数買いは未実装のものとする
		UpdateUI(id, result.GetBody.StockData[0].Amount);
	}

	public void NextPage()
	{
		_currentPageCount++;

		if (_currentPageCount >= _maxPageCount)
		{
			CloseNextButton();
		}
		OpenPreviousButton();
		CloseBuyUI();
		OpenBuyUI();
		if (_vendingMachine.IsAdminPlayer)
		{
			OpenEditerButtons();
		}
		else
		{
			OpenBuyButton();
		}
	}

	public void PreviousPage()
	{
		_currentPageCount--;
		if (_currentPageCount <= 1)
		{
			ClosePreciousButton();
		}
		OpenNextButton();
		CloseBuyUI();
		OpenBuyUI();
		if (_vendingMachine.IsAdminPlayer)
		{
			OpenEditerButtons();
		}
		else
		{
			OpenBuyButton();
		}
	}

	public async UniTaskVoid SaveEditData()
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		List<WebAPIRequester.VMSalesData> vmUpdateData = new List<WebAPIRequester.VMSalesData>();
		foreach (VendingMachineUI currentItem in _vendingUIs)
		{
			bool isHit = false;
			foreach(WebAPIRequester.ItemLineup previousItem in _previousItemLineups)
			{
				//削除されずに在庫、値段が更新された場合
				if(previousItem.ItemID == currentItem.ID && 
					(previousItem.Stock != currentItem.Stock || previousItem.Price != currentItem.Price))
				{
					WebAPIRequester.VMSalesData salesData 
						= new WebAPIRequester.VMSalesData(currentItem.ID,currentItem.Stock,currentItem.Price,false);
					vmUpdateData.Add(salesData);
				}
			}
		}
		WebAPIRequester.OnVMProductData currentVMInventory
			= await webAPIRequester.PostVMUpdate(_vendingMachine.ShopID, vmUpdateData);
		InitUI(currentVMInventory);
		OpenEditerButtons();
	}

	public void DeleteProduct(int id)
	{
		VendingMachineUI deleteUI = _vendingUIs
			.Where(vendingUI => vendingUI.ID == id)
			.FirstOrDefault();
		_vendingUIs.Remove(deleteUI);
		ReplaceBuyUI();
		OpenBuyUI();
	}

	public async UniTask ChangeProduct(int id, int price)
	{
		VendingMachineUI changingUI = _vendingUIs.Where(ui => ui.IsChanging).FirstOrDefault();
		InitBuyUI(changingUI, id, price);
		List<WebAPIRequester.VMSalesData> vmSales = new();
		vmSales.Add(new WebAPIRequester.VMSalesData(
			id,
			changingUI.Stock,
			price,
			false
			)
		);
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		await webAPIRequester.PostVMUpdate(_vendingMachine.ShopID, vmSales);
	}

	#region Open関数
	public void OpenBuyUI()
	{
		for (int i = ((int)_currentPageCount - 1) * _displayUICount; i < _vendingUIs.Count; i++)
		{
			if (i / _displayUICount == _currentPageCount) { break; }
			_vendingUIs[i].gameObject.SetActive(true);
		}
	}

	public void OpenBuyButton()
	{
		foreach (VendingMachineUI ui in _vendingUIs)
		{
			ui.BuyButton.SetActive(true);
		}
	}

	public void OpenEditerButtons()
	{
		foreach (VendingMachineUI ui in _vendingUIs)
		{
			ui.EditterButtons.SetActive(true);
		}
	}

	public void OpenEditUIButtons()
	{
		foreach (VendingMachineEditUI vendingMachineEditUI in _vendingEditUIs)
		{
			vendingMachineEditUI.Buttons.SetActive(true);
		}
	}

	public void OpenNextButton()
	{
		_nextButton.SetActive(true);
	}

	public void OpenPreviousButton()
	{
		_previousButton.SetActive(true);
	}

	public void OpenEditPanel()
	{
		_editPanel.SetActive(true);
		OpenEditReturnBuyMenuButton();
		CloseNextButton();
		ClosePreciousButton();
		CloseEditterButtons();

		int inventoryCount = 0;
		IEnumerable<ItemIDAmountPair> containItems =
			PlayerDontDestroyData.Instance.Inventory.Where(i => i.ItemID > 0);
		RectTransform prefabRectTranform = _vendingMachineEditUIPrefab.transform as RectTransform;
		float offsetX = prefabRectTranform.sizeDelta.x + _mergin.x;
		float offsetY = prefabRectTranform.sizeDelta.y + _mergin.y;
		int containItemsCount = containItems.Count();
		int editMenuRowMax = containItemsCount / _editMenuColMax;
		_vendingEditUIs.Clear();
		foreach (ItemIDAmountPair itemIDAmountPair in containItems)
		{
			ItemAsset itemAsset = _itemBundleAsset.GetItemAssetByID(itemIDAmountPair.ItemID);
			VendingMachineEditUI uiEditTemp;
			uiEditTemp = Instantiate(_vendingMachineEditUIPrefab, _editPanel.transform);
			uiEditTemp.Init(
				itemAsset.ID,
				itemAsset.Name,
				itemAsset.ItemIcon,
				this,
				itemIDAmountPair.Amount);
			_vendingEditUIs.Add(uiEditTemp);
			RectTransform rectTransform = uiEditTemp.transform as RectTransform;
			rectTransform.anchoredPosition +=
				Vector2.right *
				((inventoryCount % editMenuRowMax) * offsetX) +
				Vector2.down *
				((inventoryCount / editMenuRowMax) * offsetY);
			inventoryCount++;
		}
		float rowCenter = editMenuRowMax / 2;
		foreach (RectTransform rectTransform in _editPanel.transform as RectTransform)
		{
			rectTransform.anchoredPosition +=
				Vector2.left * (rowCenter * offsetX) +
				Vector2.up * (offsetY);
		}
	}

	public void OpenEditPricePanel(int id)
	{
		_vendingMachineEditPriceUI.gameObject.SetActive(true);
		_vendingMachineEditPriceUI.Init(id);
	}

	public void OpenEditUICountText()
	{
		foreach (VendingMachineEditUI vendingMachineEditUI in _vendingEditUIs)
		{
			vendingMachineEditUI.OpenText();
		}
	}

	public void OpenEditReturnBuyMenuButton()
	{
		_editReturnBuyMenuButton.SetActive(true);
	}


	#endregion

	#region Close関数
	public void CloseUI()
	{
		CloseBuyUI();
		CloseNextButton();
		ClosePreciousButton();
		CloseEditPanel();
	}
	public void CloseBuyUI()
	{
		foreach (VendingMachineUI ui in _vendingUIs)
		{
			ui.gameObject.SetActive(false);
			ui.BuyButton.SetActive(false);
			ui.EditterButtons.SetActive(false);
		}
	}

	public void CloseEditterButtons()
	{
		foreach (VendingMachineUI ui in _vendingUIs)
		{
			ui.EditterButtons.SetActive(false);
		}
	}

	public void CloseNextButton()
	{
		_nextButton.SetActive(false);
	}

	public void ClosePreciousButton()
	{
		_previousButton.SetActive(false);
	}

	public void CloseEditPanel()
	{
		foreach (Transform childTransform in _editPanel.transform)
		{
			Destroy(childTransform.gameObject);
		}
		_editPanel.SetActive(false);
		CloseEditReturnBuyMenuButton();
	}

	public void CloseEditReturnBuyMenuButton()
	{
		_editReturnBuyMenuButton.SetActive(false);
	}

	public void CloseEditUIButtons()
	{
		foreach (VendingMachineEditUI vendingMachineEditUI in _vendingEditUIs)
		{
			vendingMachineEditUI.Buttons.SetActive(false);
		}
	}

	public void CloseEditUICountText()
	{
		foreach (VendingMachineEditUI vendingMachineEditUI in _vendingEditUIs)
		{
			vendingMachineEditUI.CloseText();
		}
	}

	public void CloseEditPricePanel()
	{
		_vendingMachineEditPriceUI.gameObject.SetActive(false);
	}
	#endregion
}
