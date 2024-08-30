using Cysharp.Threading.Tasks;
using Oculus.Interaction;
using UnityEngine;

public interface IDisplayItem
{
	GameObject gameObject { get; }

	void Inject_ItemSelectArgsAndSelectedNotification(ItemSelectArgs itemSelectArgs, ISelectedNotification sn);
	void InjectPointableUnityEventWrapper(PointableUnityEventWrapper puew);


	#region static Method
	private static GateOfFusion GateOfFusion => GateOfFusion.Instance;

	static IDisplayItem Instantiate(ItemAsset item, ISelectedNotification caller)
	{
		var displayItem = Object.Instantiate(item.DisplayItem.gameObject).GetComponent<IDisplayItem>();
		var itemSelectArgs = new ItemSelectArgs(id: item.ID, name: item.Name, gameObject: displayItem.gameObject);
		displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, caller);
		return displayItem;
	}

	static IDisplayItem Instantiate(ItemAsset item, Transform parent, ISelectedNotification caller)
	{
		var displayItem = Object.Instantiate(item.DisplayItem.gameObject, parent).GetComponent<IDisplayItem>();
		var itemSelectArgs = new ItemSelectArgs(id: item.ID, name: item.Name, gameObject: displayItem.gameObject);
		displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, caller);
		return displayItem;
	}

	static IDisplayItem Instantiate(ItemAsset item, Vector3 position, Quaternion rotation, ISelectedNotification caller)
	{
		var displayItem = Object.Instantiate(item.DisplayItem.gameObject, position, rotation).GetComponent<IDisplayItem>();
		var itemSelectArgs = new ItemSelectArgs(item.ID, item.Name, position, displayItem.gameObject);
		displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, caller);
		return displayItem;
	}

	static IDisplayItem Instantiate(ItemAsset item, Vector3 position, Quaternion rotation, Transform parent, ISelectedNotification caller)
	{
		var displayItem = Object.Instantiate(item.DisplayItem.gameObject, position, rotation, parent).GetComponent<IDisplayItem>();
		var itemSelectArgs = new ItemSelectArgs(item.ID, item.Name, position, displayItem.gameObject);
		displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, caller);
		return displayItem;
	}


	/// <summary>
	/// これ別クラスに分ける
	/// </summary>
	static async UniTask<IDisplayItem> InstantiateSync(ItemAsset item, ISelectedNotification caller)
	{
		GameObject itemObject;
		if (GateOfFusion.IsUsePhoton)
		{
			itemObject = await GateOfFusion.SpawnAsync(item.DisplayItem.gameObject);
		}
		else
		{
			itemObject = Object.Instantiate(item.DisplayItem.gameObject);
		}
		var displayItem = itemObject.GetComponent<IDisplayItem>();
		var itemSelectArgs = new ItemSelectArgs(id: item.ID, name: item.Name, gameObject: displayItem.gameObject);
		displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, caller);
		return displayItem;
	}

	/// <summary>
	/// これ別クラスに分ける
	/// </summary>
	static async UniTask<IDisplayItem> InstantiateSync(ItemAsset item, Transform parent, ISelectedNotification caller)
	{
		GameObject itemObject;
		if (GateOfFusion.IsUsePhoton)
		{
			itemObject = await GateOfFusion.SpawnAsync(item.DisplayItem.gameObject, parent: parent);
		}
		else
		{
			itemObject = Object.Instantiate(item.DisplayItem.gameObject, parent);
		}
		var displayItem = itemObject.GetComponent<IDisplayItem>();
		var itemSelectArgs = new ItemSelectArgs(id: item.ID, name: item.Name, gameObject: displayItem.gameObject);
		displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, caller);
		return displayItem;
	}

	/// <summary>
	/// これ別クラスに分ける
	/// </summary>
	static async UniTask<IDisplayItem> InstantiateSync(ItemAsset item, Vector3 position, Quaternion rotation, ISelectedNotification caller)
	{
		GameObject itemObject;
		if (GateOfFusion.IsUsePhoton)
		{
			itemObject = await GateOfFusion.SpawnAsync(item.DisplayItem.gameObject, position, rotation);
		}
		else
		{
			itemObject = Object.Instantiate(item.DisplayItem.gameObject, position, rotation);
		}
		var displayItem = itemObject.GetComponent<IDisplayItem>();
		var itemSelectArgs = new ItemSelectArgs(item.ID, item.Name, position, displayItem.gameObject);
		displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, caller);
		return displayItem;
	}

	/// <summary>
	/// これ別クラスに分ける
	/// </summary>
	static async UniTask<IDisplayItem> InstantiateSync(ItemAsset item, Vector3 position, Quaternion rotation, Transform parent, ISelectedNotification caller)
	{
		GameObject itemObject;
		if (GateOfFusion.IsUsePhoton)
		{
			itemObject = await GateOfFusion.SpawnAsync(item.DisplayItem.gameObject, position, rotation, parent);
		}
		else
		{
			itemObject = Object.Instantiate(item.DisplayItem.gameObject, position, rotation, parent);
		}
		var displayItem = itemObject.GetComponent<IDisplayItem>();
		var itemSelectArgs = new ItemSelectArgs(item.ID, item.Name, position, displayItem.gameObject);
		displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, caller);
		return displayItem;
	}
	#endregion
}