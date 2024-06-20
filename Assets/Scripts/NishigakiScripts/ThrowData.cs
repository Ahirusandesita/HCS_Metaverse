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
        for (int positionNumber = 0; positionNumber < _throwObjectOrbitPositions.Length; positionNumber++)
        {
            // �O�����W�̏����l��ݒ肷��
            _throwObjectOrbitPositions[positionNumber] = nowPosition;
        }
    }
    #endregion

    #region �ϐ��E�萔
    // ���x�W���@�������x �� �I�u�W�F�N�g���^������ۂ̑��x �ɕϊ����邽�߂Ɏg�p���܂�
    private const float VELOCITY_COFFICIENT = 5f;

    // �����I�u�W�F�N�g�����܂�Ă���Ƃ��̋O�����W����orbit
    private Vector3[] _throwObjectOrbitPositions = new Vector3[10];
    #endregion

    #region ���\�b�h�E�v���p�e�B
    /// <summary>
    /// �V�����O�����W��ۑ����邽�߂̃v���p�e�B
    /// </summary>
    /// <param name="newPosition">�V�����O�����W</param>
    public void SetOrbitPosition(Vector3 newPosition)
    {
        // �ۑ����Ă�����W�̕ۑ��ʒu���X�V����
        for (int beforeIndex = 0; beforeIndex < _throwObjectOrbitPositions.Length - 1; beforeIndex++)
        {
            // ����Ɉڂ��Ă���
            _throwObjectOrbitPositions[beforeIndex + 1] = _throwObjectOrbitPositions[beforeIndex];
        }

        // �V�������W��ۑ�����
        _throwObjectOrbitPositions[0] = newPosition;
    }

    /// <summary>
    /// �����x�N�g�����擾���邽�߂̃v���p�e�B
    /// </summary>
    /// <returns>�����x�N�g��</returns>
    public Vector3 GetThrowVector()
    {
        // �O���x�N�g�� �O�����W�̍����狁�߂���
        Vector3 orbitVector = default;

        // �������x�@�O���x�N�g���̃m�����̕��ς����ƂɌ��߂���
        float throwVelocity = default;

        // �O���x�N�g���̌��@( �O�����W�̌� - 1 )��
        int maxOrbitIndex = _throwObjectOrbitPositions.Length - 1; 

        // �ۑ����Ă���O�����W����O���x�N�g�����쐬���� --------------------------------
        for (int positionsIndex = 0; positionsIndex < maxOrbitIndex; positionsIndex++)
        {
            // �O�����W�̍������߂�
            Vector3 positionDifference = _throwObjectOrbitPositions[positionsIndex] - _throwObjectOrbitPositions[positionsIndex + 1];

            // �O�����W�̍������Z����
            orbitVector += positionDifference;

            // �O���x�N�g���̃m�����𓊝����x�ɉ��Z����
            throwVelocity += positionDifference.magnitude;
        }
        // ---------------------------------------------------------------------------------

        // �O���x�N�g���𐳋K������
        orbitVector = orbitVector.normalized;

        // �������x�� �m�����̍��v ���� �m�����̕��� �ɕϊ�����
        throwVelocity /= maxOrbitIndex;

        Debug.Log($"<color=blue>�ۂ��x�N�g��{orbitVector.ToString("F6")} , �ۂ��X�s�[�h{throwVelocity.ToString("F8")} , �ۂ��m����{(orbitVector * throwVelocity).magnitude.ToString("F8")}</color>");

        // �O���x�N�g���Ɠ������x���|�����킹�� �����x�N�g�� �𐶐����Ēl��Ԃ�
        return orbitVector * throwVelocity * VELOCITY_COFFICIENT;
    }
    #endregion
}
