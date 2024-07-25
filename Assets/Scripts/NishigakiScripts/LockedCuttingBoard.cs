using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class LockedCuttingBoard : MonoBehaviour, IKnifeHitEvent
{
    [SerializeField, Tooltip("オブジェクトの取得範囲を指定するCollider")]
    private Collider _cuttingAreaCollider = default;

    [SerializeField, Tooltip("固定位置")]
    private Transform _machineTransform = default;

    [SerializeField, Tooltip("オブジェクトを固定する場所のTransform")]
    private Transform _objectLockPosition = default;

    // オブジェクトの取得範囲を表す値 -----------------------
    // 中心
    private Vector3 _hitBoxCenter = default;

    // サイズ
    private Vector3 _hitBoxSize = default;

    // 角度
    private Quaternion _hitBoxRotation = default;
    // -------------------------------------------------

    // GrabbableのActiveを切り替えるための変数
    private ISwitchableGrabbableActive _grabbableActiveSwicher = default;

    // オブジェクトを固定しているかどうか
    private bool _isLockedObject = default;

    // 固定しているオブジェクトのIngrodients
    private Ingrodients _lockingIngrodients = default;

    // 固定しているオブジェクトのPuttable
    private Puttable _lockedPuttable = default;

    private void Start()
    {
        // オブジェクトの取得範囲の各値を設定する
        // 中心
        _hitBoxCenter = _cuttingAreaCollider.bounds.center;

        // 角度
        _hitBoxRotation = this.transform.rotation;

        // サイズ
        _hitBoxSize = _cuttingAreaCollider.bounds.size / 2;
    }

    private void Update()
    {
        // オブジェクトを固定している場合
        if (_isLockedObject)
        {
            // 何もしない
            return;
        }

        // オブジェクトの取得範囲を形成して接触しているColliderを取得する
        Collider[] hitColliders = Physics.OverlapBox(_hitBoxCenter, _hitBoxSize, _hitBoxRotation);

        // 何も当たっていなかった場合
        if (hitColliders is null)
        {
            // 何もしない
            Debug.Log($"なにも当たってないよん");
            return;
        }

        // 範囲内のオブジェクトをすべて探索する
        foreach (Collider hitCollider in hitColliders)
        {
            // Ingrodientsがついていた場合
            if (hitCollider.transform.root.TryGetComponent<Ingrodients>(out var ingrodient))
            {
                // RigidbodyのKinematicがついている場合
                if (ingrodient.GetComponent<Rigidbody>().isKinematic)
                {
                    // 次のオブジェクトに移る
                    continue;
                }

                // 固定するオブジェクトを取得する
                GameObject lockObject = ingrodient.gameObject;

                // 固定するオブジェクトのIngrodientを取得する
                _lockingIngrodients = ingrodient;

                // 固定するオブジェクトのGrabbableを変更するためのインターフェースを取得する
                _grabbableActiveSwicher = lockObject.GetComponent<ISwitchableGrabbableActive>();

                // 固定するオブジェクトのGrabbableをfalseにする
                _grabbableActiveSwicher.Inactive();

                // 固定するオブジェクトの座標をマシンの座標に移動させる
                lockObject.transform.position = _machineTransform.position;
                lockObject.transform.rotation = _machineTransform.rotation;

                // 固定するオブジェクトのGrabbableをtrueにする
                _grabbableActiveSwicher.Active();

                // オブジェクトを固定している状態にする
                _isLockedObject = true;

                // 固定しているオブジェクトにPuttableを追加して取得する
                _lockedPuttable = lockObject.AddComponent<Puttable>();

                // Puttableに自身を渡す
                _lockedPuttable.SetLockedCuttingObject(this);
            }
        }
    }

    public void KnifeHitEvent()
    {
        // オブジェクトが固定されている　かつ　固定されているオブジェクトにIngrodientがついている場合
        if (_isLockedObject && _lockingIngrodients is not null)
        {
            // 
            bool isEndCut = _lockingIngrodients.SubToIngrodientsDetailInformationsTimeItTakes(ProcessingType.Cut, 1);

            // 
            if (isEndCut)
            {
                // 
                Commodity processedCommodity = default;

                // 
                processedCommodity = _lockingIngrodients.ProcessingStart(ProcessingType.Cut, _machineTransform);

                // 
                _lockedPuttable.DestroyThis();

                // 
                _lockedPuttable = processedCommodity.gameObject.AddComponent<Puttable>();

                // 
                if (processedCommodity.gameObject.TryGetComponent<Ingrodients>(out Ingrodients ingrodients))
                {
                    // 
                    _lockingIngrodients = ingrodients;
                }
                else
                {
                    // 
                    _lockingIngrodients = null;
                }
            }            
        }
    }

    public void CanselCutting()
    {
        // 
        _isLockedObject = false;
    }

    public void Inject(ISwitchableGrabbableActive t)
    {
        _grabbableActiveSwicher = t;
    }
}
