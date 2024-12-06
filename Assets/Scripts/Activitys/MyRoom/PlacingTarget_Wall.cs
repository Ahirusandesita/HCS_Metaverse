using UnityEngine;
using UnityEngine.InputSystem;

public class PlacingTarget_Wall : PlacingTarget
{
    private const float PLACEABLE_DISTANCE = 3f;
    private float cacheForwardOffset = default;
    private float yPositionOffset = default;

    private void Start()
    {
        cacheForwardOffset = forwardOffset;
        // 原点が下にずれている場合、高さを実際のプレイヤーの目線に合わせるための補正値
        yPositionOffset = Mathf.Abs(player.position.y - boxCollider.bounds.center.y);
    }

    protected override void LateUpdate()
    {
        ghostModel.SetPlaceableState(PreviewPlacing());

        UpdateAction?.Invoke();

        transform.position = new Vector3(xPosition, yPosition, zPosition) + player.forward * forwardOffset;
        // プレイヤーの転回に合わせたrotationと、オブジェクト自身の転回をマージ
        transform.rotation = Quaternion.Euler(new Vector3(player.rotation.x, player.rotation.eulerAngles.y + rotateAngle, player.rotation.z));
    }

    protected override bool PreviewPlacing()
    {
        // 計算誤差用の定数
        const float CALC_ERROR_OFFSET = 0.01f;

        // 自分の身長の4分の1は床判定
        // GameObject.transformはモデルによって違うので、y軸補正の際はすべて足し算で行う

        // Ghost（自分）の中心および足元の座標を取得
        // モデルによって、原点の位置が違う問題を、boundsによって一応解決した
        Vector3 center = boxCollider.bounds.center;
        // 埋まりこみ対策で厚みを考慮した中心点を求める
        Vector3 forwardOrigin = new Vector3(center.x + boxHalfSize.z * player.forward.x, center.y, center.z + boxHalfSize.z * player.forward.z);
        Vector3 rayCenter = new Vector3(player.position.x, center.y, player.position.z) + player.forward * cacheForwardOffset;

        // 壁の当たり判定
        bool isHitWallBox = Physics.BoxCast(
            center: rayCenter,
            halfExtents: boxHalfSize,
            direction: player.forward,
            hitInfo: out RaycastHit wallHitInfo,
            orientation: transform.rotation,
            maxDistance: PLACEABLE_DISTANCE,
            layerMask: Layer.GROUNDWALL
            );

        // 原点が足元にない場合は埋まってしまうので、その場合は補正する
        yPosition = placeableObject.PivotType == GhostModel.PivotType.Under
            ? player.position.y - yPositionOffset
            : player.position.y + boxHalfSize.y;
        if (!isHitWallBox)
        {
            XDebug.Log("AAA");
            xPosition = player.position.x;
            zPosition = player.position.z;
            forwardOffset = cacheForwardOffset;
            return false;
        }

        // -----------------------------------------
        // boxcastは当たってるけど、centerrayが当たっていない場合の例外処理
        // -----------------------------------------
        Ray toHitCenterRay = new Ray(rayCenter, player.forward);
        bool isHitCenter = Physics.Raycast(toHitCenterRay, out RaycastHit toHitCenterInfo, PLACEABLE_DISTANCE, Layer.GROUNDWALL);
        Debug.DrawRay(toHitCenterRay.origin, toHitCenterRay.direction * PLACEABLE_DISTANCE, Color.blue);

        xPosition = toHitCenterInfo.point.x;
        zPosition = toHitCenterInfo.point.z;
        forwardOffset = 0f;

        float rayDistance = Vector3.Distance(toHitCenterInfo.point, forwardOrigin) + CALC_ERROR_OFFSET;
        Ray checkWallRay_ru = new Ray(forwardOrigin + boxHalfSize.x * player.right + boxHalfSize.y * player.up, player.forward);
        Ray checkWallRay_lu = new Ray(forwardOrigin + -boxHalfSize.x * player.right + boxHalfSize.y * player.up, player.forward);
        Ray checkWallRay_rd = new Ray(forwardOrigin + boxHalfSize.x * player.right + -boxHalfSize.y * player.up, player.forward);
        Ray checkWallRay_ld = new Ray(forwardOrigin + -boxHalfSize.x * player.right + -boxHalfSize.y * player.up, player.forward);
        bool isHitWall_ru = Physics.Raycast(checkWallRay_ru, out RaycastHit _, rayDistance, Layer.GROUNDWALL);
        bool isHitWall_lu = Physics.Raycast(checkWallRay_lu, out RaycastHit _, rayDistance, Layer.GROUNDWALL);
        bool isHitWall_rd = Physics.Raycast(checkWallRay_rd, out RaycastHit _, rayDistance, Layer.GROUNDWALL);
        bool isHitWall_ld = Physics.Raycast(checkWallRay_ld, out RaycastHit _, rayDistance, Layer.GROUNDWALL);
#if UNITY_EDITOR
        Debug.DrawRay(checkWallRay_ru.origin, checkWallRay_ru.direction * rayDistance, Color.yellow);
        Debug.DrawRay(checkWallRay_lu.origin, checkWallRay_lu.direction * rayDistance, Color.yellow);
        Debug.DrawRay(checkWallRay_rd.origin, checkWallRay_rd.direction * rayDistance, Color.yellow);
        Debug.DrawRay(checkWallRay_ld.origin, checkWallRay_ld.direction * rayDistance, Color.yellow);
#endif




        // Rayが当たった位置をy軸の座標とする
        // もしRayが当たらない = 高すぎる位置にいるときは「設置できない」と表現するため、プレイヤーと同じ高さでキープする（浮かせる）
        //xPosition = player.position.x;

        //zPosition = isHitWallBox
        //    ? wallHitInfo.point.z - boxHalfSize.z - CALC_ERROR_OFFSET
        //    : player.position.z;



        if (Vector3.Angle(player.forward, wallHitInfo.normal) != 180f)
        {
            return false;
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
        bool isPerfectlyGrounded = isHitWall_ru && isHitWall_lu && isHitWall_rd && isHitWall_ld;
        if (!isPerfectlyGrounded)
        {
            return false;
        }
        // 1にすると誤差が出るのでちょっと引いてる
        //if (wallHitInfo.normal.y < 1 - CALC_ERROR_OFFSET)
        //{
        //    return false;
        //}
        if (isCollision)
        {
            return false;
        }

        //Vector3 toPlayer = player.position - transform.position;
        //Ray toPlayerRay = new Ray(transform.position, toPlayer.normalized);
        //bool isHitBack = Physics.Raycast(toPlayerRay, out RaycastHit _, toPlayer.magnitude, Layer.GROUNDWALL);
        //if (isHitBack)
        //{
        //    return false;
        //}
        // プレイヤーの頭の向きにRayを飛ばし、WarpPointerの要領で地面か机の上かを判定する。
        // 机のコライダーの上半分にあたったら机の上だし、下半分に当たったら地面。
        // あまりにもその壁が高かったら柱と判断し、return

        return true;

    }

    protected override void OnRotate(InputAction.CallbackContext context)
    {
        // 反転処理
    }

    protected override void OnRotateCancel(InputAction.CallbackContext context) { }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        var center = player.position + player.forward * cacheForwardOffset;
        center.y = boxCollider.bounds.center.y;
        var checkGroundCenter = center + player.forward * PLACEABLE_DISTANCE;

        Gizmos.DrawWireCube(checkGroundCenter, boxCollider.size);
        Gizmos.DrawRay(center, transform.forward * PLACEABLE_DISTANCE);
    }
#endif
}
