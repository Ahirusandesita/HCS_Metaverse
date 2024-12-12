using UnityEngine;
using Fusion;

public class RemoteView : NetworkBehaviour,IDependencyInjector<PlayerBodyDependencyInformation>
{
	[SerializeField]
	private NetworkObject _rightShoulder;

    [SerializeField]
    private NetworkObject _rightHand;

	[SerializeField]
	private NetworkObject _leftShoulder;

    [SerializeField]
	private NetworkObject _leftHand;

	private Transform _playerTransform;
	private Transform _viewTransform;
	private PlayerBodyDependencyInformation _information;

	public override void Spawned()
	{
		Debug.Log($"Spawnd:RemoteView");
		base.Spawned();
		_playerTransform = FindObjectOfType<VRPlayerController>().transform;
		_viewTransform = transform;

		PlayerInitialize.ConsignmentInject_static(this);
	}

	public override void FixedUpdateNetwork()
	{
		base.FixedUpdateNetwork();
		Vector3 viewPosition = _information.Head.Position;
		viewPosition.y = _playerTransform.position.y;
		_viewTransform.position = viewPosition;

		Vector3 rotation = _viewTransform.rotation.eulerAngles;
        rotation.y = _information.Head.Rotation.eulerAngles.y;
        _viewTransform.rotation = Quaternion.Euler(rotation);
    }

    public void Inject(PlayerBodyDependencyInformation information)
    {
		this._information = information;
    }

	public AvatarHandTracker GetNewAvatarHandTracker()
    {
		return new AvatarHandTracker(_rightShoulder, _rightHand, _leftShoulder, _leftHand);
	}
}
