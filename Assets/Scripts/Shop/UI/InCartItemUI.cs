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
	public int ID => _id;
	private int test;

	public bool CanDestory = false;
	private float testDownLimit;
	public void Init(Sprite itemSprite, ShopCartUIManager shopCartUIManager, 
		Vector2 popAnchoredPosition, int id,DragSystem dragSystem,
		ScrollTransformInject scrollTransformInject,int test,Vector2 nowPosition)
	{
		_id = id;
		RectTransform myRectTransform = this.transform as RectTransform;
		_shopCartUIManager = shopCartUIManager;
		_image.sprite = itemSprite;
		myRectTransform.anchoredPosition = nowPosition;
		dragSystem.ScrollableInject(_yScrollObject);
		scrollTransformInject.Inject(_yScrollObject);
		//_currentColLimit -= myRectTransform.localPosition.y;
		testDownLimit = popAnchoredPosition.y;
		_yScrollObject.InjectDownLimit(popAnchoredPosition.y);
		UpLimit(popAnchoredPosition.y);
		//XKumaDebugSystem.LogError("down:"+myRectTransform.localPosition.y);
		this.test = test + 1;
	}
	private void UpLimit(float limit)
    {
		_currentColLimit = limit;
		_yScrollObject.InjectUpLimit(limit);
    }
	public void UpdateLimit(float colLimitGap)
	{
		_yScrollObject.InjectUpLimit(_currentColLimit + colLimitGap);
	}
	public void UpdateDownLimit(float limit)
    {
		testDownLimit += limit;
		_yScrollObject.InjectDownLimit(testDownLimit);
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && CanDestory)
        {
			Delete();
        }
    }
}
