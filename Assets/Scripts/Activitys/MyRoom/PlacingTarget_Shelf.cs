using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacingTarget_Shelf : PlacingTarget
{
	private const float MOVE_SPEED = 3f; 
	private Vector2 inputDir = default;

	public override PlacingTarget Initialize(IEditOnlyGhost ghostModel, PlaceableObject placeableObject, Transform player)
	{
		var result = base.Initialize(ghostModel, placeableObject, player);
		Inputter.Player.Move.performed += OnMove;
		Inputter.Player.Move.canceled += OnMoveCancel;
		return result;
	}

	protected override void LateUpdate()
	{
		ghostModel.SetPlaceableState(PreviewPlacing());

		UpdateAction?.Invoke();

		transform.position = new Vector3(xPosition, yPosition, zPosition) + player.forward * forwardOffset;
		// �v���C���[�̓]��ɍ��킹��rotation�ƁA�I�u�W�F�N�g���g�̓]����}�[�W
		transform.rotation = Quaternion.Euler(new Vector3(player.rotation.x, player.rotation.eulerAngles.y + rotateAngle, player.rotation.z));
	}

	protected override bool PreviewPlacing()
	{
		// �v�Z�덷�p�̒萔
		const float CALC_ERROR_OFFSET = 0.01f;

		// �����̐g����4����1�͏�����
		// GameObject.transform�̓��f���ɂ���ĈႤ�̂ŁAy���␳�̍ۂ͂��ׂđ����Z�ōs��

		// Ghost�i�����j�̒��S����ё����̍��W���擾
		// ���f���ɂ���āA���_�̈ʒu���Ⴄ�����Abounds�ɂ���Ĉꉞ��������
		Vector3 center = boxCollider.bounds.center;
		Vector3 underOrigin = new Vector3(center.x, center.y - boxHalfSize.y, center.z);

		// �ڒn����
		bool isHitShelfBoard = Physics.BoxCast(
			center: center,
			halfExtents: boxHalfSize,
			direction: Vector3.down,
			hitInfo: out RaycastHit shelfHitInfo,
			orientation: transform.rotation,
			maxDistance: boxHalfSize.y
			);
		// �ǂɐG��Ă���ʂ̒��S�_���擾�iRaycastHit.point�ł̓����_���ȓ_���Ԃ邽�߁A���e���g�����߂�j
		Vector3 hitPoint = center + Vector3.Project(shelfHitInfo.point - center, -shelfHitInfo.normal);

		float rayDistance = Vector3.Distance(hitPoint, center) + CALC_ERROR_OFFSET;
		Ray checkGroundRay_rf = new Ray(underOrigin + boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + boxHalfSize.z * transform.forward, Vector3.down);
		Ray checkGroundRay_lf = new Ray(underOrigin + -boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + boxHalfSize.z * transform.forward, Vector3.down);
		Ray checkGroundRay_rb = new Ray(underOrigin + boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + -boxHalfSize.z * transform.forward, Vector3.down);
		Ray checkGroundRay_lb = new Ray(underOrigin + -boxHalfSize.x * transform.right + boxHalfSize.y * transform.up + -boxHalfSize.z * transform.forward, Vector3.down);
		bool isHitGround_rf = Physics.Raycast(checkGroundRay_rf, out RaycastHit _, rayDistance);
		bool isHitGround_lf = Physics.Raycast(checkGroundRay_lf, out RaycastHit _, rayDistance);
		bool isHitGround_rb = Physics.Raycast(checkGroundRay_rb, out RaycastHit _, rayDistance);
		bool isHitGround_lb = Physics.Raycast(checkGroundRay_lb, out RaycastHit _, rayDistance);
#if UNITY_EDITOR
		Debug.DrawRay(checkGroundRay_rf.origin, checkGroundRay_rf.direction * rayDistance, Color.yellow);
		Debug.DrawRay(checkGroundRay_lf.origin, checkGroundRay_lf.direction * rayDistance, Color.yellow);
		Debug.DrawRay(checkGroundRay_rb.origin, checkGroundRay_rb.direction * rayDistance, Color.yellow);
		Debug.DrawRay(checkGroundRay_lb.origin, checkGroundRay_lb.direction * rayDistance, Color.yellow);
#endif

		if (Vector3.Angle(Vector3.up, shelfHitInfo.normal) > slopeLimit)
		{
			return false;
		}

		Vector2 movePos = Time.deltaTime * inputDir * MOVE_SPEED;
		xPosition += movePos.x;
		zPosition += movePos.y;

		// Ray�����������ʒu��y���̍��W�Ƃ���
		// ����Ray��������Ȃ� = ��������ʒu�ɂ���Ƃ��́u�ݒu�ł��Ȃ��v�ƕ\�����邽�߁A�v���C���[�Ɠ��������ŃL�[�v����i��������j
		yPosition = isHitShelfBoard
			? shelfHitInfo.point.y + CALC_ERROR_OFFSET
			: default;

		// ���_�������ɂȂ��ꍇ�͖��܂��Ă��܂��̂ŁA���̏ꍇ�͕␳����
		if (placeableObject.PivotType == GhostModel.PivotType.Center)
		{
			yPosition += boxHalfSize.y;
		}

		// Ray�����ׂē������Ă��� = ���R�ɖʂ����A�����ɐݒu�\�ȏ�Ԃł��邩
		bool isPerfectlyGrounded = isHitGround_rf && isHitGround_lf && isHitGround_rb && isHitGround_lb;
		if (!isPerfectlyGrounded)
		{
			return false;
		}
		// 1�ɂ���ƌ덷���o��̂ł�����ƈ����Ă�
		if (shelfHitInfo.normal.y < 1 - CALC_ERROR_OFFSET)
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

		return true;
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		inputDir = context.ReadValue<Vector2>();
	}

	private	void OnMoveCancel(InputAction.CallbackContext context)
	{
		inputDir = Vector2.zero;
	}
}
