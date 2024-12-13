using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class LocalThrow : MonoBehaviour
{
    // 
    private NetworkView _networkView = default;

    private void Start()
    {
        // 
        _networkView = GetComponent<NetworkView>();
    }

    public void ThrowAllLocalView(Vector3 throwVector)
    {
        // 
        _networkView.LocalView.GetComponent<Throwable>().RPC_Throw(throwVector);
    }
}
