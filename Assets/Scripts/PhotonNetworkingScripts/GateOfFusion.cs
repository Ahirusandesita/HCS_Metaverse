using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GateOfFusion
{
	private NetworkRunner _networkRunner = default;
	private static GateOfFusion _instance = default;
	public static GateOfFusion Instance => _instance ??= new GateOfFusion();

	public NetworkRunner NetworkRunner 
	{
		get
		{
			if(_networkRunner == null)
			{
				_networkRunner = Object.FindObjectOfType<NetworkRunner>();
			}
			return _networkRunner;
		}
	}
}
