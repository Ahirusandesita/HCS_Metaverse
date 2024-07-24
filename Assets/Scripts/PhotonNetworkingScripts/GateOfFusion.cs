using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GateOfFusion
{
	private NetworkRunner _networkRunner = default;
	private static GateOfFusion _instance = default;
	private MasterServerConect _masterServer = default;
	public static GateOfFusion Instance => _instance ??= new GateOfFusion();
	private MasterServerConect MasterServer => _masterServer ??= Object.FindObjectOfType<MasterServerConect>();

	public NetworkRunner NetworkRunner
	{
		get
		{
			if (_networkRunner == null)
			{
				_networkRunner = Object.FindObjectOfType<NetworkRunner>();
			}
			return _networkRunner;
		}
	}

	/// <summary>
	/// �͂ނƂ��ɌĂ�
	/// </summary>
	/// <param name="networkObject">�͂񂾃I�u�W�F�N�g</param>
	public void Grab(NetworkObject networkObject)
	{
		Debug.LogWarning($"Grab:");
		StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
		if (stateAuthorityData.IsNotReleaseStateAuthority)
		{
			Debug.LogWarning($"<color=red>����������܂���ł���</color>");
			return;
		}
		if (networkObject.HasStateAuthority)
		{
			Debug.LogWarning("<color=lime>�����������������Ă��܂�</color>");
			return;
		}
		RPCManager.Instance.Rpc_GrabStateAuthorityChanged(networkObject);
		networkObject.RequestStateAuthority();

	}

	public void Release(NetworkObject networkObject)
	{
		RPCManager.Instance.Rpc_ReleseStateAuthorityChanged(networkObject);
	}
}
