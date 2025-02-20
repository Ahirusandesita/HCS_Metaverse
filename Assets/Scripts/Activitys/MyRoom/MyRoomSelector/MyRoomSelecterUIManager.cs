using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRoomSelecterUIManager : MonoBehaviour,IPageController
{
	[SerializeField]
	private MyRoomJumpButton _jumpButtonPrefab;
	[SerializeField]
	private PageButton _pageButtonPrefab;
	[SerializeField]
	private Transform _parent;
	[SerializeField]
	private RectTransform _initPosition;
	[SerializeField]
	private int _rowMax;
	[SerializeField]
	private int _colMax;
	[SerializeField]
	private Vector2 _mergin;

	public void Init()
	{
		int displayUILimit = _rowMax * _colMax;
		bool isLimitOver = false;
		int count = 0;
		RectTransform prefabRectTransform = _jumpButtonPrefab.transform as RectTransform;

		Vector2 offset = _mergin +
			new Vector2(
				prefabRectTransform.sizeDelta.x,
				prefabRectTransform.sizeDelta.y
				);
		foreach (var item in PlayerDontDestroyData.Instance.AllPlayerNames)
		{
			MyRoomJumpButton button = Instantiate(_jumpButtonPrefab, _parent);
			button.Init(item.Key, item.Value);
			RectTransform rectTransform = button.transform as RectTransform;
			rectTransform.anchoredPosition = new Vector2(
				(count % _rowMax) * offset.x + _initPosition.anchoredPosition.x,
				-(count / _rowMax) * offset.y + _initPosition.anchoredPosition.y
			);
			count++;
			if (count > displayUILimit)
			{
				isLimitOver = true;
				count = 0;
			}
			if (isLimitOver)
			{
				button.gameObject.SetActive(false);
			}
		}
	}

	public void NextPage()
	{
		throw new System.NotImplementedException();
	}

	public void PreviousPage()
	{
		throw new System.NotImplementedException();
	}
}
