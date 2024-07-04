using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeStopper : MonoBehaviour, IDependencyInjector<PlayerVisualHandDependencyInformation>
{
    [SerializeField, Tooltip("停止させる見た目用オブジェクト")]
    private GameObject _visualObject = default;

    [SerializeField, Tooltip("接触する対象のコライダー")]
    private Collider _targetCollider = default;

    // 
    private PlayerVisualHandDependencyInformation _handVisualInformation = default;

    // 
    private bool _isGrabbing = false;

    // ロックするTransform群 ---------------------------------
    private Transform _visualObjectTransform = default;

    private Transform _visualHandTransform = default;

    private Transform _visualControllerTransform = default;

    private Transform _visualControllerHandTransform = default;
    // ------------------------------------------------------

    // ロックする座標群と角度群 --------------------------------
    private Vector3 _visualObjectPosition = default;
    private Quaternion _visualObjectRotation = default;

    private Vector3 _visualHandPosition = default;
    private Quaternion _visualHandRotation = default;

    private Vector3 _visualControllerPosition = default;
    private Quaternion _visualControllerRotation = default;

    private Vector3 _visualControllerHandPosition = default;
    private Quaternion _visualControllerHandRotation = default;
    // ------------------------------------------------------

    private void Start()
    {
        
    }

    private void Update()
    {
        if (_isGrabbing)
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == _targetCollider)
        {
            // 
            _isGrabbing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == _targetCollider)
        {
            // 
            _isGrabbing = false;
        }
    }

    private void Select()
    {

    }

    public void Inject(PlayerVisualHandDependencyInformation information)
    {
        // 手のView情報を取得しておく
        _handVisualInformation = information;
    }
}
