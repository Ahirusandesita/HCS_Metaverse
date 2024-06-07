using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item2 : MonoBehaviour, IDisplayItem
{
    public string Name => "BokeBokeObject";

    void IDisplayItem.InjectSelectedNotification(ISelectedNotification sn)
    {
        throw new System.NotImplementedException();
    }
}
