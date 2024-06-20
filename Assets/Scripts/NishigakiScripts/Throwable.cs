using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField, Tooltip("自身が持つRigidbody")]
    public Rigidbody _thisRigidbody = default;

    [SerializeField, Tooltip("自身が持つTransform")]
    public Transform _thisTransform = default;

    // 使用中のThrowDataを格納するための変数
    public ThrowData _throwData = default;

    private void Awake()
    {
        // ThrowDataを生成する
        _throwData = new ThrowData(_thisTransform.position);
    }

    private void FixedUpdate()
    {
        // つかんでいる時のみ実行する
        if (_throwData is null)
        {
            // つかんでいなかったら何もしない
            return;
        }

        // 現在の座標を保存する
        _throwData.SetOrbitPosition(_thisTransform.position);
    }

    /// <summary>
    /// つかまれたときに実行する処理
    /// </summary>
    public void Select()
    {
        // Kinematicを有効にする
        _thisRigidbody.isKinematic = true;

        // ThrowDataを生成
        _throwData = new ThrowData(_thisTransform.position);
    }

    /// <summary>
    /// 離されたときに実行する処理
    /// </summary>
    public void UnSelect()
    {
        // Kinematicを無効にする
        _thisRigidbody.isKinematic = false;

        // 投擲ベクトルを取得する
        Vector3 throwVector = _throwData.GetThrowVector();

        // 投擲ベクトルを速度に代入する
        _thisRigidbody.velocity = throwVector;

        Debug.Log($"ぽいべろしちー{_thisRigidbody.velocity.ToString("F6")}");

        // 使い終わったThrowDataを消す
        _throwData = null;
    }
}
