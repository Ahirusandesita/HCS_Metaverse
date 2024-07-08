using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ReleaseStateAuthorityData : NetworkBehaviour 
{
    [Networked]
    public bool IsNotReleaseStateAuthority { get; set; }
}
