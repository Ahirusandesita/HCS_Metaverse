using UnityEngine;

public interface IDisplayItem
{
    GameObject gameObject { get; }
    int ID { get; }
    string Name { get; }

    void SetIDAndItemName(int id, string name);
    void InjectSelectedNotification(ISelectedNotification sn);


    #region static Method
    static GameObject Instantiate(ItemAsset item, ISelectedNotification caller)
    {
        var displayItem = item.DisplayItem;
        displayItem.SetIDAndItemName(item.ItemID, item.ItemName);
        displayItem.InjectSelectedNotification(caller);
        return Object.Instantiate(displayItem.gameObject);
    }

    static GameObject Instantiate(ItemAsset item, Transform parent, ISelectedNotification caller)
    {
        var displayItem = item.DisplayItem;
        displayItem.SetIDAndItemName(item.ItemID, item.ItemName);
        displayItem.InjectSelectedNotification(caller);
        return Object.Instantiate(displayItem.gameObject, parent);
    }

    static GameObject Instantiate(ItemAsset item, Vector3 position, Quaternion rotation, ISelectedNotification caller)
    {
        var displayItem = item.DisplayItem;
        displayItem.SetIDAndItemName(item.ItemID, item.ItemName);
        displayItem.InjectSelectedNotification(caller);
        return Object.Instantiate(displayItem.gameObject, position, rotation);
    }

    static GameObject Instantiate(ItemAsset item, Vector3 position, Quaternion rotation, Transform parent, ISelectedNotification caller)
    {
        var displayItem = item.DisplayItem;
        displayItem.SetIDAndItemName(item.ItemID, item.ItemName);
        displayItem.InjectSelectedNotification(caller);
        return Object.Instantiate(displayItem.gameObject, position, rotation, parent);
    }
    #endregion
}
