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
        for (int positionNumber = 0; positionNumber < _orbitDatas.Length; positionNumber++)
        {
            // 軌道座標の初期値を設定する
            _orbitDatas[positionNumber]._orbitPosition = nowPosition;
        }
    }
    #endregion

    #region 変数・定数
    /// <summary>
    /// 軌道ベクトルの生成に必要な情報をまとめた構造体
    /// </summary>
    private struct OrbitData
    {
        /// <summary>
        /// 軌道座標
        /// </summary>
        public Vector3 _orbitPosition;

        /// <summary>
        /// 保存時刻
        /// </summary>
        public float _storeTime;
    }

    // 失効時刻　軌道ベクトルの生成に使用できる情報の期限　期限切れは使わない
    private const float REVOCATION_TIME = 0.2f;

    // 軌道ベクトルの生成に必要な情報たち　軌道座標と保存時刻を持つ
    private OrbitData[] _orbitDatas = new OrbitData[9];
    #endregion

    #region メソッド・プロパティ
    /// <summary>
    /// 投擲を行うために必要な情報を初期化するためのメソッド
    /// </summary>
    /// <param name="nowPosition"></param>
    public void ReSetThrowData(Vector3 nowPosition)
    {
        // 軌道座標の初期化を行う
        for (int positionNumber = 0; positionNumber < _orbitDatas.Length; positionNumber++)
        {
            // 軌道座標の初期値を設定する
            _orbitDatas[positionNumber]._orbitPosition = nowPosition;

            // 保存時刻を初期化する
            _orbitDatas[positionNumber]._storeTime = default;
        }
    }

    /// <summary>
    /// 新しい軌道座標を保存するためのプロパティ
    /// </summary>
    /// <param name="newPosition">新しい軌道座標</param>
    public void SetOrbitPosition(Vector3 newPosition)
    {
        // 保存してある情報の保存位置を更新する
        for (int beforeIndex = 0; beforeIndex < _orbitDatas.Length - 1; beforeIndex++)
        {
            // 一つ後ろに移していく
            _orbitDatas[beforeIndex + 1] = _orbitDatas[beforeIndex];
        }

        // 新しい座標を保存する
        _orbitDatas[0]._orbitPosition = newPosition;

        // 新しい保存時刻を記録する
        _orbitDatas[0]._storeTime = Time.time;
    }

    /// <summary>
    /// 投擲ベクトルを取得するためのプロパティ
    /// </summary>
    /// <returns>投擲ベクトル</returns>
    public Vector3 GetThrowVector()
    {
        // 軌道ベクトルの生成に使用可能な情報の最後の番地
        int usableIndex = GetUsableIndex();

        // 軌道ベクトル 軌道座標の差から求められる
        Vector3 orbitVector = default;

        // 投擲速度　軌道ベクトルのノルムの合計をもとに秒間速度を求める
        float throwVelocity = default;

        // 保存してある軌道座標から軌道ベクトルを作成する --------------------------------
        for (int positionsIndex = 0; positionsIndex < usableIndex; positionsIndex++)
        {
            // 軌道座標の差を求める
            Vector3 positionDifference = _orbitDatas[positionsIndex]._orbitPosition - _orbitDatas[positionsIndex + 1]._orbitPosition;

            // 軌道座標の差を加算する
            orbitVector += positionDifference;

            // 軌道ベクトルのノルムを投擲速度に加算する
            throwVelocity += positionDifference.magnitude;
        }
        // ---------------------------------------------------------------------------------

        // 軌道ベクトルを正規化する
        orbitVector = orbitVector.normalized;

        // 投擲速度を ノルムの合計 から 秒間速度 に変換する
        throwVelocity /= _orbitDatas[0]._storeTime - _orbitDatas[usableIndex]._storeTime;

        // 軌道ベクトルと投擲速度を掛け合わせた 投擲ベクトル を生成して値を返す
        return orbitVector * throwVelocity;
    }

    /// <summary>
    ///  軌道ベクトルの生成に使用可能な情報の最後の番地を取得するためのプロパティ
    /// </summary>
    /// <returns>使用可能な情報の最後の番地</returns>
    private int GetUsableIndex()
    {
        // 最後に保存した情報の保存時刻
        float rastStoreTime = _orbitDatas[0]._storeTime;

        // 軌道ベクトルの生成に使用可能な情報をまとめる　ベクトルが生成できないといけないからorbitIndexは２から加算していく
        for (int orbitIndex = 2; orbitIndex < _orbitDatas.Length; orbitIndex++)
        {
            // 失効時刻を超えていた場合
            if (REVOCATION_TIME < rastStoreTime - _orbitDatas[orbitIndex]._storeTime)
            {
                // 軌道ベクトルの生成に使用可能な情報の最後の番地を設定する
                return orbitIndex - 1;
            }
        }

        // すべての情報が失効時刻を超えていなかった場合は情報の総数を返す
        return _orbitDatas.Length - 1;
    }
    #endregion
}

