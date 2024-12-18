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
	private int test;

	public void Init(Sprite itemSprite, ShopCartUIManager shopCartUIManager, 
		Vector2 popAnchoredPosition, int id,DragSystem dragSystem,
		ScrollTransformInject scrollTransformInject,int test)
	{
		_id = id;
		RectTransform myRectTransform = this.transform as RectTransform;
		_shopCartUIManager = shopCartUIManager;
		_image.sprite = itemSprite;
		myRectTransform.anchoredPosition = popAnchoredPosition;
		dragSystem.ScrollableInject(_yScrollObject);
		scrollTransformInject.Inject(_yScrollObject);
		_currentColLimit -= myRectTransform.localPosition.y;
		_yScrollObject.InjectDownLimit(myRectTransform.localPosition.y);
		XKumaDebugSystem.LogError("down:"+myRectTransform.localPosition.y);
		this.test = test + 1;
	}
	public void UpdateLimit(float colLimitGap)
	{
		_currentColLimit += colLimitGap;
		XKumaDebugSystem.LogError("up:"+_currentColLimit+":"+_id + colLimitGap);
		_yScrollObject.InjectUpLimit(_currentColLimit);
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
