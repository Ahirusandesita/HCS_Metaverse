using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class TunnelingVignetteManager : MonoBehaviour
{
    private static class ShaderPropertyLookup
    {
        public static readonly int apertureSize = Shader.PropertyToID("_ApertureSize");
        public static readonly int featheringEffect = Shader.PropertyToID("_FeatheringEffect");
        public static readonly int vignetteColor = Shader.PropertyToID("_VignetteColor");
        public static readonly int vignetteColorBlend = Shader.PropertyToID("_VignetteColorBlend");
    }

    [Serializable]
    private class VignetteProvider
    {
        [SerializeField]
        private InputActionReference inputActionReference = default;

        public InputActionReference InputActionReference => inputActionReference;
    }

    [SerializeField] private List<VignetteProvider> vignetteProviders = default;

    private MeshRenderer meshRenderer = default;
    private MaterialPropertyBlock vignettePropertyBlock = default;

    private void Awake()
    {
        foreach (var provider in vignetteProviders)
        {
            InputAction action = provider.InputActionReference.action;
            action.Enable();
            action.started += _ => UpdateTunnelingVignette(0);
            action.canceled += _ => UpdateTunnelingVignette(1);
        }

        meshRenderer = GetComponent<MeshRenderer>();
        vignettePropertyBlock = new MaterialPropertyBlock();
        UpdateTunnelingVignette(1);
    }

    //[Conditional("UNITY_EDITOR")]
    //private void Reset()
    //{
    //    try
    //    {
    //        meshRenderer = GetComponent<MeshRenderer>();
    //    }
    //    catch (NullReferenceException)
    //    {
    //        meshRenderer = gameObject.AddComponent<MeshRenderer>();
    //    }
    //}

    private void UpdateTunnelingVignette(int index)
    {
        if (index == 0)
        {
            meshRenderer.GetPropertyBlock(vignettePropertyBlock);
            vignettePropertyBlock.SetFloat(ShaderPropertyLookup.apertureSize, 0.8f);
            vignettePropertyBlock.SetFloat(ShaderPropertyLookup.featheringEffect, 0.25f);
            vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColor, Color.black);
            vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColorBlend, Color.black);
            meshRenderer.SetPropertyBlock(vignettePropertyBlock);
        }       
        else if (index == 1)
        {
            meshRenderer.GetPropertyBlock(vignettePropertyBlock);
            vignettePropertyBlock.SetFloat(ShaderPropertyLookup.apertureSize, 1f);
            vignettePropertyBlock.SetFloat(ShaderPropertyLookup.featheringEffect, 0.25f);
            vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColor, Color.black);
            vignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColorBlend, Color.black);
            meshRenderer.SetPropertyBlock(vignettePropertyBlock);
        }
    }
}
