using Cysharp.Threading.Tasks;
using Fusion;
using Oculus.Interaction;
using UnityEngine;

public interface IDisplayItem
{
    GameObject gameObject { get; }

    void InjectItemSelectArgs(ItemSelectArgs itemSelectArgs);
    void InjectSelectedNotification(ISelectedNotification sn);
    void InjectPointableUnityEventWrapper(PointableUnityEventWrapper puew);


    #region static Method
    static IDisplayItem Instantiate(ItemAsset item, ISelectedNotification caller)
    {
        var displayItem = Object.Instantiate(item.DisplayItem.gameObject).GetComponent<IDisplayItem>();
        var itemSelectArgs = new ItemSelectArgs(id: item.ID, name: item.Name, gameObject: displayItem.gameObject);
        displayItem.InjectItemSelectArgs(itemSelectArgs);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    static IDisplayItem Instantiate(ItemAsset item, Transform parent, ISelectedNotification caller)
    {
        var displayItem = Object.Instantiate(item.DisplayItem.gameObject, parent).GetComponent<IDisplayItem>();
        var itemSelectArgs = new ItemSelectArgs(id: item.ID, name: item.Name, gameObject: displayItem.gameObject);
        displayItem.InjectItemSelectArgs(itemSelectArgs);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    static IDisplayItem Instantiate(ItemAsset item, Vector3 position, Quaternion rotation, ISelectedNotification caller)
    {
        var displayItem = Object.Instantiate(item.DisplayItem.gameObject, position, rotation).GetComponent<IDisplayItem>();
        var itemSelectArgs = new ItemSelectArgs(item.ID, item.Name, position, displayItem.gameObject);
        displayItem.InjectItemSelectArgs(itemSelectArgs);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    static IDisplayItem Instantiate(ItemAsset item, Vector3 position, Quaternion rotation, Transform parent, ISelectedNotification caller)
    {
        var displayItem = Object.Instantiate(item.DisplayItem.gameObject, position, rotation, parent).GetComponent<IDisplayItem>();
        var itemSelectArgs = new ItemSelectArgs(item.ID, item.Name, position, displayItem.gameObject);
        displayItem.InjectItemSelectArgs(itemSelectArgs);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    private static NetworkRunner NetworkRunner => GateOfFusion.Instance.NetworkRunner;

    /// <summary>
    /// これ別クラスに分ける
    /// </summary>
    static async UniTask<IDisplayItem> InstantiateSync(ItemAsset item, ISelectedNotification caller)
    {
        var networkObject = await NetworkRunner.SpawnAsync(item.DisplayItem.gameObject);
        var displayItem = networkObject.GetComponent<IDisplayItem>();
        var itemSelectArgs = new ItemSelectArgs(id: item.ID, name: item.Name, gameObject: displayItem.gameObject);
        displayItem.InjectItemSelectArgs(itemSelectArgs);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    /// <summary>
    /// これ別クラスに分ける
    /// </summary>
    static async UniTask<IDisplayItem> InstantiateSync(ItemAsset item, Transform parent, ISelectedNotification caller)
    {
        var networkObject = await NetworkRunner.SpawnAsync(item.DisplayItem.gameObject);
        networkObject.transform.SetParent(parent);
        var displayItem = networkObject.GetComponent<IDisplayItem>();
        var itemSelectArgs = new ItemSelectArgs(id: item.ID, name: item.Name, gameObject: displayItem.gameObject);
        displayItem.InjectItemSelectArgs(itemSelectArgs);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    /// <summary>
    /// これ別クラスに分ける
    /// </summary>
    static async UniTask<IDisplayItem> InstantiateSync(ItemAsset item, Vector3 position, Quaternion rotation, ISelectedNotification caller)
    {
        var networkObject = await NetworkRunner.SpawnAsync(item.DisplayItem.gameObject, position, rotation);
        var displayItem = networkObject.GetComponent<IDisplayItem>();
        var itemSelectArgs = new ItemSelectArgs(item.ID, item.Name, position, displayItem.gameObject);
        displayItem.InjectItemSelectArgs(itemSelectArgs);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    /// <summary>
    /// これ別クラスに分ける
    /// </summary>
    static async UniTask<IDisplayItem> InstantiateSync(ItemAsset item, Vector3 position, Quaternion rotation, Transform parent, ISelectedNotification caller)
    {
        var networkObject = await NetworkRunner.SpawnAsync(item.DisplayItem.gameObject, position, rotation);
        networkObject.transform.SetParent(parent);
        var displayItem = networkObject.GetComponent<IDisplayItem>();
        var itemSelectArgs = new ItemSelectArgs(item.ID, item.Name, position, displayItem.gameObject);
        displayItem.InjectItemSelectArgs(itemSelectArgs);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }
    #endregion
}