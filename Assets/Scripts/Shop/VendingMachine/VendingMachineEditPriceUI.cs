using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class VendingMachineEditPriceUI : MonoBehaviour
{
	[SerializeField]
	private VendingMachineUIManager _vendingMachineUIManager;
	[SerializeField]
	private VendingMachineEditPriceDigitUI _digitControllerPrefab;
	[SerializeField, Hide]
	private int _price = default;
	[SerializeField, Hide]
	private int _id = default;
	[SerializeField, HideAtPlaying]
	private int _horizontalSpace;
	private VendingMachineEditPriceDigitUI[] _editPriceDigitUIs;
	private const int _MIN_PRICE = 0;
	[SerializeField]
	private int _maxPrice = 99999;
	[SerializeField, Hide]
	private int _maxDigit = default;
	private int _count;

	public int Price
	{
		get => _price;
		set
		{
			if (value < _MIN_PRICE)
			{
				_price = _MIN_PRICE;
			}
			else if (value > _maxPrice)
			{
				_price = _maxPrice;
			}
			else
			{
				_price = value;
			}
		}
	}

	public void TextUpdate()
	{
		int digitTemp = _maxDigit;
		int priceTemp = _price;
		for (int i = 0; i < _editPriceDigitUIs.Length; i++)
		{
			int currentDigit = (priceTemp / digitTemp);
			_editPriceDigitUIs[i].UpdateText(currentDigit);
			priceTemp -= currentDigit * digitTemp;
			digitTemp /= 10;
		}
	}

	public void Init(int id ,int count)
	{
		_id = id;
		_price = _MIN_PRICE;
		_count = count;
		int digitCount = _maxPrice.ToString().Length;
		if (_editPriceDigitUIs != null)
		{
			foreach (VendingMachineEditPriceDigitUI digitUI in _editPriceDigitUIs)
			{
				Destroy(digitUI.gameObject);
			}
		}
		_editPriceDigitUIs = new VendingMachineEditPriceDigitUI[digitCount];
		Vector2 startAnchoredPosition = (
			-(digitCount / 2 * _horizontalSpace) +
			((1 - digitCount % 2) * (_horizontalSpace / 2)
			)) * Vector2.right;
		for (int i = digitCount - 1; 0 <= i; i--)
		{
			_maxDigit = Pow(10, i);
			VendingMachineEditPriceDigitUI editPriceDigitUI = Instantiate(_digitControllerPrefab, this.transform);
			RectTransform rectTransform = editPriceDigitUI.transform as RectTransform;
			rectTransform.anchoredPosition =
				startAnchoredPosition
				+ Vector2.right * (_horizontalSpace * i);
			_editPriceDigitUIs[i] = editPriceDigitUI;
			editPriceDigitUI.Init(_maxDigit, this);
		}
		_maxDigit = Pow(10, digitCount - 1);
	}

	private int Pow(int value, int p)
	{
		if (p == 0) { return 1; }
		int result = value;
		for (; 1 < p; p--)
		{
			result *= value;
		}
		return result;
	}

	public async UniTask<bool> Submit()
	{
		if (_price <= 0)
		{
			return false;
		}
		await _vendingMachineUIManager.UpdateOrAddProduct(_id, _price,_count);
		_vendingMachineUIManager.CloseEditPricePanel();
		_vendingMachineUIManager.CloseEditPanel();
		_vendingMachineUIManager.CloseEditReturnBuyMenuButton();
		_vendingMachineUIManager.OpenEditerButtons();
		_vendingMachineUIManager.OpenBuyUI();

		return true;
	}

	public void Close()
	{
		_vendingMachineUIManager.CloseEditPricePanel();
		_vendingMachineUIManager.OpenEditReturnBuyMenuButton();
		_vendingMachineUIManager.OpenEditUIButtons();
		_vendingMachineUIManager.OpenEditUICountText();
	}
}
