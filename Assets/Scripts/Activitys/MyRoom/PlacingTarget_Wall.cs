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

        transform.position = new Vector3(xPosition, yPosition, zPosition) + transform.forward * forwardOffset;
        // �v���C���[�̓]��ɍ��킹��rotation�ƁA�I�u�W�F�N�g���g�̓]����}�[�W
        transform.rotation = Quaternion.Euler(new Vector3(player.rotation.x, player.rotation.eulerAngles.y + rotateAngle, player.rotation.z));
    }

    protected override bool PreviewPlacing()
    {
        // �v�Z�덷�p�̒萔
        const float CALC_ERROR_OFFSET = 0.01f;
        // �␳�\�ȍő�p�x�i�ǂɂ҂�����I�u�W�F�N�g�����킹����悤�A������x�̊p�x�͕␳����j
        const float ANGLE_OFFSET_LIMIT = 30f;

        // Ghost�i�����j�̒��S���W���擾
        Vector3 center = boxCollider.bounds.center;
        // ���܂肱�ݑ΍�Ō��݂��l���������S�_�����߂�
        Vector3 forwardOrigin = new Vector3(center.x + boxHalfSize.z * transform.forward.x, center.y, center.z + boxHalfSize.z * transform.forward.z);
        // Ray�̒��S�_�i�v���C���[�̈ʒu�j
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

        // ���_�̈ʒu�ɉ�����y����Position�𒲐�
        yPosition = placeableObject.PivotType == GhostModel.PivotType.Under
            ? player.position.y - yPositionOffset
            : player.position.y + boxHalfSize.y;

        // �����ǂ�Boxcast���������Ă��Ȃ���΁A�����l�ɖ߂�return
        if (!isHitWallBox)
        {
            ResetTransform();
            return false;
        }

        // �ǂɐG��Ă���ʂ̒��S�_���擾�iRaycastHit.point�ł̓����_���ȓ_���Ԃ邽�߁A���e���g�����߂�j
        Vector3 hitPoint = rayCenter + Vector3.Project(wallHitInfo.point - rayCenter, -wallHitInfo.normal);
        float hitPointDistance = Vector3.Distance(hitPoint, rayCenter);

        // �I�u�W�F�N�g����]�����܂�Boxcast�����������Ƃ��A���̂Ƃ���hitPoint�͐ݒu�\�ȋ����𒴂��Ă��Ȃ���
        // �i�I�u�W�F�N�g���΂߂̂܂ܕǂɐN�������ۂɊp�x�␳���������A�␳��ɋ���������Ȃ����ۂ�h�����߂̎d�l�j
        // �i�␳��ɋ���������Ă�́H���`�F�b�N�j
        bool isHitPointIntoPlaceableDistance = hitPointDistance - boxHalfSize.z < PLACEABLE_DISTANCE;
        if (isHitPointIntoPlaceableDistance)
        {
            // hitPoint�ɍ��W���X�V�i���܂肱�ݖh�~�Ō덷�l�������j
            xPosition = hitPoint.x - transform.forward.x * CALC_ERROR_OFFSET;
            zPosition = hitPoint.z - transform.forward.z * CALC_ERROR_OFFSET;
            // ���݂��l����Offset��ݒ�
            forwardOffset = -boxHalfSize.z;
            // �p�x�̕␳�̂��߁ASignedAngle���擾
            rotateAngle = Vector3.SignedAngle(wallHitInfo.normal, -player.forward, Vector3.down);

            // 30���ȏ�ł���Ίp�x�␳�͍s��Ȃ��i�ǂɉ��킹�Ȃ��j
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

        // �ݒu�\���ǂ����𔻒肷��4��Ray
        // ���ꂼ��l�p����Ray���΂��A�ǂꂩ�ЂƂł��������Ă��Ȃ���΂����͕ǂ��r�؂�Ă��遁�ݒu�ł��Ȃ��Ɣ���
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

        // Ray�����ׂē������Ă��� = ���R�ɖʂ����A�����ɐݒu�\�ȏ�Ԃł��邩
        bool isPerfectlyGrounded = isHitWall_ru && isHitWall_lu && isHitWall_rd && isHitWall_ld;
        if (!isPerfectlyGrounded)
        {
            return false;
        }

        // �����������ݒu���悤�Ƃ��Ă���ꏊ�͕ǂ��i�n�ʂɑ΂��������j
        if (Vector3.Angle(transform.forward, wallHitInfo.normal) != 180f)
        {
            return false;
        }

        // �Ȃɂ����̃I�u�W�F�N�g�ɂԂ����Ă��Ȃ���
        if (isCollision)
        {
            return false;
        }

        // ���ׂĂ̏������N���A�����Ƃ��A�ݒu�\
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
