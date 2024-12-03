using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	public async void Buy()
	{
		_visualShop.Buy();
		foreach (KeyValuePair<int, int> pair in _inCarts)
		{
			for (int i = 0;i < pair.Value ; i++)
			{
				FindObjectOfType<InventoryManager>().SendItem(pair.Key);
			}
		}
		//データベースにリクエストをとばす
		try
		{

		}
		catch
		{

		}
		_inCarts.Clear();
	}
}
