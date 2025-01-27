using System.Collections.Generic;
using UnityEngine;

public class PlayerDontDestroyData : MonoBehaviour
{
	private static PlayerDontDestroyData _instance = default;
	[SerializeField,Header("Debug")]
	private int _playerID = 5;
	[SerializeField]
	private int _money = 0;
	[SerializeField]
	private string _previousScene = "";
	[SerializeField,Hide]
	private string _token = default;
	[Space(10)]
	private readonly object _moneyLockObject = new object();
	private readonly object _inventoryLockObject = new object();

	//id count
	private List<ItemIDAmountPair> _inventory = new();
	public static PlayerDontDestroyData Instance => _instance;
	public int PlayerID { get => _playerID; set => _playerID = value; }
	public IReadOnlyList<ItemIDAmountPair> Inventory => _inventory;
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
	public string Token { get => _token; set => _token = value; } 

	public void AddInventory(ItemIDAmountPair itemIDAmountPair)
	{
		lock (_inventoryLockObject)
		{
			_inventory.Add(itemIDAmountPair);
		}
	}

	public void AddDifferenceItem(IReadOnlyList<ItemIDAmountPair> list)
	{
		lock (_inventoryLockObject)
		{
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Destroy(_instance.gameObject);
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