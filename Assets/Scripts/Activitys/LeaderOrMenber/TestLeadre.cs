using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class TestLeadre : MonoBehaviour,ILeader
{
    private NetworkBehaviour networkBehaviour;
    public void Inject(NetworkBehaviour networkBehaviour)
    {
        this.networkBehaviour = networkBehaviour;
    }
}
