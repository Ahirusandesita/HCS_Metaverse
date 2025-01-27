using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacingTarget_Shelf : PlacingTarget, IInteractionInfoReceiver
{
	private const float MOVE_SPEED = 3f;
	private Vector2 inputDir = default;
	private Collider[] shelfColliders = default;
	private IReadOnlyList<BoxCollider> shelfBoards = default;
	private int focusBoardIndex = 0;

	public override PlacingTarget Initialize(IEditOnlyGhost ghostModel, PlaceableObject placeableObject, Transform player)
	{
		base.Initialize(ghostModel, placeableObject, player);
		Inputter.Player.Move.performed += OnMove;
		Inputter.Player.Move.canceled += OnMoveCancel;
		Inputter.PlacingMode.NextOrPrevious.performed += _ =>
		{
			XDebug.Log(_.ReadValue<float>());
		};
		FindObjectOfType<PlayerInteraction>().Add(this);
		return this;
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

		// Ghost�i�����j�̒��S����ё����̍��W���擾
		// ���f���ɂ���āA���_�̈ʒu���Ⴄ�����Abounds�ɂ���Ĉꉞ��������
		Vector3 center = boxCollider.bounds.center;
		Vector3 underOrigin = new Vector3(center.x, center.y - boxHalfSize.y, center.z);

		// �I�̏ꍇ�̐ڒn����Ray�̒����͂Ȃ�ł������̂ŁA�K���Ȓ���
		float rayDistance = boxHalfSize.y / 2;
		Ray checkGroundRay_rf = new Ray(underOrigin + boxHalfSize.x * transform.right + boxHalfSize.z * transform.forward, Vector3.down);
		Ray checkGroundRay_lf = new Ray(underOrigin + -boxHalfSize.x * transform.right + boxHalfSize.z * transform.forward, Vector3.down);
		Ray checkGroundRay_rb = new Ray(underOrigin + boxHalfSize.x * transform.right + -boxHalfSize.z * transform.forward, Vector3.down);
		Ray checkGroundRay_lb = new Ray(underOrigin + -boxHalfSize.x * transform.right + -boxHalfSize.z * transform.forward, Vector3.down);
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

		Vector2 movePos = Time.deltaTime * inputDir * MOVE_SPEED;
		xPosition += movePos.x;
		zPosition += movePos.y;

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

		if (isCollision)
		{
			return false;
		}

		return true;
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		inputDir = context.ReadValue<Vector2>();
	}

	private void OnMoveCancel(InputAction.CallbackContext context)
	{
		inputDir = Vector2.zero;
	}

	void IInteractionInfoReceiver.SetInfo(IInteraction.InteractionInfo interactionInfo)
	{
		if (interactionInfo is Shelf.ShelfInteractionInfo shelfInteractionInfo)
		{
			shelfInteractionInfo.OnSafetyOpenAction += OnSafetyOpen;
			shelfInteractionInfo.OnSafetyCloseAction += OnSafetyClose;
		}

		void OnSafetyOpen(SafetyInteractionObject.SafetyInteractionInfo.OnSafetyActionInfo data)
		{
			if (data is Shelf.ShelfInteractionInfo.OnShelfInteractionInfo onShelfInteractionInfo)
			{
				shelfBoards = onShelfInteractionInfo.shelfBoards;
				// �ݒu���͒I�̃R���C�_�[�����ׂ�OFF�ɂ���
				// �������A�I�̃R���C�_�[�ɂ͊��������Ȃ����߁A�e�I�u�W�F�N�g�ɂƂǂ߂�
				shelfColliders = onShelfInteractionInfo.shelf.GetComponents<Collider>();
				foreach (var collider in shelfColliders)
				{
					collider.enabled = false;
				}

				focusBoardIndex = 0;
				SetPosition();
			}
		}

		void OnSafetyClose(SafetyInteractionObject.SafetyInteractionInfo.OnSafetyActionInfo data)
		{
			if (data is Shelf.ShelfInteractionInfo.OnShelfInteractionInfo onShelfInteractionInfo)
			{
				// OFF�ɂ����R���C�_�[�����ׂ�ON�ɖ߂�
				foreach (var collider in shelfColliders)
				{
					collider.enabled = true;
				}

				shelfInteractionInfo.OnSafetyOpenAction -= OnSafetyOpen;
				shelfInteractionInfo.OnSafetyCloseAction -= OnSafetyClose;
			}
		}
	}

	private void SetPosition()
	{
		xPosition = shelfBoards[focusBoardIndex].bounds.center.x;
		yPosition = shelfBoards[focusBoardIndex].bounds.center.y + shelfBoards[focusBoardIndex].bounds.size.y / 2 + 0.01f;
		zPosition = shelfBoards[focusBoardIndex].bounds.center.z;
	}

	protected override void OnPlacing(InputAction.CallbackContext context)
	{
		base.OnPlacing(context);

		// �ݒu������AOFF�ɂ����R���C�_�[�����ׂ�ON�ɖ߂�
		foreach (var collider in shelfColliders)
		{
			collider.enabled = true;
		}
	}
}
