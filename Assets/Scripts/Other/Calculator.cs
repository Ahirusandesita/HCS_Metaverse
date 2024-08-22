using UnityEngine;

public static class Calculator
{
    /// <summary>
    /// 指定したベクトルの方向を向く回転を取得
    /// </summary>
    /// <param name="direction">変換するベクトル</param>
    /// <param name="axis">回転の軸</param>
    /// <param name="correctionToFront">角度の補正値（正面を設定）</param>
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
    /// 指定したベクトルの方向を向く
    /// </summary>
    /// <param name="targetDir">変換するベクトル</param>
    /// <param name="axis">回転の軸</param>
    /// <param name="correctionToFront">角度の補正値（正面を設定）</param>
    public static void RotateBy2DVector(this Transform transform, Vector2 targetDir, Vector3 axis, float correctionToFront = 90f)
    {
        if (targetDir == Vector2.zero)
        {
            return;
        }

        // 指定した方向を向く
        Vector3 euler = axis * (Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - correctionToFront);
        transform.rotation = Quaternion.Euler(euler);
    }

    /// <summary>
    /// 指定したベクトルの方向を向く
    /// </summary>
    /// <param name="targetDir">変換するベクトル</param>
    /// <param name="axis">回転の軸</param>
    /// <param name="correctionToFront">角度の補正値（正面を設定）</param>
    public static void LocalRotateBy2DVector(this Transform transform, Vector2 targetDir, Vector3 axis, float correctionToFront = 90f)
    {
        if (targetDir == Vector2.zero)
        {
            return;
        }

        // 指定した方向を向く
        Vector3 euler = axis * (Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - correctionToFront);
        transform.localRotation = Quaternion.Euler(euler);
    }
}