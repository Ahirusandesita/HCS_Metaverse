using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

	public void AddCartUI(int id)
	{
		UIIcon uiIconTemp;
		if (!_itemIcons.Keys.Contains(id))
		{
			ItemAsset itemAsset = _allItemAsset.GetItemAssetByID(id);
			uiIconTemp = Instantiate(_itemIconPrefab,_parent);
			RectTransform rectTransformTemp = uiIconTemp.transform as RectTransform;
			Vector3 offset = new Vector3(100 * (_itemIcons.Count % 3), -100 * (_itemIcons.Count / 3)  , 0) ;
			rectTransformTemp.localPosition = _startPosition.localPosition + offset;
			MeshFilter[] meshFilters = itemAsset.DisplayItem.gameObject.GetComponentsInChildren<MeshFilter>();
			MeshRenderer[] meshRenderers = itemAsset.DisplayItem.gameObject.GetComponentsInChildren<MeshRenderer>();
			uiIconTemp.Init(meshFilters.Clone() as MeshFilter[],meshRenderers.Clone() as MeshRenderer[], this,id);
			_itemIcons.Add(id, uiIconTemp);
		}
		else
		{
			uiIconTemp = _itemIcons[id];
		}
		uiIconTemp.UpdateCount(_shopCart.InCarts[id]);

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
