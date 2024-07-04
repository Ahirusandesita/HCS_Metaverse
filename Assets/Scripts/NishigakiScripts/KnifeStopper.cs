using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeStopper : MonoBehaviour, IDependencyInjector<PlayerVisualHandDependencyInformation>
{
    [SerializeField, Tooltip("��~�����錩���ڗp�I�u�W�F�N�g")]
    private GameObject _visualObject = default;

    [SerializeField, Tooltip("�ڐG����Ώۂ̃R���C�_�[")]
    private Collider _targetCollider = default;

    // 
    private PlayerVisualHandDependencyInformation _handVisualInformation = default;

    // 
    private bool _isGrabbing = false;

    // ���b�N����Transform�Q ---------------------------------
    private Transform _visualObjectTransform = default;

    private Transform _visualHandTransform = default;

    private Transform _visualControllerTransform = default;

    private Transform _visualControllerHandTransform = default;
    // ------------------------------------------------------

    // ���b�N������W�Q�Ɗp�x�Q --------------------------------
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
        // ���View�����擾���Ă���
        _handVisualInformation = information;
    }
}
