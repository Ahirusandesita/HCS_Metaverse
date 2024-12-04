using Fusion;
using UnityEngine;

public delegate void AuthrityHandler(AuthrityEventArgs authrityEventArgs);
public class StateAuthorityData : NetworkBehaviour
{
	[Networked]
	public bool IsNotReleaseStateAuthority { get; set ; }
	public bool IsEnable => isEnable;
	public event AuthrityHandler OnAuthrity;
	private bool isGrabbable = true;
	private bool isEnable = false;

	public override void Spawned()
	{
		base.Spawned();
		isEnable = true;
	}

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

	[ContextMenu("GetStateAuthrity")]
	public void GetStateAuthrity()
	{
		GetComponent<NetworkObject>().RequestStateAuthority();
	} 
}