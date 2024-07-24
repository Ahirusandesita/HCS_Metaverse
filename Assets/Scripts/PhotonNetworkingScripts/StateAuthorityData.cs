using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class StateAuthorityData : NetworkBehaviour 
{
    [Networked]
    public bool IsNotReleaseStateAuthority { get; set; }

    public bool IsGrabbable { get; set; }
}
