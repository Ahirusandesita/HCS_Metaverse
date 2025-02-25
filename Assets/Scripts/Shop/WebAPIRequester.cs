using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Result = UnityEngine.Networking.UnityWebRequest.Result;

/// <summary>
/// データベースとのAPI通信を行うクラス
/// </summary>
public class WebAPIRequester
{
	private const string DATABASE_PATH_BASE = "http://10.11.33.228:8080/api/";
	private const string DATABASE_PATH_COOK_SCORE = DATABASE_PATH_BASE + "score";
	private const string DATABASE_PATH_JOIN_WORLD = DATABASE_PATH_BASE + "world";
	private const string DATABASE_PATH_JOIN_WORLD_ONLINE = DATABASE_PATH_BASE + "world/online";
	private const string DATABASE_PATH_JOIN_WORLD_OFFLINE = DATABASE_PATH_BASE + "world/offline";
	private const string DATABASE_PATH_MONEY = DATABASE_PATH_BASE + "money";
	private const string DATABASE_PATH_SHOP_RECOMMEND = DATABASE_PATH_BASE + "shop/recommend";
	private const string DATABASE_PATH_SHOP_ENTRY = DATABASE_PATH_BASE + "shop/entry";
	private const string DATABASE_PATH_SHOP_BUY = DATABASE_PATH_BASE + "shop/buy";
	private const string DATABASE_PATH_MYROOM_ENTRY = DATABASE_PATH_BASE + "myroom/entry";
	private const string DATABASE_PATH_MYROOM_SAVE = DATABASE_PATH_BASE + "myroom/save";
	private const string DATABASE_PATH_USER_LOCATION = DATABASE_PATH_BASE + "location";
	private const string DATABASE_PATH_USER_LOCATION_CATCH = DATABASE_PATH_BASE + "location/catch";
	private const string DATABASE_PATH_VENDINGMACHINE_ENTRY = DATABASE_PATH_BASE + "salesmachine";
	private const string DATABASE_PATH_VENDINGMACHINE_BUY = DATABASE_PATH_BASE + "salesmachine/buy";
	private const string DATABASE_PATH_VENDINGMACHINE_UPDATE = DATABASE_PATH_BASE + "salesmachine/update";
	private const string DATABASE_PATH_LOGIN = "http://10.11.33.228:8080/login";
	private const string DATABASE_PATH_INVENTORY_CATCH = DATABASE_PATH_BASE + "inventory/catch";
	private const string DATABASE_PATH_INVENTORY_SAVE = DATABASE_PATH_BASE + "inventory/save";
	private const string DATABASE_PATH_COSTUME_CATCH = DATABASE_PATH_BASE + "costume/catch";
	private const string DATABASE_PATH_COSTUME_SAVE = DATABASE_PATH_BASE + "costume/save";
	private const string CONTENT_TYPE = "application/json";
	private const string TOKEN_KEY = "Authorization";

	public async UniTaskVoid Test()
	{
		int score = 100;
		int itemID = 10962;
		int userIndex = 0;
		int shopID = 2;
		int userID = 1;

		var loginResult = await PostLogin("user1@hcs.ac.jp", "hcs5511");
		Debug.LogWarning($"LoginResult:{loginResult}");

		var joinWorldResult = await GetJoinWorldData();
		string temp = "";
		foreach (var item in joinWorldResult.ShopList)
		{
			temp += item + ":";
		}
		Debug.LogWarning($"AllShopID:{temp}");
		temp = "";
		foreach (var item in joinWorldResult.UserList)
		{
			temp += item.UserID + ":" + item.UserName + " ,";
		}
		Debug.LogWarning($"AllUser:{temp}");


		await PostScore(score);

		var data = new List<SendInventoryUpdateData>();
		data.Add(new SendInventoryUpdateData(itemID,1,1));
		await PostInventoryUpdate(data);

		await GetMoney();

		await PostShopRecommend(shopID);

		var result = await PostShopEntry(shopID);
		foreach(var item in result.ItemList)
		{
			XDebug.LogWarning(item.ItemID + ":" + item.Stock);
		}


		List<ItemIDAmountPricePair> a = new();
		a.Add(new ItemIDAmountPricePair(result.ItemList[0].ItemID, 1, 0));
		XDebug.LogWarning(a[0].ItemID);
		await PostShopPayment(a, shopID);

		await PostVMEntry(userID);
	}


