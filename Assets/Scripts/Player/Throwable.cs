using System.Collections;
using UnityEngine;
using Oculus.Interaction;
using Fusion;

public class Throwable : MonoBehaviour
{
    [SerializeField, Tooltip("自身が持つRigidbody")]
    public Rigidbody _thisRigidbody = default;

    [SerializeField, Tooltip("自身が持つTransform")]
    public Transform _thisTransform = default;

    [SerializeField, Tooltip("速度係数")]
    private float _velocityCoefficient = 1f;

    // 
    private LocalView _localView = default;

    // 使用中のThrowDataを格納するための変数
    public ThrowData _throwData = default;

    // オブジェクトを掴んでいるかどうかの判定用変数
    private bool _isSelected = default;

    // 掴んだ時や離した時にイベントを実行するクラス
    private PointableUnityEventWrapper pointableUnityEventWrapper;

    private void Start()
    {
        // 自身のLocalViewを取得する
        _localView = transform.root.GetComponent<LocalView>();

        // ThrowDataを生成する
        _throwData = new ThrowData(_thisTransform.position);

        // VRのEventに処理を登録する
        pointableUnityEventWrapper = this.GetComponent<PointableUnityEventWrapper>();
        pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });
        pointableUnityEventWrapper.WhenUnselect.AddListener((action) => { UnSelect(); });
    }

    private void FixedUpdate()
    {
        // どちらの手でも掴んでいない場合
        if (!_isSelected)
        {
            // 何もしない
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
        // 情報の初期化を行う
        _throwData.ReSetThrowData(_thisTransform.position);

        // 掴んでいる状態にする
        _isSelected = true;
    }

    /// <summary>
    /// 離されたときに実行するメソッド
    /// </summary>
    public void UnSelect()
    {
        // 投擲ベクトルを取得する
        Vector3 throwVector = _throwData.GetThrowVector() * _velocityCoefficient;

        // NetworkViewに処理の実行を依頼する
        _localView.NetworkView.GetComponent<NetworkThrow>().RPC_ThrowAllLocalView(throwVector);

        // 離している状態にする
        _isSelected = false;
    }

    /// <summary>
    /// ローカルで投擲挙動を行うためのRpcメソッド
    /// </summary>
    /// <param name="throwVector">投擲ベクトル</param>
    public void Throw(Vector3 throwVector)
    {
        // 1フレーム後にベクトルを上書きする
        StartCoroutine(OverwriteVelocity(throwVector));
    }

    /// <summary>
    /// 投擲速度を上書きするためのコルーチン
    /// </summary>
    /// <param name="throwVector">投擲速度</param>
    /// <returns></returns>
    private IEnumerator OverwriteVelocity(Vector3 throwVector)
    {
        // 1フレーム待機する　1フレーム待機しないとOVRに消される
        yield return null;

        // Kinematicを無効にする
        _thisRigidbody.isKinematic = false;

        // 投擲ベクトルを速度に上書きする
        _thisRigidbody.velocity = throwVector;
    }
}