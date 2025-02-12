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

	[SerializeField]
    private AnimationSelecter _animationSelecter;

    private Transform _playerTransform;
	private Transform _viewTransform;
	private Vector2 _inputDirection;
	private float _footstepsInterval;
	private const float FOOTSTEPS_INTERVAL = 0.8f;
	private PlayerBodyDependencyInformation _information;
	private PlayerSE _playerSE;

	public override void Spawned()
	{
		Debug.Log($"Spawnd:RemoteView");
		base.Spawned();
		_playerTransform = FindObjectOfType<VRPlayerController>().transform;
		_viewTransform = transform;

		_playerSE = GetComponentInChildren<PlayerSE>();

		Inputter.Player.Move.performed += dir =>
		{
			_inputDirection = dir.ReadValue<Vector2>();
		};

		Inputter.Player.Move.canceled += dir =>
		{
			_inputDirection = Vector2.zero;
		};

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

		if (!GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient) return;

		_footstepsInterval -= Time.deltaTime;

		if (_footstepsInterval > 0) return;

        if (_inputDirection != Vector2.zero)
        {
			_playerSE.PlayFootStep();
			_footstepsInterval = FOOTSTEPS_INTERVAL;
		}
    }

    public void Inject(PlayerBodyDependencyInformation information)
    {
		this._information = information;
    }

	public AvatarHandTracker GetNewAvatarHandTracker()
    {
		return new AvatarHandTracker(_rightShoulder, _rightHand, _leftShoulder, _leftHand, _animationSelecter);
	}
}
