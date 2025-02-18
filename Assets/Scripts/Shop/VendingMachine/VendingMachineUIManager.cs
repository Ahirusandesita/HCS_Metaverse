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
	private VendingMachineBuyUI _vendingMachineUIPrefab = default;
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
	private GameObject _addButton = default;
	[SerializeField]
	private VendingMachineEditPriceUI _vendingMachineEditPriceUI = default;
	private List<VendingMachineBuyUI> _vendingUIs = new();
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

	public async UniTask InitUI(WebAPIRequester webAPIRequester)
	{
		InitUI(await webAPIRequester.PostVMEntry(_vendingMachine.ShopID));
	}
	public void InitUI(WebAPIRequester.OnVMProductData data)
	{
		foreach (VendingMachineBuyUI uI in _vendingUIs)
		{
			Destroy(uI.gameObject);
		}
		_vendingUIs.Clear();
		_previousItemLineups = new List<WebAPIRequester.ItemLineup>(data.ItemList);
		foreach (WebAPIRequester.ItemLineup lineup in data.ItemList)
		{
			int discountedPrice = Mathf.FloorToInt(lineup.Price * (1 - lineup.Discount));
			VendingMachineBuyUI ui = Instantiate(_vendingMachineUIPrefab, _mainPanel.transform);
			InitBuyUI(ui, lineup.ItemID, discountedPrice, lineup.Stock);
			_vendingUIs.Add(ui);
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

	private VendingMachineBuyUI InitBuyUI(VendingMachineBuyUI ui, int id, int discountedPrice, int stock)
	{
		ItemAsset itemAsset = _itemBundleAsset.GetItemAssetByID(id);
		ui.Init(id
			, discountedPrice
			, this
			, itemAsset.ItemIcon
			, itemAsset.name
			, stock);
		return ui;
	}

	public void UpdateUI(int id, int stock)
	{
		if (stock <= 0)
		{
			VendingMachineBuyUI updateUI = _vendingUIs.
				Where(ui => ui.ID == id && ui.Stock <= 0).
				FirstOrDefault();
			updateUI.SoldOut();
		}
	}

	public async void Buy(int id)
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		WebAPIRequester.OnVMPaymentData result;
		try
		{
			int price = -1;
			foreach (var item in _vendingUIs)
			{
				if (item.ID != id) { continue; }
				price = item.Price;
			}
			if (price < 0)
			{
				XDebug.LogError("プライスが見つかりませんでした");
			}
			result = await webAPIRequester.PostVMPayment(id, price, _vendingMachine.ShopID);
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

	public async UniTask SaveEditData(WebAPIRequester webAPIRequester)
	{
		List<WebAPIRequester.VMSalesData> vmUpdateData = new List<WebAPIRequester.VMSalesData>();

		foreach (VendingMachineBuyUI currentItem in _vendingUIs)
		{
			WebAPIRequester.ItemLineup itemTemp =
				_previousItemLineups
				.Where(previousItem => previousItem.ItemID == currentItem.ID)
				.FirstOrDefault();

			//追加の場合
			if (itemTemp.ItemID <= 0)
			{
				currentItem.ID.PrintWarning();
				WebAPIRequester.VMSalesData salesData
						= new WebAPIRequester.VMSalesData(
							currentItem.ID,
							currentItem.Stock,
							currentItem.Price,
							false
							);
				vmUpdateData.Add(salesData);
			}
			//削除されずに在庫、値段が更新された場合
			else if (itemTemp.Stock != currentItem.Stock ||
				itemTemp.Price != currentItem.Price)
			{
				WebAPIRequester.VMSalesData salesData
					= new WebAPIRequester.VMSalesData(
						currentItem.ID,
						itemTemp.Stock - currentItem.Stock,
						currentItem.Price,
						false
					);
				vmUpdateData.Add(salesData);
			}
		}
		foreach (WebAPIRequester.ItemLineup item in _previousItemLineups)
		{
			item.PrintWarning();
			VendingMachineBuyUI uiTemp = _vendingUIs.Where(ui => ui.ID == item.ItemID).FirstOrDefault();
			if (uiTemp != null) { continue; }
			//削除されたもの
			WebAPIRequester.VMSalesData salesData
					= new WebAPIRequester.VMSalesData(
						item.ItemID,
						item.Stock,
						item.Price,
						true
						);
			vmUpdateData.Add(salesData);
		}

		WebAPIRequester.OnVMProductData currentVMInventory
			= await webAPIRequester.PostVMUpdate(_vendingMachine.ShopID, vmUpdateData);
		InitUI(currentVMInventory);
		OpenEditerButtons();

	}

	public void DeleteProduct(int id)
	{
		VendingMachineBuyUI deleteUI = _vendingUIs
			.Where(vendingUI => vendingUI.ID == id)
			.FirstOrDefault();
		_vendingUIs.Remove(deleteUI);
		ReplaceBuyUI();
		OpenBuyUI();
	}

	public async UniTask UpdateOrAddProduct(int id, int price, int count)
	{
		VendingMachineBuyUI updateUI = _vendingUIs.Where(ui => ui.IsChanging).FirstOrDefault();
		if (updateUI == null)
		{
			updateUI = Instantiate(_vendingMachineUIPrefab);
			_vendingUIs.Add(updateUI);
		}
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		InitBuyUI(updateUI, id, price, count);
		UpdateUI(updateUI.ID, updateUI.Stock);


		await PlayerDontDestroyData.Instance.UpdateInventory(webAPIRequester);
		await SaveEditData(webAPIRequester);
		await InitUI(webAPIRequester);

	}

	#region Open関数
	public void OpenPageControlButton()
	{
		if (_currentPageCount <= 1)
		{
			ClosePreciousButton();
		}
		if (_currentPageCount >= _maxPageCount)
		{
			CloseNextButton();
		}
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

	public void OpenBuyUI()
	{
		for (int i = ((int)_currentPageCount - 1) * _displayUICount; i < _vendingUIs.Count; i++)
		{
			if (i / _displayUICount == _currentPageCount) { break; }
			_vendingUIs[i].gameObject.SetActive(true);
		}
	}

	public void OpenAddProductButton()
	{
		_addButton.SetActive(true);
	}

	public void OpenBuyButton()
	{
		foreach (VendingMachineBuyUI ui in _vendingUIs)
		{
			ui.BuyButton.SetActive(true);
		}
	}

	public void OpenEditerButtons()
	{
		foreach (VendingMachineBuyUI ui in _vendingUIs)
		{
			ui.OpenChangeProductButton();
			ui.OpenDeleteButton();
		}
		OpenAddProductButton();
	}

	public void OpenDeleteButtons()
	{
		foreach (VendingMachineBuyUI ui in _vendingUIs)
		{
			ui.OpenDeleteButton();
		}
	}

	public void OpenEditUIButtons()
	{
		foreach (VendingMachineEditUI vendingMachineEditUI in _vendingEditUIs)
		{
			vendingMachineEditUI.Buttons.SetActive(true);
		}
	}

	private void OpenNextButton()
	{
		_nextButton.SetActive(true);
	}

	private void OpenPreviousButton()
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
		RectTransform prefabRectTranform = _vendingMachineEditUIPrefab.transform as RectTransform;
		float offsetX = prefabRectTranform.sizeDelta.x + _mergin.x;
		float offsetY = prefabRectTranform.sizeDelta.y + _mergin.y;
		int containItemsCount = PlayerDontDestroyData.Instance.InventoryToList.Count;
		int editMenuRowMax = containItemsCount / _editMenuColMax;
		if (containItemsCount % _editMenuColMax > 0)
		{
			editMenuRowMax++;
		}
		_vendingEditUIs.Clear();
		foreach (ItemIDAmountPair itemIDAmountPair in PlayerDontDestroyData.Instance.InventoryToList)
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
		editMenuRowMax--;
		float rowCenter = editMenuRowMax / 2f;
		foreach (RectTransform rectTransform in _editPanel.transform as RectTransform)
		{
			rectTransform.anchoredPosition +=
				Vector2.left * (rowCenter * offsetX) +
				Vector2.up * (offsetY);
		}
	}

	public void OpenEditPricePanel(int id, int count)
	{
		_vendingMachineEditPriceUI.gameObject.SetActive(true);
		_vendingMachineEditPriceUI.Init(id, count);
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
		CloseEditterButtons();
		CloseEditPricePanel();
	}

	public void CloseAddProductButton()
	{
		_addButton.SetActive(false);
	}

	public void CloseBuyUI()
	{
		foreach (VendingMachineBuyUI ui in _vendingUIs)
		{
			ui.gameObject.SetActive(false);
			ui.BuyButton.SetActive(false);
			ui.CloseChangeProductButton();
			ui.CloseDeleteButton();
		}
	}

	public void CloseEditterButtons()
	{
		foreach (VendingMachineBuyUI ui in _vendingUIs)
		{
			ui.CloseDeleteButton();
			ui.CloseChangeProductButton();
		}
		CloseAddProductButton();
	}

	public void ClosePageControlButton()
	{
		CloseNextButton();
		ClosePreciousButton();
	}

	private void CloseNextButton()
	{
		if (_nextButton == null) { return; }
		_nextButton.SetActive(false);
	}

	private void ClosePreciousButton()
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
