using DG.Tweening;
using System.Diagnostics;
using UniRx;
using UnityEngine;

/// <summary>
/// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.1/manual/tunneling-vignette-controller.html">
/// TunnelingVignette</see>�̊Ǘ��N���X
/// <para>
/// ���̃R�[�h�̓I���W�i���R�[�h�ł��B
/// <br>��L�̃T���v���iXR.Interaction.Toolkit StarterAsset�j��TunnelingVignetteManager���܂������V�����g�ݑւ��Ă��܂��B</br>
/// </para>
/// <exsample>
/// �T���v���ɂ��鑽���̗]���ȋ@�\��r�����A���V���v���ɂ��Ă��܂��B
/// <br>�ȉ��͋�̓I�ȕύX�_�ł��B</br>
/// <para>
/// �E�Ǝ��̃v���C���[�����ɍ��킹�邽�߁ALocomotionVignetteProviders���폜���܂����B
/// <br>�E�eLocomotion�ł̃p�����[�^�̃I�[�o�[���C�h�@�\�����폜���܂����B</br>
/// <br>�EEasing�ɂ���DOTween��p���A�ȈՓI�Ȏ����ɂ��܂����B</br>
/// </para>
/// </exsample>
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class TunnelingVignetteManager : MonoBehaviour
{
    private static class ShaderPropertyLookup
    {
        public static readonly int apertureSize = Shader.PropertyToID("_ApertureSize");
        public static readonly int featheringEffect = Shader.PropertyToID("_FeatheringEffect");
        public static readonly int vignetteColor = Shader.PropertyToID("_VignetteColor");
        public static readonly int vignetteColorBlend = Shader.PropertyToID("_VignetteColorBlend");
    }

    [SerializeField] private VRPlayerController VRPC = default;
    [Tooltip("�ړ�����TunnelingVignette��L�������邩�ǂ���")]
    [SerializeField] private bool enabledOnMoving = true;
    [Tooltip("�ړ�����Easein�̏��v����[s]")]
    [SerializeField, Min(0f)] private float easeInDurationOnMoving = 0.15f;
    [Tooltip("�ړ�����EaseOut�̏��v����[s]")]
    [SerializeField, Min(0f)] private float easeOutDurationOnMoving = 0.15f;
    [Tooltip("�W�����v����TunnelingVignette��L�������邩�ǂ���")]
    [SerializeField] private bool enabledOnJumping = true;
    [Tooltip("�W�����v����EaseIn�̏��v����[s]")]
    [SerializeField, Min(0f)] private float easeInDurationOnJumping = 0.10f;
    [Tooltip("�W�����v����EaseOut�̏��v����[s]")]
    [SerializeField, Min(0f)] private float easeOutDurationOnJumping = 0.10f;

    private MeshRenderer meshRenderer = default;
    private MaterialPropertyBlock vignettePropertyBlock = default;
    private float initialApertureSize = default;


    [Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        VRPC ??= GetComponentInParent<VRPlayerController>();
    }

    private void Awake()
    {
        #region Cache
        meshRenderer = GetComponent<MeshRenderer>();
        vignettePropertyBlock = new MaterialPropertyBlock();
        #endregion

        // Shader�̃v���p�e�B�ɏ����l����
        initialApertureSize = 0.7f;

        // Shader�̃v���p�e�B�ɏ����l��ݒ�
        meshRenderer.GetPropertyBlock(vignettePropertyBlock);
        vignettePropertyBlock.SetFloat(ShaderPropertyLookup.apertureSize, 1f);
        meshRenderer.SetPropertyBlock(vignettePropertyBlock);

        #region Subscribe
        VRPC.IsMovingRP
            // ����������̔��s�͖���
            .Skip(1)
            // Filter: �L�����ݒ肪����Ă��� ���� �W�����v���łȂ�
            .Where(isMoving => enabledOnMoving && !VRPC.IsJumpingRP.Value)
            .Subscribe(isMoving =>
            {
                // �ړ��J�n����EaseIn
                if (isMoving)
                {
                    EaseInTunnelingVignette(easeInDurationOnMoving);
                }
                // �ړ��I������EaseOut
                else
                {
                    EaseOutTunnelingVignette(easeOutDurationOnMoving);
                }
            })
            .AddTo(this);

        VRPC.IsJumpingRP
            // ����������̔��s�͖���
            .Skip(1)
            // Filter: �L�����ݒ肪����Ă��� ���� �ړ����łȂ�
            .Where(isJumping => enabledOnJumping && !VRPC.IsMovingRP.Value)
            .Subscribe(isJumping =>
            {
                // �W�����v�J�n����EaseIn
                if (isJumping)
                {
                    EaseInTunnelingVignette(easeInDurationOnJumping);
                }
                // ���n����EaseOut
                else
                {
                    EaseOutTunnelingVignette(easeOutDurationOnJumping);
                }
            })
            .AddTo(this);

        VRPC.MoveTypeRP.Subscribe(moveType =>
        {
            switch (moveType)
            {
                case VRMoveType.Natural:
                    meshRenderer.enabled = true;
                    break;

                case VRMoveType.Warp:
                    meshRenderer.enabled = false;
                    break;
            }
        });
        #endregion
    }

    /// <summary>
    /// TunnelingVignette��EaseIn�ŏo��������
    /// </summary>
    private void EaseInTunnelingVignette(float easeDuration)
    {
        // Renderer����MaterialPropertyBlock���擾
        meshRenderer.GetPropertyBlock(vignettePropertyBlock);

        // DOTween�Ńt�F�[�h�C����\��
        DOVirtual.Float(from: 1f, to: initialApertureSize, duration: easeDuration, onVirtualUpdate: value =>
        {
            // �l�̍X�V
            vignettePropertyBlock.SetFloat(ShaderPropertyLookup.apertureSize, value);
            meshRenderer.SetPropertyBlock(vignettePropertyBlock);
        });
    }

    /// <summary>
    /// TunnelingVignette��EaseOut�ŏ��ł�����
    /// </summary>
    private void EaseOutTunnelingVignette(float easeDuration)
    {
        // Renderer����MaterialPropertyBlock���擾
        meshRenderer.GetPropertyBlock(vignettePropertyBlock);

        // DOTween�Ńt�F�[�h�A�E�g��\��
        DOVirtual.Float(from: initialApertureSize, to: 1f, duration: easeDuration, onVirtualUpdate: value =>
        {
            // �l�̍X�V
            vignettePropertyBlock.SetFloat(ShaderPropertyLookup.apertureSize, value);
            meshRenderer.SetPropertyBlock(vignettePropertyBlock);
        });
    }
}