using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class TestRPC : MonoBehaviour
{
    [SerializeField]
    RPCEvent RPCEvent;
    [SerializeField]
    TestObject testObject;
    void Start()
    {
        StartCoroutine(A());
    }

    IEnumerator A()
    {
        yield return new WaitForSeconds(6f);
        NetworkObject rp = GateOfFusion.Instance.NetworkRunner.Spawn(RPCEvent.gameObject);
        NetworkObject networkObject = GateOfFusion.Instance.NetworkRunner.Spawn(testObject.gameObject);
        Debug.LogWarning("RPCEvent Spawn");

        yield return new WaitForSeconds(2f);
        networkObject.GetComponent<TestObject>().Inject(rp.GetComponent<RPCEvent>());
    }
}
