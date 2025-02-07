using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
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
	[Space(10)]
	private readonly object _moneyLockObject = new object();
	private readonly object _inventoryLockObject = new object();
	private readonly object _costumeInventoryLockObject = new object();

	//id count
	[SerializeField]
	private ItemIDAmountPair[] _inventory = new ItemIDAmountPair[_MAX_INVENTORY_COUNT];
	[SerializeField]
	private List<int> _costumeInventory = new List<int>();

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

	private async void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Destroy(_instance.gameObject);
		}
		WebAPIRequester webAPIRequester = new();
#if UNITY_EDITOR
		await webAPIRequester.PostLogin("admin@hcs.ac.jp", "hcs5511");
#endif
	}

	private async void Start()
	{
		await UpdateInventory();
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
		InventoryManager inventoryManager = FindAnyObjectByType<InventoryManager>();
		foreach (var item in inventoryData.Inventory)
		{
			ItemAsset itemAsset = _allItemAssets.GetItemAssetByID(item.ItemID);
			if (itemAsset.Genre == ItemGenre.Costume)
			{
				lock (_costumeInventoryLockObject)
				{
					_costumeInventory.Add(item.ItemID);
				}
				continue;
			}
			ItemIDAmountPair itemIDPair = new ItemIDAmountPair(item.ItemID, item.Count);
			lock (_inventoryLockObject)
			{
				_inventory[item.UserIndex] = itemIDPair;

				// インベントリビューに送信（個数分）
				for (int i = 0; i < item.Count; i++)
				{
					inventoryManager.SendItem(item.ItemID);
				}
			}

		}
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