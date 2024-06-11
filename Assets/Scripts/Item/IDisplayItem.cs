using UnityEngine;
using Oculus.Interaction;

public interface IDisplayItem
{
    GameObject gameObject { get; }
    int ID { get; }
    string Name { get; }

    void SetIDAndItemName(int id, string name);
    void InjectSelectedNotification(ISelectedNotification sn);
    void InjectPointableUnityEventWrapper(PointableUnityEventWrapper puew);


    #region static Method
    static IDisplayItem Instantiate(ItemAsset item, ISelectedNotification caller)
    {
        var displayItem = Object.Instantiate(item.DisplayItem.gameObject).GetComponent<IDisplayItem>();
        displayItem.SetIDAndItemName(item.ItemID, item.ItemName);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    static IDisplayItem Instantiate(ItemAsset item, Transform parent, ISelectedNotification caller)
    {
        var displayItem = Object.Instantiate(item.DisplayItem.gameObject, parent).GetComponent<IDisplayItem>();
        displayItem.SetIDAndItemName(item.ItemID, item.ItemName);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    static IDisplayItem Instantiate(ItemAsset item, Vector3 position, Quaternion rotation, ISelectedNotification caller)
    {
        var displayItem = Object.Instantiate(item.DisplayItem.gameObject, position, rotation).GetComponent<IDisplayItem>();
        displayItem.SetIDAndItemName(item.ItemID, item.ItemName);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }

    static IDisplayItem Instantiate(ItemAsset item, Vector3 position, Quaternion rotation, Transform parent, ISelectedNotification caller)
    {
        var displayItem = Object.Instantiate(item.DisplayItem.gameObject, position, rotation, parent).GetComponent<IDisplayItem>();
        displayItem.SetIDAndItemName(item.ItemID, item.ItemName);
        displayItem.InjectSelectedNotification(caller);
        return displayItem;
    }
    #endregion
}
