using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimedThrower : MonoBehaviour
{
    #region コンストラクタ
    /// <summary>
    /// 補正投擲を行うためのメソッド
    /// </summary>
    /// <param name="pointerTransform">ポインターのTransform</param>
    /// <param name="throwPointerType">ポインターの指定方法</param>
    public AimedThrower(Transform pointerTransform, ThrowPointerType throwPointerType)
    {
        // ポインターのTransformを設定
        _aimPointer = pointerTransform;

        // ポインターの指定方法を設定
        _pointerType = throwPointerType;
    }
    #endregion

    #region 変数・プロパティ
    [SerializeField, Tooltip("狙う方向を示すポインターのTransform")]
    private Transform _aimPointer = default;

    [SerializeField, Tooltip("ポインターの指定方法\nポインターが角度を示すならDirection\nポインターが座標を示すならTarget")]
    private ThrowPointerType _pointerType = default;

    /// <summary>
    /// 投擲補正のためのベクトルを取得するためのプロパティ
    /// </summary>
    public Vector3 GetAimVector
    { 
        get 
        {
            // ポインターの指定方法ごとに分岐
            switch (_pointerType)
            {
                case ThrowPointerType.Direction:
                    // ポインターの正面方向のベクトルを返す
                    return _aimPointer.forward;

                case ThrowPointerType.Target:
                    // ポインターの座標に向かうベクトルを返す
                    return (_aimPointer.position - this.transform.position).normalized;

                default:
                    // 例外
                    Debug.LogError($"AimedThrowerのGetAimVectorで異常：_pointerTypeが指定外");
                    return Vector3.zero;
            }
        } 
    }

    /// <summary>
    /// 狙う方向を示すポインターの指定方法
    /// </summary>
    public enum ThrowPointerType
    {
        Direction,
        Target
    }
    #endregion
}
