using Cysharp.Threading.Tasks;
using UnityEngine;
using MyRoomEntryData = WebAPIRequester.OnMyRoomEntryData;
using MyRoomObject = WebAPIRequester.MyRoomObject;

public class MyRoomLoader : MonoBehaviour
{
	[SerializeField]
	private ItemBundleAsset _itemBundleAsset;
	private PlayerDontDestroyData PlayerData => PlayerDontDestroyData.Instance;

	private async void Start()
	{
		await Load();
	}
	public async UniTask Load()
	{
		WebAPIRequester requester = new WebAPIRequester();
		MyRoomEntryData myRoomEntryData = await requester.PostMyRoomEntry(PlayerData.PlayerID);
		foreach(MyRoomObject myRoomObject in myRoomEntryData.ObjectList)
		{
			SetRoomObject(myRoomObject);
		}
		int shopID = myRoomEntryData.ShopID;
		//-1ÇÕïîâÆÇ…é©îÃã@Ç™Ç»Ç¢èÍçá
		if(shopID == -1) { return; }
		FindObjectOfType<VendingMachineUIManager>().Initialize(shopID,PlayerData.PlayerID).Forget();
	}

	public async UniTask UnLoad()
	{

	}

	private void SetRoomObject(MyRoomObject myRoomObject)
	{
		myRoomObject.ItemID.PrintWarning();
		ItemAsset asset = _itemBundleAsset.GetItemAssetByID(myRoomObject.ItemID);
		GameObject prefab =	asset.DisplayItem.gameObject;
		
		GameObject instance = Instantiate(
			prefab,
			myRoomObject.Position,
			Quaternion.Euler(myRoomObject.EulerRotation)
			);
		PlaceableObject placeableObject = instance.GetComponentInChildren<PlaceableObject>();
		placeableObject.HousingID = myRoomObject.HousingID;
	}
}
