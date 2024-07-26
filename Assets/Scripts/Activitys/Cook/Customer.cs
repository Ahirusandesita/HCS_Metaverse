using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField]
    private OrderAsset orderAsset;
    private RemoteOrder remoteOrder;
    public void Order(int index)
    {
        remoteOrder.RPC_Order(index);
    }

    public void InjectRemoteOrder(RemoteOrder remoteOrder)
    {
        this.remoteOrder = remoteOrder;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Order(0);
        }
    }
}
