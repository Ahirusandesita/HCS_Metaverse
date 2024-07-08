using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �v���C���[�̋����������N���X�iVR)
/// </summary>
public class VRPlayerController : PlayerControllerBase<VRPlayerDataAsset>, IDependencyInjector<PlayerHandDependencyInfomation>
{
    [SerializeField]
    private Transform centerEyeTransform = default;

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

    private Transform leftHand = default;

    private Vector3 warpPos = default;
    private IDisposable isMovingDisposable = default;


    protected override void Reset()
    {
        base.Reset();
        centerEyeTransform ??= transform.Find("CenterEyeAnchor").transform;
        warpPointer ??= GetComponentInChildren<WarpPointer>();
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

        Inputter.Player.Warp.performed += _ =>
        {
            if (IsMovingRP.Value)
            {
                Warp();
            }
        };

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
                    isMovingDisposable = IsMovingRP.Subscribe(isMoving =>
                    {
                        // ���̗͂L���ɂ���ăr���[�̏�Ԃ�؂�ւ���
                        warpPointer.SetActive(isMoving);
                        //warpSymbol.enabled = isMoving;
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

                // �W�����v�����͗L�����������̂ŃW�����v�������L�q
                // () is order optimizated
                characterController.Move(Vector3.up * (verticalVelocity * Time.deltaTime));
                break;
        }
    }

    /// <summary>
    /// ���[�v�ړ�
    /// </summary>
    private void WarpMove()
    {
        if (!IsMovingRP.Value)
        {
            return;
        }

        bool canWarp = warpPointer.Draw(leftHand.position, leftHand.forward, ref warpPos);
        //warpSymbol.transform.position = warpPos;

    }

    private void Warp()
    {
        myTransform.position = warpPos;
        //myTransform.rotation = 
    }

    /// <summary>
    /// Look�i��]�j�̓��͂��������Ƃ��A�v���C���[����]������
    /// </summary>
    /// <param name="leftOrRight">���E��\�������i-1f, 1f�̂����ꂩ�j</param>
    private void OnRotate(float leftOrRight)
    {
        myTransform.Rotate(Vector3.up * (playerDataAsset.RotateAngle * leftOrRight));
    }

    void IDependencyInjector<PlayerHandDependencyInfomation>.Inject(PlayerHandDependencyInfomation information)
    {
        leftHand = information.LeftHand;
    }
}
