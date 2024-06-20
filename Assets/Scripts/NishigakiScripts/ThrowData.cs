using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������邽�߂ɕK�v�ȏ����܂Ƃ߂��N���X
/// </summary>
public class ThrowData
{
    #region �R���X�g���N�^
    /// <summary>
    /// �������邽�߂ɕK�v�ȏ����܂Ƃ߂��N���X
    /// </summary>
    /// <param name="nowPosition">���݂�Position</param>
    public ThrowData(Vector3 nowPosition)
    {
        // �O�����W�̏��������s��
        for (int positionNumber = 0; positionNumber < _orbitDatas.Length; positionNumber++)
        {
            // �O�����W�̏����l��ݒ肷��
            _orbitDatas[positionNumber]._orbitPosition = nowPosition;
        }
    }
    #endregion

    #region �ϐ��E�萔
    /// <summary>
    /// �O���x�N�g���̐����ɕK�v�ȏ����܂Ƃ߂��\����
    /// </summary>
    private struct OrbitData
    {
        /// <summary>
        /// �O�����W
        /// </summary>
        public Vector3 _orbitPosition;

        /// <summary>
        /// �ۑ�����
        /// </summary>
        public float _storeTime;
    }

    // ���������@�O���x�N�g���̐����Ɏg�p�ł�����̊����@�����؂�͎g��Ȃ�
    private const float REVOCATION_TIME = 0.06f;

    // ���x�W���@�������x �� �I�u�W�F�N�g���^������ۂ̑��x �ɕϊ����邽�߂Ɏg�p����
    private const float VELOCITY_COFFICIENT = 600f;

    // �O���x�N�g���̐����ɕK�v�ȏ�񂽂��@�O�����W�ƕۑ�����������
    private OrbitData[] _orbitDatas = new OrbitData[30];
    #endregion

    #region ���\�b�h�E�v���p�e�B
    /// <summary>
    /// �V�����O�����W��ۑ����邽�߂̃v���p�e�B
    /// </summary>
    /// <param name="newPosition">�V�����O�����W</param>
    public void SetOrbitPosition(Vector3 newPosition)
    {
        // �ۑ����Ă�����̕ۑ��ʒu���X�V����
        for (int beforeIndex = 0; beforeIndex < _orbitDatas.Length - 1; beforeIndex++)
        {
            // ����Ɉڂ��Ă���
            _orbitDatas[beforeIndex + 1] = _orbitDatas[beforeIndex];
        }

        // �V�������W��ۑ�����
        _orbitDatas[0]._orbitPosition = newPosition;

        // �V�����ۑ��������L�^����
        _orbitDatas[0]._storeTime = Time.time;
    }

    /// <summary>
    /// �����x�N�g�����擾���邽�߂̃v���p�e�B
    /// </summary>
    /// <returns>�����x�N�g��</returns>
    public Vector3 GetThrowVector()
    {
        // �O���x�N�g���̐����Ɏg�p�\�ȏ��̍Ō�̔Ԓn
        int usableIndex = GetUsableIndex();

        // �O���x�N�g�� �O�����W�̍����狁�߂���
        Vector3 orbitVector = default;

        // �������x�@�O���x�N�g���̃m�����̕��ς����ƂɌ��߂���
        float throwVelocity = default;

        // �ۑ����Ă���O�����W����O���x�N�g�����쐬���� --------------------------------
        for (int positionsIndex = 0; positionsIndex < usableIndex; positionsIndex++)
        {
            // �O�����W�̍������߂�
            Vector3 positionDifference = _orbitDatas[positionsIndex]._orbitPosition - _orbitDatas[positionsIndex + 1]._orbitPosition;

            // �O�����W�̍������Z����
            orbitVector += positionDifference;

            // �O���x�N�g���̃m�����𓊝����x�ɉ��Z����
            throwVelocity += positionDifference.magnitude;
        }
        // ---------------------------------------------------------------------------------

        // �O���x�N�g���𐳋K������
        orbitVector = orbitVector.normalized;

        // �������x�� �m�����̍��v ���� �m�����̕��� �ɕϊ�����
        throwVelocity /= usableIndex;

        // ���̑������Ԃ��擾����
        float totalTime = _orbitDatas[0]._storeTime - _orbitDatas[usableIndex]._storeTime;

        Debug.Log($"<color=blue>�ۂ��x�N�g��{orbitVector.ToString("F6")} , �ۂ��X�s�[�h{throwVelocity.ToString("F8")} , �ۂ��m����{(orbitVector * throwVelocity * VELOCITY_COFFICIENT).magnitude.ToString("F8")}</color>");

        // �O���x�N�g���Ɠ������x���|�����킹�� �����x�N�g�� �𐶐����Ēl��Ԃ�
        return orbitVector * throwVelocity * VELOCITY_COFFICIENT;
    }

    private int GetUsableIndex()
    {
        // �Ō�ɕۑ��������̕ۑ�����
        float rastStoreTime = _orbitDatas[0]._storeTime;

        // �O���x�N�g���̐����Ɏg�p�\�ȏ����܂Ƃ߂�@�x�N�g���������ł��Ȃ��Ƃ����Ȃ�����orbitIndex�͂Q������Z���Ă���
        for (int orbitIndex = 2; orbitIndex < _orbitDatas.Length; orbitIndex++)
        {
            // ���������𒴂��Ă����ꍇ
            if (REVOCATION_TIME < rastStoreTime - _orbitDatas[orbitIndex]._storeTime)
            {
                // �O���x�N�g���̐����Ɏg�p�\�ȏ��̍Ō�̔Ԓn��ݒ肷��
                return orbitIndex - 1;
            }
        }

        // ���ׂĂ̏�񂪎��������𒴂��Ă��Ȃ������ꍇ�͏��̑�����Ԃ�
        return _orbitDatas.Length;
    }
    #endregion
}
