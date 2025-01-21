using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// ワールドに配置可能なオブジェクト
/// </summary>
public class PlaceableObject : MonoBehaviour
{
	[Tooltip("rootを推奨")]
	[SerializeField] private GameObject ghostOrigin = default;
	[Tooltip("原点が中心にあるか足元（中心から下方向に伸びた点）にあるか")]
	[SerializeField] private GhostModel.PivotType pivotType = default;
	[Tooltip("配置可能場所")]
	[SerializeField] private GhostModel.PlacingStyle placingStyle = default;
	[SerializeField] private List<Collider> colliders = default;
	[Hide] private int housingID = -1;

	public GameObject GhostOrigin => ghostOrigin;
	public GhostModel.PivotType PivotType => pivotType;
	public GhostModel.PlacingStyle PlacingStyle => placingStyle;
	public IReadOnlyList<Collider> Colliders => colliders;
	public int HousingID { get => housingID; set => housingID = value; }

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	protected virtual void Reset()
	{
		ghostOrigin = transform.root.gameObject;
	}
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
	[CustomEditor(typeof(PlaceableObject))]
	public class PlaceableObjectEditor : Editor
	{
		private PlaceableObject placeableObject = default;
		private FieldInfo housingIDInfo = default;

		private void OnEnable()
		{
			placeableObject = target as PlaceableObject;
			housingIDInfo = placeableObject.GetType()
				.GetField("housingID", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (housingIDInfo != null)
			{
				int id = (int)housingIDInfo.GetValue(placeableObject);
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.IntField("Housing ID", id);
				EditorGUI.EndDisabledGroup();
			}
		}
	}
}
#endif
