using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCart : MonoBehaviour
{
	private Dictionary<int, int> _inCarts = new();
	public Dictionary<int, int> InCarts { get => _inCarts; }
	[SerializeField]
	private ShopCartUIManager _shopCartUIManager = default;

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
		}
		else
		{
			_inCarts[id]--;
		}
	}

	private void Buy()
	{

		_inCarts.Clear();
	}
}
