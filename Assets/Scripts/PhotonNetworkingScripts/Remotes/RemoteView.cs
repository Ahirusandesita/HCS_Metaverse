using UnityEngine;
using Fusion;

public class RemoteView : NetworkBehaviour, IDependencyInjector<PlayerBodyDependencyInformation>
{
    [SerializeField]
    private GameObject _rightShoulder;

    [SerializeField]
    private GameObject _rightHand;

    [SerializeField]
    private GameObject _leftShoulder;

    [SerializeField]
    private GameObject _leftHand;

    [SerializeField]
    private AnimationSelecter _animationSelecter;

    private Transform _playerTransform;
    private Transform _viewTransform;
    private Vector2 _inputDirection;
    private float _footstepsInterval;
    private const float FOOTSTEPS_INTERVAL = 0.8f;
    private PlayerBodyDependencyInformation _information;
    private RemoteAvatarSE _playerSE;
    private AnimationSelecter animationSelecter;


    public override void Spawned()
    {
        Debug.Log($"Spawnd:RemoteView");
        base.Spawned();
        _playerTransform = FindObjectOfType<VRPlayerController>().transform;
        _viewTransform = transform;

        _playerSE = GetComponentInChildren<RemoteAvatarSE>();

        Inputter.Player.Move.performed += dir =>
        {
            _inputDirection = dir.ReadValue<Vector2>();
        };

        Inputter.Player.Move.canceled += dir =>
        {
            _inputDirection = Vector2.zero;
        };

        PlayerInitialize.ConsignmentInject_static(this);

        animationSelecter = GetComponentInChildren<AnimationSelecter>();
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
    private void FixedUpdate()
    {
        if (_footstepsInterval > 0) return;

        if (_inputDirection != Vector2.zero)
        {
            _playerSE.RPC_PlayFootStep();
            _footstepsInterval = FOOTSTEPS_INTERVAL;
        }
    }
    void Update()
    {
        _footstepsInterval -= Time.deltaTime;
    }

    public void Inject(PlayerBodyDependencyInformation information)
    {
        this._information = information;
    }

    public AvatarHandTracker GetNewAvatarHandTracker()
    {
        return new AvatarHandTracker(_rightShoulder, _rightHand, _leftShoulder, _leftHand, _animationSelecter);
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_Walk(Vector2 direction)
    {
        Debug.LogError("Move");
        _inputDirection = direction;
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_MoveStart()
    {
        Debug.LogError("MoveStart");
        animationSelecter.StartedMove();
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_End()
    {
        Debug.LogError("MoveEnd");
        animationSelecter.EndMove();
    }
}
