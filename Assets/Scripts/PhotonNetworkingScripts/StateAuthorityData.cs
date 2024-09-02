using Fusion;
using UnityEngine;

public delegate void AuthrityHandler(AuthrityEventArgs authrityEventArgs);
public class StateAuthorityData : NetworkBehaviour
{
	[Networked]
	public bool IsNotReleaseStateAuthority { get; set ; }
	public event AuthrityHandler OnAuthrity;
	private bool isGrabbable = true;
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

	[UnityEngine.ContextMenu("GetStateAuthrity")]
	public void GetStateAuthrity()
	{
		GetComponent<NetworkObject>().RequestStateAuthority();
	} 
}