using System.Collections;
using UniRx;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Throwable : MonoBehaviour, IDependencyInjector<PlayerHandDependencyInfomation>
{
    [SerializeField, Tooltip("自身が持つRigidbody")]
    public Rigidbody _thisRigidbody = default;

    [SerializeField, Tooltip("自身が持つTransform")]
    public Transform _thisTransform = default;

    // 掴んでいる手のTransform
    private Transform _rightHandTransform = default;

    // 掴んでいる手の種類
    private HandType _handType = default;

    // 使用中のThrowDataを格納するための変数
    public ThrowData _throwData = default;

    InteractorDetailEventIssuer _interactorDetailEventIssuer ;

    private void Awake()
    {
        // ThrowDataを生成する
        _throwData = new ThrowData(_thisTransform.position);
    }

    private void Start()
    {
        PlayerInitialize.ConsignmentInject_static(this);

        // 
        _interactorDetailEventIssuer.OnInteractor += (handler) => { 
            _handType = handler.HandType; 
            
        };

        // 

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
    /// つかまれたときに実行するメソッド
    /// </summary>
    public void Select()
    {
        // 
        Transform grabbingHandTransform = default;

        // 掴んだ手の種類を判断する
        if (_handType == HandType.Right)
        {
            // 右手を設定する
            grabbingHandTransform = _rightHandTransform;
        }
        else if (_handType == HandType.Left)
        {

        }
        else
        {

        }

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

        // 1フレーム後にベクトルを上書きする
        StartCoroutine(OverwriteVelocity(throwVector));
    }

    private IEnumerator OverwriteVelocity(Vector3 throwVector)
    {
        // 1フレーム待機する
        yield return new WaitForEndOfFrame();

        // 投擲ベクトルを速度に上書きする
        _thisRigidbody.velocity = throwVector;
    }

    public void Inject(PlayerHandDependencyInfomation information)
    {
        // 
        _rightHandTransform = information.RightHand.transform;
    }
}
