using System.Collections;
using UnityEngine;
using Oculus.Interaction;
using Fusion;

public class Throwable : MonoBehaviour
{
    [SerializeField, Tooltip("���g������Rigidbody")]
    public Rigidbody _thisRigidbody = default;

    [SerializeField, Tooltip("���g������Transform")]
    public Transform _thisTransform = default;

    [SerializeField, Tooltip("���x�W��")]
    private float _velocityCoefficient = 1f;

    // 
    private LocalView _localView = default;

    // �g�p����ThrowData���i�[���邽�߂̕ϐ�
    public ThrowData _throwData = default;

    // �I�u�W�F�N�g��͂�ł��邩�ǂ����̔���p�ϐ�
    private bool _isSelected = default;

    // �͂񂾎��◣�������ɃC�x���g�����s����N���X
    private PointableUnityEventWrapper pointableUnityEventWrapper;

    private void Start()
    {
        // ���g��LocalView���擾����
        _localView = GetComponent<LocalView>();

        // ThrowData�𐶐�����
        _throwData = new ThrowData(_thisTransform.position);

        // VR��Event�ɏ�����o�^����
        pointableUnityEventWrapper = this.GetComponent<PointableUnityEventWrapper>();
        pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });
        pointableUnityEventWrapper.WhenUnselect.AddListener((action) => { UnSelect(); });
    }

    private void FixedUpdate()
    {
        // �ǂ���̎�ł��͂�ł��Ȃ��ꍇ
        if (!_isSelected)
        {
            // �������Ȃ�
            return;
        }

        // ���݂̍��W��ۑ�����
        _throwData.SetOrbitPosition(_thisTransform.position);
    }

    /// <summary>
    /// ���܂ꂽ�Ƃ��Ɏ��s���郁�\�b�h
    /// </summary>
    public void Select()
    {
        // ���̏��������s��
        _throwData.ReSetThrowData(_thisTransform.position);

        // �͂�ł����Ԃɂ���
        _isSelected = true;
    }

    /// <summary>
    /// �����ꂽ�Ƃ��Ɏ��s���郁�\�b�h
    /// </summary>
    public void UnSelect()
    {
        // �����x�N�g�����擾����
        Vector3 throwVector = _throwData.GetThrowVector() * _velocityCoefficient;

        // NetworkView�ɏ����̎��s���˗�����
        _localView.NetworkView.GetComponent<LocalThrow>().ThrowAllLocalView(throwVector);

        // �����Ă����Ԃɂ���
        _isSelected = false;
    }

    /// <summary>
    /// ���[�J���œ����������s�����߂�Rpc���\�b�h
    /// </summary>
    /// <param name="throwVector">�����x�N�g��</param>
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_Throw(Vector3 throwVector)
    {
        // 1�t���[����Ƀx�N�g�����㏑������
        StartCoroutine(OverwriteVelocity(throwVector));
    }

    /// <summary>
    /// �������x���㏑�����邽�߂̃R���[�`��
    /// </summary>
    /// <param name="throwVector">�������x</param>
    /// <returns></returns>
    private IEnumerator OverwriteVelocity(Vector3 throwVector)
    {
        // 1�t���[���ҋ@����@1�t���[���ҋ@���Ȃ���OVR�ɏ������
        yield return new WaitForEndOfFrame();

        // Kinematic�𖳌��ɂ���
        _thisRigidbody.isKinematic = false;

        // �����x�N�g���𑬓x�ɏ㏑������
        _thisRigidbody.velocity = throwVector;
    }
}
