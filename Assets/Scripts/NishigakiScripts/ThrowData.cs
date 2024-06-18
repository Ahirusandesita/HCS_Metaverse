using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������邽�߂ɕK�v�ȏ����܂Ƃ߂��N���X
/// </summary>
public class ThrowData
{
    public ThrowData(Vector3 nowPosition)
    {
        // 
        _throwObjectOrbitPositions[0] = nowPosition;
        _throwObjectOrbitPositions[1] = nowPosition;
        _throwObjectOrbitPositions[2] = nowPosition;
    }

    // �����I�u�W�F�N�g�����܂�Ă���Ƃ��̋O�����W����orbit
    private Vector3[] _throwObjectOrbitPositions = new Vector3[3];

    /// <summary>
    /// �V�����O�����W��ۑ����邽�߂̃v���p�e�B
    /// </summary>
    /// <param name="newPosition">�V�����O�����W</param>
    public void SetOrbitPosition(Vector3 newPosition)
    {
        // �ۑ����Ă�����W�̕ۑ��ʒu���X�V����
        _throwObjectOrbitPositions[2] = _throwObjectOrbitPositions[1];
        _throwObjectOrbitPositions[1] = _throwObjectOrbitPositions[0];

        // �V�������W��ۑ�����
        _throwObjectOrbitPositions[0] = newPosition;
    }

    public void GetThrowVelocity()
    {
        // �ۑ����Ă���O�����W���瓊���p�x�N�g�����쐬���� --------------------------------
        // ���
        Vector3 firstVector = _throwObjectOrbitPositions[0] - _throwObjectOrbitPositions[1];

        // ���
        Vector3 secondVector = _throwObjectOrbitPositions[1] - _throwObjectOrbitPositions[2];
        // ---------------------------------------------------------------------------------

        // �����p�x�N�g���̃m�����̕��ς��瓊�����x���쐬����
        float magnitude = (firstVector.magnitude + secondVector.magnitude) / 2;


    }
}
