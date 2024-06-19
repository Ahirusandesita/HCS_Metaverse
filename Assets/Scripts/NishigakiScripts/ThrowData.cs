using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 投擲するために必要な情報をまとめたクラス
/// </summary>
public class ThrowData
{
    // 速度係数　投擲速度 を オブジェクトが運動する際の速度 に変換するために使用します
    private const float VELOCITY_COFFICIENT = 1f;

    /// <summary>
    /// 投擲するために必要な情報をまとめたクラス
    /// </summary>
    /// <param name="nowPosition">現在のPosition</param>
    public ThrowData(Vector3 nowPosition)
    {
        // 軌道座標の初期化を行う
        for (int positionNumber = 0; positionNumber < _throwObjectOrbitPositions.Length; positionNumber++)
        {
            // 軌道座標の初期値を設定する
            _throwObjectOrbitPositions[positionNumber] = nowPosition;
        }
    }

    // 投擲オブジェクトがつかまれているときの軌道座標たちorbit
    private Vector3[] _throwObjectOrbitPositions = new Vector3[6];

    /// <summary>
    /// 新しい軌道座標を保存するためのプロパティ
    /// </summary>
    /// <param name="newPosition">新しい軌道座標</param>
    public void SetOrbitPosition(Vector3 newPosition)
    {
        // 保存してある座標の保存位置を更新する
        for (int beforeIndex = 0; beforeIndex < _throwObjectOrbitPositions.Length - 1; beforeIndex++)
        {
            // 一つ後ろに移していく
            _throwObjectOrbitPositions[beforeIndex + 1] = _throwObjectOrbitPositions[beforeIndex];
        }

        // 新しい座標を保存する
        _throwObjectOrbitPositions[0] = newPosition;
    }

    public Vector3 GetThrowVector()
    {
        // 保存してある軌道座標から投擲ベクトルを作成する --------------------------------
        // 軌道ベクトル 軌道座標の差から求められる
        Vector3 orbitVectors = default;

        // 投擲速度　軌道ベクトルのノルムの平均をもとに決められる
        float throwVelocity = default;

        // 軌道ベクトルの個数　( 軌道座標の個数 - 1 )個
        int maxOrbitIndex = _throwObjectOrbitPositions.Length - 1; 

        // 軌道ベクトルを設定する
        for (int positionsIndex = 0; positionsIndex < maxOrbitIndex; positionsIndex++)
        {
            // 軌道座標の差を求める
            Vector3 positionDifference = _throwObjectOrbitPositions[positionsIndex] - _throwObjectOrbitPositions[positionsIndex - 1];

            // 軌道座標の差を加算する
            orbitVectors += positionDifference;

            // 軌道ベクトルのノルムを投擲速度に加算する
            throwVelocity += positionDifference.magnitude;
        }
        // ---------------------------------------------------------------------------------

        // 軌道ベクトルを正規化する
        orbitVectors = orbitVectors.normalized;

        // 投擲速度を ノルムの合計 から ノルムの平均 に変換する
        throwVelocity /= maxOrbitIndex;

        // 軌道ベクトルと投擲速度を掛け合わせた 投擲ベクトル を生成して値を返す
        return orbitVectors * throwVelocity * VELOCITY_COFFICIENT;
    }
}
