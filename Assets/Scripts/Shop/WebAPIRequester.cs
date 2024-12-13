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
	private const string DETABASE_PATH_VENDINGMACHINE_BUY = DETABASE_PATH_BASE + "user/shop";
	private const string CONTENT_TYPE = "application/json";


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
				throw new System.InvalidOperationException(request.error);
		}

		var onEntryData = JsonUtility.FromJson<OnShopEntryData>(request.downloadHandler.text);
		return onEntryData;
	}

	public async UniTask<OnPaymentData> PostShopPayment(List<OnPaymentData.Body.Inventory> inventory, int shopId, int userId)
	{
		var sendPaymentData = new SendPaymentData(inventory, shopId, userId);
		string jsonData = JsonUtility.ToJson(sendPaymentData);

		using var request = UnityWebRequest.Post(DETABASE_PATH_SHOP_BUY, jsonData, CONTENT_TYPE);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new System.InvalidOperationException(request.error);
		}

		var onPaymentData = JsonUtility.FromJson<OnPaymentData>(request.downloadHandler.text);
		return onPaymentData;
	}

	[System.Serializable]
	public class OnShopEntryData
	{
		public OnShopEntryData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private string responseCode = default;
		[SerializeField] private string message = default;
		[SerializeField] private Body body = default;

		public string ResponseCode => responseCode;
		public string Message => message;
		public Body GetBody => body;

		[System.Serializable]
		public class Body
		{
			public Body(List<Lineup> itemList)
			{
				this.itemList = itemList;
			}

			[SerializeField] private List<Lineup> itemList = default;
			public IReadOnlyList<Lineup> ItemLineup => itemList;

			[System.Serializable]
			public class Lineup
			{
				public Lineup(int itemId, int salesPrice, float discount, int stock, int size)
				{
					this.itemId = itemId;
					this.price = salesPrice;
					this.discount = discount;
					this.stock = stock;
					this.size = size;
				}

				[SerializeField] private int itemId = default;
				[SerializeField] private int price = default;
				[SerializeField] private float discount = default;
				[SerializeField] private int stock = default;
				[SerializeField] private int size = default;

				public int ItemID => itemId;
				public int Price => price;
				public float Discount => discount;
				public int Stock => stock;
				/// <summary>
				/// 0: large, 1: small
				/// </summary>
				public int Size => size;
			}
		}
	}

	[System.Serializable]
	public class OnPaymentData
	{
		public OnPaymentData(Body body)
		{
			this.body = body;
		}

		[SerializeField] private string responseCode = default;
		[SerializeField] private string message = default;
		[SerializeField] private Body body = default;

		public string ResponseCode => responseCode;
		public string Message => message;
		public Body GetBody => body;

		[System.Serializable]
		public class Body
		{
			public Body(List<Inventory> inventory, int money, int stock, int userId)
			{
				this.inventory = inventory;
				this.money = money;
				this.stock = stock;
				this.userId = userId;
			}

			[SerializeField] private List<Inventory> inventory = default;
			[SerializeField] private int money = default;
			[SerializeField] private int stock = default;
			[SerializeField] private int userId = default;

			public IReadOnlyList<Inventory> InventoryList => inventory;
			public int Money => money;
			public int Stock => stock;
			public int UserID => userId;

			[System.Serializable]
			public class Inventory
			{
				public Inventory(int itemId, int count)
				{
					this.itemId = itemId;
					this.count = count;
				}

				[SerializeField] private int itemId = default;
				[SerializeField] private int count = default;

				public int ItemID => itemId;
				public int Count => count;
			}
		}
	}

	[System.Serializable]
	private class SendPaymentData
	{
		public SendPaymentData(List<OnPaymentData.Body.Inventory> itemList, int shopId, int userId)
		{
			this.itemList = itemList;
			this.shopId = shopId;
			this.userId = userId;
		}

		[SerializeField] private List<OnPaymentData.Body.Inventory> itemList = default;
		[SerializeField] private int shopId = default;
		[SerializeField] public int userId = default;

		public IReadOnlyList<OnPaymentData.Body.Inventory> ItemList => itemList;
		public int ShopID => shopId;
		public int UserID => userId;
	}
}