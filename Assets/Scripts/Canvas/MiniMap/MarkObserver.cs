using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MarkObserver : MonoBehaviour
{
	[SerializeField]
	private Camera _mapCamera = default;
	private List<Mark> _mapMarks = new();
	private List<Mark> _cameraInMarks = new();
	[SerializeField]
	private RawImage _mapImage = default;
	private MarkManager _markManager = default;
	

#if UNITY_EDITOR
	private void Reset()
	{
		_mapCamera ??= GameObject.Find("MapCamera").GetComponent<Camera>();
	}
#endif

	private void Start()
	{
		_mapMarks = FindObjectsOfType<Mark>().ToList();
		_markManager = FindObjectOfType<MarkManager>();
	}

	private void Update()
	{
		//カメラ内マークを初期化する
		_cameraInMarks.Clear();
		for (int i = 0; i < _mapMarks.Count; i++)
		{
			Transform mapMarkTransform = _mapMarks[i].transform;
			Vector2 viewportPosition = _mapCamera.WorldToViewportPoint(mapMarkTransform.position);
			//範囲内
			if (viewportPosition.x < 1 && viewportPosition.x > 0
				&& viewportPosition.y < 1 && viewportPosition.y > 0)
			{
				_cameraInMarks.Add(_mapMarks[i]);
			}
			else { continue; }
			//pivotが違う場合に補正する
			Vector2 offset = default;
			if (_mapImage.rectTransform.pivot != Vector2.zero)
			{
				float x = _mapImage.rectTransform.rect.width * _mapImage.rectTransform.pivot.x 
					* _mapImage.rectTransform.localScale.x;
				float y = _mapImage.rectTransform.rect.height * _mapImage.rectTransform.pivot.x
					* _mapImage.rectTransform.localScale.y;
				offset = new Vector2(x, y);
			}
			//localPositionを渡す
			_mapMarks[i].MarkViewPosition((viewportPosition * _mapImage.rectTransform.sizeDelta
				* _mapImage.rectTransform.localScale) - offset);
		}
		if (_cameraInMarks.Count == 0) { return; }
		_markManager.MarkInCamera(_cameraInMarks.ToArray());
	}
}
