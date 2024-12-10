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
	private int _id;

	public void Init(Sprite itemSprite,ShopCartUIManager shopCartUIManager,Vector2 popAnchoredPosition,int id)
	{
		_id = id;
		RectTransform myTransform = this.transform as RectTransform;
		_shopCartUIManager = shopCartUIManager;
		_image.sprite = itemSprite;
		myTransform.anchoredPosition = popAnchoredPosition;
	}

	public void UpdateCount(int count)
	{
		if(count == 0)
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
