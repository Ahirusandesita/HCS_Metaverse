using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandIK : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Transform _handTarget;  // 手のターゲット位置 (VRコントローラーなど)

    [SerializeField]
    private Transform _shoulderTransform;  // 肩のTransform

    [SerializeField]
    private Transform _bodyTransform;  // 体のTransform (Spine)

    void OnAnimatorIK(int layerIndex)
    {
        Debug.Log("move");

        if (_animator)
        {
            // 左手のIKゴールのウェイトと位置・回転を設定
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, _handTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, _handTarget.rotation);

            // 左肘のヒント位置を計算して設定
            Vector3 elbowHintPosition = CalculateElbowHintPosition();
            _animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1.0f);
            _animator.SetIKHintPosition(AvatarIKHint.RightElbow, elbowHintPosition);
        }
    }

    // 肘の自然な位置を計算する関数
    Vector3 CalculateElbowHintPosition()
    {
        Vector3 shoulderPosition = _shoulderTransform.position;
        Vector3 handPosition = _handTarget.position;

        // 手から肩へのベクトルを取得
        Vector3 shoulderToHand = handPosition - shoulderPosition;

        // 手から肩へのベクトルに垂直な方向に肘をオフセット
        Vector3 perpendicularOffset = Vector3.Cross(shoulderToHand, _bodyTransform.up).normalized;

        // 肘の位置を肩と手の中間に配置し、オフセットを加える
        return shoulderPosition + shoulderToHand * 0.5f + perpendicularOffset * 0.2f;  // 調整が必要な場合、オフセット値を変更
    }
}