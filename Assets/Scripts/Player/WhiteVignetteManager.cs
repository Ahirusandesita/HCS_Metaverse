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

    [Tooltip("Easein�̏��v����[s]")]
    [SerializeField, Min(0f)] private float easeInDuration = 0.25f;
    [Tooltip("EaseOut�̏��v����[s]")]
    [SerializeField, Min(0f)] private float easeOutDuration = 0.25f;

    private MeshRenderer meshRenderer = default;
    private MaterialPropertyBlock vignettePropertyBlock = default;


    private void Awake()
    {
        #region Cache
        meshRenderer = GetComponent<MeshRenderer>();
        vignettePropertyBlock = new MaterialPropertyBlock();
        #endregion

        // Shader�̃v���p�e�B�ɏ����l��ݒ�
        meshRenderer.enabled = true;
        meshRenderer.GetPropertyBlock(vignettePropertyBlock);
        vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColor, new Color(1f, 1f, 1f, 0f));
        vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColorBlend, new Color(1f, 1f, 1f, 0f));
        meshRenderer.SetPropertyBlock(vignettePropertyBlock);
    }

    /// <summary>
    /// �z���C�g�A�E�g����
    /// <br>�t�F�[�h�C�������������^�C�~���O�Ń^�X�N�̊������ʒm�����</br>
    /// </summary>
    /// <returns></returns>
    public UniTask WhiteOut()
    {
        var tcs = new UniTaskCompletionSource();

        EaseInWhiteVignette();
        return tcs.Task;

        void EaseInWhiteVignette()
        {
            // Renderer����MaterialPropertyBlock���擾
            meshRenderer.GetPropertyBlock(vignettePropertyBlock);

            // DOTween�Ńt�F�[�h�C����\��
            DOVirtual.Float(from: 0f, to: 1f, duration: easeInDuration, onVirtualUpdate: value =>
            {
                    // �l�̍X�V
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
            // Renderer����MaterialPropertyBlock���擾
            meshRenderer.GetPropertyBlock(vignettePropertyBlock);

            // DOTween�Ńt�F�[�h�A�E�g��\��
            DOVirtual.Float(from: 1f, to: 0f, duration: easeOutDuration, onVirtualUpdate: value =>
            {
                    // �l�̍X�V
                    vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColor, new Color(1f, 1f, 1f, value));
                vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColorBlend, new Color(1f, 1f, 1f, value));
                meshRenderer.SetPropertyBlock(vignettePropertyBlock);
            });
        }
    }
}