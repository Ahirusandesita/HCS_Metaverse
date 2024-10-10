using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingTarget : MonoBehaviour
{
    private const float GROUND_OFFSET = 0.01f;
    private const float FORWARD_OFFSET = 1.2f;

    private BoxCollider boxCollider = default;
    private IEditOnlyGhost ghostModel = default;
    private PlaceableObject placeableObject = default;
    private Transform player = default;
    private Transform playerHead = default;
    private bool isCollision = default;
    private float yPosition = default;
    private Vector3 boxHalfSize = default;
    private float slopeLimit = default;
    private float playerHeight = default;


    public PlacingTarget Initialize(IEditOnlyGhost ghostModel, PlaceableObject placeableObject, Transform player)
    {
        this.ghostModel = ghostModel;
        this.placeableObject = placeableObject;
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
        XDebug.Log(placeableObject.PivotType);

        transform.SetPositionAndRotation(
            position: new Vector3(player.position.x, yPosition, player.position.z) + player.forward * FORWARD_OFFSET,
            rotation: player.rotation);
        ghostModel.SetPlaceableState(PreviewPlacing());
    }

    private bool PreviewPlacing()
    {
        // 計算誤差用の定数
        const float CALC_ERROR_OFFSET = 0.01f;
        // 設置できる段差の高さ（この高さまで、自然と補正される）
        float stepHeightLimit = playerHeight / 4;

        // 自分の身長の4分の1は床判定
        // GameObject.transformはモデルによって違うので、y軸補正の際はすべて足し算で行う

        // Ghost（自分）の中心および足元の座標を取得
        // モデルによって、原点の位置が違う問題を、boundsによって一応解決した
        Vector3 center = boxCollider.bounds.center;
        Vector3 underOrigin = new Vector3(center.x, center.y - boxHalfSize.y, center.z);
        float playerUnderOriginY = player.position.y - playerHeight / 2;

        // 接地判定
        // 
        Vector3 checkGroundCenter = center + Vector3.up * stepHeightLimit;
        float checkGroundBoxDistance = Mathf.Abs(playerUnderOriginY - stepHeightLimit - checkGroundCenter.y);

        bool isHitGroundBox = Physics.BoxCast(
            center: checkGroundCenter,
            halfExtents: boxHalfSize,
            direction: Vector3.down,
            hitInfo: out RaycastHit groundHitInfo,
            orientation: transform.rotation,
            maxDistance: checkGroundBoxDistance,
            layerMask: Layer.GROUNDWALL
            );

        float rayDistance = Mathf.Abs(groundHitInfo.point.y - checkGroundCenter.y) + CALC_ERROR_OFFSET;
        Ray checkGroundRay_rf = new Ray(underOrigin + boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + boxHalfSize.z * transform.forward + Vector3.up * stepHeightLimit, Vector3.down);
        Ray checkGroundRay_lf = new Ray(underOrigin + -boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + boxHalfSize.z * transform.forward + Vector3.up * stepHeightLimit, Vector3.down);
        Ray checkGroundRay_rb = new Ray(underOrigin + boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + -boxHalfSize.z * transform.forward + Vector3.up * stepHeightLimit, Vector3.down);
        Ray checkGroundRay_lb = new Ray(underOrigin + -boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + -boxHalfSize.z * transform.forward + Vector3.up * stepHeightLimit, Vector3.down);
        bool isHitGround_rf = Physics.Raycast(checkGroundRay_rf, out RaycastHit _, rayDistance, Layer.GROUNDWALL);
        bool isHitGround_lf = Physics.Raycast(checkGroundRay_lf, out RaycastHit _, rayDistance, Layer.GROUNDWALL);
        bool isHitGround_rb = Physics.Raycast(checkGroundRay_rb, out RaycastHit _, rayDistance, Layer.GROUNDWALL);
        bool isHitGround_lb = Physics.Raycast(checkGroundRay_lb, out RaycastHit _, rayDistance, Layer.GROUNDWALL);
        Debug.DrawRay(checkGroundRay_rf.origin, checkGroundRay_rf.direction * rayDistance, Color.yellow);
        Debug.DrawRay(checkGroundRay_lf.origin, checkGroundRay_lf.direction * rayDistance, Color.yellow);
        Debug.DrawRay(checkGroundRay_rb.origin, checkGroundRay_rb.direction * rayDistance, Color.yellow);
        Debug.DrawRay(checkGroundRay_lb.origin, checkGroundRay_lb.direction * rayDistance, Color.yellow);

        if (Vector3.Angle(Vector3.up, groundHitInfo.normal) > slopeLimit)
        {
            return false;
        }

        // Rayが当たった位置をy軸の座標とする
        // もしRayが当たらない = 高すぎる位置にいるときは「設置できない」と表現するため、プレイヤーと同じ高さでキープする（浮かせる）
        yPosition = isHitGroundBox ?
            groundHitInfo.point.y + GROUND_OFFSET :
            playerUnderOriginY;

        // 原点が足元にない場合は埋まってしまうので、その場合は補正する
        if (placeableObject.PivotType == GhostModel.PivotType.Center)
        {
            yPosition += boxHalfSize.y;
        }

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

        // Rayがすべて当たっている = 坂や崖に面さず、完璧に設置可能な状態であるか
        bool isPerfectlyGrounded = isHitGround_rf && isHitGround_lf && isHitGround_rb && isHitGround_lb;
        if (!isPerfectlyGrounded)
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

    private void OnPlacing()
    {

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
