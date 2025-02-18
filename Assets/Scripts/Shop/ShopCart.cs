using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ShopCart : MonoBehaviour
{
	//ID,å¬êî
	private Dictionary<int, int> _inCartsAmount = new();
	public Dictionary<int, int> InCarts { get => _inCartsAmount; }
	[SerializeField]
	private ShopCartUIManager _shopCartUIManager = default;
	[SerializeField]
	private VisualShop _visualShop = default;

	public void AddCart(int id)
	{
		if (_inCartsAmount.ContainsKey(id))
		{
			_inCartsAmount[id]++;
		}
		else
		{
			_inCartsAmount.Add(id, 1);
		}
		_shopCartUIManager.AddCartUI(id);
	}
	public int ClacTotalPrice()
	{
		int ans = 0;
		foreach (KeyValuePair<int, int> pair in _inCartsAmount)
		{
			ans += _visualShop.GetPrice(pair.Key) * pair.Value;
		}
		return ans;
	}
	public void RemoveCart(int id)
	{
		if (_inCartsAmount[id] <= 1)
		{
			_inCartsAmount.Remove(id);
			_inCartsAmount.TrimExcess();
		}
		else
		{
			_inCartsAmount[id]--;
		}
	}

	private void toItemStockList(List<WebAPIRequester.ItemIDAmountPricePair> itemStocks)
	{
		foreach (var item in _inCartsAmount)
		{
			WebAPIRequester.ItemIDAmountPricePair stockTemp
				= new WebAPIRequester.ItemIDAmountPricePair(item.Key, item.Value,_visualShop.GetPrice(item.Key));
			itemStocks.Add(stockTemp);
		}

	}

	public async UniTaskVoid Buy()
	{
		WebAPIRequester requester = new WebAPIRequester();
		List<WebAPIRequester.ItemIDAmountPricePair> buyItemStocks = new List<WebAPIRequester.ItemIDAmountPricePair>();
		toItemStockList(buyItemStocks);

		await requester.PostShopPayment(buyItemStocks, _visualShop.ShopID);
		await PlayerDontDestroyData.Instance.UpdateInventory(requester);
		await PlayerDontDestroyData.Instance.UpdateMoney(requester);

		_inCartsAmount.Clear();
	}
}
