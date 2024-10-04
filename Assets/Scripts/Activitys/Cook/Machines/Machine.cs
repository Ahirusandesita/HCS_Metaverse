using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class Machine : NetworkBehaviour
{
    [Tooltip("加工中のIngrodient"), HideInInspector]
    public Ingrodients _processingIngrodient = default;

    [Tooltip("加工を行っている位置")]
    public Transform _machineTransform = default;

    [Tooltip("自身のNetworkObject")]
    protected NetworkObject _networkObject = default;
    
    protected virtual void Start()
    {
        // 自身のNetworkObjectを取得する
        _networkObject = gameObject.GetComponent<NetworkObject>();
    }

    /// <summary>
    /// ProcessingIngrodientの加工を行うメソッド
    /// </summary>
    /// <param name="processingType">加工の種類</param>
    /// <param name="processingValue">加工の進行量</param>
    /// <returns>加工の完了判定</returns>
    public bool ProcessingAction(ProcessingType processingType, float processingValue)
    {
        // processingIngrodientがなかった場合
        if (_processingIngrodient == default)
        {
            // 処理を終了してfalseを返す
            return false;
        }

        // ProcessingIngrodientの加工を進める
        bool isEndProcessing = _processingIngrodient.SubToIngrodientsDetailInformationsTimeItTakes(processingType, processingValue);

        // 加工が完了しているかどうか
        if (isEndProcessing)
        {
            // オブジェクトの操作権限がある場合
            if (!_processingIngrodient.GetComponent<NetworkObject>().HasStateAuthority)
            {
                // オブジェクトを変化させる
                _processingIngrodient.ProcessingStart(processingType, _machineTransform);
            }

            // processingIngrodientを初期化する
            _processingIngrodient = default;
        }

        // 加工の完了判定を返す
        return isEndProcessing;
    }

    /// <summary>
    /// ProcessingIngrodientの加工を行うメソッド out付き
    /// </summary>
    /// <param name="processingType">加工の種類</param>
    /// <param name="processingValue">加工の進行量</param>
    /// <param name="createdCommodity">完成したCommodity/param>
    /// <returns>加工の完了判定</returns>
    public bool ProcessingAction(ProcessingType processingType, float processingValue, out Commodity createdCommodity)
    {
        // createdCommodityの初期化
        createdCommodity = default;

        // processingIngrodientがなかった場合
        if (_processingIngrodient == default)
        {
            // 処理を終了してfalseを返す
            return false;
        }

        // ProcessingIngrodientの加工を進める
        bool isEndProcessing = _processingIngrodient.SubToIngrodientsDetailInformationsTimeItTakes(processingType, processingValue);

        // 加工が完了しているかどうか
        if (isEndProcessing)
        {
            // オブジェクトの操作権限がある場合
            if (!_processingIngrodient.GetComponent<NetworkObject>().HasStateAuthority)
            {
                // オブジェクトを変化させる
                _processingIngrodient.ProcessingStart(processingType, _machineTransform);
            }
            // processingIngrodientを初期化する
            _processingIngrodient = default;
        }

        // 加工の完了判定を返す
        return isEndProcessing;
    }
}