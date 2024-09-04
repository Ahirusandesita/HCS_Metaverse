using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class OwnInformation : MonoBehaviour
{
    private NetworkObject networkObject;

    public PlayerRef MyPlayerRef => networkObject.StateAuthority;

    private void Awake()
    {
        networkObject = this.GetComponent<NetworkObject>();
    }
}
