using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using KumaDebug;

public class MiniMapImage : MonoBehaviour
{
	private List<Mark> _mapMarks = new();
	private List<Mark> _cameraInMark = new();
	private RawImage _mapImage = default;
	private Camera _mapCamera = default;
	private MarkManager _markManager = default;
	private void Start()
	{
		_mapMarks = FindObjectsOfType<Mark>().ToList();
		_mapImage = GetComponent<RawImage>();
		_mapCamera = GameObject.Find("MapCamera").GetComponent<Camera>();
		_markManager = FindObjectOfType<MarkManager>();
	}

	private void Update()
	{
		//メモ：カメラ内のMarkをマークマネージャーに送る
		for (int i = 0; i < _mapMarks.Count; i++)
		{
			Transform mapMarkTransform = _mapMarks[i].transform;
			Vector2 viewportPosition = _mapCamera.WorldToViewportPoint(mapMarkTransform.position);
			//範囲内
			if (viewportPosition.x < 1 && viewportPosition.x > 0
				&& viewportPosition.y < 1 && viewportPosition.y > 0)
			{
				_cameraInMark.Add(_mapMarks[i]);
			}
			else { continue; }
			Vector2 position = default;
			if (_mapImage.rectTransform.pivot != Vector2.zero)
			{
				float x = _mapImage.rectTransform.rect.width / 2f * _mapImage.rectTransform.localScale.x;
				float y = _mapImage.rectTransform.rect.height / 2f * _mapImage.rectTransform.localScale.y;
				position = new Vector2(x, y);
			}
			_mapMarks[i].MarkViewPosition((viewportPosition * _mapImage.rectTransform.sizeDelta
				* _mapImage.rectTransform.localScale) - position);
		}
		if(_cameraInMark.Count == 0) { return; }
		_markManager.MarkInCamera(_cameraInMark.ToArray());
		_cameraInMark.Clear();
	}
}
