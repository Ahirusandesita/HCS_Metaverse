using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 投擲するために必要な情報をまとめたクラス
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

    // 投擲オブジェクトがつかまれているときの軌道座標たちorbit
    private Vector3[] _throwObjectOrbitPositions = new Vector3[3];

    /// <summary>
    /// 新しい軌道座標を保存するためのプロパティ
    /// </summary>
    /// <param name="newPosition">新しい軌道座標</param>
    public void SetOrbitPosition(Vector3 newPosition)
    {
        // 保存してある座標の保存位置を更新する
        _throwObjectOrbitPositions[2] = _throwObjectOrbitPositions[1];
        _throwObjectOrbitPositions[1] = _throwObjectOrbitPositions[0];

        // 新しい座標を保存する
        _throwObjectOrbitPositions[0] = newPosition;
    }

    public void GetThrowVelocity()
    {
        // 保存してある軌道座標から投擲用ベクトルを作成する --------------------------------
        // 一つ目
        Vector3 firstVector = _throwObjectOrbitPositions[0] - _throwObjectOrbitPositions[1];

        // 二つ目
        Vector3 secondVector = _throwObjectOrbitPositions[1] - _throwObjectOrbitPositions[2];
        // ---------------------------------------------------------------------------------

        // 投擲用ベクトルのノルムの平均から投擲速度を作成する
        float magnitude = (firstVector.magnitude + secondVector.magnitude) / 2;


    }
}
