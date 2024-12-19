using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkKnife : NetworkBehaviour
{
    private NetworkView _networkView = default;

    private void Start()
    {
        _networkView = GetComponent<NetworkView>();
    }

    public void RPC_HitBoard()
    {

    }

    public void RPC_UnLockKnife()
    {
        //_networkView.LocalView.GetComponent<Knife>().RPC_UnlockedObject();
    }

    public void RPC_UnSelect()
    {
        //_networkView.LocalView.GetComponent<Knife>().RPC_UnSelect();
    }
}
