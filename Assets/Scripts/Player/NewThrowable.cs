using System.Collections;
using UnityEngine;
using Oculus.Interaction;
using HCSMeta.Function.Initialize;

public class NewThrowable : MonoBehaviour, IDependencyInjector<PlayerHandDependencyInfomation>
{
    [SerializeField, Tooltip("自身が持つRigidbody")]
    public Rigidbody _thisRigidbody = default;

    [SerializeField, Tooltip("自身が持つTransform")]
    public Transform _thisTransform = default;

    [SerializeField, Tooltip("速度係数")]
    private float _velocityCoefficient = 1f;

    // 現在掴んでいる手のTransform
    private Transform _grabbingHandTransform = default;

    // 右手のTransform
    private Transform _rightHandTransform = default;

    // 左手のTransform
    private Transform _leftHandTransform = default;

    // 掴んだ時に渡される手の方向を格納する変数
    private HandType _detailEventsHandType = default;

    // 使用中のThrowDataを格納するための変数
    public ThrowData _throwData = default;

    // 現在右手で掴んでいるかどうか
    private bool _isGrabbingRightHand = false;

    // 現在左手で掴んでいるかどうか
    private bool _isGrabbingLeftHand = false;

    // つかんだ瞬間の情報を取得するためのクラス
    private InteractorDetailEventIssuer _interactorDetailEventIssuer;

    // 掴んだ時や離した時にイベントを実行するクラス
    private PointableUnityEventWrapper pointableUnityEventWrapper;

    // 掴みフラグ　現在掴んでいるかどうかの判定に用いる
    private bool _isSelected = false;

    private void Awake()
    {
        // ThrowDataを生成する
        _throwData = new ThrowData(_thisTransform.position);

        // 掴んだ時のイベントを登録する
        pointableUnityEventWrapper = this.GetComponent<PointableUnityEventWrapper>();
        pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });
        pointableUnityEventWrapper.WhenUnselect.AddListener((action) => { UnSelect(); });
        PlayerInitialize.ConsignmentInject_static(this);
    }

    private void Start()
    {
        _interactorDetailEventIssuer = GameObject.FindObjectOfType<InteractorDetailEventIssuer>();
        // 掴んだ時の情報を講読できるようにする
        _interactorDetailEventIssuer.OnInteractor += (handler) => {
            // 掴んでいる場合
            if (_isSelected)
            {
                // 掴んだ手の方向をもとにフラグを立てる
                SetGrabbingHandFlag(handler.HandType, true);

                // 掴んだ時のTransformを現在掴んでいる手として登録する
                _grabbingHandTransform = GetDetailHandsTransform(_detailEventsHandType);

                // 情報の初期化を行う
                _throwData.ReSetThrowData(_grabbingHandTransform.position);

                // 掴みフラグを消す
                _isSelected = false;
            }
        };
    }

    private void FixedUpdate()
    {
        // どちらの手でも掴んでいない場合
        if (!_isGrabbingRightHand && !_isGrabbingLeftHand)
        {
            // 何もしない
            return;
        }

        // 現在の座標を保存する
        _throwData.SetOrbitPosition(_grabbingHandTransform.position);
    }

    /// <summary>
    /// つかまれたときに実行するメソッド
    /// </summary>
    public void Select()
    {
        // 掴みフラグを立てる
        _isSelected = true;
    }

    /// <summary>
    /// 離されたときに実行するメソッド
    /// </summary>
    public void UnSelect()
    {
        // 掴んだ手の方向をもとにフラグを消す
        SetGrabbingHandFlag(_detailEventsHandType, false);

        // まだどちらかの手で掴み続けていた場合
        if (_isGrabbingRightHand || _isGrabbingLeftHand)
        {
            // まだつかんでいる方の手のTransform
            Transform nowGrabbingHand = default;

            // まだ掴んでいる方の手の情報を判別する
            if (_isGrabbingRightHand)
            {
                // 右手のTransformを代入
                nowGrabbingHand = GetDetailHandsTransform(HandType.Right);
            }
            else
            {
                // 左手のTransformを代入
                nowGrabbingHand = GetDetailHandsTransform(HandType.Left);
            }

            // まだ掴んでいる手の情報で初期化を行う
            _throwData.ReSetThrowData(nowGrabbingHand.position);

            // 投げないで終了する
            return;
        }

        // Kinematicを無効にする
        _thisRigidbody.isKinematic = false;

        // 投擲ベクトルを取得する
        Vector3 throwVector = _throwData.GetThrowVector() * _velocityCoefficient;

        // 1フレーム後にベクトルを上書きする
        StartCoroutine(OverwriteVelocity(throwVector));
    }

    /// <summary>
    /// 掴んでいる手のフラグを変更するためのメソッド
    /// </summary>
    /// <param name="handType">手の方向</param>
    /// <param name="setState">変更する値</param>
    private void SetGrabbingHandFlag(HandType handType, bool setState)
    {
        // 掴んだ手の方向をもとにフラグを変更する
        switch (handType)
        {
            // 右手の場合
            case HandType.Right:
                // 右手の掴んでいるかどうかのフラグを変更する
                _isGrabbingRightHand = setState;
                break;

            // 左手の場合
            case HandType.Left:
                // 左手の掴んでいるかどうかのフラグを変更する
                _isGrabbingLeftHand = setState;
                break;

            // 例外処理
            default:
                // 何もしない
                Debug.LogError($"SetGrabbingHandFlagにて手の方向に例外が発生　手の方向：{handType}");
                return;
        }
    }

    /// <summary>
    /// 掴んだ手のTransformを返すプロパティ
    /// </summary>
    /// <param name="handType">手の方向</param>
    /// <returns>掴んだ手のTransform</returns>
    private Transform GetDetailHandsTransform(HandType handType)
    {
        // 手の方向をもとに分岐
        switch (handType)
        {
            // 右手の場合
            case HandType.Right:
                // 右手のTransformを返す
                return _rightHandTransform;

            // 左手の場合
            case HandType.Left:
                // 左手のTransformを返す
                return _leftHandTransform;

            // 例外処理
            default:
                // 何も返さない
                Debug.LogError($"GetDetailHandsTransformにて手の方向に例外が発生　手の方向：{handType}");
                return null;
        }
    }

    /// <summary>
    /// 投擲補正を行うかどうかを判定するプロパティ
    /// </summary>
    /// <returns></returns>
    private bool DoAimedThrow()
    {
        return false;
    }

    /// <summary>
    /// 投擲速度を上書きするためのコルーチン
    /// </summary>
    /// <param name="throwVector">投擲速度</param>
    /// <returns></returns>
    private IEnumerator OverwriteVelocity(Vector3 throwVector)
    {
        // 1フレーム待機する　1フレーム待機しないとOVRに消される
        yield return new WaitForEndOfFrame();

        // 投擲ベクトルを速度に上書きする
        _thisRigidbody.velocity = throwVector;
    }

    public void Inject(PlayerHandDependencyInfomation information)
    {
        // 手のTransformを登録する
        _rightHandTransform = information.RightHand;
        _leftHandTransform = information.LeftHand;
    }
}