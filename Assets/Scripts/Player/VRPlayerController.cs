using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

/// <summary>
/// �v���C���[�̋����������N���X�iVR)
/// </summary>
public class VRPlayerController : PlayerControllerBase<VRPlayerDataAsset>, IDependencyInjector<PlayerHandDependencyInfomation>
{
	[SerializeField]
	private Transform centerEyeTransform = default;
	[SerializeField]
	private WhiteVignetteManager whiteVignetteManager = default;

	// Inspector�g����Atrribute���߁B���C�ɂȂ��炸�I
	[Header("�ړ�����")]
	[SerializeField, CustomField("Move Type", CustomFieldAttribute.DisplayType.Replace)]
	private MoveTypeReactiveProperty moveTypeRP = default;

	[SerializeField, HideForMoveType(nameof(moveTypeEditor), VRMoveType.Natural)]
	private WarpPointer warpPointer = default;


	[SerializeField, HideInInspector]
	private VRMoveType moveTypeEditor = default;

	[Tooltip("���E�ǂ���ɉ�]���邩")]
	private FloatReactiveProperty lookDirX_RP = default;
	private bool canWarp = default;

	private Transform leftHand = default;
	private Vector3 warpPos = default;
	private IDisposable isMovingDisposable = default;

	public IReadOnlyReactiveProperty<VRMoveType> MoveTypeRP => moveTypeRP;


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
			// Filter: LookAction�̍��E�����ꂩ�����͂��ꂽ�Ƃ��iNuetral�͒e���j
			.Where(value => value != 0f)
			// �v���C���[����]������
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
				// Natural�ɕς�����Ƃ��A�w�ǂ����݂����Dispose
				if (value == VRMoveType.Natural)
				{
					isMovingDisposable?.Dispose();
				}
				// Warp�ɕς�����Ƃ��AIsMoving�C�x���g���w�ǂ���
				else
				{
					isMovingDisposable = isMovingRP.Subscribe(isMoving =>
					{
						// ���̗͂L���ɂ���ăr���[�̏�Ԃ�؂�ւ���
						// �����A�󒆂ł͂�������͂������Ă��o���Ă͂����Ȃ����߁A���������t����
						warpPointer.SetActive(isMoving);
					})
					.AddTo(this);
				}
			});
	}

	protected override void Update()
	{
		base.Update();

#if UNITY_EDITOR
		// �}�E�X�ł̓��͂͒e��
		if (lastLookedDevice == DeviceType.Mouse)
		{
			return;
		}
#endif

		// Look�i��]�j�̓��͂�[-1f, 0f, 1f]�ɉ��H���������
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
	/// ���[�v�ړ�
	/// </summary>
	private void WarpMove()
	{
		// ���͂�����Ƃ���WarpPointer���o����
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
		// ���[�v�����̂Ńr���[�͔�\����
		warpPointer.SetActive(false);

		// ���[�v��ɓ��͂���Ă����������������߂̌v�Z
		// VR�̓s�������͏�肭�����Ȃ��̂ŁA�u���������p�x�v - �uCenterEye�̊p�x�v�����߁A�]�񂷂�
		// ��Ɍv�Z���Ȃ��ƁAawait���ɂ��낢��ς���Ă��܂�
		float targetRotationY = Calculator.GetEulerBy2DVector(moveDir, Vector3.down).y;
		float centerEyeRotationY = centerEyeTransform.rotation.y;

		// ���W���X�V�i���[�v�I�j
		await Warp(warpPos);

		// �������X�V
		myTransform.Rotate(Vector3.up * (targetRotationY - centerEyeRotationY));
	}

	/// <summary>
	/// ���[�v�����iRotation�͊e���ł���āj
	/// </summary>
	/// <param name="warpPos">�ڕW���W</param>
	public async UniTask Warp(Vector3 warpPos)
	{
		// �z���C�g�A�E�g�̉��o�i��ʂ���x�����Ȃ��Ȃ�܂őҋ@�j
		await whiteVignetteManager.WhiteOut();

		// ���W���X�V�i���[�v�I�j
		// ���̂܂܂�WarpPos���ƒn�ʂɖ��܂����Ⴄ�̂ŁA�����ɗ���悤�␳
		// �Փ˔�����ꎞ�I��OFF�ɂ���
		characterController.detectCollisions = false;
		Vector3 correctedWarpPos = warpPos + Vector3.up * (characterController.height / 2 + characterController.skinWidth);
		characterController.Move(correctedWarpPos - myTransform.position);
		characterController.detectCollisions = true;
	}

	/// <summary>
	/// Look�i��]�j�̓��͂��������Ƃ��A�v���C���[����]������
	/// </summary>
	/// <param name="leftOrRight">���E��\�������i-1f, 1f�̂����ꂩ�j</param>
	private void OnRotate(float leftOrRight)
	{
		myTransform.Rotate(Vector3.up * (playerDataAsset.RotateAngle * leftOrRight));
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