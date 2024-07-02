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
	/// �z�X�g�������疼�O�ς��ăV�[����ǂݍ���
	/// </summary>
	public void ActivityStart()
	{
		_rpcManager.Rpc_SessionNaming("dadadad");
		Debug.LogWarning("ActivityStart");
	}

	/// <summary>
	/// ���[���ɓ���B�Ȃ��ꍇ�͍��
	/// </summary>
	/// <param name="sessionName">�Z�b�V������</param>
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
