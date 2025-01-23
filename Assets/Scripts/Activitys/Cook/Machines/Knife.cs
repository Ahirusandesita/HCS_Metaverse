using Fusion;
using UnityEngine;
using Oculus.Interaction;

public class Knife : NetworkBehaviour
{
    private bool _isGrab = false;

    public bool IsGrab => _isGrab;

    [SerializeField]
    private Transform _knifeOrigin = default;

    private LocalView _localView = default;

    public LocalView LocalView => _localView;

    [SerializeField]
    private NetworkView _networkView = default;

    private PointableUnityEventWrapper _pointableUnityEventWrapper;
    

    private void Start()
    {
        // 
        _localView = GetComponent<LocalView>();
        _localView.NetworkViewInject(_networkView);

        // 
        _pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();

        // 
        _pointableUnityEventWrapper.WhenSelect.AddListener((data) => _localView.Grab());
        _pointableUnityEventWrapper.WhenUnselect.AddListener((data) => _localView.Release());
        _pointableUnityEventWrapper.WhenUnselect.AddListener((data) => UnSelect());
        _pointableUnityEventWrapper.WhenSelect.AddListener((data) => _isGrab = true);
        _pointableUnityEventWrapper.WhenUnselect.AddListener((data) => _isGrab = false);
    }

    private void FixedUpdate()
    {
        if (_isGrab)
        {
            _localView.NetworkView.RPC_Position(this.transform.position, this.transform.rotation.eulerAngles);
        }
    }

    private void UnSelect()
    {
        _localView.NetworkView.GetComponent<NetworkKnife>().RPC_UnSelect();
    }

    public void RPC_UnSelect()
    {
        transform.position = _knifeOrigin.position;
        transform.rotation = _knifeOrigin.rotation;
    }
}