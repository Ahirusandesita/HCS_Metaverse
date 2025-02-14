using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VendingMachineBuyUI : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _discountedPriceText;
	[SerializeField]
	private TextMeshProUGUI _nameText;
	[SerializeField]
	private Image _image;
	[SerializeField]
	private GameObject _soldoutImageObject;
	[SerializeField]
	private GameObject _buyButton;
	[SerializeField]
	private GameObject _editChangeProductButton;
	[SerializeField]
	private GameObject _editDeleteButton;

	private VendingMachineUIManager _uiManager;
	[SerializeField,Hide]
	private int _id = -1;
	[SerializeField,Hide]
	private bool _isChanging = false;
	[SerializeField, Hide]
	private int _stock = 0;
	[SerializeField, Hide]
	private int _price;
	public int Stock { get => _stock;}
	public int ID => _id;
	public int Price => _price;
	public bool IsChanging => _isChanging;
	public GameObject BuyButton => _buyButton;
	public void Init(int id,int discountedPrice
		,VendingMachineUIManager uiManager,Sprite sprite
		,string name,int stock)
	{
		_id = id;
		_price = discountedPrice;
		_image.sprite = sprite;
		_uiManager = uiManager;
		_nameText.text = name;
		_discountedPriceText.text = discountedPrice.ToString("c");
		_soldoutImageObject.SetActive(false);
		_isChanging = false;
		_stock = stock;
	}

	public void SoldOut()
	{
		_soldoutImageObject.SetActive(true);
		_buyButton.SetActive(false);
	}

	public void DeleteProduct()
	{
		_uiManager.DeleteProduct(_id);
		Destroy(this.gameObject);
	}

	public void OpenProductChangeMode()
	{
		_uiManager.OpenEditPanel();
		_isChanging = true;
	}

	public void Buy()
	{
		_uiManager.Buy(_id);
	}

	public void OpenChangeProductButton()
	{
		_editChangeProductButton.SetActive(true);
	}

	public void OpenDeleteButton()
	{
		_editDeleteButton.SetActive(true);
	}

	public void CloseChangeProductButton()
	{
		_editChangeProductButton.SetActive(false);
	}

	public void CloseDeleteButton()
	{
		_editDeleteButton.SetActive(false);
	}
}
