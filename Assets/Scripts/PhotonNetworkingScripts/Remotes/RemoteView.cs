using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RemoteView : MonoBehaviour
{
	private Transform myTransform = default;

	private MasterServerConect _masterServerConect = default;
	private void Start()
	{
		myTransform = transform;
		_masterServerConect = (MasterServerConect)FindObjectOfType(typeof(MasterServerConect));

	}

	/// <summary>
	/// �z�X�g�������疼�O�ς��ăV�[����ǂݍ���
	/// </summary>
	public void ActivityStart()
	{
		
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
		Debug.LogWarning(vector);

		myTransform.position = vector;
	}
}
