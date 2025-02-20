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
	private int _currentPage = 1;
	private List<MyRoomJumpButton> _buttons;

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
		_buttons = new();
		foreach (var item in PlayerDontDestroyData.Instance.AllPlayerNames)
		{
			MyRoomJumpButton button = Instantiate(_jumpButtonPrefab, _parent);
			_buttons.Add(button);
			button.Init(item.Key, item.Value);
			RectTransform rectTransform = button.transform as RectTransform;
			rectTransform.anchoredPosition = new Vector2(
				(count % _rowMax) * offset.x + _initPosition.anchoredPosition.x,
				-(count / _rowMax) * offset.y + _initPosition.anchoredPosition.y
			);
			count++;
			if (isLimitOver)
			{
				button.gameObject.SetActive(false);
			}
			if (count >= displayUILimit)
			{
				isLimitOver = true;
				count = 0;
			}
		}
	}

	public void NextPage()
	{
		if (_currentPage > _buttons.Count / (_rowMax * _colMax)) { return; }
		XDebug.LogWarning("next");
		_currentPage++;
		ClosePage();
		UpdatePage();
	}

	private void UpdatePage()
	{
		int displayCount = _rowMax * _colMax;
		for (int i = ((int)_currentPage - 1) * displayCount; i < _buttons.Count; i++)
		{
			if (i / displayCount == _currentPage) { break; }
			_buttons[i].gameObject.SetActive(true);
		}
	}

	private void ClosePage()
	{
		foreach(var item in _buttons)
		{
			item.gameObject.SetActive(false);
		}
	}

	public void PreviousPage()
	{
		if(_currentPage <= 1) { return; }
		XDebug.LogWarning("previous");
		_currentPage--;
		ClosePage();
		UpdatePage();
	}
}
