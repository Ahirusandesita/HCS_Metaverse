using UnityEngine;

public interface IDisplayItem
{
    GameObject gameObject { get; }

    string Name { get; }

    void InjectSelectedNotification(ISelectedNotification sn);

    #region static Method
    static GameObject Instantiate(IDisplayItem item, ISelectedNotification caller)
    {
        item.InjectSelectedNotification(caller);
        return Object.Instantiate(item.gameObject);
    }

    static GameObject Instantiate(IDisplayItem item, Transform parent, ISelectedNotification caller)
    {
        item.InjectSelectedNotification(caller);
        return Object.Instantiate(item.gameObject, parent);
    }

    static GameObject Instantiate(IDisplayItem item, Vector3 position, Quaternion rotation, ISelectedNotification caller)
    {
        item.InjectSelectedNotification(caller);
        return Object.Instantiate(item.gameObject, position, rotation);
    }

    static GameObject Instantiate(IDisplayItem item, Vector3 position, Quaternion rotation, Transform parent, ISelectedNotification caller)
    {
        item.InjectSelectedNotification(caller);
        return Object.Instantiate(item.gameObject, position, rotation, parent);
    }
    #endregion
}
