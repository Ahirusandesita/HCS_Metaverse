using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの配置モード（ハウジング）
/// </summary>
public class Placing : MonoBehaviour, IInputControllable
{
	[SerializeField] private PlayerState playerState = default;
	[Tooltip("設置（ハウジング）モード")]
	[SerializeField, HideAtPlaying] private PlaceableObject testOrigin;
	private GhostModel ghostModel = default;
	private PlaceableObject placeableObject = default;
	private PlaceableObject deleteObject = default;
	private MyRoomLoader myRoomLoader = default;

	[SerializeField] private ItemBundleAsset allItemAsset = default;

#if UNITY_EDITOR
	private int inventoryIndexTest = 0;
	private List<ItemIDAmountPair> debugData = new List<ItemIDAmountPair>();
#endif

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	private void Reset()
	{
		try
		{
			playerState = GetComponent<PlayerState>();
		}
		catch (System.NullReferenceException) { }
	}

	private void Awake()
	{
		Inputter.PlacingMode.Place.performed += OnPlacing;
#if UNITY_EDITOR
		debugData.Add(new ItemIDAmountPair(10093, 1));
		debugData.Add(new ItemIDAmountPair(10540, 1));
		debugData.Add(new ItemIDAmountPair(10120, 1));
		debugData.Add(new ItemIDAmountPair(10096, 1));
		debugData.Add(new ItemIDAmountPair(10058, 1));
		debugData.Add(new ItemIDAmountPair(10962, 1));
#endif
	}

	private void Start()
	{
		myRoomLoader = FindAnyObjectByType<MyRoomLoader>();
	}

	private void OnDestroy()
	{
		Inputter.PlacingMode.Place.performed -= OnPlacing;
	}

#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			CreateGhost(testOrigin);
		}
		if (Input.GetKeyDown(KeyCode.Keypad4))
		{
			TryDestroyGhost();
			inventoryIndexTest -= inventoryIndexTest == 0 ? 0 : 1;
			int placeableObjectID = /*PlayerDontDestroyData.Instance.InventoryToList*/debugData[inventoryIndexTest].ItemID;
			placeableObject = allItemAsset.GetItemAssetByID(placeableObjectID).DisplayItem.gameObject.GetComponent<PlaceableObject>();
			placeableObject.ItemID = placeableObjectID;
			CreateGhost(placeableObject);
		}
		if (Input.GetKeyDown(KeyCode.Keypad6))
		{
			TryDestroyGhost();
			inventoryIndexTest += inventoryIndexTest == 19 ? 0 : 1;
			int placeableObjectID = /*PlayerDontDestroyData.Instance.InventoryToList*/debugData[inventoryIndexTest].ItemID;
			placeableObject = allItemAsset.GetItemAssetByID(placeableObjectID).DisplayItem.gameObject.GetComponent<PlaceableObject>();
			placeableObject.ItemID = placeableObjectID;
			CreateGhost(placeableObject);
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			myRoomLoader.UnLoad().Forget();
		}
	}
#endif

	public void CreateGhost(PlaceableObject origin)
	{
		if (!playerState.PlacingMode.Value)
		{
			playerState.ChangePlacingMode();
		}
		deleteObject = origin;
		placeableObject = allItemAsset.GetItemAssetByID(origin.ItemID).DisplayItem.gameObject.GetComponent<PlaceableObject>();
		placeableObject.ItemID = origin.ItemID;
		ghostModel = new GhostModel().CreateModel(placeableObject, transform, null);
		ghostModel.Spawn();
	}

	public bool TryDestroyGhost()
	{
		if (ghostModel != null)
		{
			ghostModel.DisposeModel();
			ghostModel = null;
			return true;
		}

		return false;
	}

	private void OnPlacing(InputAction.CallbackContext context)
	{
		if (!ghostModel.CanPlace)
		{
			return;
		}

		if (playerState.PlacingMode.Value)
		{
			playerState.ChangePlacingMode();
		}

		ghostModel.PlacingTarget.OnPlaced();

		// Ghostから実体を生成
		var placedObject = Instantiate(placeableObject, ghostModel.GetGhostPosition(), ghostModel.GetGhostChildRotation());
		placedObject.gameObject.SetActive(true);
		// ItemIDを渡しておく
		placedObject.ItemID = placeableObject.ItemID;
		// セーブAPI用にリストに追加しておく
		myRoomLoader.InteriorInfo.AddPlacedObject(placedObject);
		// Ghostを消す
		TryDestroyGhost();
		// もしオブジェクトを移動させていたら、残置オブジェクトを消しとく
		if (deleteObject.gameObject.activeInHierarchy)
		{
			deleteObject.Close();
			myRoomLoader.InteriorInfo.DeletePlacedObject(deleteObject);
			Destroy(deleteObject.gameObject);
		}
		placeableObject = placedObject;
	}
}
