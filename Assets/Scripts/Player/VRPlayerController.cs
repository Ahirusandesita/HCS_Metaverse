using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �v���C���[�̋����������N���X�iVR)
/// </summary>
public class VRPlayerController : PlayerControllerBase<VRPlayerDataAsset>
{
    [SerializeField] private Transform centerEyeTransform = default;

    [Tooltip("���E�ǂ���ɉ�]���邩")]
    private FloatReactiveProperty lookDirX_RP = default;


    protected override void Reset()
    {
        base.Reset();

        try
        {
            centerEyeTransform ??= transform.Find("CenterEyeAnchor").transform;
        }
        catch (NullReferenceException) { }
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

    /// <summary>
    /// Look�i��]�j�̓��͂��������Ƃ��A�v���C���[����]������
    /// </summary>
    /// <param name="leftOrRight">���E��\�������i-1f, 1f�̂����ꂩ�j</param>
    private void OnRotate(float leftOrRight)
    {
        myTransform.Rotate(Vector3.up * (playerDataAsset.RotateAngle * leftOrRight));
    }
}
