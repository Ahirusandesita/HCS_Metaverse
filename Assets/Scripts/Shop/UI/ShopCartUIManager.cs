using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class ShopCartUIManager : MonoBehaviour
{
	[SerializeField]
	private ItemBundleAsset _allItemAsset;
	[SerializeField]
	private InCartItemUI _itemIconPrefab;
	[SerializeField]
	private ShopCart _shopCart;
	[SerializeField]
	private Transform _shopUIParent;
	[SerializeField]
	private RectTransform _startPosition;
	[SerializeField]
	private GameObject _shopCanvasObject;
	[SerializeField]
	private ProductUI _productUIPrefab;
	[SerializeField]
	private Transform _productParent;
	[SerializeField]
	private TextMeshProUGUI _totalPriceText;
	[SerializeField]
	private RectTransform _startPositionTransform;
	[SerializeField]
	private Vector3 _priceCardPositionOffset = default;
	private Dictionary<int, ProductUI> _productUIs = new();
	private Dictionary<int, InCartItemUI> _itemIcons = new();
	private float _offsetX = 100;
	private float _offsetY = 100;
	private int _horizontalLimit = 3;


	private void Start()
	{
		RectTransform iconTransform = _itemIconPrefab.transform as RectTransform;
		TatalPriceDisplay();
		_offsetX = iconTransform.sizeDelta.x;
		_offsetY = iconTransform.sizeDelta.y;
		_horizontalLimit = Mathf.FloatToHalf((_shopUIParent.transform as RectTransform).sizeDelta.x / _offsetX);
	}

	public void AddProductUI(int id, int price,int discountedPrice, int stock,float discount, Vector3 position)
	{
		ProductUI productUITemp = Instantiate(_productUIPrefab,_productParent);
		productUITemp.Init(price,discountedPrice,discount,stock);
		_productUIs.Add(id, productUITemp);

		productUITemp.transform.position = position + _priceCardPositionOffset;
	}

	public void AddCartUI(int id)
	{
		if (!_shopCanvasObject.activeSelf)
		{
			_shopCanvasObject.SetActive(true);
		}
		InCartItemUI uiIconTemp;
		if (!_itemIcons.Keys.Contains(id))
		{
			ItemAsset itemAsset = _allItemAsset.GetItemAssetByID(id);
			Vector2 popAnchoredPosition =
				_startPosition.anchoredPosition
				+ Vector2.right * (_itemIcons.Count % _horizontalLimit) * _offsetX
				+ Vector2.up * (_itemIcons.Count / _horizontalLimit) * -_offsetY;
			uiIconTemp = Instantiate(_itemIconPrefab, _shopUIParent);
			uiIconTemp.Init(itemAsset.ItemIcon, this, popAnchoredPosition, id);
			_itemIcons.Add(id, uiIconTemp);
		}
		else
		{
			uiIconTemp = _itemIcons[id];
		}
		uiIconTemp.UpdateCount(_shopCart.InCarts[id]);
		TatalPriceDisplay();
	}

	private void TatalPriceDisplay()
	{
		_totalPriceText.text = _shopCart.ClacTotalPrice().ToString("c");
	}

	public void BuyButtonPush()
	{
		Clear();
		_shopCart.Buy();
	}
	private void Clear()
	{
		foreach (InCartItemUI uiIcon in _itemIcons.Values)
		{
			Destroy(uiIcon.gameObject);
		}
		_itemIcons.Clear();
		_shopCanvasObject.SetActive(false);
	}

	public void DestoryCartUI(int id)
	{
		_shopCart.RemoveCart(id);
		if (!_shopCart.InCarts.Keys.Contains(id))
		{
			_itemIcons[id].UpdateCount(0);
			_itemIcons.Remove(id);
			if (0 >= _shopCart.InCarts.Keys.Count)
			{
				_shopCanvasObject.SetActive(false);
			}
		}
		else
		{
			_itemIcons[id].UpdateCount(_shopCart.InCarts[id]);
		}
	}
}
