using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

/// <summary>
/// プレイヤーの挙動を扱うクラス（VR)
/// </summary>
public class VRPlayerController : PlayerControllerBase<VRPlayerDataAsset>, IDependencyInjector<PlayerHandDependencyInfomation>
{
	[SerializeField]
	private Transform centerEyeTransform = default;
	[SerializeField]
	private WhiteVignetteManager whiteVignetteManager = default;

	// Inspector拡張のAtrribute多め。お気になさらず！
	[Header("移動方式")]
	[SerializeField, CustomField("Move Type", CustomFieldAttribute.DisplayType.Replace)]
	private MoveTypeReactiveProperty moveTypeRP = default;

	[SerializeField, HideForMoveType(nameof(moveTypeEditor), VRMoveType.Natural)]
	private WarpPointer warpPointer = default;

	[SerializeField, HideInInspector]
	private VRMoveType moveTypeEditor = default;

	[SerializeField, CustomField("Rotate Type", CustomFieldAttribute.DisplayType.Replace)]
	private RotateTypeReactiveProperty rotateTypeRP = default;

	[Tooltip("左右どちらに回転するか")]
	private FloatReactiveProperty lookDirX_RP = default;
	private bool canWarp = default;

	[Tooltip("カメラの勾配（垂直方向の角度）")]
	private float cinemachineTargetPitch = default;

	private Transform leftHand = default;
	private Vector3 warpPos = default;
	private IDisposable isMovingDisposable = default;

	public IReadOnlyReactiveProperty<VRMoveType> MoveTypeRP => moveTypeRP;
	public IReadOnlyReactiveProperty<VRRotateType> RotateTypeRP => rotateTypeRP;


