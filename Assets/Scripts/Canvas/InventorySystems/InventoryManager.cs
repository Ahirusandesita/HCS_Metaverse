using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
/// <summary>
/// １つのインベントリ管理
/// </summary>
public class InventoryManager : MonoBehaviour
{
	[SerializeField]
	private NotExistIcon notExistIcon;
	private IInventoryOneFrame[] inventories;
	[SerializeField]
	private ItemBundleAsset itemBundle;
	[SerializeField]
	private SelectItem selectItem;

	private void Awake()
	{
		NotExistIcon oject = Instantiate(notExistIcon);
		inventories = GetComponentsInChildren<IInventoryOneFrame>(true);

		foreach (IInventoryOneFrame inventoryOneFrame in inventories)
		{
			inventoryOneFrame.Inject(this);
			inventoryOneFrame.SelectItemInject(selectItem, notExistIcon);
		}


		foreach (InventoryOneFrame inventoryOneFrame in GetComponentsInChildren<InventoryOneFrame>(true))
		{
			inventoryOneFrame.Inject(oject);
		}
	}

	public async void ReturnItem(ItemAsset itemAsset)
	{
		if (itemAsset.DisplayItem.IsAvailable())
		{
			IDisplayItem item = await SpawnItem(itemAsset);

			Debug.LogError(item.CanUseAtStart);
			if (item.CanUseAtStart)
			{
				item.Use();
			}
		}
		else
		{
			Debug.LogError("使用できない場所");
		}
	}
	public bool IsAvailableItem(ItemAsset itemAsset)
	{
		return itemAsset.DisplayItem.IsAvailable();
	}

	private async UniTask<IDisplayItem> SpawnItem(ItemAsset itemAsset)
	{
		GameObject item = await GateOfFusion.Instance.SpawnAsync(itemAsset.DisplayItem.gameObject, this.transform.position);
		if (item.GetComponent<IDisplayItem>() is ISelectedNotificationInjectable)
		{
			GameObject.FindObjectOfType<PlayerInteraction>().Add(item.GetComponent<IDisplayItem>() as ISelectedNotificationInjectable);
		}
		else
		{
			Debug.LogWarning("掴んだ時にイベント発行したければ、ISelectedNotificationInjectableを実装してね");
		}

		if (item.TryGetComponent(out PlaceableObject placeableObject))
		{
			placeableObject.ItemID = itemAsset.ID;
		}

		return item.GetComponent<IDisplayItem>();
	}

	public void SendItem(int id)
	{
		foreach (IInventoryOneFrame inventory in inventories)
		{
			if (!inventory.HasItem && inventory.MatchItem(itemBundle.GetItemAssetByID(id)))
			{
				inventory.PutAway(itemBundle.GetItemAssetByID(id));
				break;
			}
		}
	}
}
