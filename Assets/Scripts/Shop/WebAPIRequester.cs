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

	public async UniTask<OnShopPaymentData> PostShopPayment(List<ItemStock> inventory, int shopId, int userId)
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

		var onPaymentData = JsonUtility.FromJson<OnShopPaymentData>(request.downloadHandler.text);
		return onPaymentData;
	}

	public async UniTask<OnVMEntryData> PostVMEntry(int shopId)
	{
		return default;
	}

	public async UniTask<OnVMPaymentData> PostVMPayment(int itemId, int shopId, int userId)
	{
		return default;
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
			public Body(List<ItemLineup> itemList)
			{
				this.itemList = itemList;
			}

			[SerializeField] private List<ItemLineup> itemList = default;
			public IReadOnlyList<ItemLineup> ItemList => itemList;
		}
	}

	[System.Serializable]
	public class OnShopPaymentData
	{
		public OnShopPaymentData(Body body)
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

	[System.Serializable]
	public class OnVMEntryData
	{

	}

	[System.Serializable]
	public class OnVMPaymentData
	{

	}

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

	[SerializeField]
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
}