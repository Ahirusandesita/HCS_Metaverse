using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Result = UnityEngine.Networking.UnityWebRequest.Result;

/// <summary>
/// Shopとデータベースの送受信を行う。現時点では各Shopごとにインスタンスを所持する設計。
/// </summary>
public class WebAPIRequester
{
	private const string DETABASE_PATH_BASE = "http://10.11.33.228:8080/api/";
	private const string DETABASE_PATH_JOIN_WORLD = DETABASE_PATH_BASE + "world";
	private const string DETABASE_PATH_SHOP_ENTRY = DETABASE_PATH_BASE + "shop/entry";
	private const string DETABASE_PATH_SHOP_BUY = DETABASE_PATH_BASE + "shop/buy";
	private const string DETABASE_PATH_MYROOM_ENTRY = DETABASE_PATH_BASE + "myroom/entry";
	private const string DETABASE_PATH_USER_LOCATION = DETABASE_PATH_BASE + "user/location";
	private const string DETABASE_PATH_VENDINGMACHINE_ENTRY = DETABASE_PATH_BASE + "salesmachine";
	private const string DETABASE_PATH_VENDINGMACHINE_BUY = DETABASE_PATH_BASE + "salesmachine/buy";
	private const string CONTENT_TYPE = "application/json";


	#region Post/Get メソッド
	/// <summary>
	/// ショップ入店時のAPI通信
	/// </summary>
	/// <returns>・商品ラインナップ</returns>
	public async UniTask<OnShopEntryData> PostShopEntry(int shopId)
	{
		WWWForm form = new WWWForm();
		form.AddField("shopId", shopId);
		using var request = UnityWebRequest.Post(DETABASE_PATH_SHOP_ENTRY, form);
		await request.SendWebRequest();

		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var onShopEntryData = JsonUtility.FromJson<OnShopEntryData>(request.downloadHandler.text);
		return onShopEntryData;
	}

	/// <summary>
	/// ショップ購入時のAPI通信
	/// </summary>
	/// <returns>・購入後の所持品リスト
	/// <br>・購入後の金額</br>
	/// <br>・ショップの在庫リスト</br>
	/// <br>・ユーザーID</br></returns>
	public async UniTask<OnShopPaymentData> PostShopPayment(List<ItemStock> inventory, int shopId, int userId)
	{
		var sendShopPaymentData = new SendPaymentData(inventory, shopId, userId);
		string jsonData = JsonUtility.ToJson(sendShopPaymentData);

		using var request = UnityWebRequest.Post(DETABASE_PATH_SHOP_BUY, jsonData, CONTENT_TYPE);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var onShopPaymentData = JsonUtility.FromJson<OnShopPaymentData>(request.downloadHandler.text);
		return onShopPaymentData;
	}

	/// <summary>
	/// 自販機アクセス時（マイルーム入室時の初期化など更新したいとき）のAPI通信
	/// </summary>
	/// <param name="shopId"></param>
	/// <returns>・商品ラインナップ
	/// <br>・自販機のアクティブ状況</br></returns>
	public async UniTask<OnVMEntryData> PostVMEntry(int shopId)
	{
		WWWForm form = new WWWForm();
		form.AddField("shopId", shopId);
		using var request = UnityWebRequest.Post(DETABASE_PATH_VENDINGMACHINE_ENTRY, form);
		await request.SendWebRequest();

		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var onVMEntryData = JsonUtility.FromJson<OnVMEntryData>(request.downloadHandler.text);
		return onVMEntryData;
	}

	/// <summary>
	/// 自販機購入時のAPI通信
	/// </summary>
	/// <returns>・購入後の所持品リスト
	/// <br>・購入後の金額</br>
	/// <br>・自販機の在庫リスト</br>
	/// <br>・ユーザーID</br>
	/// <br>・自販機のアップデートフラグ</br></returns>
	public async UniTask<OnVMPaymentData> PostVMPayment(int itemId, int shopId, int userId)
	{
		var itemList = new List<ItemStock>();
		itemList.Add(new ItemStock(itemId, 1));
		var sendPaymentData = new SendPaymentData(itemList, shopId, userId);
		string jsonData = JsonUtility.ToJson(sendPaymentData);

		using var request = UnityWebRequest.Post(DETABASE_PATH_VENDINGMACHINE_BUY, jsonData, CONTENT_TYPE);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				XDebug.LogError(request.result, "red");
				throw new APIConnectException(request.error);
		}

