using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacingTarget : MonoBehaviour, IDependencyInjector<PlayerBodyDependencyInformation>
{
    private const float GROUND_OFFSET = 0.01f;
    protected const float ROTATE_DURATION = 30f;  // 1秒間に回転する角度（度数法）

    protected BoxCollider boxCollider = default;
    protected IEditOnlyGhost ghostModel = default;
    protected PlaceableObject placeableObject = default;
    protected Transform player = default;
    protected IReadonlyTransformAdapter playerHead = default;
    protected bool isCollision = default;
    protected float xPosition = default;
    protected float yPosition = default;
    protected float zPosition = default;
    protected Vector3 boxHalfSize = default;
    protected float slopeLimit = default;
    protected float playerHeight = default;
    protected float rotateAngle = default;

    protected float forwardOffset = default;
    protected Action UpdateAction = default;


    public virtual PlacingTarget Initialize(IEditOnlyGhost ghostModel, PlaceableObject placeableObject, Transform player)
    {
        PlayerInitialize.ConsignmentInject_static(this);

        this.ghostModel = ghostModel;
        this.placeableObject = placeableObject;
        this.player = player;
        var cc = player.GetComponent<CharacterController>();
        slopeLimit = cc.slopeLimit;
        playerHeight = cc.height;
        boxCollider = transform.GetComponent<BoxCollider>();
        boxHalfSize = boxCollider.size / 2;

        // xとzで大きい方
        forwardOffset = boxCollider.size.x > boxCollider.size.z
            ? boxCollider.size.x
            : boxCollider.size.z;
        // プレイヤーの身長程度の距離は保証する
        forwardOffset = forwardOffset < playerHeight
            ? playerHeight
            : forwardOffset;

        // Tmporary（Inputの変更はどこか別の場所で行う）--------------
        Inputter.PlacingMode.Signed.Enable();
        // -------------------------------------------------------
        Inputter.PlacingMode.Signed.performed += OnSigned;
        Inputter.PlacingMode.Signed.canceled += OnSignedCancel;

        return this;
    }

    public void Inject(PlayerBodyDependencyInformation information)
    {
        playerHead = information.Head;
    }

    protected virtual void LateUpdate()
    {
        ghostModel.SetPlaceableState(PreviewPlacing());

        UpdateAction?.Invoke();

        transform.position = new Vector3(xPosition, yPosition, zPosition) + player.forward * forwardOffset;
        // プレイヤーの転回に合わせたrotationと、オブジェクト自身の転回をマージ
        transform.rotation = Quaternion.Euler(new Vector3(player.rotation.x, player.rotation.eulerAngles.y + rotateAngle, player.rotation.z));
    }

    protected virtual void OnDestroy()
    {
        // Tmporary
        Inputter.PlacingMode.Signed.Disable();
        UpdateAction = null;
    }

    /// <summary>
    /// オブジェクトを設置する際のプレビューを出現させる
    /// </summary>
    /// <returns>設置可能かどうか</returns>
    protected virtual bool PreviewPlacing()
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
#if UNITY_EDITOR
        Debug.DrawRay(checkGroundRay_rf.origin, checkGroundRay_rf.direction * rayDistance, Color.yellow);
        Debug.DrawRay(checkGroundRay_lf.origin, checkGroundRay_lf.direction * rayDistance, Color.yellow);
        Debug.DrawRay(checkGroundRay_rb.origin, checkGroundRay_rb.direction * rayDistance, Color.yellow);
        Debug.DrawRay(checkGroundRay_lb.origin, checkGroundRay_lb.direction * rayDistance, Color.yellow);
#endif

        if (Vector3.Angle(Vector3.up, groundHitInfo.normal) > slopeLimit)
        {
            return false;
        }

        xPosition = player.position.x;
        zPosition = player.position.z;

        // Rayが当たった位置をy軸の座標とする
        // もしRayが当たらない = 高すぎる位置にいるときは「設置できない」と表現するため、プレイヤーと同じ高さでキープする（浮かせる）
        yPosition = isHitGroundBox
            ? groundHitInfo.point.y + GROUND_OFFSET
            : playerUnderOriginY;

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

    protected virtual void OnSigned(InputAction.CallbackContext context)
    {
        // オブジェクト（ゴースト）自身の転回処理
        // ボタンを押している間回る
        UpdateAction += () => rotateAngle += Time.deltaTime * context.ReadValue<float>() * ROTATE_DURATION;
    }

    protected virtual void OnSignedCancel(InputAction.CallbackContext context)
    {
        UpdateAction = null;
    }

    protected virtual void OnPlacing()
    {

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        isCollision = true;
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        isCollision = false;
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        var center = boxCollider.bounds.center;
        var underOrigin = new Vector3(center.x, center.y - boxHalfSize.y, center.z);
        var checkGroundCenter = center + Vector3.up * 2;

        Gizmos.DrawWireCube(checkGroundCenter, boxCollider.size);
        Gizmos.DrawWireCube(new Vector3(underOrigin.x, player.position.y - playerHeight / 2 - 2, underOrigin.z), boxCollider.size);
        Gizmos.DrawRay(checkGroundCenter, Vector3.down * Mathf.Abs(player.position.y - playerHeight / 2 - 2 - checkGroundCenter.y));
    }
#endif
}
