using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class WhiteVignetteManager : MonoBehaviour
{
    private static class ShaderPropertyLookup
    {
        public static readonly int apertureSize = Shader.PropertyToID("_ApertureSize");
        public static readonly int featheringEffect = Shader.PropertyToID("_FeatheringEffect");
        public static readonly int vignetteColor = Shader.PropertyToID("_VignetteColor");
        public static readonly int vignetteColorBlend = Shader.PropertyToID("_VignetteColorBlend");
    }

    [Tooltip("Easeinの所要時間[s]")]
    private const float EASEIN_DURATION = 0.25f;
    [Tooltip("EaseOutの所要時間[s]")]
    private const float EASEOUT_DURATION = 0.25f;

    private MeshRenderer meshRenderer = default;
    private MaterialPropertyBlock vignettePropertyBlock = default;


    private void Awake()
    {
        #region Cache
        meshRenderer = GetComponent<MeshRenderer>();
        vignettePropertyBlock = new MaterialPropertyBlock();
        #endregion

        // Shaderのプロパティに初期値を設定
        meshRenderer.enabled = true;
        meshRenderer.GetPropertyBlock(vignettePropertyBlock);
        vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColor, new Color(1f, 1f, 1f, 0f));
        vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColorBlend, new Color(1f, 1f, 1f, 0f));
        meshRenderer.SetPropertyBlock(vignettePropertyBlock);
    }

    /// <summary>
    /// ホワイトアウトする
    /// <br>フェードインが完了したタイミングでタスクの完了が通知される</br>
    /// </summary>
    /// <param name="easeInDuration">Easeinの所要時間[s]</param>
    /// <param name="easeOutDuration">EaseOutの所要時間[s]</param>
    public UniTask WhiteOut(float easeInDuration = EASEIN_DURATION, float easeOutDuration = EASEOUT_DURATION)
    {
        var tcs = new UniTaskCompletionSource();

        EaseInWhiteVignette();
        return tcs.Task;

        void EaseInWhiteVignette()
        {
            // RendererからMaterialPropertyBlockを取得
            meshRenderer.GetPropertyBlock(vignettePropertyBlock);

            // DOTweenでフェードインを表現
            DOVirtual.Float(from: 0f, to: 1f, duration: easeInDuration, onVirtualUpdate: value =>
            {
                // 値の更新
                vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColor, new Color(1f, 1f, 1f, value));
                vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColorBlend, new Color(1f, 1f, 1f, value));
                meshRenderer.SetPropertyBlock(vignettePropertyBlock);
            }).OnComplete(() =>
            {
                tcs.TrySetResult();
                EaseOutWhiteVignette();
            });
        }

        void EaseOutWhiteVignette()
        {
            // RendererからMaterialPropertyBlockを取得
            meshRenderer.GetPropertyBlock(vignettePropertyBlock);

            // DOTweenでフェードアウトを表現
            DOVirtual.Float(from: 1f, to: 0f, duration: easeOutDuration, onVirtualUpdate: value =>
            {
                // 値の更新
                vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColor, new Color(1f, 1f, 1f, value));
                vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColorBlend, new Color(1f, 1f, 1f, value));
                meshRenderer.SetPropertyBlock(vignettePropertyBlock);
            });
        }
    }
}