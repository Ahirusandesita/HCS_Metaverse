using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopperObject : MonoBehaviour
{
    [SerializeField, Tooltip("接触判定を行うCollider")]
    private Collider _stopperColliter = default;

    // 接触判定を行うColliderの中心座標
    private Vector3 _hitBoxCenter = default;

    // 接触判定を行うColliderの大きさ
    private Vector3 _hitBoxSize = default;

    // 接触判定を行うColliderの角度
    private Quaternion _hitBoxRotation = default;

    private void Start()
    {
        // 接触判定を行うColliderの各値を取得する
        _hitBoxCenter = _stopperColliter.bounds.center;
        _hitBoxSize = _stopperColliter.bounds.size / 2;
        _hitBoxRotation = this.transform.rotation;
    }

    private void Update()
    {
        // 接触したColliderを判定して格納する
        Collider[] hitColliders = Physics.OverlapBox(_hitBoxCenter, _hitBoxSize, _hitBoxRotation);

        // 接触したColliderがなかった場合
        if (hitColliders is null)
        {
            // なにもしない
            Debug.Log($"なにも当たってないよん");
            return;
        }

        // 接触したColliderすべてに判定を行う
        foreach (Collider hitCollider in hitColliders)
        {
            // Stoppableを持っていない場合
            if (!hitCollider.transform.root.TryGetComponent<Stoppable>(out var stoppable))
            {
                // 次のColliderへ
                continue;
            }

            // StopDataを持っている場合
            if (hitCollider.transform.root.TryGetComponent<StopData>(out var stopData))
            {
                // StopDataの停止フラグを立てる
                stopData.SetIsHitStopper(true);
            }
            // StopDataを持っていない場合
            else
            {
                // 接触しているオブジェクトにStopDataを加える
                hitCollider.transform.root.gameObject.AddComponent<StopData>();

                // 停止時の処理を実行する
                stoppable.StoppingEvent();
            }
        }
    }
}
