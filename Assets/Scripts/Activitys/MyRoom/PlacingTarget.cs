using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacingTarget : MonoBehaviour, IDependencyInjector<PlayerBodyDependencyInformation>
{
    private const float GROUND_OFFSET = 0.01f;
    protected const float ROTATE_DURATION = 30f;  // 1�b�Ԃɉ�]����p�x�i�x���@�j

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

        // x��z�ő傫����
        forwardOffset = boxCollider.size.x > boxCollider.size.z
            ? boxCollider.size.x
            : boxCollider.size.z;
        // �v���C���[�̐g�����x�̋����͕ۏ؂���
        forwardOffset = forwardOffset < playerHeight
            ? playerHeight
            : forwardOffset;

        // Tmporary�iInput�̕ύX�͂ǂ����ʂ̏ꏊ�ōs���j--------------
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
        // �v���C���[�̓]��ɍ��킹��rotation�ƁA�I�u�W�F�N�g���g�̓]����}�[�W
        transform.rotation = Quaternion.Euler(new Vector3(player.rotation.x, player.rotation.eulerAngles.y + rotateAngle, player.rotation.z));
    }

    protected virtual void OnDestroy()
    {
        // Tmporary
        Inputter.PlacingMode.Signed.Disable();
        UpdateAction = null;
    }

    /// <summary>
    /// �I�u�W�F�N�g��ݒu����ۂ̃v���r���[���o��������
    /// </summary>
    /// <returns>�ݒu�\���ǂ���</returns>
    protected virtual bool PreviewPlacing()
    {
        // �v�Z�덷�p�̒萔
        const float CALC_ERROR_OFFSET = 0.01f;
        // �ݒu�ł���i���̍����i���̍����܂ŁA���R�ƕ␳�����j
        float stepHeightLimit = playerHeight / 4;

        // �����̐g����4����1�͏�����
        // GameObject.transform�̓��f���ɂ���ĈႤ�̂ŁAy���␳�̍ۂ͂��ׂđ����Z�ōs��

        // Ghost�i�����j�̒��S����ё����̍��W���擾
        // ���f���ɂ���āA���_�̈ʒu���Ⴄ�����Abounds�ɂ���Ĉꉞ��������
        Vector3 center = boxCollider.bounds.center;
        Vector3 underOrigin = new Vector3(center.x, center.y - boxHalfSize.y, center.z);
        float playerUnderOriginY = player.position.y - playerHeight / 2;

        // �ڒn����
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

        // Ray�����������ʒu��y���̍��W�Ƃ���
        // ����Ray��������Ȃ� = ��������ʒu�ɂ���Ƃ��́u�ݒu�ł��Ȃ��v�ƕ\�����邽�߁A�v���C���[�Ɠ��������ŃL�[�v����i��������j
        yPosition = isHitGroundBox
            ? groundHitInfo.point.y + GROUND_OFFSET
            : playerUnderOriginY;

        // ���_�������ɂȂ��ꍇ�͖��܂��Ă��܂��̂ŁA���̏ꍇ�͕␳����
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

        // Ray�����ׂē������Ă��� = ���R�ɖʂ����A�����ɐݒu�\�ȏ�Ԃł��邩
        bool isPerfectlyGrounded = isHitGround_rf && isHitGround_lf && isHitGround_rb && isHitGround_lb;
        if (!isPerfectlyGrounded)
        {
            return false;
        }
        // 1�ɂ���ƌ덷���o��̂ł�����ƈ����Ă�
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
        // �v���C���[�̓��̌�����Ray���΂��AWarpPointer�̗v�̂Œn�ʂ����̏ォ�𔻒肷��B
        // ���̃R���C�_�[�̏㔼���ɂ�����������̏ゾ���A�������ɓ���������n�ʁB
        // ���܂�ɂ����̕ǂ����������璌�Ɣ��f���Areturn

        return true;
    }

    protected virtual void OnSigned(InputAction.CallbackContext context)
    {
        // �I�u�W�F�N�g�i�S�[�X�g�j���g�̓]�񏈗�
        // �{�^���������Ă���ԉ��
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
