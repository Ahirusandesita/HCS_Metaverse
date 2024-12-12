using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory = WebAPIRequester.OnPaymentData.Inventory;
using Cysharp.Threading.Tasks;

public class ShopCart : MonoBehaviour
{
	private Dictionary<int, int> _inCarts = new();
	//ID,個数
	public Dictionary<int, int> InCarts { get => _inCarts; }
	[SerializeField]
	private ShopCartUIManager _shopCartUIManager = default;
	[SerializeField]
	private VisualShop _visualShop = default;

	public void AddCart(int id)
	{
		if (_inCarts.ContainsKey(id))
		{
			_inCarts[id]++;
		}
		else
		{
			_inCarts.Add(id, 1);
		}
		_shopCartUIManager.AddCartUI(id);
	}
	public int ClacTotalPrice()
	{
		int ans = 0;
		foreach (KeyValuePair<int, int> pair in _inCarts)
		{
			ans += _visualShop.GetPrice(pair.Key) * pair.Value;
		}
		return ans;
	}
	public void RemoveCart(int id)
	{
		if (_inCarts[id] <= 1)
		{
			_inCarts.Remove(id);
			_inCarts.TrimExcess();
		}
		else
		{
			_inCarts[id]--;
		}
	}

	private void InventoryNiTumeru(List<Inventory> inventories)
	{
		foreach (var item in _inCarts)
		{
			Inventory inventoryTemp = new Inventory(item.Key,item.Value);
			inventories.Add(inventoryTemp);
		}

	}

	public async void Buy()
	{
		WebAPIRequester requester = new WebAPIRequester();
		List<Inventory> inventories = new List<Inventory>();
		InventoryNiTumeru(inventories);
		_visualShop.Buy();

		foreach (KeyValuePair<int, int> pair in _inCarts)
		{
			for (int i = 0; i < pair.Value; i++)
			{
				FindObjectOfType<InventoryManager>().SendItem(pair.Key);
			}
		}
		//データベースにリクエストをとばす
			var data = await requester.PostShopPayment(inventories,0,0);
		try
		{
		XDebug.LogError(inventories.Count);
		}
		catch
		{

		}
		_inCarts.Clear();
	}
}
