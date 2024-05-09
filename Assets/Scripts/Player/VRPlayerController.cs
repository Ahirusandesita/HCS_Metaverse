using UniRx;
using UnityEngine;
using System;

/// <summary>
/// �v���C���[�̋����������N���X�iVR)
/// </summary>
public class VRPlayerController : PlayerControllerBase<VRPlayerDataAsset>
{
    [Tooltip("���E�ǂ���ɉ�]���邩")]
    private FloatReactiveProperty lookDirX_RP = default;

    protected override void Awake()
    {
        base.Awake();

        // Subscribe
        lookDirX_RP = new FloatReactiveProperty().AddTo(this);
        lookDirX_RP
            // Filter: LookAction�̍��E�����ꂩ�����͂��ꂽ�Ƃ��iNuetral�͒e���j
            .Where(value => value != 0f)
            // �v���C���[����]������
            .Subscribe(value => OnRotate(value));
    }

    protected override void Update()
    {
        base.Update();

#if UNITY_EDITOR
        // �}�E�X�ł̓��͂͒e��
        if (inputter.LastLookedDevice == Inputter.DeviceType.Mouse)
        {
            return;
        }
#endif

        // Look�i��]�j�̓��͂�[-1f, 0f, 1f]�ɉ��H���������
        lookDirX_RP.Value = inputter.LookDir.x == 0f
            ? 0f
            : Mathf.Sign(inputter.LookDir.x);
    }

    /// <summary>
    /// Look�i��]�j�̓��͂��������Ƃ��A�v���C���[����]������
    /// </summary>
    /// <param name="leftOrRight">���E��\�������i-1f, 1f�̂����ꂩ�j</param>
    private void OnRotate(float leftOrRight)
    {
        myTransform.Rotate(Vector3.up * (15f * leftOrRight));
    }
}
