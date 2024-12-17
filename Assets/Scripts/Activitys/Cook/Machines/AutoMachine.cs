using System;
using UnityEngine;
using Fusion;
using Oculus.Interaction;

public class AutoMachine : Machine, IObjectLocker
{
    [SerializeField, Tooltip("�s�����H�̎��")]
    private ProcessingType _processingType = default;

    [SerializeField, Tooltip("�I�u�W�F�N�g�̎擾�͈͂��w�肷��Collider")]
    private Collider _cuttingAreaCollider = default;

    // 
    private IngrodientCatcher _ingrodientCatcher = default;

    // �Œ肵�Ă���I�u�W�F�N�g��Puttable
    private Puttable _processingPuttable = default;

    // �͂񂾎��◣�������ɃC�x���g�����s����N���X
    private PointableUnityEventWrapper _pointableUnityEventWrapper;

    // 
    public Transform GetObjectLockTransform => _machineTransform;

    protected override void Start()
    {
        // �e�̏��������s����
        base.Start();

        // IngrodientCatcher�̃C���X�^���X�𐶐�����
        _ingrodientCatcher = new IngrodientCatcher();
    }

    protected override void Update()
    {
        base.Update();

        // processingIngrodient��ݒ肷��
        ProcessingIngrodientSetting();

        // ��ɉ��H��i�߂Ă���
        //bool isEndProcessing = ProcessingAction(_processingType, Time.deltaTime);
    }

    public void Select()
    {
        RPC_Select();
    }

    [Rpc]
    private void RPC_Select() //RPC
    {
        // 
        if (_processingPuttable is not null)
        {
            // 
            _processingPuttable.DestroyThis();
        }
    }

    private void ProcessingIngrodientSetting()
    {
        // ���ł�processingIngrodient���ݒ肳��Ă����ꍇ
        //if (_processingIngrodient != default)
        //{
        //    // �������Ȃ�
        //    return;
        //}

        // �w�肵������ɐڐG����Ingrodient���������ꍇ
        if (_ingrodientCatcher.SearchIngrodient(_cuttingAreaCollider.bounds.center, _cuttingAreaCollider.bounds.size / 2, transform.rotation, out NetworkObject hitObject))
        {
            // processingIngrodient��ݒ肷��
            //RPC_HitIngrodients(hitObject);

            // �������I������
            return;
        }
    }

    /// <summary>
    /// �H�ނɓ��������Ƃ��̏���
    /// </summary>
    /// <param name="hitObject">���������H�ނ�NetworkObject</param>
    [Rpc]
    private void RPC_HitIngrodients(GameObject hitObject) // RPC
    {
        // 
        

        // ���������I�u�W�F�N�g��Ingrodient���擾����
        //_processingIngrodient = hitObject.GetComponent<Ingrodients>();

        // ���������I�u�W�F�N�g��Puttable��ǉ����Ď擾����
        _processingPuttable = hitObject.gameObject.AddComponent<Puttable>();

        // Puttable�Ɏ��g��n��
        _processingPuttable.SetLockedCuttingObject(this);

        // 
        _pointableUnityEventWrapper = hitObject.GetComponentInChildren<PointableUnityEventWrapper>();
        _pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });

        Debug.Log($"<color=green>{gameObject.name}</color>�@���@<color=blue>{hitObject.name}</color>�@���Œ�");
    }

    public void CanselLock()
    {
        // processingIngrodient������������
        //_processingIngrodient = default;

        // Select�̓o�^����������
        _pointableUnityEventWrapper.WhenSelect.RemoveListener((action) => { Select(); });
    }
}