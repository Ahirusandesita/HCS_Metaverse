using DG.Tweening;
using System.Diagnostics;
using UniRx;
using UnityEngine;

/// <summary>
/// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.1/manual/tunneling-vignette-controller.html">
/// TunnelingVignette</see>の管理クラス
/// <para>
/// このコードはオリジナルコードです。
/// <br>上記のサンプル（XR.Interaction.Toolkit StarterAsset）のTunnelingVignetteManagerをまったく新しく組み替えています。</br>
/// </para>
/// <exsample>
/// サンプルにある多くの余分な機能を排除し、よりシンプルにしています。
/// <br>以下は具体的な変更点です。</br>
/// <para>
/// ・独自のプレイヤー挙動に合わせるため、LocomotionVignetteProvidersを削除しました。
/// <br>・各Locomotionでのパラメータのオーバーライド機能をを削除しました。</br>
/// <br>・EasingについてDOTweenを用い、簡易的な実装にしました。</br>
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
    [Tooltip("移動時にTunnelingVignetteを有効化するかどうか")]
    [SerializeField] private bool enabledOnMoving = true;
    [Tooltip("移動時のEaseinの所要時間[s]")]
    [SerializeField, Min(0f)] private float easeInDurationOnMoving = 0.15f;
    [Tooltip("移動時のEaseOutの所要時間[s]")]
    [SerializeField, Min(0f)] private float easeOutDurationOnMoving = 0.15f;
    [Tooltip("ジャンプ時にTunnelingVignetteを有効化するかどうか")]
    [SerializeField] private bool enabledOnJumping = true;
    [Tooltip("ジャンプ時のEaseInの所要時間[s]")]
    [SerializeField, Min(0f)] private float easeInDurationOnJumping = 0.10f;
    [Tooltip("ジャンプ時のEaseOutの所要時間[s]")]
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

        // Shaderのプロパティに初期値を代入
        initialApertureSize = 0.7f;

        // Shaderのプロパティに初期値を設定
        meshRenderer.GetPropertyBlock(vignettePropertyBlock);
        vignettePropertyBlock.SetFloat(ShaderPropertyLookup.apertureSize, 1f);
        meshRenderer.SetPropertyBlock(vignettePropertyBlock);

        #region Subscribe
        VRPC.IsMovingRP
            // 初期代入時の発行は無視
            .Skip(1)
            // Filter: 有効化設定がされている かつ ジャンプ中でない
            .Where(isMoving => enabledOnMoving && !VRPC.IsJumpingRP.Value)
            .Subscribe(isMoving =>
            {
                // 移動開始時はEaseIn
                if (isMoving)
                {
                    EaseInTunnelingVignette(easeInDurationOnMoving);
                }
                // 移動終了時はEaseOut
                else
                {
                    EaseOutTunnelingVignette(easeOutDurationOnMoving);
                }
            })
            .AddTo(this);

        VRPC.IsJumpingRP
            // 初期代入時の発行は無視
            .Skip(1)
            // Filter: 有効化設定がされている かつ 移動中でない
            .Where(isJumping => enabledOnJumping && !VRPC.IsMovingRP.Value)
            .Subscribe(isJumping =>
            {
                // ジャンプ開始時はEaseIn
                if (isJumping)
                {
                    EaseInTunnelingVignette(easeInDurationOnJumping);
                }
                // 着地時はEaseOut
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
    /// TunnelingVignetteをEaseInで出現させる
    /// </summary>
    private void EaseInTunnelingVignette(float easeDuration)
    {
        // RendererからMaterialPropertyBlockを取得
        meshRenderer.GetPropertyBlock(vignettePropertyBlock);

        // DOTweenでフェードインを表現
        DOVirtual.Float(from: 1f, to: initialApertureSize, duration: easeDuration, onVirtualUpdate: value =>
        {
            // 値の更新
            vignettePropertyBlock.SetFloat(ShaderPropertyLookup.apertureSize, value);
            meshRenderer.SetPropertyBlock(vignettePropertyBlock);
        });
    }

    /// <summary>
    /// TunnelingVignetteをEaseOutで消滅させる
    /// </summary>
    private void EaseOutTunnelingVignette(float easeDuration)
    {
        // RendererからMaterialPropertyBlockを取得
        meshRenderer.GetPropertyBlock(vignettePropertyBlock);

        // DOTweenでフェードアウトを表現
        DOVirtual.Float(from: initialApertureSize, to: 1f, duration: easeDuration, onVirtualUpdate: value =>
        {
            // 値の更新
            vignettePropertyBlock.SetFloat(ShaderPropertyLookup.apertureSize, value);
            meshRenderer.SetPropertyBlock(vignettePropertyBlock);
        });
    }
}