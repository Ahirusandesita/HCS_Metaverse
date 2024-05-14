using DG.Tweening;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

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
    [Tooltip("�ړ�����Easing�̏��v����[s]")]
    [SerializeField, Min(0f)] private float easeDurationOnMoving = 0.15f;
    [Tooltip("�W�����v����TunnelingVignette��L�������邩�ǂ���")]
    [SerializeField] private bool enabledOnJumping = true;
    [Tooltip("�W�����v����Easing�̏��v����[s]")]
    [SerializeField, Min(0f)] private float easeDurationOnJumping = 0.10f;

    private MeshRenderer meshRenderer = default;
    private MaterialPropertyBlock vignettePropertyBlock = default;
    private float initialApertureSize = default;
    private bool isEaseIn = false;
    private bool isEaseOut = false;


    private void Awake()
    {
        #region Cache
        meshRenderer = GetComponent<MeshRenderer>();
        vignettePropertyBlock = new MaterialPropertyBlock();
        #endregion

        // Shader�̃v���p�e�B�������l�Ƃ��Ď擾
        initialApertureSize = meshRenderer.material.GetFloat(ShaderPropertyLookup.apertureSize);

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
                    EaseInTunnelingVignette(easeDurationOnMoving);
                }
                // �ړ��I������EaseOut
                else
                {
                    EaseOutTunnelingVignette(easeDurationOnMoving);
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
                    EaseInTunnelingVignette(easeDurationOnJumping);
                }
                // ���n����EaseOut
                else
                {
                    EaseOutTunnelingVignette(easeDurationOnJumping);
                }
            })
            .AddTo(this);
        #endregion
    }

    [Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        VRPC ??= GetComponentInParent<VRPlayerController>();
    }

    /// <summary>
    /// TunnelingVignette��EaseIn�ŏo��������
    /// </summary>
    private void EaseInTunnelingVignette(float easeDuration)
    {
        if (isEaseIn)
        {
            return;
        }

        isEaseIn = true;
        // Renderer����MaterialPropertyBlock���擾
        meshRenderer.GetPropertyBlock(vignettePropertyBlock);

        // DOTween�Ńt�F�[�h�C����\��
        DOVirtual.Float(from: 1f, to: initialApertureSize, duration: easeDuration, onVirtualUpdate: value =>
        {
            // �l�̍X�V
            vignettePropertyBlock.SetFloat(ShaderPropertyLookup.apertureSize, value);
            meshRenderer.SetPropertyBlock(vignettePropertyBlock);
        })
        .OnComplete(() => isEaseIn = false);
    }

    /// <summary>
    /// TunnelingVignette��EaseOut�ŏ��ł�����
    /// </summary>
    private void EaseOutTunnelingVignette(float easeDuration)
    {
        if (isEaseOut)
        {
            return;
        }

        isEaseOut = true;
        // Renderer����MaterialPropertyBlock���擾
        meshRenderer.GetPropertyBlock(vignettePropertyBlock);

        // DOTween�Ńt�F�[�h�A�E�g��\��
        DOVirtual.Float(from: initialApertureSize, to: 1f, duration: easeDuration, onVirtualUpdate: value =>
        {
            // �l�̍X�V
            vignettePropertyBlock.SetFloat(ShaderPropertyLookup.apertureSize, value);
            meshRenderer.SetPropertyBlock(vignettePropertyBlock);
        })
        .OnComplete(() => isEaseOut = false);
    }
}
