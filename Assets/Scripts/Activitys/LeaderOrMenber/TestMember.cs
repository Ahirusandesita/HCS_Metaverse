using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TestMember : MonoBehaviour,IMember
{
    private NetworkBehaviour networkBehaviour;
    public void Inject(NetworkBehaviour networkBehaviour)
    {
        this.networkBehaviour = networkBehaviour;
    }
}