		var onVMPaymentData = JsonUtility.FromJson<OnVMPaymentData>(request.downloadHandler.text);
		return onVMPaymentData;
	}

	/// <summary>
	/// マイルーム入室時のAPI通信
	/// </summary>
	/// <returns>・オブジェクトリスト
	/// <br>・ショップID（自販機）</br></returns>
	public async UniTask<OnMyRoomEntryData> PostMyRoomEntry(int userId)
	{
		WWWForm form = new WWWForm();
		form.AddField("userId", userId);
		using var request = UnityWebRequest.Post(DETABASE_PATH_MYROOM_ENTRY, form);
		await request.SendWebRequest();

		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var onMyroomEntryData = JsonUtility.FromJson<OnMyRoomEntryData>(request.downloadHandler.text);
		return onMyroomEntryData;
	}

	/// <summary>
	/// ユーザー場所情報登録時のAPI通信
	/// </summary>
	/// <returns>なし</returns>
	public async UniTask PostUserLocation(int userId, string sessionName, string locationName)
	{
		var sendUserLocationData = new SendUserLocationData(userId, sessionName, locationName);
		string jsonData = JsonUtility.ToJson(sendUserLocationData);

		using var request = UnityWebRequest.Post(DETABASE_PATH_USER_LOCATION, jsonData, CONTENT_TYPE);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}
	}
	#endregion

	#region レスポンスデータ
	/// <summary>
	/// API通信の基底レスポンスデータ
	/// </summary>
	[System.Serializable]
	public class ResponseData
	{
		[SerializeField] private string responseCode = default;
		[SerializeField] private string message = default;

		public string ResponseCode => responseCode;
		public string Message => message;
	}

	/// <summary>
	/// ショップ入店時のレスポンスデータ
	/// </summary>
	[System.Serializable]
	public class OnShopEntryData : ResponseData
	{
		public OnShopEntryData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private Body body = default;

		public Body GetBody => body;

		[System.Serializable]
		public class Body
		{
			public Body(List<ItemLineup> itemList)
			{
				this.itemList = itemList;
			}

			[SerializeField] private List<ItemLineup> itemList = default;
			public IReadOnlyList<ItemLineup> ItemList => itemList;
		}
	}

	/// <summary>
	/// ショップ購入時のレスポンスデータ
	/// </summary>
	[System.Serializable]
	public class OnShopPaymentData : ResponseData
	{
		public OnShopPaymentData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private Body body = default;

		public Body GetBody => body;

		[System.Serializable]
		public class Body
		{
			public Body(List<ItemStock> inventory, int money, List<ItemStock> stockData, int userId)
			{
				this.inventory = inventory;
				this.money = money;
				this.stockData = stockData;
				this.userId = userId;
			}

			[SerializeField] private List<ItemStock> inventory = default;
			[SerializeField] private int money = default;
			[SerializeField] private List<ItemStock> stockData = default;
			[SerializeField] private int userId = default;

			public IReadOnlyList<ItemStock> Inventory => inventory;
			public int Money => money;
			public IReadOnlyList<ItemStock> StockData => stockData;
			public int UserID => userId;
		}
	}

	/// <summary>
	/// 自販機アクセス時のレスポンスデータ
	/// </summary>
	[System.Serializable]
	public class OnVMEntryData : ResponseData
	{
		public OnVMEntryData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private Body body = default;

		public Body GetBody => body;

		[System.Serializable]
		public class Body
		{
			public Body(List<ItemLineup> itemList, bool active)
			{
				this.itemList = itemList;
				this.active = active;
			}

			[SerializeField] private List<ItemLineup> itemList = default;
			[SerializeField] private bool active = default;

			public IReadOnlyList<ItemLineup> ItemList => itemList;
			public bool Active => active;
		}
	}

	/// <summary>
	/// 自販機購入時のレスポンスデータ
	/// </summary>
	[System.Serializable]
	public class OnVMPaymentData : ResponseData
	{
		public OnVMPaymentData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private Body body = default;

		public Body GetBody => body;

		[System.Serializable]
		public class Body
		{
			public Body(List<ItemStock> inventory, int money, List<ItemStock> stockData, int userId, bool updateFlg)
			{
				this.inventory = inventory;
				this.money = money;
				this.stockData = stockData;
				this.userId = userId;
				this.updateFlg = updateFlg;
			}

			[SerializeField] private List<ItemStock> inventory = default;
			[SerializeField] private int money = default;
			[SerializeField] private List<ItemStock> stockData = default;
			[SerializeField] private int userId = default;
			[SerializeField] private bool updateFlg = default;

			public IReadOnlyList<ItemStock> Inventory => inventory;
			public int Money => money;
			public IReadOnlyList<ItemStock> StockData => stockData;
			public int UserID => userId;
			public bool UpdateFlg => updateFlg;
		}
	}

	/// <summary>
	/// マイルーム入室時のレスポンスデータ
	/// </summary>
	[System.Serializable]
	public class OnMyRoomEntryData : ResponseData
	{
		public OnMyRoomEntryData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private Body body = default;
		public Body GetBody => body;

		[System.Serializable]
		public class Body
		{
			public Body(List<MyRoomObject> objectList, int shopId)
			{
				this.objectList = objectList;
				this.shopId = shopId;
			}

			[SerializeField] private List<MyRoomObject> objectList = default;
			[SerializeField] private int shopId = default;

			public IReadOnlyList<MyRoomObject> ObjectList => objectList;
			/// <summary>
			/// 自販機が存在しない場合は-1
			/// </summary>
			public int ShopID => shopId;
		}
	}
	#endregion

	#region 送信データ
	/// <summary>
	/// 購入時の送信データ（内部パースに使用）
	/// </summary>
	[System.Serializable]
	private class SendPaymentData
	{
		public SendPaymentData(List<ItemStock> itemList, int shopId, int userId)
		{
			this.itemList = itemList;
			this.shopId = shopId;
			this.userId = userId;
		}

		[SerializeField] private List<ItemStock> itemList = default;
		[SerializeField] private int shopId = default;
		[SerializeField] public int userId = default;

		public IReadOnlyList<ItemStock> ItemList => itemList;
		public int ShopID => shopId;
		public int UserID => userId;
	}

	/// <summary>
	/// ユーザー場所情報登録時の送信データ
	/// </summary>
	[System.Serializable]
	public class SendUserLocationData
	{
		public SendUserLocationData(int userId, string sessionName, string locationName)
		{
			this.userId = userId;
			this.sessionName = sessionName;
			this.locationName = locationName;
		}

		[SerializeField] private int userId = default;
		[SerializeField] private string sessionName = default;
		[SerializeField] private string locationName = default;

		public int UserID => userId;
		public string SessionName => sessionName;
		public string LocationName => locationName;
	}
	#endregion

	#region Struct
	/// <summary>
	/// ・アイテムID
	/// <br>・価格</br>
	/// <br>・割引率</br>
	/// <br>・在庫数</br>
	/// <br>・サイズ</br>
	/// </summary>
	[System.Serializable]
	public struct ItemLineup
	{
		public ItemLineup(int itemId, int salesPrice, float discount, int stock, int size)
		{
			this.itemId = itemId;
			this.price = salesPrice;
			this.discount = discount;
			this.stock = stock;
			this.size = size;
		}

		[SerializeField] private int itemId;
		[SerializeField] private int price;
		[SerializeField] private float discount;
		[SerializeField] private int stock;
		[SerializeField] private int size;

		public int ItemID => itemId;
		public int Price => price;
		public float Discount => discount;
		public int Stock => stock;
		/// <summary>
		/// 0: large, 1: small
		/// </summary>
		public int Size => size;
	}

	/// <summary>
	/// ・アイテムID
	/// <br>・数</br>
	/// </summary>
	[System.Serializable]
	public struct ItemStock
	{
		public ItemStock(int itemId, int amount)
		{
			this.itemId = itemId;
			this.amount = amount;
		}

		[SerializeField] private int itemId;
		[SerializeField] private int amount;

		public int ItemID => itemId;
		public int Amount => amount;
	}

	/// <summary>
	/// ・アイテムID
	/// <br>・ハウジングID</br>
	/// <br>・絶対座標</br>
	/// <br>・EulerRotation</br>
	/// </summary>
	[System.Serializable]
	public struct MyRoomObject
	{
		public MyRoomObject(int itemId, int housingId, Vector3 position, Vector3 eulerRotation)
		{
			this.itemId = itemId;
			this.housingId = housingId;
			x_position = position.x;
			y_position = position.y;
			z_position = position.z;
			x_direction = eulerRotation.x;
			y_direction = eulerRotation.y;
			z_direction = eulerRotation.z;
		}

		[SerializeField] private int itemId;
		[SerializeField] private int housingId;
		[SerializeField] private float x_position;
		[SerializeField] private float y_position;
		[SerializeField] private float z_position;
		[SerializeField] private float x_direction;
		[SerializeField] private float y_direction;
		[SerializeField] private float z_direction;

		public int ItemID => itemId;
		public int HousingID => housingId;
		public Vector3 Position => new Vector3(x_position, y_position, z_position);
		public Vector3 EulerRotation => new Vector3(x_direction, y_direction, z_direction);
	}
	#endregion

	#region 例外
	/// <summary>
	/// ・API通信中の例外
	/// </summary>
	public class APIConnectException : System.Exception
	{
		public APIConnectException() : base() { }
		public APIConnectException(string message) : base(message) { }
		public APIConnectException(string message, System.Exception innerException) : base(message, innerException) { }
	}
	#endregion
}