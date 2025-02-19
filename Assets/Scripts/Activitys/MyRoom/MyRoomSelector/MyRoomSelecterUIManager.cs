using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRoomSelecterUIManager : MonoBehaviour
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
		int count = 0;
		Vector2 offset = _mergin +
			new Vector2(
				_jumpButtonPrefab.transform.lossyScale.x,
				_jumpButtonPrefab.transform.lossyScale.y
				);
		foreach (var item in PlayerDontDestroyData.Instance.AllPlayerNames)
		{
			count++;
			MyRoomJumpButton button = Instantiate(_jumpButtonPrefab, _parent);
			button.Init(item.Key, item.Value);
			RectTransform rectTransform = button.transform as RectTransform;
			rectTransform.localPosition = new Vector2(
				(count % _rowMax) * offset.x,
				(count / _colMax) * offset.y
			);
		}
	}
}
