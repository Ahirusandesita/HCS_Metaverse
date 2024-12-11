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

        transform.position = new Vector3(xPosition, yPosition, zPosition) + transform.forward * forwardOffset;
        // プレイヤーの転回に合わせたrotationと、オブジェクト自身の転回をマージ
        transform.rotation = Quaternion.Euler(new Vector3(player.rotation.x, player.rotation.eulerAngles.y + rotateAngle, player.rotation.z));
    }

    protected override bool PreviewPlacing()
    {
        // 計算誤差用の定数
        const float CALC_ERROR_OFFSET = 0.01f;
        // 補正可能な最大角度（壁にぴったりオブジェクトを沿わせられるよう、ある程度の角度は補正する）
        const float ANGLE_OFFSET_LIMIT = 30f;

        // Ghost（自分）の中心座標を取得
        Vector3 center = boxCollider.bounds.center;
        // 埋まりこみ対策で厚みを考慮した中心点を求める
        Vector3 forwardOrigin = new Vector3(center.x + boxHalfSize.z * transform.forward.x, center.y, center.z + boxHalfSize.z * transform.forward.z);
        // Rayの中心点（プレイヤーの位置）
        Vector3 rayCenter = new Vector3(player.position.x, center.y, player.position.z);

        // 壁の当たり判定
        bool isHitWallBox = Physics.BoxCast(
            center: rayCenter,
            halfExtents: boxHalfSize,
            direction: transform.forward,
            hitInfo: out RaycastHit wallHitInfo,
            orientation: transform.rotation,
            maxDistance: PLACEABLE_DISTANCE,
            layerMask: Layer.GROUNDWALL
            );

        // 原点の位置に応じてy軸のPositionを調整
        yPosition = placeableObject.PivotType == GhostModel.PivotType.Under
            ? player.position.y - yPositionOffset
            : player.position.y + boxHalfSize.y;

        // もし壁にBoxcastが当たっていなければ、初期値に戻しreturn
        if (!isHitWallBox)
        {
            ResetTransform();
            return false;
        }

        // 壁に触れている面の中心点を取得（RaycastHit.pointではランダムな点が返るため、投影を使い求める）
        Vector3 hitPoint = rayCenter + Vector3.Project(wallHitInfo.point - rayCenter, -wallHitInfo.normal);
        float hitPointDistance = Vector3.Distance(hitPoint, rayCenter);

        // オブジェクトを回転したままBoxcastが当たったとき、そのときのhitPointは設置可能な距離を超えていないか
        // （オブジェクトが斜めのまま壁に侵入した際に角度補正が働くが、補正後に距離が足りない現象を防ぐための仕様）
        // （補正後に距離が足りてるの？をチェック）
        bool isHitPointIntoPlaceableDistance = hitPointDistance - boxHalfSize.z < PLACEABLE_DISTANCE;
        if (isHitPointIntoPlaceableDistance)
        {
            // hitPointに座標を更新（埋まりこみ防止で誤差値を引く）
            xPosition = hitPoint.x - transform.forward.x * CALC_ERROR_OFFSET;
            zPosition = hitPoint.z - transform.forward.z * CALC_ERROR_OFFSET;
            // 厚みを考慮しOffsetを設定
            forwardOffset = -boxHalfSize.z;
            // 角度の補正のため、SignedAngleを取得
            rotateAngle = Vector3.SignedAngle(wallHitInfo.normal, -player.forward, Vector3.down);

            // 30°以上であれば角度補正は行わない（壁に沿わせない）
            if (Mathf.Abs(rotateAngle) - CALC_ERROR_OFFSET > ANGLE_OFFSET_LIMIT)
            {
                ResetTransform();
                return false;
            }
        }
        else
        {
            ResetTransform();
            return false;
        }

        void ResetTransform()
        {
            xPosition = player.position.x;
            zPosition = player.position.z;
            forwardOffset = cacheForwardOffset;
            rotateAngle = 0f;
        }

        // 設置可能かどうかを判定する4つのRay
        // それぞれ四つ角からRayを飛ばし、どれかひとつでも当たっていなければそこは壁が途切れている＝設置できないと判定
        float rayDistance = Vector3.Distance(hitPoint, forwardOrigin) + CALC_ERROR_OFFSET;
        Ray checkWallRay_ru = new Ray(forwardOrigin + boxHalfSize.x * transform.right + boxHalfSize.y * transform.up, transform.forward);
        Ray checkWallRay_lu = new Ray(forwardOrigin + -boxHalfSize.x * transform.right + boxHalfSize.y * transform.up, transform.forward);
        Ray checkWallRay_rd = new Ray(forwardOrigin + boxHalfSize.x * transform.right + -boxHalfSize.y * transform.up, transform.forward);
        Ray checkWallRay_ld = new Ray(forwardOrigin + -boxHalfSize.x * transform.right + -boxHalfSize.y * transform.up, transform.forward);
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

        // Rayがすべて当たっている = 坂や崖に面さず、完璧に設置可能な状態であるか
        bool isPerfectlyGrounded = isHitWall_ru && isHitWall_lu && isHitWall_rd && isHitWall_ld;
        if (!isPerfectlyGrounded)
        {
            return false;
        }

        // そもそも今設置しようとしている場所は壁か（地面に対し垂直か）
        if (Vector3.Angle(transform.forward, wallHitInfo.normal) != 180f)
        {
            return false;
        }

        // なにか他のオブジェクトにぶつかっていないか
        if (isCollision)
        {
            return false;
        }

        // すべての条件をクリアしたとき、設置可能
        return true;
    }

    protected override void OnRotate(InputAction.CallbackContext context) { }

    protected override void OnRotateCancel(InputAction.CallbackContext context) { }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        var center = player.position + transform.forward * cacheForwardOffset;
        center.y = boxCollider.bounds.center.y;
        var checkGroundCenter = center + transform.forward * PLACEABLE_DISTANCE;

        Gizmos.DrawWireCube(checkGroundCenter, boxCollider.size);
        Gizmos.DrawRay(center, transform.forward * PLACEABLE_DISTANCE);
    }
#endif
}
