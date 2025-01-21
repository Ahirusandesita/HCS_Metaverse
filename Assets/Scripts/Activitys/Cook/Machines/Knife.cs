using Fusion;
using UnityEngine;
using Oculus.Interaction;

public class Knife : NetworkBehaviour
{
    private bool _isGrab = false;

    public bool IsGrab => _isGrab;

    private LocalView _localView = default;

    public LocalView LocalView => _localView;

    private PointableUnityEventWrapper _pointableUnityEventWrapper;
    

    private void Start()
    {
        // 
        _localView = GetComponent<LocalView>();

        _pointableUnityEventWrapper.WhenSelect.AddListener((data) => _localView.Grab());
        _pointableUnityEventWrapper.WhenUnselect.AddListener((data) => _localView.Release());

        _pointableUnityEventWrapper.WhenSelect.AddListener((data) => _isGrab = true);
        _pointableUnityEventWrapper.WhenUnhover.AddListener((data) => _isGrab = false);
    }

    private void FixedUpdate()
    {
        if (_isGrab)
        {
            _localView.NetworkView.RPC_Position(this.transform.position, this.transform.rotation.eulerAngles);
        }
    }
}