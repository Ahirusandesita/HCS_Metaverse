using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CookActivitySound : MonoBehaviour
{
    [System.Serializable]
    public struct CookSE
    {
        public AudioClip cut;
        public AudioClip bake;
        public AudioClip mix;
    }

    [SerializeField]
    private CookSE _cookSE = default;

    public enum SEName_Cook
    {
        cut,
        bake,
        mix
    }

    public AudioClip SelectClip(SEName_Cook name)
    {
        switch (name)
        {
            case SEName_Cook.cut:
                return _cookSE.cut;

            case SEName_Cook.bake:
                return _cookSE.bake;

            case SEName_Cook.mix:
                return _cookSE.mix;

            default:
                Debug.Log("SENameÇ™ë∂ç›ÇµÇ‹ÇπÇÒ by CookActivity");
                return default;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_PlayOneShotSE(SEName_Cook name, Vector3 position)
    {
        AudioClip clip = SelectClip(name);

        AudioSource.PlayClipAtPoint(clip, position);
    }
}
