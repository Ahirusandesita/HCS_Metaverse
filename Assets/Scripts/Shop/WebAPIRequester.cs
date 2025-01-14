using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Result = UnityEngine.Networking.UnityWebRequest.Result;

/// <summary>
/// Shop�ƃf�[�^�x�[�X�̑���M���s���B�����_�ł͊eShop���ƂɃC���X�^���X����������݌v�B
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


	#region Post/Get ���\�b�h
	/// <summary>
	/// �V���b�v���X����API�ʐM
	/// </summary>
	/// <returns>�E���i���C���i�b�v</returns>
	public async UniTask<OnShopEntryData> PostShopEntry(int shopId)
	{
		WWWForm form = new WWWForm();
		form.AddField("shopId", shopId);
		using var request = UnityWebRequest.Post(DETABASE_PATH_SHOP_ENTRY, form);
		await request.SendWebRequest();

		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("�l�b�g���[�N�ʐM�������i�s���B");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var onShopEntryData = JsonUtility.FromJson<OnShopEntryData>(request.downloadHandler.text);
		return onShopEntryData;
	}

	/// <summary>
	/// �V���b�v�w������API�ʐM
	/// </summary>
	/// <returns>�E�w����̏����i���X�g
	/// <br>�E�w����̋��z</br>
	/// <br>�E�V���b�v�̍݌Ƀ��X�g</br>
	/// <br>�E���[�U�[ID</br></returns>
	public async UniTask<OnShopPaymentData> PostShopPayment(List<ItemStock> inventory, int shopId, int userId)
	{
		var sendShopPaymentData = new SendPaymentData(inventory, shopId, userId);
		string jsonData = JsonUtility.ToJson(sendShopPaymentData);

		using var request = UnityWebRequest.Post(DETABASE_PATH_SHOP_BUY, jsonData, CONTENT_TYPE);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("�l�b�g���[�N�ʐM�������i�s���B");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var onShopPaymentData = JsonUtility.FromJson<OnShopPaymentData>(request.downloadHandler.text);
		return onShopPaymentData;
	}

	/// <summary>
	/// ���̋@�A�N�Z�X���i�}�C���[���������̏������ȂǍX�V�������Ƃ��j��API�ʐM
	/// </summary>
	/// <param name="shopId"></param>
	/// <returns>�E���i���C���i�b�v
	/// <br>�E���̋@�̃A�N�e�B�u��</br></returns>
	public async UniTask<OnVMEntryData> PostVMEntry(int shopId)
	{
		WWWForm form = new WWWForm();
		form.AddField("shopId", shopId);
		using var request = UnityWebRequest.Post(DETABASE_PATH_VENDINGMACHINE_ENTRY, form);
		await request.SendWebRequest();

		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("�l�b�g���[�N�ʐM�������i�s���B");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var onVMEntryData = JsonUtility.FromJson<OnVMEntryData>(request.downloadHandler.text);
		return onVMEntryData;
	}

	/// <summary>
	/// ���̋@�w������API�ʐM
	/// </summary>
	/// <returns>�E�w����̏����i���X�g
	/// <br>�E�w����̋��z</br>
	/// <br>�E���̋@�̍݌Ƀ��X�g</br>
	/// <br>�E���[�U�[ID</br>
	/// <br>�E���̋@�̃A�b�v�f�[�g�t���O</br></returns>
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
				throw new System.InvalidOperationException("�l�b�g���[�N�ʐM�������i�s���B");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				XDebug.LogError(request.result, "red");
				throw new APIConnectException(request.error);
		}

		var onVMPaymentData = JsonUtility.FromJson<OnVMPaymentData>(request.downloadHandler.text);
		return onVMPaymentData;
	}

	/// <summary>
	/// �}�C���[����������API�ʐM
	/// </summary>
	/// <returns>�E�I�u�W�F�N�g���X�g
	/// <br>�E�V���b�vID�i���̋@�j</br></returns>
	public async UniTask<OnMyRoomEntryData> PostMyRoomEntry(int userId)
	{
		WWWForm form = new WWWForm();
		form.AddField("userId", userId);
		using var request = UnityWebRequest.Post(DETABASE_PATH_MYROOM_ENTRY, form);
		await request.SendWebRequest();

		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("�l�b�g���[�N�ʐM�������i�s���B");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}

		var onMyroomEntryData = JsonUtility.FromJson<OnMyRoomEntryData>(request.downloadHandler.text);
		return onMyroomEntryData;
	}

	/// <summary>
	/// ���[�U�[�ꏊ���o�^����API�ʐM
	/// </summary>
	/// <returns>�Ȃ�</returns>
	public async UniTask PostUserLocation(int userId, string sessionName, string locationName)
	{
		var sendUserLocationData = new SendUserLocationData(userId, sessionName, locationName);
		string jsonData = JsonUtility.ToJson(sendUserLocationData);

		using var request = UnityWebRequest.Post(DETABASE_PATH_USER_LOCATION, jsonData, CONTENT_TYPE);
		await request.SendWebRequest();
		switch (request.result)
		{
			case Result.InProgress:
				throw new System.InvalidOperationException("�l�b�g���[�N�ʐM�������i�s���B");

			case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
				throw new APIConnectException(request.error);
		}
	}
	#endregion

	#region ���X�|���X�f�[�^
	/// <summary>
	/// API�ʐM�̊�ꃌ�X�|���X�f�[�^
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
	/// �V���b�v���X���̃��X�|���X�f�[�^
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
	/// �V���b�v�w�����̃��X�|���X�f�[�^
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
	/// ���̋@�A�N�Z�X���̃��X�|���X�f�[�^
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
	/// ���̋@�w�����̃��X�|���X�f�[�^
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
	/// �}�C���[���������̃��X�|���X�f�[�^
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
			/// ���̋@�����݂��Ȃ��ꍇ��-1
			/// </summary>
			public int ShopID => shopId;
		}
	}
	#endregion

	#region ���M�f�[�^
	/// <summary>
	/// �w�����̑��M�f�[�^�i�����p�[�X�Ɏg�p�j
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
	/// ���[�U�[�ꏊ���o�^���̑��M�f�[�^
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
	/// �E�A�C�e��ID
	/// <br>�E���i</br>
	/// <br>�E������</br>
	/// <br>�E�݌ɐ�</br>
	/// <br>�E�T�C�Y</br>
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
	/// �E�A�C�e��ID
	/// <br>�E��</br>
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
	/// �E�A�C�e��ID
	/// <br>�E�n�E�W���OID</br>
	/// <br>�E��΍��W</br>
	/// <br>�EEulerRotation</br>
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

	#region ��O
	/// <summary>
	/// �EAPI�ʐM���̗�O
	/// </summary>
	public class APIConnectException : System.Exception
	{
		public APIConnectException() : base() { }
		public APIConnectException(string message) : base(message) { }
		public APIConnectException(string message, System.Exception innerException) : base(message, innerException) { }
	}
	#endregion
}