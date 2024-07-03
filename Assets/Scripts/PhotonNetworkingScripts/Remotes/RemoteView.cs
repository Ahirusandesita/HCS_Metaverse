using UnityEngine;
using Fusion;

public class RemoteView : MonoBehaviour
{
	[SerializeField]
	private RPCManager _rpcManager = default;
	private Transform myTransform = default;
	private NetworkRunner _networkRunner = default;
	private MasterServerConect _masterServerConect = default;

	private void Start()
	{
		myTransform = transform;
		_masterServerConect = (MasterServerConect)FindObjectOfType(typeof(MasterServerConect));
		_rpcManager = (RPCManager)FindObjectOfType(typeof(RPCManager));
		_networkRunner = _rpcManager.Runner;
	}

	/// <summary>
	/// �A�N�e�B�r�e�B���J�n����
	/// </summary>
	/// <param name="sceneName">�J�n����A�N�e�B�r�e�B�̃V�[����</param>
	public void ActivityStart(string sceneName)
	{
		if (!_networkRunner.IsServer) { return; }

		_rpcManager.Rpc_SessionNaming("dadadad");
		Debug.LogWarning("ActivityStart");
		_networkRunner.LoadScene(sceneName);
	}

	/// <summary>
	/// ���[���ɓ���B�Ȃ��ꍇ�͍��
	/// </summary>
	/// <param name="sessionName">�Z�b�V������</param>
	public async void JoinOrCreateRoom(string sessionName)
	{
		if (_networkRunner.IsServer)
		{
			_masterServerConect.UpdateNetworkRunner();
		}

		await _masterServerConect.Runner.StartGame(new StartGameArgs
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
