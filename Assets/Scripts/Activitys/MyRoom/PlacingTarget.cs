using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingTarget : MonoBehaviour
{
    private const float GROUND_OFFSET = 0.01f;
    private const float FORWARD_OFFSET = 1.2f;

    private BoxCollider boxCollider = default;
    private IEditOnlyGhost ghostModel = default;
    private Transform player = default;
    private Transform playerHead = default;
    private Collider[] hitResults = new Collider[4];
    private bool isCollision = default;
    private float yPosition = default;
    private Vector3 boxHalfSize = default;
    private float slopeLimit = default;
    private float playerHeight = default;


    public PlacingTarget Initialize(IEditOnlyGhost ghostModel, Transform player)
    {
        this.ghostModel = ghostModel;
        this.player = player;
        // Tmporary
        playerHead = player.Find("OVRCameraRig (1)").Find("TrackingSpace").Find("CenterEyeAnchor");
        var cc = player.GetComponent<CharacterController>();
        slopeLimit = cc.slopeLimit;
        playerHeight = cc.height;
        boxCollider = transform.GetComponent<BoxCollider>();
        boxHalfSize = boxCollider.size / 2;
        return this;
    }

    private void LateUpdate()
    {
        transform.SetPositionAndRotation(
            position: new Vector3(player.position.x, yPosition, player.position.z) + player.forward * FORWARD_OFFSET,
            rotation: player.rotation);
        ghostModel.SetPlaceableState(PreviewPlacing());
    }

    private bool PreviewPlacing()
    {
        // 計算誤差用の定数
        const float CALC_ERROR_OFFSET = 0.01f;
        // 地面とみなす段差の高さ（この高さまで、自然と補正される）
        const float STEP_HEIGHT_LIMIT = 1f;

        // 自分の身長の4分の1は床判定
        // GameObject.transformはモデルによって違うので、y軸補正の際はすべて足し算で行う

        // Ghost（自分）の中心および足元の座標を取得
        // モデルによって、原点の位置が違う問題を、boundsによって一応解決した
        Vector3 center = boxCollider.bounds.center;
        Vector3 underOrigin = new Vector3(center.x, center.y - boxHalfSize.y, center.z);

        // 接地判定
        // 
        Vector3 checkGroundCenter = center + Vector3.up * STEP_HEIGHT_LIMIT;
        float distance = Mathf.Abs(player.position.y - playerHeight / 2 - STEP_HEIGHT_LIMIT - checkGroundCenter.y);
        distance = distance < STEP_HEIGHT_LIMIT + boxHalfSize.y ? STEP_HEIGHT_LIMIT + boxHalfSize.y : distance;

        bool isHitGround = Physics.BoxCast(
            center: checkGroundCenter,
            halfExtents: boxHalfSize,
            direction: Vector3.down,
            hitInfo: out RaycastHit groundHitInfo,
            orientation: transform.rotation,
            maxDistance: distance,
            layerMask: Layer.GROUNDWALL
            );

        float rayDistance = Mathf.Abs(groundHitInfo.point.y - checkGroundCenter.y) + CALC_ERROR_OFFSET;
        Ray saikyouRay1 = new Ray(underOrigin + boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + boxHalfSize.z * transform.forward + Vector3.up * STEP_HEIGHT_LIMIT, Vector3.down);
        Ray saikyouRay2 = new Ray(underOrigin + -boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + boxHalfSize.z * transform.forward + Vector3.up * STEP_HEIGHT_LIMIT, Vector3.down);
        Ray saikyouRay3 = new Ray(underOrigin + boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + -boxHalfSize.z * transform.forward + Vector3.up * STEP_HEIGHT_LIMIT, Vector3.down);
        Ray saikyouRay4 = new Ray(underOrigin + -boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + -boxHalfSize.z * transform.forward + Vector3.up * STEP_HEIGHT_LIMIT, Vector3.down);
        bool isHitSaikyou1 = Physics.Raycast(saikyouRay1, out RaycastHit saikyouInfo1, rayDistance, Layer.GROUNDWALL);
        bool isHitSaikyou2 = Physics.Raycast(saikyouRay2, out RaycastHit saikyouInfo2, rayDistance, Layer.GROUNDWALL);
        bool isHitSaikyou3 = Physics.Raycast(saikyouRay3, out RaycastHit saikyouInfo3, rayDistance, Layer.GROUNDWALL);
        bool isHitSaikyou4 = Physics.Raycast(saikyouRay4, out RaycastHit saikyouInfo4, rayDistance, Layer.GROUNDWALL);
        Debug.DrawRay(saikyouRay1.origin, saikyouRay1.direction * rayDistance, Color.yellow);
        Debug.DrawRay(saikyouRay2.origin, saikyouRay2.direction * rayDistance, Color.yellow);
        Debug.DrawRay(saikyouRay3.origin, saikyouRay3.direction * rayDistance, Color.yellow);
        Debug.DrawRay(saikyouRay4.origin, saikyouRay4.direction * rayDistance, Color.yellow);

        if (Vector3.Angle(Vector3.up, groundHitInfo.normal) > slopeLimit)
        {
            return false;
        }

        // memo: rayが当たってないときに0になって落下しちゃう
        yPosition = groundHitInfo.point.y + GROUND_OFFSET;

        //if (isHitFront && wallHitInfo.normal.y == 0f && wallHitInfo.collider.bounds.extents.y < 2f)
        //{
        //    // top half
        //    if (wallHitInfo.point.y > wallHitInfo.transform.position.y)
        //    {

        //    }
        //    // bottom half
        //    else
        //    {

        //    }
        //}
        //else
        //{

        //}

        //Ray toGroundRay = new Ray(transform.position, Vector3.down);
        //bool isHitGround = Physics.Raycast(toGroundRay, out RaycastHit groundHitInfo, RAY_LIMIT, Layer.GROUNDWALL);
        //const float NORMAL_OFFSET = 0.001f;

        if (!(isHitSaikyou1 && isHitSaikyou2 && isHitSaikyou3 && isHitSaikyou4))
        {
            return false;
        }
        // 1にすると誤差が出るのでちょっと引いてる
        if (groundHitInfo.normal.y < 1 - CALC_ERROR_OFFSET)
        {
            return false;
        }
        if (isCollision)
        {
            return false;
        }

        Vector3 toPlayer = player.position - transform.position;
        Ray toPlayerRay = new Ray(transform.position, toPlayer.normalized);
        bool isHitBack = Physics.Raycast(toPlayerRay, out RaycastHit _, toPlayer.magnitude, Layer.GROUNDWALL);
        if (isHitBack)
        {
            return false;
        }
        // プレイヤーの頭の向きにRayを飛ばし、WarpPointerの要領で地面か机の上かを判定する。
        // 机のコライダーの上半分にあたったら机の上だし、下半分に当たったら地面。
        // あまりにもその壁が高かったら柱と判断し、return

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        isCollision = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isCollision = false;
    }

    private void OnDrawGizmos()
    {
        var center = boxCollider.bounds.center;
        var underOrigin = new Vector3(center.x, center.y - boxHalfSize.y, center.z);
        var checkGroundCenter = center + Vector3.up * 2;

        Gizmos.DrawWireCube(checkGroundCenter, boxCollider.size);
        Gizmos.DrawWireCube(new Vector3(underOrigin.x, player.position.y - playerHeight / 2 - 2, underOrigin.z), boxCollider.size);
        Gizmos.DrawRay(checkGroundCenter, Vector3.down * Mathf.Abs(player.position.y - playerHeight / 2 - 2 - checkGroundCenter.y));
    }
}
