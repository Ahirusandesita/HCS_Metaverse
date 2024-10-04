using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MarkObserver : MonoBehaviour
{
	private class MarkData
	{
		public MarkData(Mark mark, MeshRenderer meshRenderer, Transform transform)
		{
			Mark = mark;
			MeshRenderer = meshRenderer;
			Transform = transform;
		}
		public MeshRenderer MeshRenderer { get; private set; }
		public Mark Mark { get; private set; }
		public Transform Transform { get; private set; }
	}
	[SerializeField]
	private Camera _mapCamera = default;
	private List<MarkData> _mapMarks = new();
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
		Mark[] marks = FindObjectsOfType<Mark>();
		for (int i = 0; i < marks.Length; i++)
		{
			_mapMarks.Add(
				 new MarkData(marks[i], marks[i].GetComponent<MeshRenderer>(), marks[i].transform));
		}
		_markManager = FindObjectOfType<MarkManager>();
	}

	private void Update()
	{
		//カメラ内マークを初期化する
		_cameraInMarks.Clear();
		for (int i = 0; i < _mapMarks.Count; i++)
		{

			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_mapCamera);
			System.Array.Resize(ref planes, 4);     // near,far面を排除			
			//範囲内
			if (GeometryUtility.TestPlanesAABB(planes, _mapMarks[i].MeshRenderer.bounds))
			{
				_cameraInMarks.Add(_mapMarks[i].Mark);
			}
			else { continue; }
			Vector3 viewportPosition = _mapCamera.WorldToViewportPoint(_mapMarks[i].Transform.position, Camera.MonoOrStereoscopicEye.Mono);
			//pivotが違う場合に補正する
			Vector2 offset = default;
			if (_mapImage.rectTransform.pivot != Vector2.zero)
			{
				float x = _mapImage.rectTransform.rect.width * _mapImage.rectTransform.pivot.x
					* _mapImage.rectTransform.localScale.x;
				float y = _mapImage.rectTransform.rect.height * _mapImage.rectTransform.pivot.y
					* _mapImage.rectTransform.localScale.y;
				offset = new Vector2(x, y);
			}
			//localPositionを渡す
			_mapMarks[i].Mark.MarkViewPosition((viewportPosition * _mapImage.rectTransform.sizeDelta
				* _mapImage.rectTransform.localScale) - offset);
		}
		if (_cameraInMarks.Count == 0) { return; }
		_markManager.MarkInCamera(_cameraInMarks.ToArray());
	}
}
