using UnityEngine;

public static class Calculator
{
    /// <summary>
    /// �w�肵���x�N�g���̕�����������]���擾
    /// </summary>
    /// <param name="direction">�ϊ�����x�N�g��</param>
    /// <param name="axis">��]�̎�</param>
    /// <param name="correctionToFront">�p�x�̕␳�l�i���ʂ�ݒ�j</param>
    /// <returns></returns>
    public static Vector3 GetEulerBy2DVector(Vector2 direction, Vector3 axis, float correctionToFront = 90f)
    {
        if (direction == Vector2.zero)
        {
            return Vector3.zero;
        }

        return axis * (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - correctionToFront);
    }

    /// <summary>
    /// �w�肵���x�N�g���̕���������
    /// </summary>
    /// <param name="targetDir">�ϊ�����x�N�g��</param>
    /// <param name="axis">��]�̎�</param>
    /// <param name="correctionToFront">�p�x�̕␳�l�i���ʂ�ݒ�j</param>
    public static void RotateBy2DVector(this Transform transform, Vector2 targetDir, Vector3 axis, float correctionToFront = 90f)
    {
        if (targetDir == Vector2.zero)
        {
            return;
        }

        // �w�肵������������
        Vector3 euler = axis * (Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - correctionToFront);
        transform.rotation = Quaternion.Euler(euler);
    }

    /// <summary>
    /// �w�肵���x�N�g���̕���������
    /// </summary>
    /// <param name="targetDir">�ϊ�����x�N�g��</param>
    /// <param name="axis">��]�̎�</param>
    /// <param name="correctionToFront">�p�x�̕␳�l�i���ʂ�ݒ�j</param>
    public static void LocalRotateBy2DVector(this Transform transform, Vector2 targetDir, Vector3 axis, float correctionToFront = 90f)
    {
        if (targetDir == Vector2.zero)
        {
            return;
        }

        // �w�肵������������
        Vector3 euler = axis * (Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - correctionToFront);
        transform.localRotation = Quaternion.Euler(euler);
    }
}