using Fusion;
using UnityEngine;
using Cysharp.Threading.Tasks;

public delegate void SessionNameChanged(string name);

public class RPCManager : NetworkBehaviour
{
	public event SessionNameChanged SessionNameChangedHandler;

	private static RPCManager _instance;
	public static RPCManager Instance { get => _instance; }

	private void Awake()
	{
		if (_instance is null)
		{
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// �����Z�b�V�����S���̃Z�b�V���������X�V����
	/// </summary>
	/// <param name="sessionName">�ύX��̃Z�b�V������</param>
	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public void Rpc_SessionNaming(string sessionName)
	{
		Debug.LogWarning("RpcExecute");

		//���s
		SessionNameChangedHandler?.Invoke(sessionName);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_ReleaseStateAuthority(NetworkObject networkObject, [RpcTarget] PlayerRef player)
	{		
		networkObject.ReleaseStateAuthority();
	}
}
