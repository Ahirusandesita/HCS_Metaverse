using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyRoomEntryData = WebAPIRequester.OnMyRoomEntryData;
using MyRoomObject = WebAPIRequester.MyRoomObject;

public class MyRoomLoader : MonoBehaviour
{
	public class InteriorManager : System.IDisposable
	{
		public class NotExistInteriorException : System.Exception
		{
			public NotExistInteriorException() : base() { }
			public NotExistInteriorException(string message) : base(message) { }
			public NotExistInteriorException(string message, System.Exception innnerException) : base(message, innnerException) { }
		}

		private List<PlaceableObject> _placedObjects = default;
		public IReadOnlyList<PlaceableObject> PlacedObjects => _placedObjects;

		public InteriorManager()
		{
			_placedObjects = new List<PlaceableObject>();
		}

		public void AddPlacedObject(PlaceableObject placedObject)
		{
			_placedObjects.Add(placedObject);
		}

		public void DeletePlacedObject(PlaceableObject deleteObject)
		{
			var deleteTarget = _placedObjects.Where(placedObject => placedObject == deleteObject).FirstOrDefault();
			if (deleteTarget is null)
			{
				return;
			}

			_placedObjects.Remove(deleteTarget);
		}

		public void Dispose()
		{
			foreach (var item in _placedObjects)
			{
				Destroy(item.gameObject);
			}
			_placedObjects = null;
		}
	}

	[SerializeField]
	private ItemBundleAsset _itemBundleAsset;
	private InteriorManager _interiorManager;

	private PlayerDontDestroyData PlayerData => PlayerDontDestroyData.Instance;
	public InteriorManager InteriorInfo => _interiorManager;


	private async void Start()
	{
#if UNITY_EDITOR
		// Debug
		_interiorManager = new InteriorManager();
#endif
		await Load(PlayerDontDestroyData.Instance.PlayerID);
	}

	public async UniTask Load(int myRoomAdminPlayerID)
	{
		WebAPIRequester requester = new WebAPIRequester();
		MyRoomEntryData myRoomEntryData = await requester.PostMyRoomEntry(myRoomAdminPlayerID);
		foreach (MyRoomObject myRoomObject in myRoomEntryData.ObjectList)
		{
			SetRoomObject(myRoomObject);
		}

		_interiorManager = new InteriorManager();

		int shopID = myRoomEntryData.ShopID;
		//-1ÇÕïîâÆÇ…é©îÃã@Ç™Ç»Ç¢èÍçá
		if (shopID == -1) { return; }
		VendingMachine vendingMachine = FindObjectOfType<VendingMachine>();
		if (vendingMachine == null) { return; }
		vendingMachine.Initialize(shopID, myRoomAdminPlayerID).Forget();
	}

	public async UniTask UnLoad()
	{
		WebAPIRequester requester = new WebAPIRequester();
		List<WebAPIRequester.MyRoomObjectSaved> myRoomObjectSaves = new List<WebAPIRequester.MyRoomObjectSaved>();
		foreach (var placedObject in _interiorManager.PlacedObjects)
		{
			myRoomObjectSaves.Add(
				new WebAPIRequester.MyRoomObjectSaved(
					itemId: placedObject.ItemID,
					position: placedObject.transform.position,
					eulerRotation: placedObject.transform.eulerAngles
					)
				);
		}
		await requester.PostMyRoomSave(myRoomObjectSaves);

		_interiorManager.Dispose();
		_interiorManager = null;
	}

	private void SetRoomObject(MyRoomObject myRoomObject)
	{
		ItemAsset asset = _itemBundleAsset.GetItemAssetByID(myRoomObject.ItemID);
		GameObject prefab;
		if (asset.DisplayItem == null)
		{
			prefab = asset.Prefab;
		}
		else
		{
			prefab = asset.DisplayItem.gameObject;
		}

		GameObject instance = Instantiate(
			prefab,
			myRoomObject.Position,
			Quaternion.Euler(myRoomObject.EulerRotation)
			);
		PlaceableObject placeableObject = instance.GetComponentInChildren<PlaceableObject>();
		placeableObject.HousingID = myRoomObject.HousingID;
	}
}
