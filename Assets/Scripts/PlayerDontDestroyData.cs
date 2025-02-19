using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using KumaDebug;
public class PlayerDontDestroyData : MonoBehaviour
{
	private static PlayerDontDestroyData _instance = default;
	[SerializeField]
	private ItemBundleAsset _allItemAssets = default;
	private const int _MAX_INVENTORY_COUNT = 20;
	[SerializeField, Header("Debug")]
	private int _playerID = 5;
	[SerializeField]
	private int _money = 0;
	[SerializeField]
	private string _previousScene = "";
	[SerializeField, Hide]
	private string _token = default;
	private Dictionary<int, string> _allPlayerNames = new();
	[SerializeField, Hide]
	private int[] _shopIDs;
	[SerializeField,Hide]
	private int? _movableMyRoomUserID = null;
	[Space(10)]
	private readonly object _moneyLockObject = new object();
	private readonly object _inventoryLockObject = new object();
	private readonly object _costumeInventoryLockObject = new object();
	private readonly object _wallpaperInventoryLockObject = new object();
	private readonly object _flooringInventoryLockObject = new object();

	//id count
	[SerializeField]
	private ItemIDAmountPair[] _inventory = new ItemIDAmountPair[_MAX_INVENTORY_COUNT];
	[SerializeField]
	private List<int> _costumeInventory = new List<int>();
	[SerializeField]
	private List<int> _wallpaperInventory = new();
	[SerializeField]
	private List<int> _flooringInventory = new();
	[SerializeField]
	private List<ItemIDAmountPair> _itemBoxInventory = new List<ItemIDAmountPair>();
	public static PlayerDontDestroyData Instance => _instance;
	public int PlayerID { get => _playerID; set => _playerID = value; }
	public ItemIDAmountPair[] Inventory => _inventory;
	public List<ItemIDAmountPair> InventoryToList => _inventory.Where(item => item.ItemID > 0).ToList();
	public IReadOnlyList<int> CostumeInventory => _costumeInventory;
	public string PreviousScene { get => _previousScene; set => _previousScene = value; }
	public int Money
	{
		get => _money;
		set
		{
			lock (_moneyLockObject)
			{
				_money = value;
			}
		}
	}
	public string Token
	{
		get
		{
			return _token;
		}
		set => _token = value;
	}
	public IReadOnlyDictionary<int, string> AllPlayerNames => _allPlayerNames;
	public int[] ShopIDs => _shopIDs;
	/// <summary>
	/// 一度取得すると次入るまでnullを返す
	/// </summary>
	public int? MovableMyRoomUserID
	{
		get
		{
			int? returnValue = _movableMyRoomUserID;
			_movableMyRoomUserID = null;
			return returnValue;
		}
		set
		{
			_movableMyRoomUserID = value;
		}
	}


	private async void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(_instance.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
		WebAPIRequester webAPIRequester = new();
#if UNITY_EDITOR
		var result = await webAPIRequester.PostLogin("user1@hcs.ac.jp", "hcs5511");
		XDebug.LogWarning(result, Color.cyan);
#endif
		MovableMyRoomUserID = PlayerDontDestroyData.Instance.PlayerID;
	}

	private async void Start()
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		await UpdateInventory(webAPIRequester);
		await UpdateMoney(webAPIRequester);
		await UpdateJoinData(webAPIRequester);
	}

	public async UniTask UpdateJoinData()
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		await UpdateJoinData(webAPIRequester);
	}
	public async UniTask UpdateJoinData(WebAPIRequester webAPIRequester)
	{
		var result = await webAPIRequester.GetJoinWorldData();
		foreach (var item in result.UserList)
		{
			_allPlayerNames.Add(item.UserID, item.UserName);
		}
		_shopIDs = result.ShopList;
	}

	public bool AddInventory(ItemIDAmountPair itemIDAmountPair)
	{
		ItemAsset itemAsset = _allItemAssets.GetItemAssetByID(itemIDAmountPair.ItemID);
		if (itemAsset.Genre == ItemGenre.Costume)
		{
			lock (_costumeInventoryLockObject)
			{
				_costumeInventory.Add(itemAsset.ID);
			}
			return true;
		}

		int index = -1;
		for (int i = 0; i < _inventory.Length; i++)
		{
			if (_inventory[i].ItemID <= 0)
			{
				index = i;
				break;
			}
		}

		if (index <= -1)
		{
			return false;
		}

		lock (_inventoryLockObject)
		{
			_inventory[index] = itemIDAmountPair;
		}
		return true;
	}

	public async UniTask UpdateInventory()
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		await UpdateInventory(webAPIRequester);
	}

	public async UniTask UpdateInventory(WebAPIRequester webAPIRequester)
	{
		WebAPIRequester.OnCatchUserInventory inventoryData = await webAPIRequester.GetInventory();
		_inventory = new ItemIDAmountPair[_MAX_INVENTORY_COUNT];
		InventoryManager inventoryManager = FindObjectOfType<InventoryManager>(true);
		foreach (var item in inventoryData.Inventory)
		{
			ItemAsset itemAsset = _allItemAssets.GetItemAssetByID(item.ItemID);
			if (itemAsset.Genre is ItemGenre.Costume)
			{
				lock (_costumeInventoryLockObject)
				{
					_costumeInventory.Add(item.ItemID);
				}
				continue;
			}
			else if (itemAsset.Genre is ItemGenre.Flooring)
			{
				lock (_flooringInventoryLockObject)
				{
					_flooringInventory.Add(item.ItemID);
				}
				continue;
			}
			else if (itemAsset.Genre is ItemGenre.Wallpaper)
			{
				lock (_wallpaperInventoryLockObject)
				{
					_wallpaperInventory.Add(item.ItemID);
				}
				continue;
			}
			else if (itemAsset.Genre is ItemGenre.Interior or ItemGenre.Usable or ItemGenre.Food)
			{
				ItemIDAmountPair itemIDPair = new ItemIDAmountPair(item.ItemID, item.Amount);
				lock (_inventoryLockObject)
				{
					if (item.UserIndex < 0)
					{
						_itemBoxInventory.Add(itemIDPair);

					}
					else if (item.UserIndex < _inventory.Length)
					{
						_inventory[item.UserIndex] = itemIDPair;

						if (inventoryManager != null)
						{
							// インベントリビューに送信（個数分）
							for (int i = 0; i < item.Amount; i++)
							{
								inventoryManager.SendItem(item.ItemID);
							}
						}
					}
					else
					{
						XDebug.LogWarning("aaa");
						_itemBoxInventory.Add(itemIDPair);
					}
				}
			}
			else
			{
				XDebug.LogError($"インベントリに入るはずのないIDが検知されましたID:{item.ItemID}", KumaDebugColor.ErrorColor);
			}
		}
	}

	public async UniTask UpdateMoney()
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		await UpdateMoney(webAPIRequester);
	}

	public async UniTask UpdateMoney(WebAPIRequester webAPIRequester)
	{
		_money = await webAPIRequester.GetMoney();
	}
}


/// <summary>
/// ・アイテムID
/// <br>・数</br>
/// </summary>
[System.Serializable]
public struct ItemIDAmountPair
{
	public ItemIDAmountPair(int itemId, int amount)
	{
		this.itemId = itemId;
		this.amount = amount;
	}

	[SerializeField] private int itemId;
	[SerializeField] private int amount;

	public int ItemID => itemId;
	public int Amount => amount;
}