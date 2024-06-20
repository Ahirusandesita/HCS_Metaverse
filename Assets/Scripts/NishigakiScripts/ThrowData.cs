using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 投擲するために必要な情報をまとめたクラス
/// </summary>
public class ThrowData
{
    #region コンストラクタ
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
    #endregion

    #region 変数・定数
    // 速度係数　投擲速度 を オブジェクトが運動する際の速度 に変換するために使用します
    private const float VELOCITY_COFFICIENT = 5f;

    // 投擲オブジェクトがつかまれているときの軌道座標たちorbit
    private Vector3[] _throwObjectOrbitPositions = new Vector3[10];
    #endregion

    #region メソッド・プロパティ
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

    /// <summary>
    /// 投擲ベクトルを取得するためのプロパティ
    /// </summary>
    /// <returns>投擲ベクトル</returns>
    public Vector3 GetThrowVector()
    {
        // 軌道ベクトル 軌道座標の差から求められる
        Vector3 orbitVector = default;

        // 投擲速度　軌道ベクトルのノルムの平均をもとに決められる
        float throwVelocity = default;

        // 軌道ベクトルの個数　( 軌道座標の個数 - 1 )個
        int maxOrbitIndex = _throwObjectOrbitPositions.Length - 1; 

        // 保存してある軌道座標から軌道ベクトルを作成する --------------------------------
        for (int positionsIndex = 0; positionsIndex < maxOrbitIndex; positionsIndex++)
        {
            // 軌道座標の差を求める
            Vector3 positionDifference = _throwObjectOrbitPositions[positionsIndex] - _throwObjectOrbitPositions[positionsIndex + 1];

            // 軌道座標の差を加算する
            orbitVector += positionDifference;

            // 軌道ベクトルのノルムを投擲速度に加算する
            throwVelocity += positionDifference.magnitude;
        }
        // ---------------------------------------------------------------------------------

        // 軌道ベクトルを正規化する
        orbitVector = orbitVector.normalized;

        // 投擲速度を ノルムの合計 から ノルムの平均 に変換する
        throwVelocity /= maxOrbitIndex;

        Debug.Log($"<color=blue>ぽいベクトル{orbitVector.ToString("F6")} , ぽいスピード{throwVelocity.ToString("F8")} , ぽいノルム{(orbitVector * throwVelocity).magnitude.ToString("F8")}</color>");

        // 軌道ベクトルと投擲速度を掛け合わせた 投擲ベクトル を生成して値を返す
        return orbitVector * throwVelocity * VELOCITY_COFFICIENT;
    }
    #endregion
}
