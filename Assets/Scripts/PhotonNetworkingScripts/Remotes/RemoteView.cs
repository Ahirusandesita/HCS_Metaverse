using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RemoteView : MonoBehaviour
{
	private Transform myTransform = default;

	private MasterServerConect _masterServerConect = default;
	[SerializeField]
	private RPCManager _rpcManager = default;
	private void Start()
	{
		myTransform = transform;
		_masterServerConect = (MasterServerConect)FindObjectOfType(typeof(MasterServerConect));
		_rpcManager = (RPCManager)FindObjectOfType(typeof(RPCManager));
	}

	[ContextMenu("dadadad")]
	/// <summary>
	/// ホストだったら名前変えてシーンを読み込む
	/// </summary>
	public void ActivityStart()
	{
		_rpcManager.Rpc_SessionNaming("dadadad");
		Debug.LogWarning("ActivityStart");
	}

	/// <summary>
	/// ルームに入る。ない場合は作る
	/// </summary>
	/// <param name="sessionName">セッション名</param>
	public void JoinRoom(string sessionName)
	{
		_masterServerConect.UpdateNetworkRunner();

		_masterServerConect.Runner.StartGame(new StartGameArgs
		{
			GameMode = GameMode.AutoHostOrClient,
			SessionName = sessionName,
			SceneManager = _masterServerConect.Runner.GetComponent<NetworkSceneManagerDefault>()
		});
	}

	public void SetVector3(Vector3 vector)
	{
		//Debug.LogWarning(vector);

		myTransform.position = vector;
	}
}
