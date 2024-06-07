using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item2 : MonoBehaviour, IDisplayItem
{
    public int ID { get; private set; }
    public string Name { get; private set; }

    void IDisplayItem.SetIDAndItemName(int id, string name)
    {
        ID = id;
        Name = name;
    }

    void IDisplayItem.InjectSelectedNotification(ISelectedNotification sn)
    {
        throw new System.NotImplementedException();
    }
}
