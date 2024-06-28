using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ViewLocker : MonoBehaviour, IDependencyInjector<PlayerVisualHandDependencyInformation>
{
    [SerializeField, Tooltip("見た目用オブジェクト")]
    private GameObject _visualObject = default;

    [SerializeField, Tooltip("見た目を固定するための境界線との判定を行う座標群")]
    private Transform[] _lockPositionCheckerTransforms = new Transform[4];

    [SerializeField, Tooltip("見た目を固定するための境界線の高さ")]
    private float _lockPositionHeight = default;

    // 現在掴まれているかどうか
    private bool _isGrabbing = false;

    // 掴んだ時の情報を取得するためのクラスのインスタンス用変数
    private InteractorDetailEventIssuer _detailEventer = default;

    // 手の見た目オブジェクトの情報を渡してくれるクラスのインスタンス用変数
    private PlayerVisualHandDependencyInformation _handVisualInformation = default;

    // ロック用のTransformが設定されているかどうか
    private bool _isSetTransforms = default;

    // 掴んだ手の方向
    private HandType _detailHandType = default;

    // 現在掴んでいる手の方向
    private HandType _grabbingHandType = default;

    // ロックする見た目オブジェクトのTransform
    private Vector3 _lockingvisualObjectPosition = default;
    private Quaternion _lockingVisualObjectRotation = default;

    // 右手の見た目オブジェクトのTransform
    private Vector3 _lockingVisualRightHandPosition = default;
    private Quaternion _lockingVisualRightHandRotation = default;

    // 左手の見た目オブジェクトのTransform
    private Vector3 _lockingVisualLeftHandPosition = default;
    private Quaternion _lockingVisualLeftHandRotation = default;

    // 右手のコントローラーの見た目オブジェクトのTransform
    private Vector3 _lockingVisualRightControllerPosition = default;
    private Quaternion _lockingVisualRightControllerRotation = default;

    // 左手のコントローラーの見た目オブジェクトののTransform
    private Vector3 _lockingVisualLeftControllerPosition = default;
    private Quaternion _lockingVisualLeftControllerRotation = default;

    // 右手のコントローラーを持っているときの右手の見た目オブジェクトのTransform
    private Vector3 _lockingVisualRightControllerHandPosition = default;
    private Quaternion _lockingVisualRightControllerHandRotation = default;

    // 左手のコントローラーを持っているときの左手の見た目オブジェクトのTransform
    private Vector3 _lockingVisualLeftControllerHandPosition = default;
    private Quaternion _lockingVisualLeftControllerHandRotation = default;

    private void Start()
    {
        // 掴んだ時の手の方向を講読しておく
        _detailEventer.OnInteractor += (handler) => { _detailHandType = handler.HandType; };
    }

    private void LateUpdate()
    {
        // 掴まれているかどうか
        if (_isGrabbing)
        {
            // 境界線を超えているかどうか
            if (CheckInLockPosition())
            {
                // ロック用Transformが設定されているかどうか
                if (_isSetTransforms)
                {
                    // 設定されていた場合は各Transformをロック用Transformで上書きする
                    LockingViewTransforms(_grabbingHandType);

                    // 上書きが終わったら終了する
                    return;
                }

                // 設定されていなかった場合はTransformを設定する
                SetViewParameters();
            }
            // 境界線を越えていない かつ ロック用Transformが設定されているかどうか
            else if(_isSetTransforms)
            {
                // 設定されていたら初期化する
                ResetViewParameters();
            }
        }
        // 掴まれていない かつ ロック用Transformが設定されているかどうか
        else if(_isSetTransforms)
        {
            // 設定されていたら初期化する
            ResetViewParameters();
        }
    }

    public void Select()
    {
        // 掴まれている状態にする
        _isGrabbing = true;

        // 掴んでいる手の方向を記録する
        _grabbingHandType = _detailHandType;
    }

    public void UnSelect()
    {
        // 掴まれていない状態にする
        _isGrabbing = false;
    }

    /// <summary>
    /// 現在のTransformからロック用Transformを設定するメソッド
    /// </summary>
    private void SetViewParameters()
    {
        // ロックする見た目オブジェクトのTransformを記録する
        _lockingvisualObjectPosition = _visualObject.transform.position;
        _lockingVisualObjectRotation = _visualObject.transform.rotation;

        // 手の方向をもとに分岐
        switch (_grabbingHandType)
        {
            // 右手
            case HandType.Right:
                // 右手の見た目オブジェクトのTransformを記録する
                _lockingVisualRightHandPosition = _handVisualInformation.VisualRightHand.transform.position;
                _lockingVisualRightHandRotation = _handVisualInformation.VisualRightHand.transform.rotation;

                // 右手のコントローラーの見た目オブジェクトのTransformを記録する
                _lockingVisualRightControllerPosition = _handVisualInformation.VisualRightController.transform.position;
                _lockingVisualRightControllerRotation = _handVisualInformation.VisualRightController.transform.rotation;

                // 右手のコントローラーを持っているときの見た目オブジェクトのTransformを記録する
                _lockingVisualRightControllerHandPosition = _handVisualInformation.VisualRightControllerHand.transform.position;
                _lockingVisualRightControllerHandRotation = _handVisualInformation.VisualRightControllerHand.transform.rotation;

                break;

            // 左手
            case HandType.Left:
                // 左手の見た目オブジェクトのTransformを記録する
                _lockingVisualLeftHandPosition = _handVisualInformation.VisualLeftHand.transform.position;
                _lockingVisualLeftHandRotation = _handVisualInformation.VisualLeftHand.transform.rotation;

                // 左手のコントローラーの見た目オブジェクトのTransformを記録する
                _lockingVisualLeftControllerPosition = _handVisualInformation.VisualLeftController.transform.position;
                _lockingVisualLeftControllerRotation = _handVisualInformation.VisualLeftController.transform.rotation;

                // 左手のコントローラーを持っているときの見た目オブジェクトのTransformを記録する
                _lockingVisualLeftControllerHandPosition = _handVisualInformation.VisualLeftControllerHand.transform.position;
                _lockingVisualLeftControllerHandRotation = _handVisualInformation.VisualLeftControllerHand.transform.rotation;

                break;
        }
    }

    /// <summary>
    /// 現在記録されているロック用Transformを初期化するメソッド
    /// </summary>
    private void ResetViewParameters()
    {
        // ロックする見た目オブジェクトのロック用Transformを初期化する
        _lockingvisualObjectPosition = default;
        _lockingVisualObjectRotation = default;

        // 右手の見た目オブジェクトのロック用Transformを初期化する
        _lockingVisualRightHandPosition = default;
        _lockingVisualRightHandRotation = default;

        // 左手の見た目オブジェクトのロック用Transformを初期化する
        _lockingVisualLeftHandPosition = default;
        _lockingVisualLeftHandRotation = default;

        // 右手のコントローラーの見た目オブジェクトのロック用Transformを初期化する
        _lockingVisualRightControllerPosition = default;
        _lockingVisualRightControllerRotation = default;

        // 左手のコントローラーの見た目オブジェクトのロック用Transformを初期化する
        _lockingVisualLeftControllerPosition = default;
        _lockingVisualLeftControllerRotation = default;

        // 右手のコントローラーを持っているときの見た目オブジェクトのロック用Transformを初期化する
        _lockingVisualRightControllerHandPosition = default;
        _lockingVisualRightControllerHandRotation = default;

        // 左手のコントローラーを持っているときの見た目オブジェクトのロック用Transformを初期化する
        _lockingVisualLeftControllerHandPosition = default;
        _lockingVisualLeftControllerHandRotation = default;
    }

    private void LockingViewTransforms(HandType grabbingHandType)
    {
        // 
        _visualObject.transform.position = _lockingvisualObjectPosition;
        _visualObject.transform.rotation = _lockingVisualObjectRotation;

        // 
        switch (grabbingHandType)
        {
            case HandType.Right:
                // 
                _handVisualInformation.VisualRightHand.transform.position = _lockingVisualRightHandPosition;
                _handVisualInformation.VisualRightHand.transform.rotation = _lockingVisualRightHandRotation;

                // 
                _handVisualInformation.VisualRightController.transform.position = _lockingVisualRightControllerPosition;
                _handVisualInformation.VisualRightController.transform.rotation = _lockingVisualRightControllerRotation;

                // 
                _handVisualInformation.VisualRightControllerHand.transform.position = _lockingVisualRightControllerHandPosition;
                _handVisualInformation.VisualRightControllerHand.transform.rotation = _lockingVisualRightControllerHandRotation;

                break;

            case HandType.Left:
                // 
                _handVisualInformation.VisualLeftHand.transform.position = _lockingVisualLeftHandPosition;
                _handVisualInformation.VisualLeftHand.transform.rotation = _lockingVisualLeftHandRotation;

                // 
                _handVisualInformation.VisualLeftController.transform.position = _lockingVisualLeftControllerPosition;
                _handVisualInformation.VisualLeftController.transform.rotation = _lockingVisualLeftControllerRotation;

                // 
                _handVisualInformation.VisualLeftControllerHand.transform.position = _lockingVisualLeftControllerHandPosition;
                _handVisualInformation.VisualLeftControllerHand.transform.rotation = _lockingVisualLeftControllerHandRotation;

                break;
        }
    }

    /// <summary>
    /// 見た目を固定するための境界線より下に位置する座標がないか判定するプロパティ
    /// </summary>
    /// <returns>境界線より下の座標があるかないか<br/>trueならある　falseならない</returns>
    private bool CheckInLockPosition()
    {
        // 各座標で判定を行う
        foreach (Transform lockingPositionChecker in _lockPositionCheckerTransforms)
        {
            // 境界線より下にあるかどうか
            if (lockingPositionChecker.position.y <= _lockPositionHeight)
            {
                // 境界線より下の座標があった場合
                return true;
            }
        }

        // すべての座標が境界線より上だった場合
        return false;
    }

    public void Inject(PlayerVisualHandDependencyInformation information)
    {
        // 手のView情報を取得しておく
        _handVisualInformation = information;
    }
}
