using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour,IAction
{
    [Networked,UnityNonSerialized]
    public bool A { get; set; }

    void Awake()
    {
        A = false;
    }
    RPCEvent RPCEvent;
    public void Inject(IPracticableRPCEvent rPCEvent)
    {
        rPCEvent.RPC_Event<TestObject>(this.GetComponent<NetworkObject>());
        rPCEvent.RPC_Event<TestObject>(this.gameObject);
    }
    public void Action()
    {
        Debug.Log("RPC!");
    }

    private void Update()
    {

        if (A)
        {
            Debug.LogError("やなーぎきーもーいぃー");
            Debug.LogWarning("やなーぎきーもーいぃー");
            Debug.Log("やなーぎきーもーいぃー");
        }
    }
}
