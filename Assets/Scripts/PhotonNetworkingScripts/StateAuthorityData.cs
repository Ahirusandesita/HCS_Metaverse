using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class AuthrityEventArgs : System.EventArgs
{
    public readonly bool Authrity;
    public AuthrityEventArgs(bool authrity)
    {
        this.Authrity = authrity;
    }
}
public delegate void AuthrityHandler(AuthrityEventArgs authrityEventArgs);
public class StateAuthorityData : NetworkBehaviour 
{
    [Networked]
    public bool IsNotReleaseStateAuthority { get; set; }
    public event AuthrityHandler OnAuthrity;
    private bool isGrabbable;
    public bool IsGrabbable
    {
        get
        {
            return isGrabbable;
        }
        set
        {
            isGrabbable = value;
            OnAuthrity?.Invoke(new AuthrityEventArgs(value));
        }
    }
}
