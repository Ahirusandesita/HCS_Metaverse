using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������邽�߂ɕK�v�ȏ����܂Ƃ߂��N���X
/// </summary>
public class ThrowData
{
    // ���x�W���@�������x �� �I�u�W�F�N�g���^������ۂ̑��x �ɕϊ����邽�߂Ɏg�p���܂�
    private const float VELOCITY_COFFICIENT = 1f;

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

    // �����I�u�W�F�N�g�����܂�Ă���Ƃ��̋O�����W����orbit
    private Vector3[] _throwObjectOrbitPositions = new Vector3[6];

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

    public Vector3 GetThrowVector()
    {
        // �ۑ����Ă���O�����W���瓊���x�N�g�����쐬���� --------------------------------
        // �O���x�N�g�� �O�����W�̍����狁�߂���
        Vector3 orbitVectors = default;

        // �������x�@�O���x�N�g���̃m�����̕��ς����ƂɌ��߂���
        float throwVelocity = default;

        // �O���x�N�g���̌��@( �O�����W�̌� - 1 )��
        int maxOrbitIndex = _throwObjectOrbitPositions.Length - 1; 

        // �O���x�N�g����ݒ肷��
        for (int positionsIndex = 0; positionsIndex < maxOrbitIndex; positionsIndex++)
        {
            // �O�����W�̍������߂�
            Vector3 positionDifference = _throwObjectOrbitPositions[positionsIndex] - _throwObjectOrbitPositions[positionsIndex - 1];

            // �O�����W�̍������Z����
            orbitVectors += positionDifference;

            // �O���x�N�g���̃m�����𓊝����x�ɉ��Z����
            throwVelocity += positionDifference.magnitude;
        }
        // ---------------------------------------------------------------------------------

        // �O���x�N�g���𐳋K������
        orbitVectors = orbitVectors.normalized;

        // �������x�� �m�����̍��v ���� �m�����̕��� �ɕϊ�����
        throwVelocity /= maxOrbitIndex;

        // �O���x�N�g���Ɠ������x���|�����킹�� �����x�N�g�� �𐶐����Ēl��Ԃ�
        return orbitVectors * throwVelocity * VELOCITY_COFFICIENT;
    }
}
