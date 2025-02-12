using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// ワールドに配置可能なオブジェクト
/// </summary>
public class PlaceableObject : SafetyInteractionObject
{
	[Tooltip("rootを推奨")]
	[SerializeField] private GameObject ghostOrigin = default;
	[Tooltip("原点が中心にあるか足元（中心から下方向に伸びた点）にあるか")]
	[SerializeField] private GhostModel.PivotType pivotType = default;
	[Tooltip("配置可能場所")]
	[SerializeField] private GhostModel.PlacingStyle placingStyle = default;
	[SerializeField] private List<Collider> colliders = default;
	private int itemID = default;
	private int housingID = -1;

	public GameObject GhostOrigin => ghostOrigin;
	public GhostModel.PivotType PivotType => pivotType;
	public GhostModel.PlacingStyle PlacingStyle => placingStyle;
	public IReadOnlyList<Collider> Colliders => colliders;
	public int ItemID { get => itemID; set => itemID = value; }
	public int HousingID { get => housingID; set => housingID = value; }

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	protected virtual void Reset()
	{
		ghostOrigin = transform.root.gameObject;
	}

	public override IInteraction.InteractionInfo OpenLooking()
	{
		return base.OpenLooking();
	}

	public override void Select(SelectArgs selectArgs) { }

	public override void Unselect(SelectArgs selectArgs) { }

	protected override void SafetyClose() { }

	protected override void SafetyOpen() { }
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
	[CustomEditor(typeof(PlaceableObject))]
	public class PlaceableObjectEditor : Editor
	{
		private PlaceableObject placeableObject = default;
		private FieldInfo itemIDInfo = default;
		private FieldInfo housingIDInfo = default;

		private void OnEnable()
		{
			placeableObject = target as PlaceableObject;
			itemIDInfo = placeableObject.GetType()
				.GetField("itemID", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			housingIDInfo = placeableObject.GetType()
				.GetField("housingID", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (housingIDInfo != null)
			{
				int itemId = (int)itemIDInfo.GetValue(placeableObject);
				int housingId = (int)housingIDInfo.GetValue(placeableObject);
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.IntField("Item ID", itemId);
				EditorGUILayout.IntField("Housing ID", housingId);
				EditorGUI.EndDisabledGroup();
			}
		}
	}
}
#endif
