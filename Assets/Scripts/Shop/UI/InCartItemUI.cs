using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using KumaDebug;
using UnityEngine.UI;

public class InCartItemUI : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _countText = default;
	[SerializeField]
	private Image _image = default;
	private ShopCartUIManager _shopCartUIManager = default;
	[SerializeField]
	private YScrollObject _yScrollObject;
	private float _currentColLimit;
	private int _id;


	public void Init(Sprite itemSprite, ShopCartUIManager shopCartUIManager, Vector2 popAnchoredPosition, int id)
	{
		_id = id;
		RectTransform myRectTransform = this.transform as RectTransform;
		_yScrollObject.InjectDownLimit(myRectTransform.localPosition.y);
		_shopCartUIManager = shopCartUIManager;
		_image.sprite = itemSprite;
		myRectTransform.anchoredPosition = popAnchoredPosition;
	}
	public void UpdateLimit(float colLimitGap)
	{
		_currentColLimit += colLimitGap;
		_yScrollObject.InjectDownLimit(_currentColLimit);
	}

	public void UpdateCount(int count)
	{
		if (count == 0)
		{
			Destroy(this.gameObject);
			return;
		}
		_countText.SetText(count.ToString("x#"));
	}

	public void Delete()
	{
		_shopCartUIManager.DestoryCartUI(_id);
	}
}
