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
		// プレイヤーの転回に合わせたrotationと、オブジェクト自身の転回をマージ
		transform.rotation = Quaternion.Euler(new Vector3(player.rotation.x, player.rotation.eulerAngles.y + rotateAngle, player.rotation.z));
	}

	protected override bool PreviewPlacing()
	{
		// 計算誤差用の定数
		const float CALC_ERROR_OFFSET = 0.01f;

		// Ghost（自分）の中心および足元の座標を取得
		// モデルによって、原点の位置が違う問題を、boundsによって一応解決した
		Vector3 center = boxCollider.bounds.center;
		Vector3 underOrigin = new Vector3(center.x, center.y - boxHalfSize.y, center.z);

		// 棚の場合の接地判定Rayの長さはなんでもいいので、適当な長さ
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

		// 原点が足元にない場合は埋まってしまうので、その場合は補正する
		if (placeableObject.PivotType == GhostModel.PivotType.Center)
		{
			yPosition += boxHalfSize.y;
		}

		// Rayがすべて当たっている = 坂や崖に面さず、完璧に設置可能な状態であるか
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
				// 設置中は棚のコライダーをすべてOFFにする
				// ただし、棚板のコライダーには干渉したくないため、親オブジェクトにとどめる
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
				// OFFにしたコライダーをすべてONに戻す
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

		// 設置完了後、OFFにしたコライダーをすべてONに戻す
		foreach (var collider in shelfColliders)
		{
			collider.enabled = true;
		}
	}
}