	#region Post/Get メソッド

	public async UniTask PostOnline()
	{
		using var request = UnityWebRequest.Post(DATABASE_PATH_JOIN_WORLD_ONLINE, "", CONTENT_TYPE);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}
	}

	public async UniTask PostOffline()
	{
		using var request = UnityWebRequest.Post(DATABASE_PATH_JOIN_WORLD_OFFLINE, "", CONTENT_TYPE);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}
	}

	public async UniTask<OnGetWorldData> GetJoinWorldData()
	{
		WWWForm form = new WWWForm();
		using var request = UnityWebRequest.Post(DATABASE_PATH_JOIN_WORLD, form);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				XDebug.LogError(request.result, "red");
				throw new APIConnectException(request.error);
		}
		var responseData = JsonUtility.FromJson<OnGetWorldData>(request.downloadHandler.text);
		return responseData;
	}

	public async UniTask PostScore(int score)
	{
		WWWForm form = new WWWForm();
		form.AddField("score", score);
		using var request = UnityWebRequest.Post(DATABASE_PATH_COOK_SCORE, form);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				XDebug.LogError(request.result, "red");
				throw new APIConnectException(request.error);
		}
	}

	public async UniTask<int> GetMoney()
	{
		using var request = UnityWebRequest.Post(DATABASE_PATH_MONEY, new WWWForm());
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();

		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}
		var money = JsonUtility.FromJson<OnGetMoneyData>(request.downloadHandler.text);
		return money.Money;
	}

	public async UniTask<ResponseData> PostInventoryUpdate(List<SendInventoryUpdateData> data)
	{	
		string jsonData = JsonUtility.ToJson(data);

		using var request = UnityWebRequest.Post(DATABASE_PATH_INVENTORY_SAVE, jsonData, CONTENT_TYPE);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var responseData = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
		return responseData;
	}

	public async UniTask<OnShopProductData> PostShopRecommend(int shopId)
	{
		WWWForm form = new WWWForm();
		form.AddField("shopId", shopId);
		using var request = UnityWebRequest.Post(DATABASE_PATH_SHOP_RECOMMEND, form);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				XDebug.LogError(request.result, "red");
				throw new APIConnectException(request.error);
		}
		var onShopEntryData = JsonUtility.FromJson<OnShopProductData>(request.downloadHandler.text);
		return onShopEntryData;
	}

	/// <summary>
	/// ショップ入店時のAPI通信
	/// </summary>
	/// <returns>・商品ラインナップ</returns>
	public async UniTask<OnShopProductData> PostShopEntry(int shopId)
	{

		WWWForm form = new WWWForm();
		form.AddField("shopId", shopId);
		using var request = UnityWebRequest.Post(DATABASE_PATH_SHOP_ENTRY, form);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();

		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var onShopEntryData = JsonUtility.FromJson<OnShopProductData>(request.downloadHandler.text);
		return onShopEntryData;
	}

	/// <summary>
	/// ログイン時のAPI通信
	/// </summary>
	/// <param name="userName">ユーザー名または、メールアドレス</param>
	/// <param name="password">パスワード</param>
	/// <returns>ログインの成否</returns>
	public async UniTask<bool> PostLogin(string userName, string password)
	{
		var sendLoginData = new SendLoginData(userName, password);
		string jsonData = JsonUtility.ToJson(sendLoginData);
		using var request = UnityWebRequest.Post(DATABASE_PATH_LOGIN, jsonData, CONTENT_TYPE);

		try
		{
			await request.SendWebRequest();
		}
		catch
		{
			switch (request.result)
			{
				case Result.InProgress:
					throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

				case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
					return false;
			}
		}

		string token = request.GetResponseHeader(TOKEN_KEY);
		PlayerDontDestroyData.Instance.Token = token;
		return true;
	}

	/// <summary>
	/// ショップ購入時のAPI通信
	/// </summary>
	/// <returns>・購入後の所持品リスト
	/// <br>・購入後の金額</br>
	/// <br>・ショップの在庫リスト</br>
	/// <br>・ユーザーID</br></returns>
	public async UniTask<OnShopProductData> PostShopPayment(List<ItemIDAmountPricePair> buyItemList, int shopId)
	{
		var sendShopPaymentData = new SendPaymentData(buyItemList, shopId);
		string jsonData = JsonUtility.ToJson(sendShopPaymentData);
		XDebug.LogWarning(jsonData);
		using var request = UnityWebRequest.Post(DATABASE_PATH_SHOP_BUY, jsonData, CONTENT_TYPE);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var onShopPaymentData = JsonUtility.FromJson<OnShopProductData>(request.downloadHandler.text);
		return onShopPaymentData;
	}

	/// <summary>
	/// 自販機アクセス時（マイルーム入室時の初期化など更新したいとき）のAPI通信
	/// </summary>
	/// <param name="userId"></param>
	/// <returns>・商品ラインナップ
	/// <br>・自販機のアクティブ状況</br></returns>
	public async UniTask<OnVMProductData> PostVMEntry(int userId)
	{
		WWWForm form = new WWWForm();
		form.AddField("shopId", userId);
		using var request = UnityWebRequest.Post(DATABASE_PATH_VENDINGMACHINE_ENTRY, form);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();

		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}
		var onVMEntryData = JsonUtility.FromJson<OnVMProductData>(request.downloadHandler.text);
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
	public async UniTask<OnVMPaymentData> PostVMPayment(int itemId, int price, int shopId)
	{
		var itemList = new List<ItemIDAmountPricePair>();
		itemList.Add(new ItemIDAmountPricePair(itemId, price, 1));
		var sendPaymentData = new SendPaymentData(itemList, shopId);
		string jsonData = JsonUtility.ToJson(sendPaymentData);

		using var request = UnityWebRequest.Post(DATABASE_PATH_VENDINGMACHINE_BUY, jsonData, CONTENT_TYPE);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
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
	/// 自販機更新時のAPI通信
	/// </summary>
	/// <returns>・インベントリ差分</returns>
	public async UniTask<OnVMProductData> PostVMUpdate(int shopId, List<VMSalesData> vmSalesData)
	{
		var sendVMSalesdata = new SendVMSalesData(shopId, vmSalesData);
		string jsonData = JsonUtility.ToJson(sendVMSalesdata);

		using var request = UnityWebRequest.Post(DATABASE_PATH_VENDINGMACHINE_UPDATE, jsonData, CONTENT_TYPE);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				XDebug.LogError(request.result, "red");
				throw new APIConnectException(request.error);
		}

		var onVMUpdateData = JsonUtility.FromJson<OnVMProductData>(request.downloadHandler.text);
		return onVMUpdateData;
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
		using var request = UnityWebRequest.Post(DATABASE_PATH_MYROOM_ENTRY, form);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);

		await request.SendWebRequest();

		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}
		var onMyRoomEntryData = JsonUtility.FromJson<OnMyRoomEntryData>(request.downloadHandler.text);
		return onMyRoomEntryData;
	}

	/// <summary>
	/// マイルーム退室時のAPI通信
	/// </summary>
	public async UniTask PostMyRoomSave(List<MyRoomObjectSaved> myRoomObjectSaveds)
	{
		var sendMyRoomSaveData = new SendMyRoomSaveData(myRoomObjectSaveds);
		string jsonData = JsonUtility.ToJson(sendMyRoomSaveData);

		using var request = UnityWebRequest.Post(DATABASE_PATH_MYROOM_SAVE, jsonData, CONTENT_TYPE);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				XDebug.LogError(request.result, "red");
				throw new APIConnectException(request.error);
		}
	}

	/// <summary>
	/// ユーザー場所情報登録時のAPI通信
	/// </summary>
	/// <returns>なし</returns>
	public async UniTask PostUserLocation(string sessionName, string locationName)
	{
		var sendUserLocationData = new UserLocationData(sessionName, locationName);
		string jsonData = JsonUtility.ToJson(sendUserLocationData);

		using var request = UnityWebRequest.Post(DATABASE_PATH_USER_LOCATION, jsonData, CONTENT_TYPE);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}
	}

	/// <summary>
	/// ユーザー場所情報取得時のAPI通信
	/// </summary>
	/// <returns>・ユーザーID
	/// <br>・セッション名</br>
	/// <br>・ロケーション名</br></returns>
	public async UniTask<OnCatchUserLocationData> GetUserLocation()
	{
		using var request = UnityWebRequest.Post(DATABASE_PATH_USER_LOCATION_CATCH, new WWWForm());
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);

		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}
		var onCatchUserLocationData = JsonUtility.FromJson<OnCatchUserLocationData>(request.downloadHandler.text);
		return onCatchUserLocationData;
	}

	/// <summary>
	/// ユーザーインベントリの取得
	/// </summary>
	/// <returns></returns>
	public async UniTask<OnCatchUserInventory> GetInventory()
	{
		using var request = UnityWebRequest.Post(DATABASE_PATH_INVENTORY_CATCH, new WWWForm());
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);

		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}
		var onCatchUserInventoryData = JsonUtility.FromJson<OnCatchUserInventory>(request.downloadHandler.text);
		return onCatchUserInventoryData;
	}

	/// <summary>
	/// コスチュームの取得
	/// </summary>
	/// <returns></returns>
	public async UniTask<OnCatchCostumeData> GetCosutume()
	{
		using var request = UnityWebRequest.Post(DATABASE_PATH_COSTUME_CATCH, new WWWForm());
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);

		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var onCatchCostumeData = JsonUtility.FromJson<OnCatchCostumeData>(request.downloadHandler.text);
		return onCatchCostumeData;
	}

	/// <summary>
	/// コスチュームのセーブ
	/// </summary>
	/// <param name="hair"></param>
	/// <param name="face"></param>
	/// <param name="headGear"></param>
	/// <param name="top"></param>
	/// <param name="bottom"></param>
	/// <param name="bag"></param>
	/// <param name="shoes"></param>
	/// <param name="glove"></param>
	/// <param name="eyeWaar"></param>
	/// <param name="body"></param>
	/// <returns></returns>
	public async UniTask PostCostume(int hair, int face, int headGear, int top, int bottom, int bag, int shoes, int glove, int eyeWaar, int body)
	{

		var data = new SendCostumeData(hair, face, headGear, top, body, bag, shoes, glove, eyeWaar, body);
		var jsonData = JsonUtility.ToJson(data);
		using var request = UnityWebRequest.Post(DATABASE_PATH_COSTUME_SAVE,jsonData ,CONTENT_TYPE);
		request.SetRequestHeader(TOKEN_KEY, PlayerDontDestroyData.Instance.Token);

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

	[System.Serializable]
	public class OnGetWorldData : ResponseData
	{
		[SerializeField] private Body body = default;
		public int[] ShopList => body.ShopList;
		public UserIDNamePair[] UserList => body.UserList;
		[System.Serializable]
		private class Body
		{
			[SerializeField] private List<int> shopList;
			[SerializeField] private UserIDNamePair[] userList;
			public int[] ShopList => shopList.ToArray();
			public UserIDNamePair[] UserList => userList;
		}
	}

	[System.Serializable]
	public class OnGetMoneyData : ResponseData
	{
		public OnGetMoneyData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private Body body = default;

		public int Money => body.Money;

		[System.Serializable]
		public class Body
		{
			public Body(int money)
			{
				this.money = money;
			}

			[SerializeField] private int money = default;
			public int Money => money;
		}
	}

	[System.Serializable]
	public class SendInventoryUpdateData
	{
		public SendInventoryUpdateData(int itemId, int userIndex, int amount)
		{
			this.itemId = itemId;
			this.userIndex = userIndex;
			this.amount = amount;
		}
		[SerializeField] private int itemId = default;
		[SerializeField] private int userIndex = default;
		[SerializeField] private int amount = default;
	}

	[System.Serializable]
	public class SendCostumeData
	{
		public SendCostumeData(int hair, int face, int headGear, int top, int bottom, int bag, int shoes, int glove, int eyeWear, int body)
		{
			this.hair = hair;
			this.face = face;
			this.headGear = headGear;
			this.top = top;
			this.bottom = bottom;
			this.bag = bag;
			this.shoes = shoes;
			this.glove = glove;
			this.eyeWear = eyeWear;
			this.body = body;
		}

		[SerializeField] private int hair = default;
		[SerializeField] private int face = default;
		[SerializeField] private int headGear = default;
		[SerializeField] private int top = default;
		[SerializeField] private int bottom = default;
		[SerializeField] private int bag = default;
		[SerializeField] private int shoes = default;
		[SerializeField] private int glove = default;
		[SerializeField] private int eyeWear = default;
		[SerializeField] private int body = default;
	}

	/// <summary>
	/// ショップ入店時のレスポンスデータ
	/// </summary>
	[System.Serializable]
	public class OnShopProductData : ResponseData
	{
		public OnShopProductData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private Body body = default;

		public IReadOnlyList<ItemLineup> ItemList => body.ItemList;


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
	/// 自販機アクセス時のレスポンスデータ
	/// </summary>
	[System.Serializable]
	public class OnVMProductData : ResponseData
	{
		public OnVMProductData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private Body body = default;

		public IReadOnlyList<ItemLineup> ItemList => body.ItemList;
		public bool Active => body.Active;

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
			public Body(List<ItemIDAmountPair> inventory, int money, List<ItemIDAmountPair> stockData, int userId, bool updateFlg)
			{
				this.inventory = inventory;
				this.money = money;
				this.stockData = stockData;
				this.userId = userId;
				this.updateFlg = updateFlg;
			}

			[SerializeField] private List<ItemIDAmountPair> inventory = default;
			[SerializeField] private int money = default;
			[SerializeField] private List<ItemIDAmountPair> stockData = default;
			[SerializeField] private int userId = default;
			[SerializeField] private bool updateFlg = default;

			public IReadOnlyList<ItemIDAmountPair> Inventory => inventory;
			public int Money => money;
			public IReadOnlyList<ItemIDAmountPair> StockData => stockData;
			public int UserID => userId;
			public bool UpdateFlg => updateFlg;
		}
	}

	[System.Serializable]
	private class OnVMUpdateData : ResponseData
	{
		public OnVMUpdateData(List<ItemIDAmountPair> inventory)
		{
			this.inventory = inventory;
		}

		[SerializeField] private List<ItemIDAmountPair> inventory = default;

		public IReadOnlyList<ItemIDAmountPair> Inventory => inventory;
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
		public IReadOnlyList<MyRoomObject> ObjectList => body.ObjectList;
		/// <summary>
		/// 自販機が存在しない場合は-1
		/// </summary>
		public int ShopID => body.ShopID;

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

	/// <summary>
	/// ユーザー場所情報取得時のレスポンスデータ
	/// </summary>
	[System.Serializable]
	public class OnCatchUserLocationData : ResponseData
	{
		public OnCatchUserLocationData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private Body body = default;
		public IReadOnlyList<UserLocationData> SessionList => body.SessionList;


		[System.Serializable]
		public class Body
		{
			public Body(List<UserLocationData> sessionList)
			{
				this.sessionList = sessionList;
			}

			[SerializeField] private List<UserLocationData> sessionList = default;
			public IReadOnlyList<UserLocationData> SessionList => sessionList;
		}
	}

	[System.Serializable]
	public class OnCatchUserInventory : ResponseData
	{
		public OnCatchUserInventory(Body body)
		{
			this.body = body;
		}
		[SerializeField] private Body body = default;
		public IReadOnlyList<UserInventoryData> Inventory => body.Inventory;
		[System.Serializable]
		public class Body
		{
			public Body(List<UserInventoryData> itemList)
			{
				this.itemList = itemList;
			}

			[SerializeField] private List<UserInventoryData> itemList = default;
			public IReadOnlyList<UserInventoryData> Inventory => itemList;
		}
	}

	[System.Serializable]
	public class OnCatchCostumeData : ResponseData
	{
		public OnCatchCostumeData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private Body body;
		public Body GetBody => body;

		[System.Serializable]
		public class Body
		{
			public Body(int hair, int face, int headGear, int top, int bottom, int bag, int shoes, int glove, int eyeWear, int body)
			{
				this.hair = hair;
				this.face = face;
				this.headGear = headGear;
				this.top = top;
				this.bottom = bottom;
				this.bag = bag;
				this.shoes = shoes;
				this.glove = glove;
				this.eyeWear = eyeWear;
				this.body = body;
			}

			[SerializeField] private int hair = default;
			[SerializeField] private int face = default;
			[SerializeField] private int headGear = default;
			[SerializeField] private int top = default;
			[SerializeField] private int bottom = default;
			[SerializeField] private int bag = default;
			[SerializeField] private int shoes = default;
			[SerializeField] private int glove = default;
			[SerializeField] private int eyeWear = default;
			[SerializeField] private int body = default;

			public int Hair => hair;
			public int Face => face;
			public int HeadGear => headGear;
			public int Top => top;
			public int Bottom => bottom;
			public int Bag => bag;
			public int Shoes => shoes;
			public int Glove => glove;
			public int EyeWear => eyeWear;
			public int BodyCos => body;
		}
	}
	#endregion

	#region 送信データ
	[System.Serializable]
	public struct ItemIDAmountPricePair
	{
		public ItemIDAmountPricePair(int itemId, int amount, int price)
		{
			this.itemId = itemId;
			this.amount = amount;
			this.price = price;
		}
		[SerializeField] private int itemId;
		[SerializeField] private int amount;
		[SerializeField] private int price;
		public int ItemID => itemId;
	}
	/// <summary>
	/// 購入時の送信データ（内部パースに使用）
	/// </summary>
	[System.Serializable]
	private class SendPaymentData
	{
		public SendPaymentData(List<ItemIDAmountPricePair> itemList, int shopId)
		{
			this.itemList = itemList;
			this.shopId = shopId;
		}

		[SerializeField] private List<ItemIDAmountPricePair> itemList = default;
		[SerializeField] private int shopId = default;
	}

	[System.Serializable]
	private class SendLoginData
	{
		public SendLoginData(string userName, string password)
		{

			this.userId = userName;
			this.password = password;
		}
		//命名と意味が違うけど気にしないで
		[SerializeField] private string userId = default;
		[SerializeField] private string password = default;
	}

	[System.Serializable]
	private class SendVMSalesData
	{
		public SendVMSalesData(int shopId, List<VMSalesData> updateList)
		{
			this.shopId = shopId;
			this.updateList = updateList;
		}

		[SerializeField] private int shopId = default;
		[SerializeField] private List<VMSalesData> updateList = default;

		public int ShopId => shopId;
		public IReadOnlyList<VMSalesData> UpdateList => updateList;
	}

	/// <summary>
	/// MyRoom情報の送信データ（内部パースに使用）
	/// </summary>
	[System.Serializable]
	private class SendMyRoomSaveData
	{
		public SendMyRoomSaveData(List<MyRoomObjectSaved> objectList)
		{
			this.objectList = objectList;
		}

		[SerializeField] private List<MyRoomObjectSaved> objectList = default;
	}
	#endregion

	#region 送受信データ
	/// <summary>
	/// ユーザー場所情報登録時の送信データ
	/// </summary>
	[System.Serializable]
	public class UserLocationData
	{
		public UserLocationData(string sessionName, string locationName)
		{
			this.sessionName = sessionName;
			this.locationName = locationName;
		}

		[SerializeField] private string sessionName = default;
		[SerializeField] private string locationName = default;

		public string SessionName => sessionName;
		public string LocationName => locationName;
	}
	[System.Serializable]
	public struct UserIDNamePair
	{
		[SerializeField] private int userId;
		[SerializeField] private string userName;

		public int UserID => userId;
		public string UserName => userName;
	}

	[System.Serializable]
	public struct UserInventoryData
	{
		[SerializeField] private int itemId;
		[SerializeField] private string itemName;
		[SerializeField] private int userIndex;
		[SerializeField] private int quantity;
		public UserInventoryData(int itemId, string itemName, int userIndex, int quantity)
		{
			this.itemId = itemId;
			this.itemName = itemName;
			this.userIndex = userIndex;
			this.quantity = quantity;
		}
		public int ItemID => itemId;
		public string ItemName => itemName;
		public int UserIndex => userIndex;
		public int Amount => quantity;
	}


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
	/// <br>・在庫数差分</br>
	/// <br>・金額</br>
	/// <br>・削除フラグ</br>
	/// </summary>
	[System.Serializable]
	public struct VMSalesData
	{
		public VMSalesData(int itemId, int stock, int price, bool deleteFlg)
		{
			this.itemId = itemId;
			this.stock = stock;
			this.price = price;
			this.deleteFlg = deleteFlg;
		}

		[SerializeField] private int itemId;
		[SerializeField] private int stock;
		[SerializeField] private int price;
		[SerializeField] private bool deleteFlg;

		public int ItemID => itemId;
		/// <summary>
		/// 前回在庫量と今回在庫量の差分
		/// </summary>
		public int StockDiff => stock;
		public int Price => price;
		/// <summary>
		/// true: 削除済み
		/// </summary>
		public bool DeleteFlg => deleteFlg;
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
			positionX = position.x;
			positionY = position.y;
			positionZ = position.z;
			directionX = eulerRotation.x;
			directionY = eulerRotation.y;
			directionZ = eulerRotation.z;
		}

		[SerializeField] private int itemId;
		[SerializeField] private int housingId;
		[SerializeField] private float positionX;
		[SerializeField] private float positionY;
		[SerializeField] private float positionZ;
		[SerializeField] private float directionX;
		[SerializeField] private float directionY;
		[SerializeField] private float directionZ;

		public int ItemID => itemId;
		public int HousingID => housingId;
		public Vector3 Position
		{
			get
			{
				return new Vector3(positionX, positionY, positionZ);
			}
		}
		public Vector3 EulerRotation => new Vector3(directionX, directionY, directionZ);
	}

	/// <summary>
	/// ・アイテムID
	/// <br>・絶対座標</br>
	/// <br>・EulerRotation</br>
	/// <br>・削除フラグ</br>
	/// </summary>
	[System.Serializable]
	public struct MyRoomObjectSaved
	{
		public MyRoomObjectSaved(int itemId, Vector3 position, Vector3 eulerRotation)
		{
			this.itemId = itemId;
			positionX = position.x;
			positionY = position.y;
			positionZ = position.z;
			directionX = eulerRotation.x;
			directionY = eulerRotation.y;
			directionZ = eulerRotation.z;
		}

		[SerializeField] private int itemId;
		[SerializeField] private float positionX;
		[SerializeField] private float positionY;
		[SerializeField] private float positionZ;
		[SerializeField] private float directionX;
		[SerializeField] private float directionY;
		[SerializeField] private float directionZ;

		public int ItemID => itemId;
		public Vector3 Position
		{
			get
			{
				return new Vector3(positionX, positionY, positionZ);
			}
		}
		public Vector3 EulerRotation => new Vector3(directionX, directionY, directionZ);
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

#if UNITY_EDITOR
/// <summary>
/// エディターでデータベースとのAPI通信を行うクラス
/// <br>このクラスはビルドには含まれません</br>
/// </summary>
public class EditorWebAPIRequester
{
	private const string DETABASE_PATH_BASE = "http://10.11.33.228:8080/api/";
	private const string ID_TRANSFER_ADD = DETABASE_PATH_BASE + "transfer/add";
	private const string ID_TRANSFER_UPDATE = DETABASE_PATH_BASE + "transfer/update";
	private const string ID_TRANSFER_DELETE = DETABASE_PATH_BASE + "transfer/delete";
	private const string CONTENT_TYPE = "application/json";


	public async UniTask PostAddID(List<ItemData> addDataList)
	{
		var sendIDData = new SendIDData(addDataList);
		string jsonData = JsonUtility.ToJson(sendIDData);
		using var request = UnityWebRequest.Post(ID_TRANSFER_ADD, jsonData, CONTENT_TYPE);
		//request.SetRequestHeader("Authorization", PlayerDontDestroyData.Instance.Token);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new WebAPIRequester.APIConnectException(request.error);
		}
	}

	public async UniTask PostUpdateID(List<ItemData> updateDataList)
	{
		var sendIDData = new SendIDData(updateDataList);
		string jsonData = JsonUtility.ToJson(sendIDData);

		using var request = UnityWebRequest.Post(ID_TRANSFER_UPDATE, jsonData, CONTENT_TYPE);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new WebAPIRequester.APIConnectException(request.error);
		}
	}

	public async UniTask PostDeleteID(int itemId)
	{
		WWWForm form = new WWWForm();
		form.AddField("itemId", itemId);

		using var request = UnityWebRequest.Post(ID_TRANSFER_DELETE, form);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new WebAPIRequester.APIConnectException(request.error);
		}
	}

	[System.Serializable]
	private class SendIDData
	{
		[SerializeField] private List<ItemData> itemDataList = default;
		public IReadOnlyList<ItemData> ItemDataList => itemDataList;

		public SendIDData(List<ItemData> itemDataList)
		{
			this.itemDataList = itemDataList;
		}
	}

	[System.Serializable]
	public class ItemData
	{
		[SerializeField] private int itemId;
		[SerializeField] private string itemName;
		[SerializeField] private int size;
		[SerializeField] private int genre;

		public int ItemID => itemId;
		public string ItemName => itemName;
		public int Size => size;
		public int Genre => genre;

		public ItemData(int itemId, string itemName, int size, int genre)
		{
			this.itemId = itemId;
			this.itemName = itemName;
			this.size = size;
			this.genre = genre;
		}
	}
}
#endif