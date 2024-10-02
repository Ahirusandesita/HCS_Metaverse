using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class TestRPC : MonoBehaviour
{
    [SerializeField]
    TestObject testObject;
    void Start()
    {
        StartCoroutine(A());
    }
    NetworkObject networkObject;
    IEnumerator A()
    {
        yield return new WaitForSeconds(6f);
        //NetworkObject rp = GateOfFusion.Instance.NetworkRunner.Spawn(RPCEvent.gameObject);
        networkObject = GateOfFusion.Instance.NetworkRunner.Spawn(testObject.gameObject);

        //yield return new WaitForSeconds(2f);
        //networkObject.GetComponent<TestObject>().Inject(rp.GetComponent<RPCEvent>());
    }
}
