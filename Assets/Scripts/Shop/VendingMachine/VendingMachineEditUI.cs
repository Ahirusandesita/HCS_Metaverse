using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachineEditUI : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _nameText;
	[SerializeField]
	private TextMeshProUGUI _countText;
	[SerializeField]
	private Image _iconImage;
	[SerializeField]
	private GameObject _buttons;
	private VendingMachineUIManager _uiManager;
	private int _count;
	[SerializeField, Hide]
	private int _id = -1;
	[SerializeField,Hide]
	private int _inventoryHaveCount = default;

	public GameObject Buttons => _buttons;
	public int Count => _count;
	public int ID => _id;

	public void Init(int id, string productName
		, Sprite icon, VendingMachineUIManager vendingMachineUIManager,int inventoryHaveCount)
	{
		_count = 1;
		_inventoryHaveCount = inventoryHaveCount;
		this._uiManager = vendingMachineUIManager;
		this._id = id;
		this._iconImage.sprite = icon;
		this._nameText.text = productName;
		this._countText.text = _count.ToString();
	}

	public void AddCount()
	{
		if(_inventoryHaveCount <= _count) { return; }
		_count++;
		this._countText.text = _count.ToString();
	}

	public void SubtractCount()
	{
		if (_count <= 1) { return; }
		_count--;
		this._countText.text = _count.ToString();
	}

	public void Submit()
	{
		_uiManager.OpenEditPricePanel(_id);
		_uiManager.CloseEditUIButtons();
		_uiManager.CloseEditUICountText();
		_uiManager.CloseEditReturnBuyMenuButton();
	}

	public void CloseText()
	{
		_countText.gameObject.SetActive(false);
	}

	public void OpenText()
	{
		_countText.gameObject.SetActive(true);
	}
}
