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
        // ���_�����ɂ���Ă���ꍇ�A���������ۂ̃v���C���[�̖ڐ��ɍ��킹�邽�߂̕␳�l
        yPositionOffset = Mathf.Abs(player.position.y - boxCollider.bounds.center.y);
    }

    protected override void LateUpdate()
    {
        ghostModel.SetPlaceableState(PreviewPlacing());

        UpdateAction?.Invoke();

        transform.position = new Vector3(xPosition, yPosition, zPosition) + transform.forward * forwardOffset;
        // �v���C���[�̓]��ɍ��킹��rotation�ƁA�I�u�W�F�N�g���g�̓]����}�[�W
        transform.rotation = Quaternion.Euler(new Vector3(player.rotation.x, player.rotation.eulerAngles.y + rotateAngle, player.rotation.z));
    }

    protected override bool PreviewPlacing()
    {
        // �v�Z�덷�p�̒萔
        const float CALC_ERROR_OFFSET = 0.01f;
        const float ANGLE_OFFSET_LIMIT = 30f;

        // �����̐g����4����1�͏�����
        // GameObject.transform�̓��f���ɂ���ĈႤ�̂ŁAy���␳�̍ۂ͂��ׂđ����Z�ōs��
        // Ghost�i�����j�̒��S����ё����̍��W���擾
        // ���f���ɂ���āA���_�̈ʒu���Ⴄ�����Abounds�ɂ���Ĉꉞ��������
        Vector3 center = boxCollider.bounds.center;
        // ���܂肱�ݑ΍�Ō��݂��l���������S�_�����߂�
        Vector3 forwardOrigin = new Vector3(center.x + boxHalfSize.z * transform.forward.x, center.y, center.z + boxHalfSize.z * transform.forward.z);
        Vector3 rayCenter = new Vector3(player.position.x, center.y, player.position.z);

        // �ǂ̓����蔻��
        bool isHitWallBox = Physics.BoxCast(
            center: rayCenter,
            halfExtents: boxHalfSize,
            direction: transform.forward,
            hitInfo: out RaycastHit wallHitInfo,
            orientation: transform.rotation,
            maxDistance: PLACEABLE_DISTANCE,
            layerMask: Layer.GROUNDWALL
            );


        // ���_�������ɂȂ��ꍇ�͖��܂��Ă��܂��̂ŁA���̏ꍇ�͕␳����
        yPosition = placeableObject.PivotType == GhostModel.PivotType.Under
            ? player.position.y - yPositionOffset
            : player.position.y + boxHalfSize.y;
        if (!isHitWallBox)
        {
            XDebug.Log("AAA");
            xPosition = player.position.x;
            zPosition = player.position.z;
            forwardOffset = cacheForwardOffset;
            rotateAngle = 0f;
            return false;
        }



        //Ray toHitCenterRay = new Ray(rayCenter, player.forward);
        //bool isHitCenter = Physics.Raycast(toHitCenterRay, out RaycastHit toHitCenterInfo, PLACEABLE_DISTANCE, Layer.GROUNDWALL);
        Vector3 hitPoint = rayCenter + Vector3.Project(wallHitInfo.point - rayCenter, -wallHitInfo.normal);
        float distance = Vector3.Distance(hitPoint, rayCenter);
        Debug.DrawRay(hitPoint, rayCenter - hitPoint, Color.blue);
        //Debug.DrawRay(wallHitInfo.point, Vector3.up * 5f, Color.cyan);
        //Debug.DrawRay(rayCenter, Vector3.up * 5f, Color.gray);

        if (distance - boxHalfSize.z < PLACEABLE_DISTANCE)
        {
            xPosition = hitPoint.x - transform.forward.x * CALC_ERROR_OFFSET;
            zPosition = hitPoint.z - transform.forward.z * CALC_ERROR_OFFSET;
            forwardOffset = -boxHalfSize.z;
            rotateAngle = Vector3.SignedAngle(wallHitInfo.normal, -player.forward, Vector3.down);

            if (Mathf.Abs(rotateAngle) - CALC_ERROR_OFFSET > ANGLE_OFFSET_LIMIT)
            {
                xPosition = player.position.x;
                zPosition = player.position.z;
                forwardOffset = cacheForwardOffset;
                rotateAngle = 0f;
                return false;
            }
        }
        else
        {
            xPosition = player.position.x;
            zPosition = player.position.z;
            forwardOffset = cacheForwardOffset;
            rotateAngle = 0f;
            return false;
        }

        // -----------------------------------------
        // distance�܂��̐����BCCC���Ă΂�ĂȂ�
        // -----------------------------------------


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




        // Ray�����������ʒu��y���̍��W�Ƃ���
        // ����Ray��������Ȃ� = ��������ʒu�ɂ���Ƃ��́u�ݒu�ł��Ȃ��v�ƕ\�����邽�߁A�v���C���[�Ɠ��������ŃL�[�v����i��������j
        //xPosition = player.position.x;

        //zPosition = isHitWallBox
        //    ? wallHitInfo.point.z - boxHalfSize.z - CALC_ERROR_OFFSET
        //    : player.position.z;



        if (Vector3.Angle(transform.forward, wallHitInfo.normal) != 180f)
        {
            XDebug.Log("BBB");

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

        // Ray�����ׂē������Ă��� = ���R�ɖʂ����A�����ɐݒu�\�ȏ�Ԃł��邩
        bool isPerfectlyGrounded = isHitWall_ru && isHitWall_lu && isHitWall_rd && isHitWall_ld;
        if (!isPerfectlyGrounded)
        {
            XDebug.Log("CCC");

            return false;
        }
        // 1�ɂ���ƌ덷���o��̂ł�����ƈ����Ă�
        //if (wallHitInfo.normal.y < 1 - CALC_ERROR_OFFSET)
        //{
        //    return false;
        //}
        if (isCollision)
        {
            XDebug.Log("DDD");

            return false;
        }

        //Vector3 toPlayer = player.position - transform.position;
        //Ray toPlayerRay = new Ray(transform.position, toPlayer.normalized);
        //bool isHitBack = Physics.Raycast(toPlayerRay, out RaycastHit _, toPlayer.magnitude, Layer.GROUNDWALL);
        //if (isHitBack)
        //{
        //    return false;
        //}
        // �v���C���[�̓��̌�����Ray���΂��AWarpPointer�̗v�̂Œn�ʂ����̏ォ�𔻒肷��B
        // ���̃R���C�_�[�̏㔼���ɂ�����������̏ゾ���A�������ɓ���������n�ʁB
        // ���܂�ɂ����̕ǂ����������璌�Ɣ��f���Areturn

        return true;
    }

    protected override void OnRotate(InputAction.CallbackContext context)
    {
        // ���]����
    }

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