	protected override void Reset()
	{
		base.Reset();
		centerEyeTransform ??= transform.Find("CenterEyeAnchor").transform;
		warpPointer ??= GetComponentInChildren<WarpPointer>();
		whiteVignetteManager ??= GetComponentInChildren<WhiteVignetteManager>();
	}

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		moveTypeEditor = moveTypeRP.Value;
	}

	protected override void Awake()
	{
		base.Awake();

		followTransform = centerEyeTransform;

		// Subscribe
		lookDirX_RP = new FloatReactiveProperty().AddTo(this);
		lookDirX_RP
			.Where(value => rotateTypeRP.Value == VRRotateType.Degital)
			// Filter: LookActionの左右いずれかが入力されたとき（Nuetralは弾く）
			.Where(value => value != 0f)
			// プレイヤーを回転させる
			.Subscribe(value => OnRotate(value));

#if UNITY_EDITOR
		PlayerActions.Look.AddCompositeBinding("2DVector")
			.With("Left", "<Keyboard>/k")
			.With("Right", "<Keyboard>/semicolon");
		PlayerActions.Look.AddCompositeBinding("2DVector")
			.With("Left", "<Mouse>/leftButton")
			.With("Right", "<Mouse>/rightButton");
#endif

		moveTypeRP
			.AddTo(this)
			.Subscribe(value =>
			{
				// Naturalに変わったとき、購読が存在すればDispose
				if (value == VRMoveType.Natural)
				{
					isMovingDisposable?.Dispose();
				}
				// Warpに変わったとき、IsMovingイベントを購読する
				else
				{
					isMovingDisposable = isMovingRP.Subscribe(isMoving =>
					{
						// 入力の有無によってビューの状態を切り替える
						// ただ、空中ではいくら入力があっても出せてはいけないため、複数条件付ける
						warpPointer.SetActive(isMoving);
					})
					.AddTo(this);
				}
			});
		rotateTypeRP.AddTo(this);
	}

	protected override void Update()
	{
		base.Update();

#if UNITY_EDITOR
		// マウスでの入力は弾く
		if (lastLookedDevice == DeviceType.Mouse)
		{
			return;
		}
#endif

		// Look（回転）の入力を[-1f, 0f, 1f]に加工し代入する
		lookDirX_RP.Value = lookDir.x == 0f
			? 0f
			: Mathf.Sign(lookDir.x);
	}

	protected override void Move()
	{
		switch (moveTypeRP.Value)
		{
			case VRMoveType.Natural:
				base.Move();
				break;

			case VRMoveType.Warp:
				WarpMove();
				break;
		}
	}

	/// <summary>
	/// ワープ移動
	/// </summary>
	private void WarpMove()
	{
		// 入力があるときにWarpPointerを出せる
		if (isMovingRP.Value)
		{
			canWarp = warpPointer.Draw(leftHand.position, leftHand.forward, ref warpPos, moveDir);
		}
		else
		{
			canWarp = false;
		}
	}

	private async UniTaskVoid WarpAtPointer()
	{
		// ワープしたのでビューは非表示に
		warpPointer.SetActive(false);

		// ワープ後に入力されていた方向を向くための計算
		// VRの都合上代入は上手くいかないので、「向きたい角度」 - 「CenterEyeの角度」を求め、転回する
		// 先に計算しないと、await中にいろいろ変わってしまう
		float targetRotationY = Calculator.GetEulerBy2DVector(moveDir, Vector3.down).y;
		float centerEyeRotationY = centerEyeTransform.rotation.y;

		// 座標を更新（ワープ！）
		await Warp(warpPos);

		// 方向を更新
		myTransform.Rotate(Vector3.up * (targetRotationY - centerEyeRotationY));
	}

	/// <summary>
	/// ワープ処理（Rotationは各自でやって）
	/// </summary>
	/// <param name="warpPos">目標座標</param>
	public async UniTask Warp(Vector3 warpPos)
	{
		// ホワイトアウトの演出（画面が一度見えなくなるまで待機）
		await whiteVignetteManager.WhiteOut();

		// 座標を更新（ワープ！）
		// そのままのWarpPosだと地面に埋まっちゃうので、足元に来るよう補正
		// 衝突判定を一時的にOFFにするために、最後にその場ワープ
		characterController.enabled = false;
		Vector3 correctedWarpPos = warpPos + Vector3.up * (characterController.height / 2 + characterController.skinWidth);
		transform.position = correctedWarpPos;
		characterController.enabled = true;
		// ワープ後に衝突判定が欲しいため、その場に移動
		characterController.Move(Vector3.zero);
	}

	/// <summary>
	/// Look（回転）の入力があったとき、プレイヤーを回転させる
	/// </summary>
	/// <param name="leftOrRight">左右を表す符号（-1f, 1fのいずれか）</param>
	private void OnRotate(float leftOrRight)
	{
		myTransform.Rotate(Vector3.up * (playerDataAsset.RotateAngle * leftOrRight));
	}

	protected override void CameraRotation()
	{
		if (rotateTypeRP.Value == VRRotateType.Degital)
		{
			return;
		}

		// Inputがない場合、処理を終了
		if (lookDir == Vector2.zero)
		{
			return;
		}

		// マウスの入力にはTime.deltaTimeを掛けない
		float deltaTimeMultiplier = lastLookedDevice == DeviceType.Mouse
			? 1.0f
			: Time.deltaTime;

		// y軸の入力量に応じて、カメラの勾配（垂直方向の角度）を加算する
		cinemachineTargetPitch += lookDir.y * playerDataAsset.RotationSpeed * deltaTimeMultiplier;
		// カメラの勾配をクランプする
		cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, playerDataAsset.VerticalMinAngle, playerDataAsset.VerticalMaxAngle);

		// x軸の入力方向への回転を取得
		float rotationVelocity = lookDir.x * playerDataAsset.RotationSpeed * deltaTimeMultiplier;

		if (lastLookedDevice == DeviceType.Mouse)
		{
			// CinemachineCameraのtargetの回転を更新
			centerEyeTransform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);
		}

		// プレイヤーを左右に回転させる
		myTransform.Rotate(Vector3.up * rotationVelocity);
	}

	protected override void OnSprintOrWarp(InputAction.CallbackContext context)
	{
		switch (moveTypeRP.Value)
		{
			case VRMoveType.Natural:
				base.OnSprintOrWarp(context);
				break;

			case VRMoveType.Warp:
				if (canWarp)
				{
					WarpAtPointer().Forget();
				}
				break;
		}
	}

	protected override void OnSprintOrWarpCancel(InputAction.CallbackContext context)
	{
		switch (moveTypeRP.Value)
		{
			case VRMoveType.Natural:
				base.OnSprintOrWarpCancel(context);
				break;

			case VRMoveType.Warp:
				break;
		}
	}

	void IDependencyInjector<PlayerHandDependencyInfomation>.Inject(PlayerHandDependencyInfomation information)
	{
		leftHand = information.LeftHand;
	}
}
public static class VRPlayerControllerExtens
{
	public static async UniTask Warp(this VRPlayerController VRPlayerController, Vector3 position, Quaternion rotation)
	{
		VRPlayerController.transform.rotation = rotation;
		await VRPlayerController.Warp(position);
	}
}