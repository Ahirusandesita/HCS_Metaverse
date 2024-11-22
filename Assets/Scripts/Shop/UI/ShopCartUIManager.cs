using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using KumaDebug;

public class ShopCartUIManager : MonoBehaviour
{
	[SerializeField]
	private ItemBundleAsset _allItemAsset;
	[SerializeField]
	private UIIcon _itemIconPrefab;
	[SerializeField]
	private ShopCart _shopCart;
	[SerializeField]
	private Transform _parent;
	[SerializeField]
	private RectTransform _startPosition;
	private Dictionary<int, UIIcon> _itemIcons = new();
	private float _offsetX = 100;
	private float _offsetY = 100;
	private int _horizontalLimit = 3;

	private void Start()
	{
		RectTransform iconTransform = _itemIconPrefab.transform as RectTransform;

		_offsetX = iconTransform.sizeDelta.x;
		_offsetY = iconTransform.sizeDelta.y;
		_horizontalLimit = Mathf.FloatToHalf((_parent.transform as RectTransform).sizeDelta.x / _offsetX);
	}

	public void AddCartUI(int id)
	{
		UIIcon uiIconTemp;
		if (!_itemIcons.Keys.Contains(id))
		{
			ItemAsset itemAsset = _allItemAsset.GetItemAssetByID(id);
			Vector2 popAnchoredPosition =
				_startPosition.anchoredPosition
				+ Vector2.right * (_itemIcons.Count % _horizontalLimit) * _offsetX
				+ Vector2.up * (_itemIcons.Count / _horizontalLimit) * -_offsetY;
			uiIconTemp = Instantiate(_itemIconPrefab, _parent);
			uiIconTemp.Init(itemAsset.ItemIcon, this, popAnchoredPosition, id);
			_itemIcons.Add(id, uiIconTemp);
		}
		else
		{
			uiIconTemp = _itemIcons[id];
		}
		uiIconTemp.UpdateCount(_shopCart.InCarts[id]);

	}

	public void BuyButtonPush()
	{
		_shopCart.Buy();
		
	}

	public void DestoryCartUI(int id)
	{
		_shopCart.RemoveCart(id);
		if (!_shopCart.InCarts.Keys.Contains(id))
		{
			_itemIcons[id].UpdateCount(0);
			_itemIcons.Remove(id);
		}
		else
		{
			_itemIcons[id].UpdateCount(_shopCart.InCarts[id]);
		}
	}
}
