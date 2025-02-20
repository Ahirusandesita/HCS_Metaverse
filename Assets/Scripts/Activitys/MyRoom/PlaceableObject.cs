using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

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
	private Component[] disableComponents = default;
	private Placing placing = default;
	private float canceledTime = default;
	private System.Action UpdateAction = default;
	private Vector3 defaultPosition = default;
	private Quaternion defaultRotation = default;

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

	protected override void Awake()
	{
		base.Awake();
		Inputter.PlacingMode.Cancel.performed += OnCancel;
		Inputter.PlacingMode.Cancel.canceled += OnCancelCancel;
	}

	protected void Start()
	{
		disableComponents = GetComponentsInChildren<Component>(true);
		SetActiveNotIncludeThis(true);
		placing = FindAnyObjectByType<Placing>();
	}

	private void Update()
	{
		UpdateAction?.Invoke();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Inputter.PlacingMode.Cancel.performed -= OnCancel;
		Inputter.PlacingMode.Cancel.canceled -= OnCancelCancel;
	}

	public override IInteraction.InteractionInfo OpenLooking()
	{
		return base.OpenLooking();
	}

	protected override void SafetyOpenLooking()
	{
		SetActiveNotIncludeThis(false);
		placing.CreateGhost(this);
		defaultPosition = transform.position;
		defaultRotation = transform.rotation;
	}

	public override void Close()
	{
		base.Close();
		SetActiveNotIncludeThis(true);
	}

	private void SetActiveNotIncludeThis(bool value)
	{
		// このスクリプト以外のすべてのコンポーネントを非表示
		foreach (var component in disableComponents)
		{
			if (component == this)
			{
				continue;
			}

			if (component is Outline _)
			{
				continue;
			}
			else if (component is Behaviour behaviour)
			{
				behaviour.enabled = value;
			}
			else if (component is Collider collider)
			{
				collider.enabled = value;
			}
			else if (component is Renderer renderer)
			{
				renderer.enabled = value;
			}
		}
	}

	private void OnCancel(InputAction.CallbackContext context)
	{
		UpdateAction += () => canceledTime += Time.deltaTime;
	}

	private void OnCancelCancel(InputAction.CallbackContext context)
	{
		// Cancel
		if (canceledTime < 1.5f)
		{
			transform.position = defaultPosition;
			transform.rotation = defaultRotation;
			Close();
			placing.TryDestroyGhost();
			placing.Cancel();
			// キャンセルした後にまたOpenLookingが呼ばれるよう、再チェック可能に
			IsFiredTriggerStay = false;
		}
		// Delete
		else
		{
			PlayerDontDestroyData.Instance.AddInventory(new ItemIDAmountPair(itemID, 1));
			placing.TryDestroyGhost();
			placing.Cancel();
			Destroy(gameObject);
		}

		canceledTime = 0f;
		UpdateAction = null;
	}
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
