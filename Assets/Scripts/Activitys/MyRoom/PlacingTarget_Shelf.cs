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
		Vector3 underOrigin = new Vector3(center.x, center.y - boxHalfSize.y, center.z);

		// 接地判定
		bool isHitShelfBoard = Physics.BoxCast(
			center: center,
			halfExtents: boxHalfSize,
			direction: Vector3.down,
			hitInfo: out RaycastHit shelfHitInfo,
			orientation: transform.rotation,
			maxDistance: boxHalfSize.y
			);
		// 壁に触れている面の中心点を取得（RaycastHit.pointではランダムな点が返るため、投影を使い求める）
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

		// Rayが当たった位置をy軸の座標とする
		// もしRayが当たらない = 高すぎる位置にいるときは「設置できない」と表現するため、プレイヤーと同じ高さでキープする（浮かせる）
		yPosition = isHitShelfBoard
			? shelfHitInfo.point.y + CALC_ERROR_OFFSET
			: default;

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
		// 1にすると誤差が出るのでちょっと引いてる
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
