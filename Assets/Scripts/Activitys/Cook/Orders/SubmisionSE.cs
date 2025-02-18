using UnityEngine;
using Fusion;

public class SubmisionSE : MonoBehaviour
{
    [SerializeField]
    private AudioClip 
        _chainSE = default,
        _successSE = default,
        _missSE = default;

    private AudioSource _audioSource = default;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_Chain()
    {
        _audioSource.PlayOneShot(_chainSE);
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_Success()
    {
        _audioSource.PlayOneShot(_successSE);
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_Miss()
    {
        _audioSource.PlayOneShot(_missSE);
        Debug.LogError($"MissSE");
    }
}
