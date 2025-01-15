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
		for(int i = 1;i <= 10 ; i++)
		{
			MyRoomEntryData myRoomEntryDataa = await requester.PostMyRoomEntry(i);
			XDebug.LogWarning(myRoomEntryDataa.GetBody.ShopID);
		}
		MyRoomEntryData myRoomEntryData = await requester.PostMyRoomEntry(PlayerData.PlayerID);
		XDebug.LogWarning(myRoomEntryData.GetBody.ObjectList[0].HousingID);
		foreach(MyRoomObject myRoomObject in myRoomEntryData.GetBody.ObjectList)
		{
			SetRoomObject(myRoomObject);
		}
		int shopID = myRoomEntryData.GetBody.ShopID;
		//-1‚Í•”‰®‚ÉŽ©”Ì‹@‚ª‚È‚¢ê‡
		if(shopID == -1) { return; }
		FindObjectOfType<VendingMachineUIManager>().Initialize(shopID).Forget();
	}

	private void SetRoomObject(MyRoomObject myRoomObject)
	{
		GameObject prefab = _itemBundleAsset.GetItemAssetByID(myRoomObject.ItemID).DisplayItem.gameObject;
		
		GameObject instance = Instantiate(
			prefab,
			myRoomObject.Position,
			Quaternion.Euler(myRoomObject.EulerRotation)
			);
		PlaceableObject placeableObject = instance.GetComponentInChildren<PlaceableObject>();
		placeableObject.HousingID = myRoomObject.HousingID;
	}
}
