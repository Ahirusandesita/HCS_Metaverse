using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

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

	private int inventoryIndexTest = 0;
	[SerializeField] private ItemBundleAsset allItemAsset = default;

	private List<ItemIDAmountPair> debugData = new List<ItemIDAmountPair>();

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
		debugData.Add(new ItemIDAmountPair(10001, 1));
		debugData.Add(new ItemIDAmountPair(10256, 1));
		debugData.Add(new ItemIDAmountPair(10120, 1));
		debugData.Add(new ItemIDAmountPair(10096, 1));
		debugData.Add(new ItemIDAmountPair(10058, 1));
		debugData.Add(new ItemIDAmountPair(10962, 1));
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
			int placeableObjectID = PlayerDontDestroyData.Instance.InventoryToList[inventoryIndexTest].ItemID;
			placeableObject = allItemAsset.GetItemAssetByID(placeableObjectID).DisplayItem.gameObject.GetComponent<PlaceableObject>();
			placeableObject.ItemID = placeableObjectID;
			CreateGhost(placeableObject);
		}
		if (Input.GetKeyDown(KeyCode.Keypad6))
		{
			TryDestroyGhost();
			inventoryIndexTest += inventoryIndexTest == 19 ? 0 : 1;
			int placeableObjectID = PlayerDontDestroyData.Instance.InventoryToList[inventoryIndexTest].ItemID;
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

	public void CreateGhost(PlaceableObject origin, bool updateMode = false)
	{
		if (!playerState.PlacingMode.Value)
		{
			playerState.ChangePlacingMode();
		}
		deleteObject = origin;
		placeableObject = allItemAsset.GetItemAssetByID(origin.ItemID).DisplayItem.gameObject.GetComponent<PlaceableObject>();
		placeableObject.ItemID = origin.ItemID;
		ghostModel = new GhostModel().CreateModel(placeableObject, transform, null, false);
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
		var placedObject = Instantiate(placeableObject, ghostModel.GetGhostPosition(), ghostModel.GetGhostChildRotation());
		placedObject.gameObject.SetActive(true);
		placedObject.ItemID = placeableObject.ItemID;
		myRoomLoader.InteriorInfo.AddPlacedObject(placedObject);
		TryDestroyGhost();
		if (deleteObject.gameObject.activeInHierarchy)
		{
			deleteObject.Close();
			myRoomLoader.InteriorInfo.DeletePlacedObject(deleteObject);
			Destroy(deleteObject.gameObject);
		}
		placeableObject = placedObject;
	}
}
