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

        Debug.Log($"<color=yellow>なうぽいすぴーど＝{_thisRigidbody.velocity}</color>");
    }

    /// <summary>
    /// つかまれたときに実行するメソッド
    /// </summary>
    public void Select()
    {
        // Kinematicを有効にする
        _thisRigidbody.isKinematic = true;

        // 情報の初期化を行う
        _throwData.ReSetThrowData(_thisTransform.position);
    }

    /// <summary>
    /// 離されたときに実行するメソッド
    /// </summary>
    public void UnSelect()
    {
        // Kinematicを無効にする
        _thisRigidbody.isKinematic = false;

        // 投擲ベクトルを取得する
        Vector3 throwVector = _throwData.GetThrowVector();

        Debug.Log($"<color=red>ぽいしたよ</color>");

        // 投擲ベクトルを速度に代入する
        _thisRigidbody.velocity = throwVector;
    }
}
