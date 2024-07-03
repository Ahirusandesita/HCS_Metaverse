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
	/// アクティビティを開始する
	/// </summary>
	/// <param name="sceneName">開始するアクティビティのシーン名</param>
	public void ActivityStart(string sceneName)
	{
		if (!_networkRunner.IsServer) { return; }

		_rpcManager.Rpc_SessionNaming("dadadad");
		Debug.LogWarning("ActivityStart");
		_networkRunner.LoadScene(sceneName);
	}

	/// <summary>
	/// ルームに入る。ない場合は作る
	/// </summary>
	/// <param name="sessionName">セッション名</param>
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
