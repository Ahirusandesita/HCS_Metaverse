using UnityEngine;
using Oculus.Interaction;

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
        displayItem.InjectItemSelectArgs(new ItemSelectArgs(item.ItemID, item.ItemName));
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    static IDisplayItem Instantiate(ItemAsset item, Transform parent, ISelectedNotification caller)
    {
        var displayItem = Object.Instantiate(item.DisplayItem.gameObject, parent).GetComponent<IDisplayItem>();
        displayItem.InjectItemSelectArgs(new ItemSelectArgs(item.ItemID, item.ItemName));
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    static IDisplayItem Instantiate(ItemAsset item, Vector3 position, Quaternion rotation, ISelectedNotification caller)
    {
        var displayItem = Object.Instantiate(item.DisplayItem.gameObject, position, rotation).GetComponent<IDisplayItem>();
        displayItem.InjectItemSelectArgs(new ItemSelectArgs(item.ItemID, item.ItemName, position));
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    static IDisplayItem Instantiate(ItemAsset item, Vector3 position, Quaternion rotation, Transform parent, ISelectedNotification caller)
    {
        var displayItem = Object.Instantiate(item.DisplayItem.gameObject, position, rotation, parent).GetComponent<IDisplayItem>();
        displayItem.InjectItemSelectArgs(new ItemSelectArgs(item.ItemID, item.ItemName, position));
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }
    #endregion
}
